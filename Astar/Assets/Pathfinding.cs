using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System;

public class Pathfinding : MonoBehaviour
{

    Grid grid;
    //WriteToFile writer;
    public Transform seeker, target;
    public bool printMap;
    public bool findPath;
    public bool findPathWithHeap;
    public bool findPathWithLazyHeap;
    public bool findPathWithCachedMinHeap;//public int Depth;
    public bool findPathWithPartialMinHeap;
    public bool printCloseList;
    public bool printItterations;
    public int NumberOfOps;
    static string FileCreated;
    
 
    System.Random rnd = new System.Random();

    private void Awake()
    {
        grid = GetComponent<Grid>();
        grid.PassingValues(printMap);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (findPath)
            {
                FindPath(seeker.position, target.position);
            }
            if (findPathWithHeap)
            {
                FindPathHeap(seeker.position, target.position);
            }
            if (findPathWithLazyHeap)
            {
                FindPathLazyHeap(seeker.position, target.position);
            }
            if (findPathWithCachedMinHeap)
            {
                FindPathCachedMinHeap(seeker.position, target.position,10,7);
            }
            if (findPathWithPartialMinHeap)
            {
                FindPathPartialHeap(seeker.position, target.position,7);
            }
        }

        if (Input.GetKeyDown("x"))
        {
            grid.regenrateGrid();
        }
        if (Input.GetKeyDown("z")) {
            RandomPosition(seeker);
            RandomPosition(target);
        }
        //if (Input.GetKeyDown("f")) {
        //    FindPathCachedMinHeap(seeker.position, target.position, 10, 7);
        //}
    }

    public void motherOfAllMethods() {
        //generate a random Map with Obstacle Chance
        TestOps(NumberOfOps, grid.ObstacleChance);
    }

    public void RandomPosition(Transform _object) {
        rnd.Next();
        int tempx = rnd.Next(-(int)grid.gridWorldSize.x / 2 - 1, (int)grid.gridWorldSize.x / 2 - 1);
        int tempz = rnd.Next(-(int)grid.gridWorldSize.y / 2 - 1, (int)grid.gridWorldSize.y / 2 - 1);
        _object.position = new Vector3(tempx, 0, tempz);
        if (grid.NodeFromWorldPoint(_object.position).walkable)
        {
            return;
        }
        else {
            RandomPosition(_object);
        }
    }

    public void TestOps(int numberOfTests,int ObstacleChance) {
        //besmellahe rahman rahim
        OutputMethod RawFile = new OutputMethod();
        OutputMethod resultsOutput = new OutputMethod();
        RawFile.CreateDirectory("Raw Outputs!");
        resultsOutput.CreateDirectory("Overall Test - 2nd Nov");
        resultsOutput.CreateFile("Map Size 120in120 k = 6");
        resultsOutput.ContinueFile("Obs Chance,Total Success,Total Failed,OverAll Avg Ops,Success Ops,Ops Variance,Ops SD,Max Ops,Min Ops,Overall Time,Success Time,Number Of Nodes Expanded,Success Nodes Expanded");

        for (int ObsChance = 0; ObsChance < 60; ObsChance+=5)
        {
            RawFile.CreateFile("RawFile - MapSize = 120IN120 k=6 Chance of "+ObsChance);
            List<ResultsClass> Results = new List<ResultsClass>();

            List<AvgClass> AverageList = new List<AvgClass>();
            List<AvgClass> AverageHeap = new List<AvgClass>();
            List<AvgClass> AveragePartialMinHeap = new List<AvgClass>();
            List<AvgClass> AverageCachedMinHeap = new List<AvgClass>();

           
            grid.ObstacleChance = ObsChance;
            grid.regenrateGrid();

            for (int i = 0; i < numberOfTests; i++)
            {
                RandomPosition(seeker);
                RandomPosition(target);

                Results.Add(FindPath(seeker.position, target.position));
                Results.Add(FindPathHeap(seeker.position, target.position));
                Results.Add(FindPathPartialHeap(seeker.position, target.position, 6));
                Results.Add(FindPathCachedMinHeap(seeker.position, target.position, 10, 7));


                //resultsOutput.ContinueFile(grid.NodeFromWorldPoint(seeker.position).gridX + "," + grid.NodeFromWorldPoint(seeker.position).gridY + " -- " + grid.NodeFromWorldPoint(target.position).gridX + "," + grid.NodeFromWorldPoint(target.position).gridY);
                RawFile.ContinueFile(Results[4 * i].Success + " - " + Results[4 * i].HeapType + " - " + Results[4 * i].Ops + " - " + Results[4 * i].Time + " - " + Results[4 * i].PathLength + " - " + Results[4 * i].PathCost + grid.NodeFromWorldPoint(seeker.position).gridX + "," + grid.NodeFromWorldPoint(seeker.position).gridY + " -- " + grid.NodeFromWorldPoint(target.position).gridX + "," + grid.NodeFromWorldPoint(target.position).gridY);
                RawFile.ContinueFile(Results[4 * i + 1].Success + " - " + Results[4 * i + 1].HeapType + " - " + Results[4 * i + 1].Ops + " - " + Results[4 * i + 1].Time + " - " + Results[4 * i + 1].PathLength + " - " + Results[4 * i + 1].PathCost);
                RawFile.ContinueFile(Results[4 * i + 2].Success + " - " + Results[4 * i + 2].HeapType + " - " + Results[4 * i + 2].Ops + " - " + Results[4 * i + 2].Time + " - " + Results[4 * i + 2].PathLength + " - " + Results[4 * i + 2].PathCost);
                RawFile.ContinueFile(Results[4 * i + 3].Success + " - " + Results[4 * i + 3].HeapType + " - " + Results[4 * i + 3].Ops + " - " + Results[4 * i + 3].Time + " - " + Results[4 * i + 3].PathLength + " - " + Results[4 * i + 3].PathCost);

                AverageList.Add(new AvgClass { HeapType = "List", TimeTaken = Results[4 * i].Time, Success = Results[4 * i].Success == true ? 1 : 0 , Failed = Results[4 * i].Success == false ? 1 : 0 , ops = Results[4 * i].Ops, PathCost = Results[4 * i].PathCost,ObstacleChance = ObsChance, NumberOfNodesExpanded = Results[4*i].NodesExpanded});
                AverageHeap.Add(new AvgClass { HeapType = "Heap", TimeTaken = Results[4 * i + 1].Time, Success = Results[4 * i + 1].Success == true ? 1 : 0 , Failed = Results[4 * i + 1].Success == false ? 1 : 0 , ops = Results[4 * i + 1].Ops, PathCost = Results[4 * i + 1].PathCost, ObstacleChance = ObsChance, NumberOfNodesExpanded = Results[4 * i + 1].NodesExpanded });
                AveragePartialMinHeap.Add(new AvgClass { HeapType = "Partial Min Heap", TimeTaken = Results[4 * i + 2].Time, Success = Results[4 * i + 2].Success == true ? 1 : 0 , Failed = Results[4 * i + 2].Success == false ? 1 : 0 , ops = Results[4 * i + 2].Ops, PathCost = Results[4 * i + 2].PathCost, ObstacleChance = ObsChance, NumberOfNodesExpanded = Results[4 * i + 2].NodesExpanded });
                AverageCachedMinHeap.Add(new AvgClass { HeapType = "Cached Min Heap", TimeTaken = Results[4 * i + 3].Time, Success = Results[4 * i + 3].Success == true ? 1 : 0, Failed = Results[4 * i + 3].Success == false ? 1 : 0, ops = Results[4 * i + 3].Ops, PathCost = Results[4 * i + 3].PathCost, ObstacleChance = ObsChance, NumberOfNodesExpanded = Results[4 * i + 3].NodesExpanded });

                //print(Results[4 * i].Success + " - " + Results[4 * i].HeapType + " - " + Results[4 * i].Ops + " - " + Results[4 * i].Time + " - " + Results[4 * i].PathLength + " - " + Results[4 * i].PathCost);
                //print(Results[4 * i + 1].Success + " - " + Results[4 * i + 1].HeapType + " - " + Results[4 * i + 1].Ops + " - " + Results[4 * i + 1].Time + " - " + Results[4 * i + 1].PathLength + " - " + Results[4 * i + 1].PathCost);
                //print(Results[4 * i + 2].Success + " - " + Results[4 * i + 2].HeapType + " - " + Results[4 * i + 2].Ops + " - " + Results[4 * i + 2].Time + " - " + Results[4 * i + 2].PathLength + " - " + Results[4 * i + 2].PathCost);
                //print(Results[4 * i + 3].Success + " - " + Results[4 * i + 3].HeapType + " - " + Results[4 * i + 3].Ops + " - " + Results[4 * i + 3].Time + " - " + Results[4 * i + 3].PathLength + " - " + Results[4 * i + 3].PathCost);
            }

            AvgClass AvgList = CalculateAverage(AverageList);
            AvgClass AvgHeap = CalculateAverage(AverageHeap);
            AvgClass AvgPartialMinHeap = CalculateAverage(AveragePartialMinHeap);
            AvgClass AvgCachedMinHeap = CalculateAverage(AverageCachedMinHeap);
            //"Obs Chance,Total Success,Total Failed,OverAll Ops,Success Ops,Ops Variance,Ops SD,Max Ops,Min Ops,Overall Time,Success Time,Number Of Nodes Expanded,Success Nodes Expanded"
            resultsOutput.ContinueFile(ObsChance + "," + AvgList.Success + "," + AvgList.Failed + "," + AvgList.ops + "," + AvgList.PosOps + "," + AvgList.Variance + "," + AvgList.StandardDeviation +","+AvgList.Max + "," + AvgList.Min + "," + AvgList.PosTime + "," + AvgList.NumberOfNodesExpanded + "," + AvgList.PathCost + "," + AvgList.HeapType);
            resultsOutput.ContinueFile(ObsChance + "," + AvgHeap.Success + "," + AvgHeap.Failed + "," + AvgHeap.ops + "," + AvgHeap.PosOps + "," + AvgHeap.Variance + "," + AvgHeap.StandardDeviation + "," + AvgHeap.Max + "," + AvgHeap.Min  + "," + AvgHeap.PosTime + "," + AvgHeap.NumberOfNodesExpanded + "," + AvgHeap.PathCost + "," + AvgHeap.HeapType);
            resultsOutput.ContinueFile(ObsChance + "," + AvgPartialMinHeap.Success + "," + AvgPartialMinHeap.Failed + "," + AvgPartialMinHeap.ops + "," + AvgPartialMinHeap.PosOps + "," + AvgPartialMinHeap.Variance + "," + AvgPartialMinHeap.StandardDeviation + "," + AvgPartialMinHeap.Max + "," + AvgPartialMinHeap.Min +  "," + + AvgPartialMinHeap.PosTime + "," + AvgPartialMinHeap.NumberOfNodesExpanded + "," + AvgPartialMinHeap.PathCost + "," + AvgPartialMinHeap.HeapType);
            resultsOutput.ContinueFile(ObsChance + "," + AvgCachedMinHeap.Success + "," + AvgCachedMinHeap.Failed + "," + AvgCachedMinHeap.ops + "," + AvgCachedMinHeap.PosOps + "," + AvgCachedMinHeap.Variance+ "," + AvgPartialMinHeap.StandardDeviation + "," + AvgCachedMinHeap.Max + "," + AvgCachedMinHeap.Min + "," + AvgCachedMinHeap.PosTime + "," + AvgCachedMinHeap.NumberOfNodesExpanded + "," + AvgCachedMinHeap.PathCost + "," + AvgCachedMinHeap.HeapType);
            //Wait();
        }
        print("Done!");
    }

    public AvgClass CalculateAverage(List<AvgClass> avg) {
        int success = 0, failure = 0, ops = 0, pathCost = 0, NumbOfNodes = 0; ;
        long time = 0;
        int successCounter = 0, succOps = 0, succPathCost = 0, succNumbOfNodes = 0, succCurrentMinOps = int.MaxValue, succCurrentMaxOps = int.MinValue, SumOps = 0;
        long succTime = 0;


        foreach (AvgClass x in avg) {
            if (x.Success == 1) // Successful path found 
            {
                successCounter++;
                succNumbOfNodes = x.NumberOfNodesExpanded;
                succOps += x.ops;
                SumOps += x.ops;
                succPathCost += x.PathCost;
                succTime += x.TimeTaken;
                if (x.ops < succCurrentMinOps) succCurrentMinOps = x.ops;
                if (x.ops > succCurrentMaxOps) succCurrentMaxOps = x.ops;
            }
            if (x.Failed == 1) // failed to find a path.
            { // need to work on this part!
                x.PathCost = 0;
            }
            NumbOfNodes += x.NumberOfNodesExpanded;
            success += x.Success;
            failure += x.Failed;
            ops += x.ops;
            pathCost += x.PathCost;
            time += x.TimeTaken;
        }
        //Overall Average
        NumbOfNodes = NumbOfNodes / avg.Count;
        ops = ops / avg.Count;
        pathCost = pathCost / avg.Count;
        time = time / avg.Count;

        // Pos Average
        succNumbOfNodes = succNumbOfNodes / successCounter;
        succOps = succOps / successCounter;
        succPathCost = succPathCost / successCounter;
        succTime = succTime / successCounter;
        // Variance and standard deviation
        double Sigma = 0;
        foreach (AvgClass x in avg) // calculating Sigma
        {
            if (x.Success == 1)
            {
                Sigma += Math.Pow(x.ops - succOps, 2);
            }
        }
        double Variance = Math.Sqrt(Sigma / successCounter);
        double StandardDeviation = Math.Sqrt(Variance);

        return new AvgClass {
            HeapType = avg[0].HeapType,
            Success = success,
            Failed = failure,
            ops = ops,
            PathCost = pathCost,
            TimeTaken = time,
            ObstacleChance = avg[0].ObstacleChance,
            NumberOfNodesExpanded = NumbOfNodes,
            PosOps = succOps,
            PosNumbNodes = succNumbOfNodes,
            PosPathCost = succPathCost,
            PosTime = succTime,
            Variance = Variance,
            StandardDeviation = StandardDeviation,
            Max = succCurrentMaxOps,
            Min = succCurrentMinOps,
        };
    }

    public AvgClass CalculatePositiveAverage(List<AvgClass> avg) {
        // Variance and standard deviation
        int successCounter = 0, succOps = 0, succPathCost = 0, succNumbOfNodes = 0, succCurrentMin = int.MaxValue, succCurrentMax = int.MinValue, SumOps=0;
        long succTime = 0;
        foreach (AvgClass x in avg)
        {
            if (x.Success == 1)
            {
                successCounter++;
                succNumbOfNodes = x.NumberOfNodesExpanded;
                succOps = x.ops;
                SumOps += x.ops;
                succPathCost = x.PathCost;
                succTime = x.TimeTaken;
                if (x.ops < succCurrentMin) succCurrentMin = x.ops;
                if (x.ops > succCurrentMax) succCurrentMax = x.ops;
            }
        }
        // Average
        succNumbOfNodes = succNumbOfNodes / successCounter;
        succOps = succOps / successCounter;
        succPathCost = succPathCost / successCounter;
        succTime = succTime/successCounter;
        // Variance and standard deviation
        double Sigma = 0;
        foreach (AvgClass x in avg) {
            if (x.Success == 1) {
                Sigma += Math.Pow(x.ops - succOps, 2);
            }
        }
        double Variance = Math.Sqrt(Sigma / successCounter);
        double StandardDeviation = Math.Sqrt(Variance);
        // End Of Variance
        return new AvgClass { HeapType = avg[0].HeapType, Success = successCounter, ops = succOps, PathCost = succPathCost, TimeTaken = succTime, ObstacleChance = avg[0].ObstacleChance, NumberOfNodesExpanded = succNumbOfNodes , Variance = Variance, StandardDeviation = StandardDeviation, Max = succCurrentMax,Min = succCurrentMin };
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(1);
    }

    ResultsClass FindPath(Vector3 startPos, Vector3 targetPos)
    {
        ResultsClass FindPathResults;
        int ops = 0;
        int NodeExpanded = 0;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(startNode);
        NodeExpanded++;
        ops++;

        while (openSet.Count > 0)
        {
            ops++;
            NodeExpanded++;
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {//if the node in the openset[i] has a fcost less than the current node fcost, or if current node fcost has the same weight and its hcost is less than our current node we swap the current node with the new node!
             //each loop is a check operation here for the node with the minimum value of fcost. 
                ops++;
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    ops++;
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);
            ops += 2;

            if (currentNode == targetNode)
            {   //if the goal is found process will stop and the path is found!
                ops++;
                sw.Stop();
                Tuple<int, int> RetracePathResult = RetracePath(startNode, targetNode);
                //print("Find path found the path in: " + sw.ElapsedMilliseconds + ". Number of Ops: " + ops + ". Number of Nodes Expanded: " + NodeExpanded);
                //print("Path Cost: " + RetracePathResult.Item1 + " and Path Length: " + RetracePathResult.Item2);
                return FindPathResults = new ResultsClass() { ObstacleChance = grid.ObstacleChance, HeapType = "Unsorted Array", Success = true, Time = sw.ElapsedMilliseconds, Ops = ops, PathCost = RetracePathResult.Item1, PathLength = RetracePathResult.Item2, NodesExpanded = NodeExpanded };
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                ops++;
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                { // in this if statement if the neighbour node is not walkable or is already in the closed list we skip ahead into the next node!
                    ops++;
                    continue;
                }
                //calculating the gcost to each neighbour in the neighbour list!
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //in this if bracket we check if the new path to neighbour is shorter or neighbour is not in the open set
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour; //setting the gcost for neighbour!
                    neighbour.hCost = GetDistance(neighbour, targetNode); //calculating the hcost to the target node from the current node!
                    neighbour.parent = currentNode; //setting each node's parent to the current neighbour!
                    if (!openSet.Contains(neighbour)) // if the neighbour node is not in the open set we will add it to the open set in this if bracket!
                    {
                        openSet.Add(neighbour);
                        ops++;
                    }
                }
            }
        }
        sw.Stop();
        return FindPathResults = new ResultsClass() { ObstacleChance = grid.ObstacleChance, HeapType = "Unsorted Array", Success = false, Time = sw.ElapsedMilliseconds, Ops = ops, PathCost = int.MaxValue, PathLength = int.MaxValue, NodesExpanded = NodeExpanded };
    }

    ResultsClass FindPathHeap(Vector3 startPos, Vector3 targetPos)
    {
        ResultsClass FindPathResults = new ResultsClass();
        Stopwatch sw = new Stopwatch();
        List<int> openSetValues;
        int NodeExpanded = 0;
        sw.Start();
        
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // creating an array of heap with the max size of the grid!
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        //adding the first element to the heap!
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            NodeExpanded++;
            // removing the minimum node and adding this node to the open set!
            Node currentNode = openSet.RemoveFirst();
            //currentNode.flag = "removed";
            closedSet.Add(currentNode);
           
            if (currentNode == targetNode)
            {
                //currentNode.flag = "Goal Node";
                sw.Stop();
                Tuple<int,int> RetracePathResult = RetracePath(startNode, targetNode,true);
                openSetValues = openSet.HeapValues();
                //print("Find Path heap found the path in: " + sw.ElapsedMilliseconds + " Open Set Count: " + openSet.Count + "--- path Cost" + RetracePathResult.Item1 + "--- Path Size" + RetracePathResult.Item2);

                return FindPathResults = new ResultsClass() {ObstacleChance = grid.ObstacleChance, HeapType = "Heap", Success = true, Time = sw.ElapsedMilliseconds, Ops = openSetValues[6], PathCost = RetracePathResult.Item1, PathLength = RetracePathResult.Item2, NodesExpanded = NodeExpanded
                ,MainInsertCount = openSetValues[0], RemoveMinCount = openSetValues[1],ContainCount = openSetValues[3],UpdateItemCount = openSetValues[2],SortDownCount = openSetValues[4],SortUpCount = openSetValues[5]};
                
                //print("Open Set Values were for add: " + (openSetValues[0]) + ". Values for RemoveFirst was: " + openSetValues[1] + ". Values for Update Item was: " + openSetValues[2] +".\nsort down counts: " + openSetValues[4] + ". sort up count was: " +openSetValues[5]);
                //calling Priting methode for the close list!
                //DisplayItterationHeap(ItterationsOpenSet);
                //DisplayHeapCloseList(closedSet);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {   // in this if statement if the neighbour node is not walkable or is already in the closed list we skip ahead into the next node!
                    continue;
                }

                //calculating the gcost to each neighbour in the neighbour list!
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        neighbour.flag = "Neighbour Insert";
                        //ItterationsOpenSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                        neighbour.flag = "Gcost update";
                        //ItterationsOpenSet.Add(neighbour);
                    }
                }
            }
        }
        sw.Stop();
        openSetValues = openSet.HeapValues();
        return FindPathResults = new ResultsClass() { ObstacleChance = grid.ObstacleChance, HeapType = "Heap", Success = false, Time = sw.ElapsedMilliseconds, Ops = openSet.HeapValues()[6], NodesExpanded = NodeExpanded
                ,MainInsertCount = openSetValues[0], RemoveMinCount = openSetValues[1],ContainCount = openSetValues[3],UpdateItemCount = openSetValues[2],SortDownCount = openSetValues[4],SortUpCount = openSetValues[5]};
    }

    ResultsClass FindPathPartialHeap(Vector3 startPos, Vector3 targetPos,int depth)
    {
        ResultsClass FindPathResults = new ResultsClass();
        Stopwatch sw = new Stopwatch();
        List<int> openSetValues;
        int nodeExpanded = 0;
        sw.Start();
        
        int partialMinHeapSize = (int)Math.Pow(2, depth) - 1;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // creating an array of heap with the max size of the grid!
        PartialMinHeap<Node> openSet = new PartialMinHeap<Node>(partialMinHeapSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        //adding the first element to the heap!
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // removing the minimum node and adding this node to the open set!
            Node currentNode = openSet.Remove();
            currentNode.flag = "removed";
            closedSet.Add(currentNode);
            nodeExpanded++;

            if (currentNode == targetNode)
            {
                sw.Stop();
                openSetValues = openSet.PartialMinHeapValues();
                Tuple<int, int> RetracePathResults = RetracePath(startNode, targetNode, false);
                return FindPathResults = new ResultsClass()
                {
                    ObstacleChance = grid.ObstacleChance,
                    HeapType = "Partial Min Heap",
                    Success = true,
                    Time = sw.ElapsedMilliseconds,
                    Ops = openSetValues[6],
                    PathCost = RetracePathResults.Item1,
                    PathLength = RetracePathResults.Item2,
                    NodesExpanded = nodeExpanded,
                    MainInsertCount = openSetValues[0],
                    KickedCount = openSetValues[1],
                    RemoveMinCount = openSetValues[4],
                    ContainCount = openSetValues[8],
                    UpdateItemCount = openSetValues[7],
                    SortDownCount = openSetValues[2],
                    SwapCount = openSetValues[5],
                    SortUpCount = openSetValues[3]
                };
                //print("Find Path Partial heap found the path in: " + sw.ElapsedMilliseconds + " Open Set Count: " + openSet.Count + "--- PathFcost: " + RetracePathResults.Item1 + " --- Length: " + RetracePathResults.Item2);
                //print("Open Set Values were for add: " + (openSetValues[0]) + ". Values for RemoveFirst was: " + openSetValues[4] + ". Values for Discarded: " + openSetValues[1] + ". Values for SwapCount: " + openSetValues[5] + ".\nsort down counts: " + openSetValues[2] + ". sort up count was: " + openSetValues[3]);//
                ////calling Priting methode for the close list
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                nodeExpanded++;
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {   // in this if statement if the neighbour node is not walkable or is already in the closed list we skip ahead into the next node!
                    continue;
                }

                //calculating the gcost to each neighbour in the neighbour list!
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        sw.Stop();
        openSetValues = openSet.PartialMinHeapValues();
        return FindPathResults = new ResultsClass()
        {
            ObstacleChance = grid.ObstacleChance,
            HeapType = "Partial Min Heap",
            Success = false,
            Time = sw.ElapsedMilliseconds,
            Ops = openSetValues[6],
            NodesExpanded = nodeExpanded,
            MainInsertCount = openSetValues[0],
            KickedCount = openSetValues[1],
            RemoveMinCount = openSetValues[4],
            ContainCount = openSetValues[8],
            UpdateItemCount = openSetValues[7],
            SortDownCount = openSetValues[2],
            SwapCount = openSetValues[5],
            SortUpCount = openSetValues[3]
        };
    }

    ResultsClass FindPathCachedMinHeap(Vector3 startPos, Vector3 targetPos, int depth,int cacheDepth)
    {
        ResultsClass FindPathResults = new ResultsClass();
        List<int> openSetValues = new List<int>();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        int nodeExpanded=0;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (depth == 0 && cacheDepth == 0) {
            depth = 10;
            cacheDepth = 7;
            print("Depth not assigned!");
        }

        int numberOfNodes = (int)Math.Pow(2, depth) - 1;
        int numberOfCacheNodes = (int)Math.Pow(2, cacheDepth) - 1;

        //creating an array of heap with the max size of the grid!
        CachedMinHeap<Node> openSet = new CachedMinHeap<Node>(numberOfCacheNodes, numberOfNodes );
        HashSet<Node> closedSet = new HashSet<Node>();

        //adding the first element to the heap!
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // removing the minimum node and adding this node to the open set!
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            nodeExpanded++;

            if (currentNode == targetNode)
            {
                sw.Stop();
                //currentNode.flag = "Goal Node";
                Tuple<int,int> RetracePathResults = RetracePath(startNode, targetNode,true);
                openSetValues = openSet.cacheMinHeapValues();

                //print("Find Path CachedMinheap found the path in: " + sw.ElapsedMilliseconds + " Open set count: "+ openSet.cacheCountValue());
                //(MainAddCount, discardCount, SortDownCount, SortUpCount, RemoveFirstCount,ops)
                //print("Open Set Values were for add: " + (openSetValues[0]) + ". Values for RemoveFirst was: " + openSetValues[4]+ ". Values for Discarded: " + openSetValues[1] + ". Values for SwapCount: " + openSetValues[5] + ".\nsort down counts: " + openSetValues[2] + ". sort up count was: " + openSetValues[3]);//
                //calling Priting methode for the close list!

                return FindPathResults = new ResultsClass()
                {
                    ObstacleChance = grid.ObstacleChance,
                    HeapType = "Cached Min Heap",
                    Success = true,
                    Time = sw.ElapsedMilliseconds,
                    Ops = openSetValues[5],
                    PathCost = RetracePathResults.Item1,
                    PathLength = RetracePathResults.Item2,
                    NodesExpanded = nodeExpanded,
                    MainInsertCount = openSetValues[0],
                    ReserveInsertCount = openSetValues[1],
                    RemoveMinCount = openSetValues[6],
                    UpdateItemCount = openSetValues[7],
                    SortDownCount = openSetValues[3],
                    SwapCount = openSetValues[2],
                    SortUpCount = openSetValues[4],
                    ContainCount = openSetValues[8]
                };
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                nodeExpanded++;
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {   // in this if statement if the neighbour node is not walkable or is already in the closed list we skip ahead into the next node!
                    continue;
                }

                //calculating the gcost to each neighbour in the neighbour list!
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        // Path not Found.
        sw.Stop();
        openSetValues = openSet.cacheMinHeapValues();
        return FindPathResults = new ResultsClass()
        {
            ObstacleChance = grid.ObstacleChance,
            HeapType = "Cached Min Heap",
            Success = false,
            Time = sw.ElapsedMilliseconds,
            Ops = openSetValues[5],
            NodesExpanded = nodeExpanded,
            MainInsertCount = openSetValues[0],
            ReserveInsertCount = openSetValues[1],
            RemoveMinCount = openSetValues[6],
            UpdateItemCount = openSetValues[7],
            SortDownCount = openSetValues[3],
            SwapCount = openSetValues[2],
            SortUpCount = openSetValues[4],
            ContainCount = openSetValues[8]
        };

    }

    void FindPathLazyHeap(Vector3 startPos, Vector3 targetPos) {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        int nodeExpanded = 0;
        //Itteration counter!
        List<Node> ItterationsOpenSet = new List<Node>();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // creating an array of heap with the max size of the grid!
        LazyHeap<Node> openSet = new LazyHeap<Node>(510,7);
        HashSet<Node> closedSet = new HashSet<Node>();

        //adding the first element to the heap!
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {

            // removing the minimum node and adding this node to the open set!
            Node currentNode = openSet.RemoveFirst();
            currentNode.flag = "removed";
            ItterationsOpenSet.Add(currentNode);
            closedSet.Add(currentNode);
            nodeExpanded++;

            if (currentNode == targetNode)
            {
                currentNode.flag = "Goal Node";
                ItterationsOpenSet.Add(currentNode);
                RetracePath(startNode, targetNode);
                sw.Stop();
                //print(nodeExpanded);
                List<int> openSetValues = openSet.LazyHeapValues();
                print("Find Path Lazy-heap found the path in: " + sw.ElapsedMilliseconds);
                print("Open Set Values were for add: " + (openSetValues[0]) + ". Values ReserveInsert: " + openSetValues[1] +". Values for removeFirst: " + openSetValues[2] + ". Values for Update Item was: " + openSetValues[3] + ". sort up count was: " +openSetValues[6]);
                //calling Priting methode for the close list!
                //DisplayItterationHeap(ItterationsOpenSet);
                //DisplayHeapCloseList(closedSet);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                nodeExpanded++;
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {   // in this if statement if the neighbour node is not walkable or is already in the closed list we skip ahead into the next node!
                    continue;
                }

                //calculating the gcost to each neighbour in the neighbour list!
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        neighbour.flag = "Neighbour Insert";
                        ItterationsOpenSet.Add(neighbour);
                    }
                    else
                    {
                        //openSet.UpdateItem(neighbour);
                        //neighbour.flag = "Gcost update";
                        //ItterationsOpenSet.Add(neighbour);
                    }
                }
            }
        }
    }

    Tuple<int,int> RetracePath(Node startNode, Node endNode)
    { // this method will retrace the path from the start node to the goal node!
        List<Node> path = new List<Node>(); // memory for the path array
        Node currentNode = endNode;
        
        while (currentNode != startNode)
        {
            path.Add(currentNode); // adding the current node to the path memory
            currentNode = currentNode.parent; // setting current node's parent to the current node to reverse the path
        }

        path.Reverse(); // since the path is reverse, we have to reverse the path to reach the correct path!

        grid.path = path; // passing the path memory to the grid class for visualization!
        Tuple<int, int> Result = new Tuple<int, int>(endNode.fCost, path.Count);
        return Result;
    }

    Tuple<int, int> RetracePath(Node startNode, Node endNode,bool flag)
    { // this method will retrace the path from the start node to the goal node!
        if (flag == true)
        {
            List<Node> path = new List<Node>(); // memory for the path array
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode); // adding the current node to the path memory
                currentNode = currentNode.parent; // setting current node's parent to the current node to reverse the path
            }
            path.Reverse(); // since the path is reverse, we have to reverse the path to reach the correct path!
            grid.pathx = path; // passing the path memory to the grid class for visualization!
            Tuple<int, int> Result = new Tuple<int, int>(endNode.fCost, path.Count);
            return Result;
        }
        else {
            List<Node> path = new List<Node>(); // memory for the path array
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode); // adding the current node to the path memory
                currentNode = currentNode.parent; // setting current node's parent to the current node to reverse the path
            }
            path.Reverse(); // since the path is reverse, we have to reverse the path to reach the correct path!
            grid.PathPartialHeap = path; // passing the path memory to the grid class for visualization!
            Tuple<int, int> Result = new Tuple<int, int>(endNode.fCost, path.Count);
            return Result;
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {   //This method will return the distance between two each node!
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);

        }
        return 14 * distX + 10 * (distY - distX);
    }

    void DisplayItteration(List<Node> Itt)
    {

        if (printItterations)
        {
            var time = System.DateTime.Now;
            //format the time when you retrieve it, not when you store it - i.e. -
            string formattedtime = time.ToString("yyyy-mm-dd-hh-mm-ss");
            string path = "Assets/Test/" + formattedtime + " Itterations!.txt";
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("This is the Path for the file: {0}\n", path);
                // Add some text to the file.
                sw.WriteLine("-------------------");
                sw.WriteLine("This is a newly generated File!");
                // Arbitrary objects can also be written to the file.
                sw.Write("The date is: ");
                sw.WriteLine(System.DateTime.Now);
                foreach (Node n in Itt)
                {
                    sw.WriteLine("for thie node at grid[{0},{1}], fcost was {2} = {3} + {4} , Itteration for this node was {5}", n.gridX, n.gridY, n.fCost,n.gCost,n.hCost, n.flag);
                }
            }
        }
    }
  }

