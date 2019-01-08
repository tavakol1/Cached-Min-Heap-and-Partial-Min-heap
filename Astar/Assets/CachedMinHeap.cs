using System;
using System.Collections.Generic;
using System.Text;

public class CachedMinHeap<T> where T : ICachedMinHeap<T>
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
    private int cacheCount;
    private int extraCount;
    private int capacity;
    private int cacheCapacity;
    private int extraCapacity;
    private float cacheMax;
    private float extraMin;
    public int MainAddCount, ExtraAddCount, SwapCount, SortDownCount, SortUpCount, RemoveFirstCount,ops,containsCount;
    private int updateItemCount;

    public int cacheCountValue() {
        return cacheCount;
    }

    public List<int> cacheMinHeapValues()
    {
        List<int> cmhValues = new List<int>() { MainAddCount, ExtraAddCount, SwapCount, SortDownCount, SortUpCount , ops,RemoveFirstCount,updateItemCount, containsCount };
        return cmhValues;
    }

    public CachedMinHeap(int cacheCapacity, int capacity)
    {
        if (capacity < 1)
            throw new ArgumentException("Capacity must be greater than zero");

        if (capacity < cacheCapacity)
            throw new ArgumentException("Capacity must be greater than or equal cache capacity");

        items = new T[capacity];
        this.count = 0;
        this.cacheCount = 0;
        this.extraCount = 0;
        this.capacity = capacity;
        this.cacheCapacity = cacheCapacity;
        this.extraCapacity = capacity - cacheCapacity;
        this.cacheMax = float.MinValue;
        this.extraMin = float.MaxValue;
    }

    public void Clear()
    {
        this.count = 0;
        this.cacheCount = 0;
        this.extraCount = 0;
        this.cacheMax = float.MinValue;
        this.extraMin = float.MaxValue;
        Array.Clear(items, 0, items.Length);
    }

    public void Add(T item)
    {

        if (cacheCount < cacheCapacity)
        {
            if (item.fCost >= extraMin)
            {
                item.HeapIndex = cacheCapacity + extraCount;
                items[item.HeapIndex] = item;
                extraCount++;
                ExtraAddCount++;
                ops++;
            }
            else
            {
                if (item.fCost > cacheMax)
                {
                    cacheMax = item.fCost;
                }
                item.HeapIndex = cacheCount;
                items[item.HeapIndex] = item;
                SortUp(item);
                cacheCount++;
                MainAddCount++;
                ops++;
            }
        }
        else
        {
            if (item.fCost >= cacheMax)
            {
                if (item.fCost < extraMin)
                {
                    extraMin = item.fCost;
                }
                item.HeapIndex = cacheCapacity + extraCount;
                items[cacheCapacity + extraCount] = item;
                extraCount++;
                ExtraAddCount++;
                ops++;
            }
            else
            {
                //find max in the cache
                T maxItem = FindMax();
                int tempMax = maxItem.HeapIndex;
                //replace max item with new value
                item.HeapIndex = maxItem.HeapIndex;
                items[maxItem.HeapIndex] = item;
                SortUp(item);
                ops++;
                //add max item to extra
                if (maxItem.fCost < extraMin)
                {
                    extraMin = maxItem.fCost;
                }

                maxItem.HeapIndex = cacheCapacity + extraCount;
                items[cacheCapacity + extraCount] = maxItem;
                extraCount++;
                ExtraAddCount++;
                SwapCount++;
                cacheMax = FindMax().fCost;
                ops++;
            }
        }
        count++;
    }

    public void DisplayDs()
    {
        for (int i = 0; i < cacheCount; i++)
        {
            Console.WriteLine("{0}---{1}", i, items[i].fCost);
        }
        Console.WriteLine("---------");
        for (int i = cacheCount; i < cacheCount + extraCount; i++)
        {
            Console.WriteLine("{0}---{1}", i, items[i].fCost);
        }
    }

    public T FindMax()
    {
        int maxIndex = cacheCount / 2;
        for (int i = maxIndex + 1; i < cacheCount; i++)
        {
            ops++;
            if (items[i].fCost > items[maxIndex].fCost)
            {
                maxIndex = i;
            }
        }
        return items[maxIndex];
    }

    public T FindMin()
    {
        // assumes extra is the lower part of a min heap (after build min heap)
        int minIndex = cacheCapacity;
        int cellsToCheck = 2 * cacheCapacity;
        ops++; 
        if (cellsToCheck > extraCount)
        {
            cellsToCheck = extraCount;
        }
        // first layer of extra is at most 2 * cacheCapacity wide
        for (int i = minIndex; i < cellsToCheck + cacheCapacity; i++)
        {
            ops++;
            if (items[i].fCost < items[minIndex].fCost)
            {
                minIndex = i;
            }
        }
        return items[minIndex];
    }

    public string Peek()
    {
        return Peek(0);
    }

    public string Peek(int index)
    {
        if (this.count == 0)
        {
            throw new InvalidOperationException("Peek: heap is empty.");
        }

        if (index >= this.capacity)
        {
            throw new InvalidOperationException("Peek: index out of range.");
        }
        return Convert.ToString(items[index].fCost);
    }

    public T RemoveFirst()
    {
        RemoveFirstCount++;
        if (this.count == 0)
        {
            throw new InvalidOperationException("Remove: Heap is empty.");
        }
        if (cacheCount == 0)
        {
            ReplenishCache();
        }
        ops++;
        T firstItem = items[0];
        count -= 1;
        cacheCount -= 1;
        items[0] = items[cacheCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        if (item.HeapIndex < cacheCapacity - 1)
        {
            ops++;
            SortUp(item);
            updateItemCount++;
        }
        else if (item.HeapIndex >= cacheCapacity - 1 && item.fCost < cacheMax)
        {
            //we should do something!
            //find max in the cache
            //replace max item with new value
            T maxItem = FindMax();
            items[item.HeapIndex] = maxItem;
            items[maxItem.HeapIndex] = item;
            int tempMax = maxItem.HeapIndex;
            maxItem.HeapIndex = item.HeapIndex;
            item.HeapIndex = tempMax;
            SortUp(item);
            cacheMax = FindMax().fCost;
            extraMin = FindMin().fCost;
        }
    }

    public bool Contains(T item)
    {
        ops++;
        containsCount++;
        return Equals(items[item.HeapIndex], item);
    }

    public void ReplenishCache()
    {//working here
        int moveCount = extraCount < cacheCapacity ? extraCount : cacheCapacity;
        int offset = cacheCapacity + extraCount - 1;

        for (int i = 0; i < moveCount; i++)
        {
            items[offset].HeapIndex = i;
            items[i] = items[offset];
            items[offset] = default(T);
            offset--;
        }
        // Build min heap 
        cacheCount = moveCount;
        extraCount = extraCount < moveCount ? 0 : extraCount - moveCount;
        int n = extraCount > 0 ? cacheCapacity + extraCount : cacheCount;
         
        Quicksort(items, 0, n - 1);
       
        cacheMax = items[cacheCount - 1].fCost;
        extraMin = extraCount > 0 ? items[cacheCapacity].fCost : float.MaxValue;

    }

    public static void Quicksort(T[] elements, int left, int right)
    {
        int i = left, j = right;
        IComparable pivot = elements[(left + right) / 2].fCost;

        while (i <= j)
        {
            while (elements[i].fCost.CompareTo(pivot) < 0)
            {
                i++;
            }

            while (elements[j].fCost.CompareTo(pivot) > 0)
            {
                j--;
            }

            if (i <= j)
            {
                // Swap
                T swapItem = elements[i];
                int tmpIndex = elements[i].HeapIndex;
                elements[i].HeapIndex = elements[j].HeapIndex;
                elements[i] = elements[j];
                elements[j].HeapIndex = tmpIndex;
                elements[j] = swapItem;

                i++;
                j--;
            }
        }

        // Recursive calls
        if (left < j)
        {
            Quicksort(elements, left, j);
        }

        if (i < right)
        {
            Quicksort(elements, i, right);
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

    void SortDown(T item)
    {
        while (true)
        {
            ops++;
            SortDownCount++;
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < cacheCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < cacheCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
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
    
    void Swap(T itemA, T itemB)
    {
        ops++;
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

}



public interface ICachedMinHeap<T> : IComparable<T>
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