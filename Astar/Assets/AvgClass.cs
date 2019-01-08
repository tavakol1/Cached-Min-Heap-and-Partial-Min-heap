using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvgClass
{

    public string HeapType { get; set; }
    public int Success { get; set; }
    public int Failed { get; set; }
    public int ops { get; set; }
    public int ObstacleChance { get; set; }

    public int NumberOfNodesExpanded { get; set; }
    public long TimeTaken { get; set; }
    public int PathCost { get; set; }

    //public int PosSucc { get; set; }
    public int PosOps { get; set; }
    public int PosNumbNodes { get; set; }
    public int PosPathCost { get; set; }
    public long PosTime { get; set; }

    public double Variance { get; set; }
    public double StandardDeviation { get; set; }
    public int Max { get; set; }
    public int Min { get; set; }
}
