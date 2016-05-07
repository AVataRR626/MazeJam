using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
public class MazeGenerator : MonoBehaviour {

	[SerializeField]
	GameObject wallPiece;

    [SerializeField]
	int width;

	[SerializeField]
	int height;

    [SerializeField]
    GameArea[] starts;

    [SerializeField]
    GameArea[] goals;

    //////

  
    [SerializeField]
    IntVector2 AlgorithmStartingPosition;

    [SerializeField]
    bool useRandomAlgorithmStartingPos = false;

    /*
  [SerializeField]
  IntVector2 endPosition;

  [SerializeField]
  bool useRandomEndPos = false;

  [SerializeField]
  int deletionRange = 0;
  */

    ////////

    Cell[][] maze;

    void Awake () {
		Debug.Log ("Awake");
	}

    [System.Serializable]
    struct GameArea
    {
        public GameObject prefab;
        public int positionY;
        public int xOffset;
    }

    [System.Serializable]
    struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public void GenerateMaze()
    {
        Debug.Log("Generating Maze");

        //delete previous maze
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        //initialize maze array
        maze = new Cell[width][];
        for (int x = 0; x < width; x++)
        {
            maze[x] = new Cell[height];

            for (int y = 0; y < height; y++)
            {
                maze[x][y] = null;
            }
        }

        CreateBlankMaze();

        DrawBorder();

        List<Cell> cells = new List<Cell>();

        //Add one cell to C, at random, mark as visited
        if (useRandomAlgorithmStartingPos)
        {
            AlgorithmStartingPosition.x = (int)Random.Range(0, width);
            AlgorithmStartingPosition.y = (int)Random.Range(0, height);
        }

        cells.Add(maze[AlgorithmStartingPosition.x][AlgorithmStartingPosition.y]);
        DestroyImmediate(cells[cells.Count - 1].associatedWallPiece);
        cells[cells.Count - 1].state = State.Visited;
        Debug.Log("Starting Cell - x:" + cells[cells.Count - 1].position.x + " y:" + cells[cells.Count - 1].position.y);

        while (cells.Count > 0) {
            //get neighbours
            List<Cell> neighbours = cells[cells.Count - 1].GetNeighbours(maze);

            //pick random neighbour and add to active cells list, mark as visited
            if (neighbours.Count != 0)
            {
                cells.Add(neighbours[Random.Range(0, neighbours.Count)]);
                DestroyImmediate(cells[cells.Count - 1].associatedWallPiece);
                cells[cells.Count - 1].state = State.Visited;
            } else //all neighbours have been visited so backtrack
            {
                cells[cells.Count - 1].state = State.Backtracked;
                cells.RemoveAt(cells.Count - 1);
            }
        }

        /*
        if (useRandomEndPos)
        {
            endPosition.x = (int)Random.Range(0, width);
            endPosition.y = (int)Random.Range(0, height);
        }
        DeleteBlocksAround(startingPosition);
        DeleteBlocksAround(endPosition);
        
        
        DeleteBlocks(starts, -1);
        DeleteBlocks(starts, 0);
        DeleteBlocks(goals, width - 1);
        DeleteBlocks(goals, width);

        for (int i = 0; i < starts.Length; i++)
        {
            SpawnObject(PlayerSpawner);
        }
        for (int i = 0; i < goals.Length; i++)
        {
            SpawnObject(GoalSpawner);
        }
        */

        SetupGameAreas(starts);
        SetupGameAreas(goals);

    }

    void SetupGameAreas(GameArea[] area)
    {
        if (area == null)
        {
            Debug.Log("Game Areas not setup in inscpector");
        }
        else {
            for (int i = 0; i < area.Length; i++)
            {
                DeleteBlock(-1, area[i].positionY);
                DeleteBlock(0, area[i].positionY);

                GameObject tempGameArea = Instantiate(area[i].prefab, new Vector3(transform.position.x + area[i].xOffset, 0, transform.position.z + area[i].positionY), Quaternion.identity) as GameObject;
                tempGameArea.transform.parent = transform;
            }
        }
    }

    void DeleteBlock(int xValue, int yValue) //
    {
        for (int k = transform.childCount - 1; k >= 0; k--)
        {
            if ((int)transform.GetChild(k).transform.position.x == (int)(xValue + transform.position.x) && (int)transform.GetChild(k).transform.position.z == (int)(yValue + transform.position.z))
            {
                Debug.Log(xValue + transform.position.x + " : " + transform.GetChild(k).transform.position.x);
                DestroyImmediate(transform.GetChild(k).gameObject);
            }
        }
    }

    void DeleteBlocks2(int[] positions, int xValue) //
    {

        for (int i = 0; i < positions.Length; i++)
        {
            for (int k = transform.childCount - 1; k >= 0; k--)
            {
                if ((int)transform.GetChild(k).transform.position.x == (int)(xValue + transform.position.x) && (int)transform.GetChild(k).transform.position.z == (int)(positions[i] + transform.position.z))
                {
                    Debug.Log(xValue + transform.position.x + " : " + transform.GetChild(k).transform.position.x);
                    DestroyImmediate(transform.GetChild(k).gameObject);
                }
            }
        }
    }

