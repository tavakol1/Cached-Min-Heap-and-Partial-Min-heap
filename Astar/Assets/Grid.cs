using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//using System;

public class Grid : MonoBehaviour {
    //public Transform player;
    //public Transform goal;
    public bool randomObstacles;
    public int ObstacleChance;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize; 
    public float nodeRadius;
    public bool drawObstaclesGizmos = false;
	public bool onlyDisplayPathGizmos = false;

    Node[,] grid;

    public float nodeDiameter;
    int gridSizeX, gridSizeY;
    
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt( gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt( gridWorldSize.y / nodeDiameter);
        CreateGrid(randomObstacles, ObstacleChance);
    }

	public int MaxSize{
		get { return gridSizeX * gridSizeY; }
	}

    public void regenrateGrid() {
        path = null;
        pathx = null;
        PathPartialHeap = null;
        CreateGrid(randomObstacles, ObstacleChance);
    }

    void CreateGrid(bool ObstacleFlag, int ObstacleChance) {

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        System.Random rnd = new System.Random();
        for (int x = 0;x<gridSizeX;x++) {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask));
                if (walkable && ObstacleFlag) {
                    if (rnd.Next(1,100) < ObstacleChance){
                        walkable = false;
                    }
                }
                grid[x, y] = new Node(walkable,worldPoint,x,y);
            }
        }
        //PrintMap(grid);
        //File();
    }

	public void PassingValues(bool x){
		//this method will pass some values from the pathfiding class to the grid class;
		if (x){
			PrintMap(x ,grid);
		}
	}

    public List<Node> GetNeighbours(Node node) { //in this method based on the grid position of each node, neighbours will be generated based on an octile situation! neighbours[] will be returned with nodes surrounding the element!
        List<Node> neighbours 	= new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x==0 && y==0)
                continue;

            int checkX = node.gridX + x;
            int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    void ImportMap() {
        //This Method will take care of importing a new Map into the already impelemented System.

    }

    public void PrintMap(bool flag ,Node[,] grid) {
		if (flag){
			var time = DateTime.Now;
			//format the time when you retrieve it, not when you store it - i.e. -
			string formattedtime = time.ToString("yyyy-mm-dd-hh-mm-ss");
			string path = "Assets/Test/" + formattedtime + ".txt";
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine("This is the Path for the file: {0}\n", path);
				// Add some text to the file.
				sw.WriteLine("-------------------");
				sw.WriteLine("This is a newly generated File!");
				// Arbitrary objects can also be written to the file.
				sw.Write("The date is: ");
				sw.WriteLine(System.DateTime.Now);

				string[] MapLane = new string[gridSizeY];
				for (int x = 0;x<gridSizeX;x++) {
					for (int y=0;y<gridSizeY;y++) {
						if (grid[x, y].walkable)
						{
							MapLane[y] = "1";
						}
						else {
							MapLane[y] = "0";
						}
					}
					sw.WriteLine(string.Join(",", MapLane));
				}
			}

		}
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x,y];
    }

    public List<Node> path;
    public List<Node> pathx;
    public List<Node> PathPartialHeap;

    private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos) {
			if (path != null) {
				foreach (Node n in path) {
					Gizmos.color = Color.black;
					Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
            if (pathx != null) {
                foreach (Node n in pathx) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
            if (PathPartialHeap != null) {
                foreach (Node n in PathPartialHeap) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }

        if (drawObstaclesGizmos == true)
        {
            if (grid != null)
            {
                foreach (Node n in grid)
                    {
                    Gizmos.color = (!n.walkable) ? Color.yellow : Color.white;
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }

}
