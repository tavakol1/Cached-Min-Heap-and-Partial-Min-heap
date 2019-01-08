using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PartialMinHeap<T> where T : IPartialMinHeapItem<T>
{
    public int Count
    {
        get { return count; }
    }

    public int Capacity
    {
        get { return capacity; }
    }

    private T[] items;
    private int count;
    private int capacity;
    private float currentMax;
    private int currentMaxIndex;
    private float discardThreshold;
    private int discardCount;
    private bool failed;

    public int ops = 0, MainAddCount,swapCount, SortDownCount, SortUpCount, RemoveFirstCount,updateItemCount;
    private int containCount;

    public PartialMinHeap(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Capacity must be greater than zero");
        }
            
        items = new T[capacity];
        this.count = 0; ;
        this.capacity = capacity;
        this.currentMax = float.MinValue;
        this.currentMaxIndex = -1;
        this.discardThreshold = float.MaxValue;
        this.discardCount = 0;
        this.failed = false;
    }

    public void Clear()
    {
        this.count = 0;
        this.currentMax = float.MinValue;
        this.currentMaxIndex = -1;
        this.discardThreshold = float.MaxValue;
        this.discardCount = 0;
        this.failed = false;
        Array.Clear(items, 0, items.Length);
    }

    public void Add(T item)
    {
        if (count < capacity)
        {
            if (item.fCost >= discardThreshold)
            {
                discardCount++;
            }
            else
            {
                if (item.fCost > currentMax)
                {
                    currentMax = item.fCost;
                    currentMaxIndex = count;
                    item.HeapIndex = count;
                    items[count] = item;
                    SortUp(item);
                    ops++; MainAddCount++;
                }
                else
                {
                    item.HeapIndex = count;
                    items[count] = item;
                    ops++; MainAddCount++;
                    SortUp(item);
                }
                count++;
            }
        }
        else
        {
            if (item.fCost >= currentMax)
            {
                if (item.fCost < discardThreshold)
                {
                    discardThreshold = item.fCost;
                }
                discardCount++;
            }
            else
            {
                //T maxItem;
                if (currentMax < discardThreshold)
                {
                    discardThreshold = currentMax;
                }
                item.HeapIndex = currentMaxIndex;
                items[currentMaxIndex] = item;
                SortUp(item);
                T maxItem = FindMax();
                currentMax = maxItem.fCost;
                currentMax = maxItem.HeapIndex;
                swapCount++;
                ops++;
            }
        }
    }

    public T Remove()
    {
        ops++;
        if (this.count == 0)
        {
            failed = true;
            throw new InvalidOperationException("Remove: heap is empty.");
            //name = null;
        }
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        RemoveFirstCount++;
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        ops++;
        SortUp(item);
        updateItemCount++;
    }

    public bool Contains(T item)
    {
        containCount++;
        if (item.HeapIndex < count)
        {
            ops++;
            return Equals(items[item.HeapIndex], item);
        }
        else
        {
            ops++;
            return false;
        }
    }

    public T FindMax()
    {
        int maxIndex = count / 2;
        for (int i = maxIndex + 1; i < count; i++)
        {
            ops++;
            if (items[i].fCost > items[maxIndex].fCost)
            {
                maxIndex = i;
            }
        }
        return items[maxIndex];
    }

    void SortDown(T item)
    {
        while (true)
        {
            ops++;
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            SortDownCount++;

            if (childIndexLeft < count)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < count)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                        ops++;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                    ops++;
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }

        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            ops++;
            SortUpCount++;
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        ops++;
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

    public List<int> PartialMinHeapValues() {
        List<int> heapValues = new List<int>() { MainAddCount, discardCount, SortDownCount, SortUpCount, RemoveFirstCount,swapCount,ops,updateItemCount,containCount };
        return heapValues;
    }

}

public interface IPartialMinHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
    int fCost
    {
        get;
    }
}