using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsClass {
    public string HeapType { get; set; }
    public bool Success { get; set; }
    public int Ops { get; set; }
    public long Time { get; set; }
    public int PathCost { get; set; }
    public int PathLength { get; set; }
    public Vector2 MapSize { get; set; }
    public int ObstacleChance { get; set; }

    public int NodesExpanded { get; set; }
    public int MainInsertCount { get; set; }
    public int SecondaryInsertCount { get; set; }
    public int KickedCount { get; set; }
    public int SwapCount { get; set; }

    public int UpdateItemCount { get; set; }
    public int SortDownCount { get; set; }
    public int SortUpCount { get; set; }

    public int ReserveInsertCount { get; set; }
    public int RemoveMinCount { get; set; }
    public int ContainCount { get; set; }

    public ResultsClass() {
        this.HeapType = "";
        this.Success = false;
        this.Ops = int.MaxValue;
        this.Time = long.MaxValue;
        this.PathCost = int.MaxValue;
        this.PathLength = int.MaxValue;
        this.MapSize = new Vector2(0,0);
        
        this.NodesExpanded = 0;
        this.MainInsertCount = 0;
        this.SecondaryInsertCount = 0;
        this.KickedCount = 0;
        this.SwapCount = 0;

        this.UpdateItemCount = 0;
        this.SortDownCount = 0;
        this.SortUpCount = 0;

        this.ReserveInsertCount = 0;
        this.RemoveMinCount = 0;
        this.ContainCount = 0;
    }
}
