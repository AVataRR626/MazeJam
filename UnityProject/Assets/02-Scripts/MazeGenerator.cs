using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public float initialWaitPeriod = 4.0f;

    float iterationWaitingPeriod;

    public float approximateBuildingTime = 2.0f;

    //////


    [SerializeField]
    IntVector2 algorithmStartingPosition;

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

    List<Cell> cells;

    List<CellLink> allCellLinks;

    List<Cell> pathToGoal;

    void Awake () {
		Debug.Log ("Awake");

        Invoke("GenerateMaze", initialWaitPeriod);

        iterationWaitingPeriod = approximateBuildingTime / (width * height);
    }

    [System.Serializable]
    struct GameArea
    {
        public GameObject prefab;
        public IntVector2 holePosition;
        public int PrefabXOffsetFromHole;
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

        
        public override string ToString()
        {
            return "x: " + x + " y: " + y;
        }

        public float DistanceTo(IntVector2 other)
        {
            return Mathf.Sqrt(Mathf.Pow(x - other.x, 2) + Mathf.Pow(y - other.y, 2));
        }
    }

    class CellLink
    {
        public Cell start;
        public Cell end;

        public CellLink(Cell _start, Cell _end)
        {
            start = _start;
            end = _end;
        }

        public override string ToString()
        {
            if (end != null)
            {
                return "start: " + start.ToString() + " end: " + end.ToString();
            } else
            {
                return "start: " + start.ToString() + " end: null";
            }
        }
    }

    public void GenerateMaze()
    {
        Debug.Log("Generating Maze");

        //delete previous maze
        for (int i = transform.childCount - 1; i >= 0; i--)
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

        cells = new List<Cell>();
        allCellLinks = new List<CellLink>();
        pathToGoal = new List<Cell>();

        //Add one cell to C, at random, mark as visited
        if (useRandomAlgorithmStartingPos)
        {
            algorithmStartingPosition.x = (int)Random.Range(0, width);
            algorithmStartingPosition.y = (int)Random.Range(0, height);
        }

        VisitCell(maze[algorithmStartingPosition.x][algorithmStartingPosition.y]);
        maze[algorithmStartingPosition.x][algorithmStartingPosition.y].associatedWallPiece.AddComponent<BlockMover>();

        Debug.Log("Starting Cell - x:" + cells[cells.Count - 1].position.x + " y:" + cells[cells.Count - 1].position.y);

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == true)
        {
#else
        if (false)
        {
#endif
            
            MazeIterate();
        }
        else
        {
            Debug.Log("Created in edit mode");
            while (cells.Count > 0)
            {
                MazeIterateInnerLoop();
            }

            SetupAllGameAreas();
            DestroyStartingPieceAndChildren();
        }

    }

    void SetupAllGameAreas()
    {
        SetupHolePositions();
        SetupGameAreas(starts);
        SetupGameAreas(goals);
    }

    void MazeIterate()
    {
        if (cells.Count > 0)
        {
            Invoke("MazeIterate", iterationWaitingPeriod);

            MazeIterateInnerLoop();
        } else
        {
            Debug.Log("Finished");
            SetupAllGameAreas();
            Invoke("DestroyStartingPieceAndChildren", 0.5f);
        }
    }

    void DestroyStartingPieceAndChildren()
    {
        DestroyImmediate(maze[algorithmStartingPosition.x][algorithmStartingPosition.y].associatedWallPiece);
    }

    void MazeIterateInnerLoop()
    {
        //get neighbours
        List<Cell> neighbours = cells[cells.Count - 1].GetNeighbours(maze);

        //pick random neighbour and add to active cells list, mark as visited
        if (neighbours.Count != 0)
        {
            VisitCell(neighbours[Random.Range(0, neighbours.Count)]);
        }
        else //all neighbours have been visited so backtrack
        {
            cells[cells.Count - 1].state = State.Backtracked;

            CellLink newCellLink;

            if (cells.Count - 2 < 0)
            {
                newCellLink = new CellLink(cells[cells.Count - 1], null);
            }
            else {
                newCellLink = new CellLink(cells[cells.Count - 1], cells[cells.Count - 2]);
            }

            allCellLinks.Add(newCellLink);

            cells.RemoveAt(cells.Count - 1);
        }
    }

    void VisitCell(Cell cell)
    {
        cells.Add(cell);

        cells[cells.Count - 1].state = State.Visited;

        //parent to start block to create down movement
        cells[cells.Count - 1].associatedWallPiece.transform.parent = maze[algorithmStartingPosition.x][algorithmStartingPosition.y].associatedWallPiece.transform;
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
                DeleteBlock(area[i].holePosition.x, area[i].holePosition.y);
                DeleteBlock(area[i].holePosition.x - 1, area[i].holePosition.y);

                //goal areas
                if (area[i].holePosition.x - 1 >= width - 1)
                {
                    if (!HasPathNextToCell(maze[area[i].holePosition.x - 1][area[i].holePosition.y]))
                    {
                        Debug.Log("Deleted Extra block at x: " + (area[i].holePosition.x - 2) + " y: " + area[i].holePosition.y);
                        DeleteBlock(area[i].holePosition.x - 2, area[i].holePosition.y);
                    }
                }

                //start areas
                if(area[i].holePosition.x - 1 < 0) {
                    if (!HasPathNextToCell(maze[area[i].holePosition.x][area[i].holePosition.y]))
                    {
                        Debug.Log("Deleted Extra block at x: " + (area[i].holePosition.x + 1) + " y: " + area[i].holePosition.y);
                        DeleteBlock(area[i].holePosition.x + 1, area[i].holePosition.y);
                    }
                }

                GameObject tempGameArea = Instantiate(area[i].prefab, new Vector3(transform.position.x + area[i].PrefabXOffsetFromHole + area[i].holePosition.x, transform.position.y, transform.position.z + area[i].holePosition.y), Quaternion.identity) as GameObject;
                tempGameArea.transform.parent = transform;
            }
        }
    }

    bool HasPathNextToCell(Cell cell)
    {
        //one above
        Cell above = maze[cell.position.x][cell.position.y + 1];

        //one below
        Cell below = maze[cell.position.x][cell.position.y - 1];

        if(above.state == State.Backtracked || below.state == State.Backtracked)
        {
            return true;
        }

        return false;
    }


    void SetupHolePositions()
    {
        for (int i = 0; i < starts.Length; i++)
        {
            starts[i].holePosition.x = 0;
        }

        for (int i = 0; i < goals.Length; i++)
        {
            goals[i].holePosition.x = width;
        }
    }

    void DeleteBlock(int xValue, int yValue) //
    {
        for (int k = transform.childCount - 1; k >= 0; k--)
        {
            if ((int)transform.GetChild(k).transform.position.x == (int)(xValue + transform.position.x) && (int)transform.GetChild(k).transform.position.z == (int)(yValue + transform.position.z))
            {
                 DestroyImmediate(transform.GetChild(k).gameObject);
            }
        }
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

    void FindPathToCell(Cell goal)
    {
        CellLink currentLink = null;
        Cell destination = maze[algorithmStartingPosition.x][algorithmStartingPosition.y];
        // Cell start = maze[goals[0].holePosition.x - 1][goals[0].holePosition.y];
        Cell start = goal; //we start at thegoal and backtrace

        Debug.Log("Destination of path will be at, " + destination.position.ToString());


        foreach (CellLink link in allCellLinks)
        {
            if (link.start == start)
            {
                Debug.Log("Found Start of Path: " + link.ToString());
                currentLink = link;
                pathToGoal.Add(currentLink.start);
                break;
            }
        }
        //didnt find starting cell next to goal pos
        CellLink closestLink = null;
        float closestDistance = 99999999;
        if (currentLink == null)
        {
            foreach (CellLink link in allCellLinks)
            {
                if(link.start.position.DistanceTo(start.position) < closestDistance)
                {
                    closestLink = link;
                    closestDistance = link.start.position.DistanceTo(start.position);
                }
            }

            currentLink = closestLink;
            pathToGoal.Add(currentLink.start);
            Debug.Log("Found Almost Start of Path: " + currentLink.ToString());
        }

        foreach (CellLink link in allCellLinks)
        {
            if (link.end == destination)
            {
                Debug.Log("Found Destination of Path: " + link.ToString());

                break;
            }
        }

        while (currentLink.end != null && currentLink.end != destination)
        {
            foreach (CellLink link in allCellLinks)
            {
                if (link.start == currentLink.end)
                {
                    currentLink = link;
                    pathToGoal.Add(currentLink.start);
                    break;
                }
            }
        }

        Debug.Log("Found path of length " + pathToGoal.Count);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < starts.Length; i++)
        {
            Gizmos.DrawCube(new Vector3(transform.position.x - 1, 0, transform.position.z + starts[i].holePosition.y), Vector3.one);
        }

        Gizmos.color = Color.magenta;
        for (int i = 0; i < goals.Length; i++)
        {
            Gizmos.DrawCube(new Vector3(transform.position.x + width, 0, transform.position.z + goals[i].holePosition.y), Vector3.one);
        }

        if (allCellLinks != null)
        {
            Gizmos.color = Color.green;
            foreach (CellLink link in allCellLinks)
            {
                if (link.end != null)
                {
                    Vector3 startPos = new Vector3(link.start.position.x, 0, link.start.position.y) + transform.position;
                    Vector3 endPos = new Vector3(link.end.position.x, 0, link.end.position.y) + transform.position;
                    Gizmos.DrawLine(startPos, endPos);
                }
            }
        }

        if (pathToGoal != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 1; i < pathToGoal.Count; i++)
            {
                Vector3 startPos = new Vector3(pathToGoal[i].position.x, 0, pathToGoal[i].position.y) + transform.position;
                Vector3 endPos = new Vector3(pathToGoal[i - 1].position.x, 0, pathToGoal[i - 1].position.y) + transform.position;
                Gizmos.DrawLine(startPos, endPos);
            }
        }

        Gizmos.color = Color.black;
        Gizmos.DrawCube(new Vector3(transform.position.x + algorithmStartingPosition.x, 0, transform.position.z + algorithmStartingPosition.y), Vector3.one);
    }

    private class Cell
    {
        public IntVector2 position;
        public GameObject associatedWallPiece = null;
        public State state = State.Unvisited;
        private Cell[][] mazeParent;

        public Cell(IntVector2 pos, GameObject _g, Cell[][] parent)
        {
            position = pos;
            associatedWallPiece = _g;
            mazeParent = parent;
        }

        public override string ToString()
        {
            return position.ToString();
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