    void SpawnObject(GameObject objectToSpawn)
    {

    }

    void CreateBlankMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(x, 0, y);

                GameObject tempPiece = Instantiate(wallPiece, transform.position + spawnPosition, Quaternion.identity) as GameObject;
                tempPiece.transform.parent = transform;

                Cell tempCell = new Cell(new IntVector2(x, y), tempPiece, maze);
                maze[x][y] = tempCell;
            }
        }
    }

    void DrawBorder() { 
        for (int x = -1; x <= width; x+= width+1)
        {
            for (int y = -1; y < height+1; y++)
            {
                Vector3 spawnPosition = new Vector3(x, 0, y);

                GameObject tempPiece = Instantiate(wallPiece, transform.position + spawnPosition, Quaternion.identity) as GameObject;
                tempPiece.transform.parent = transform;
            }
        }
        for (int y = -1; y <= height; y += height + 1)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 spawnPosition = new Vector3(x, 0, y);

                GameObject tempPiece = Instantiate(wallPiece, transform.position + spawnPosition, Quaternion.identity) as GameObject;
                tempPiece.transform.parent = transform;
            }
        }
    }

    /*
    void DeleteBlocksAround(IntVector2 position)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if(Mathf.Abs(transform.GetChild(i).transform.position.x - (position.x + transform.position.x)) <= deletionRange && Mathf.Abs(transform.GetChild(i).transform.position.z - (position.y + transform.position.z)) <= deletionRange)
            {
                DestroyImmediate(transform.GetChild(i).gameObject); 
            }
        }
    }
    */

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < starts.Length; i++)
        {
            Gizmos.DrawCube(new Vector3(transform.position.x - 1, 0, transform.position.z + starts[i].positionY), Vector3.one);
        }

        Gizmos.color = Color.magenta;
        for (int i = 0; i < goals.Length; i++)
        {
            Gizmos.DrawCube(new Vector3(transform.position.x + width, 0, transform.position.z + goals[i].positionY), Vector3.one);
        }
    }

    private class Cell
    {
        public IntVector2 position;
        public GameObject associatedWallPiece;
        public State state = State.Unvisited;
        private Cell[][] mazeParent;

        public Cell(IntVector2 pos, GameObject _g, Cell[][] parent)
        {
            position = pos;
            associatedWallPiece = _g;
            mazeParent = parent;
        }

        public bool IsCellOnGrid(int x, int y)
        {
            if (x >= 0 && x < mazeParent.Length && y >= 0 && y < mazeParent[0].Length)
            {
                return true;
            }

            return false;
        }

        public List<Cell> GetNeighbours(Cell[][] maze)
        {
            List<Cell> neighbours = new List<Cell>();

            if(IsCellOnGrid(position.x - 1, position.y))
            {
                neighbours.Add(maze[position.x - 1][position.y]);
            }

            if (IsCellOnGrid(position.x + 1, position.y))
            {
                neighbours.Add(maze[position.x + 1][position.y]);
            }

            if (IsCellOnGrid(position.x, position.y - 1))
            {
                neighbours.Add(maze[position.x][position.y - 1]);
            }

            if (IsCellOnGrid(position.x, position.y + 1))
            {
                neighbours.Add(maze[position.x][position.y + 1]);
            }

            //check that chosen neighbours havent been visited
            for (int i = neighbours.Count - 1; i >= 0; i--)
            {
                if(neighbours[i].state != State.Unvisited)
                {
                    neighbours.RemoveAt(i);
                }
            }

            //check that chosen neighbours arent neighbouring to already chosen cells diagonally, (would cause loop otherwise)
            for (int i = neighbours.Count - 1; i >= 0; i--)
            {
                bool canMoveToCell = true;

                if (neighbours[i].position.x < position.x) //we've moved in -x so check +/- y
                {

                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x - 1][ neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y))
                    {
                        if (maze[neighbours[i].position.x - 1][ neighbours[i].position.y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x - 1][ neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x][neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x][neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (neighbours[i].position.x > position.x) //we've moved in +x so check +/- y
                {

                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x][neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x][neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (neighbours[i].position.y > position.y) //we've moved in +y so check +/- x
                {

                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x - 1][neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x][neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y + 1))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y))
                    {
                        if (maze[neighbours[i].position.x - 1][neighbours[i].position.y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (neighbours[i].position.y < position.y) //we've moved in -y so check +/- x
                {

                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x - 1][neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x][neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y - 1))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x - 1, neighbours[i].position.y))
                    {
                        if (maze[neighbours[i].position.x - 1][neighbours[i].position.y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].position.x + 1, neighbours[i].position.y))
                    {
                        if (maze[neighbours[i].position.x + 1][neighbours[i].position.y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (!canMoveToCell)
                {
                    neighbours.RemoveAt(i);
                }
            }

            return neighbours;
        }
    }

    public enum State
    {
        Visited,
        Backtracked,
        Unvisited,
    }
}
