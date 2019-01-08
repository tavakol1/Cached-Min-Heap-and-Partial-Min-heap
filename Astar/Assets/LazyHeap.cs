using System;
using System.Collections.Generic;


public class LazyHeap<T> where T : ILazyHeapItem<T>
{
    T[] items;
    List<T> Reserves;
    int currentItemCount;
    static int currentDepth;

    //  keeping the Stats values 
    int addCount;
    int swapCount;
    int addReserveCount;
    //int updateItemCount;
    int findMaxHeapCount;
    int minUpdateCount;
    int removeFirstCount;
    int containsCount;
    int sortDownCount;
    int sortUpCount;
    int sortReservesCount;

    static int SecArraySize;
    static int heapSize;
    readonly int GenralSize;
    static Tuple<T, int, int> CurrentMax;
    static Tuple<T, int> CurrentMin;

    public LazyHeap(int _Size, int _depth)
    {
        GenralSize = _Size;
        currentDepth = _depth;
        heapSize = Convert.ToInt32(Math.Pow(2, currentDepth)) - 1;
        SecArraySize = GenralSize - heapSize;
        //Console.WriteLine("Heap Size with depth of {0} is {1} and Secondary Array Size is {2}", _depth, heapSize, SecArraySize);
        items = new T[heapSize];
        Reserves = new List<T>(SecArraySize);
    }

    public void Add(T item)
    {
        if (currentItemCount == 0)
        {   //initiating the Max!
            //Console.WriteLine("Max Got Initiated! Fcost: {0}", item.fCost);
            CurrentMax = new Tuple<T, int, int>(item, item.fCost, currentItemCount);
            CurrentMin = new Tuple<T, int>(item, 999999);
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
            addCount++;
        }
        else if (item.fCost <= CurrentMax.Item2 && item.fCost < CurrentMin.Item2 && currentItemCount < heapSize)
        {   //if the input item is smaller than the current max in the heam, then the Item will be added to the heap!
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
            addCount++;
        }
        else if (item.fCost <= CurrentMax.Item2 && item.fCost < CurrentMin.Item2 && currentItemCount == heapSize)
        {
            //Console.WriteLine("Heap is full item with smaller value => Swap!");
            int tempIndex = CurrentMax.Item1.HeapIndex;
            AddReserves(CurrentMax.Item1);
            item.HeapIndex = tempIndex;
            items[tempIndex] = item;
            SortUp(item);
            CurrentMax = FindMaxHeap();
            addCount++;
            //Console.WriteLine("current Max changed to: {0}",CurrentMax.Item2);
        }
        else
        {
            AddReserves(item);
        }
    }

    public void SortReserves()
    {
        sortReservesCount++;
        Reserves.Sort();
    }

    public void Heapfy(int n)
    {   //saving the Ending point of reserve Item count in case of looping in the structure.
        //Console.WriteLine("Heapfy called!");
        SortReserves();

        List<T> temp = new List<T>();
        int counter = 0;
        for (int i = Reserves.Count - 1; Reserves.Count > 0 && counter < n; i--)
        {
            counter++;
            temp.Add(Reserves[i]);
            Reserves.RemoveAt(i);
        }
        //Console.WriteLine("reserves count:" + Reserves.Count);
        //Console.WriteLine("temp Count: " + temp.Count);
        temp.Sort((a, b) => a.CompareTo(b));
        foreach (var node in temp)
        {
            Add(node);
        }

    }

    public void DisplayDs()
    {
        for (int i = 0; i < currentItemCount; i++)
        {
            Console.WriteLine(items[i].HeapIndex + "--" + items[i].fCost);
        }
        Console.WriteLine("---");
        for (int i = 0; i < Reserves.Count; i++)
        {
            Console.WriteLine(i + "--" + Reserves[i].fCost);
        }
        Console.WriteLine("End of Display!\n");
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        removeFirstCount++;
        if (currentItemCount == 0 && Reserves.Count > 0)
        {
            Heapfy(10);
        }
        //if (currentItemCount == 0 && Reserves.Count == 0)
        //{
        //    Console.WriteLine("Data Structure Is empty!");
        //}
        return firstItem;
    }

    public void AddReserves(T item)
    {
        addReserveCount++;
        Reserves.Add(item);
        if (Reserves.Count == 1)
        {
            //Console.WriteLine("Min Got Initiated! Fcost: {0}", item.fCost);
            CurrentMin = new Tuple<T, int>(item, item.fCost);
        }
        else if (item.fCost < CurrentMin.Item2)
        {
            minUpdateCount++;
            //Console.WriteLine("Min Got Updated! Fcost: {0}, Previous Min Fcost: {1}, current Max Fcost: {2}", item.fCost, CurrentMin.Item2,CurrentMax.Item2);
            CurrentMin = new Tuple<T, int>(item, item.fCost);
        }

    }

    public Tuple<T, int, int> FindMaxHeap()
    {
        findMaxHeapCount++;
        int MaxFCost = 0;
        int TempIndex = 0;
        //For a depth of d left most child position in the array is lMchild = (2^d-1)-1 
        for (int Child = currentItemCount / 2; Child < currentItemCount; Child++)
        {
            if (items[Child].fCost > MaxFCost)
            {
                MaxFCost = items[Child].fCost;
                TempIndex = Child;
            }
        }
        var Tuple = new Tuple<T, int, int>(items[TempIndex], MaxFCost, TempIndex);
        return Tuple;
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
        if (item.HeapIndex < heapSize)
        {
            if (Equals(items[item.HeapIndex], item))
            {
                return Equals(items[item.HeapIndex], item);
            }
            else if (Equals(Reserves[item.HeapIndex], item))
            {
                return Equals(Reserves[item.HeapIndex], item);
            }
        }
        else
        {
            return Equals(Reserves[item.HeapIndex], item);
        }
        containsCount++;
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while (true)
        {
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

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
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
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

    public List<int> LazyHeapValues()
    {
        List<int> LazyHeapValues = new List<int>() { addCount, addReserveCount, removeFirstCount, 0/*updateItemCount*/, containsCount, sortDownCount, sortUpCount,findMaxHeapCount, minUpdateCount, sortReservesCount};
        return LazyHeapValues;
    }
}


public interface ILazyHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
    int fCost
    {
        get;
        //set;
    }

}

