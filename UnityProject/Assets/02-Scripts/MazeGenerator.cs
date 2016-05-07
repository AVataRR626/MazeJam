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

    Cell[][] maze;

    [SerializeField]
    int startingX; //just for visualization purposes

    [SerializeField]
    int startingY; //just for visualization purposes

    [SerializeField]
    bool useRandomStartingPos = false;

    [SerializeField]
    int endX; //just for visualization purposes

    [SerializeField]
    int endY; //just for visualization purposes

    [SerializeField]
    bool useRandomEndPos = false;

    [SerializeField]
    int deletionRange = 0;

    void Awake () {
		Debug.Log ("Awake");
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
        if (useRandomStartingPos)
        {
            startingX = Random.Range(0, width);
            startingY = Random.Range(0, height);
        }
        cells.Add(maze[startingX][startingY]);
        DestroyImmediate(cells[cells.Count - 1].associatedWallPiece);
        cells[cells.Count - 1].state = State.Visited;
        Debug.Log("Starting Cell: x-" + cells[cells.Count - 1].x + " y-" + cells[cells.Count - 1].y);

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

        if (useRandomEndPos)
        {
            endX = Random.Range(0, width);
            endY = Random.Range(0, height);
        }
        DeleteBlocksAround(startingX, startingY);
        DeleteBlocksAround(endX, endY);
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

                Cell tempCell = new Cell(x, y, tempPiece, maze);
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

    void DeleteBlocksAround(int x, int y)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if(Mathf.Abs(transform.GetChild(i).transform.position.x - x) <= deletionRange && Mathf.Abs(transform.GetChild(i).transform.position.z - y) <= deletionRange)
            {
                DestroyImmediate(transform.GetChild(i).gameObject); 
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(startingX, 0 , startingY), Vector3.one);

        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(new Vector3(endX, 0, endY), Vector3.one);
    }

    private class Cell
    {
        public int x;
        public int y;
        public GameObject associatedWallPiece;
        public State state = State.Unvisited;
        private Cell[][] mazeParent;

        public Cell(int _x, int _y, GameObject _g, Cell[][] parent)
        {
            x = _x;
            y = _y;
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

            if(IsCellOnGrid(x - 1, y))
            {
                neighbours.Add(maze[x - 1][y]);
            }

            if (IsCellOnGrid(x + 1, y))
            {
                neighbours.Add(maze[x + 1][y]);
            }

            if (IsCellOnGrid(x, y - 1))
            {
                neighbours.Add(maze[x][y - 1]);
            }

            if (IsCellOnGrid(x, y + 1))
            {
                neighbours.Add(maze[x][y + 1]);
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

                if (neighbours[i].x < x) //we've moved in -x so check +/- y
                {

                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x - 1][ neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y))
                    {
                        if (maze[neighbours[i].x - 1][ neighbours[i].y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x - 1][ neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x][neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x][neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (neighbours[i].x > x) //we've moved in +x so check +/- y
                {

                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x][neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x][neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (neighbours[i].y > y) //we've moved in +y so check +/- x
                {

                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x - 1][neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x][neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y + 1))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y + 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y))
                    {
                        if (maze[neighbours[i].x - 1][neighbours[i].y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                }

                if (neighbours[i].y < y) //we've moved in -y so check +/- x
                {

                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x - 1][neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x][neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y - 1))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y - 1].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x - 1, neighbours[i].y))
                    {
                        if (maze[neighbours[i].x - 1][neighbours[i].y].state != State.Unvisited)
                        {
                            canMoveToCell = false;
                        }
                    }
                    if (IsCellOnGrid(neighbours[i].x + 1, neighbours[i].y))
                    {
                        if (maze[neighbours[i].x + 1][neighbours[i].y].state != State.Unvisited)
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
