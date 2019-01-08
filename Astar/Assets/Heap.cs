using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;


public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    //keeping the values 
    int addCount;
    int removeFirstCount;
    int updateItemCount;
    int containsCount;
    int sortDownCount;
    int sortUpCount;
    int heapSize;
    int ops;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
        heapSize = maxHeapSize;
    }


    /// <summary>
    /// This method will the heap Size for each Heap.
    /// </summary>
    public int HeapSize()
    {
        return heapSize;
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
        addCount++;
        ops++;
    }

    public T getItem(int HeapIndex)
    {
        T thistestitem = items[HeapIndex];
        return thistestitem;
    }

    public T RemoveFirst()
    {   //returning the min item swaping the last item with the item[0] and running sort down to find the correct place for the node!
        ops++;
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        removeFirstCount++;
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
        updateItemCount++;
        ops++;
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        containsCount++;
        ops++;
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while (true)
        {
            ops++;
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            sortDownCount++;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
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
                ops++;
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
            sortUpCount++;
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

    /// <summary>
    /// This method will return a List of int that has the following values [addCount, removeFirstCount,updateItemCount, containsCount, sortDownCount, sortUpCount] that are values regarded the heap!
    /// </summary>
    /// <returns></returns>
    public List<int> HeapValues()
    {
        List<int> heapValues = new List<int>() { addCount, removeFirstCount, updateItemCount, containsCount, sortDownCount, sortUpCount,ops };
        //Console.WriteLine();
        return heapValues;
    }

    /// <summary>
    /// This method is a test method!
    /// </summary>
    public void Shout()
    {
        Console.WriteLine("Im here bois!");
        //Console.ReadKey();
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
