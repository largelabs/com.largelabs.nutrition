using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class pooler : MonoBehaviour
{
}




/// <summary>
/// Implement this interface on objects that will be pools in a managed pool
/// </summary>
public interface IPoolable : IDisposable
{
    void Reset();
}


/// <summary>
/// Pool that can be used for any type of object that implements IPoolable
/// </summary>
/// <typeparam name="T">IPoolable object</typeparam>
public class ManagedPool<T> : IDisposable where T : class, IPoolable, new()
{
    

    // Pool collections
    List<T> available = new List<T>();
    Dictionary<T, T> used = new Dictionary<T, T>();
    [SerializeField] private int maxCount;
    // CTOR
    public ManagedPool(int maxCount)   // -1: Unlimited by default
    {
        MaxCount = maxCount;
    }

    #region PUBLIC API

    public int AvailableCount { get { return available.Count; } }

    public int UsedCount { get { return used.Count; } }

    public int TotalCount { get { return AvailableCount + UsedCount; } }

    public int MaxCount { get; private set; }

    public bool IsFull { get { return TotalCount == MaxCount; } }

    public T GetItem()
    {
        if (available.Count > 0)
        {
            T pooledItem = available[0];
            available.RemoveAt(0);
            used.Add(pooledItem, pooledItem);


            return pooledItem;
        }

        if (IsFull) return null;

        T user = new T();
        used.Add(user, user);

        return user;
    }

    public void ResetAll()
    {
        

        foreach (KeyValuePair<T, T> pair in used)
        {
            pair.Value.Reset();
            available.Add(pair.Value);
        }

        used.Clear();

      
    }

    public List<T> ResetAll(Func<T, bool> canResetItemFunction)
    {
        List<T> removedItems = new List<T>();

        if (canResetItemFunction == null) return removedItems;

    

        foreach (KeyValuePair<T, T> pair in used)
        {
            if (canResetItemFunction(pair.Value))
            {
                pair.Value.Reset();
                available.Add(pair.Value);

                removedItems.Add(pair.Value);
            }
        }

        for (int i = 0; i < removedItems.Count; i++)
            used.Remove(removedItems[i]);

       

        return removedItems;
    }

    public void Dispose()
    {
        

        foreach (KeyValuePair<T, T> pair in used) pair.Value.Dispose();
        for (int i = 0; i < available.Count; i++) available[i].Dispose();

        used.Clear();
        available.Clear();

       
    }

    public void DisposeItem(T item)
    {
    

        // Processes the item only if it was removed from used.
        // Otherwise, the object is already available, or does not belong to the pool

        if (used.Remove(item))
        {
            item.Dispose();
        }
    }

    public void ResetItem(T item)
    {
        

        // Processes the item only if it was removed from used.
        // Otherwise, the object is already available, or does not belong to the pool

        if (used.Remove(item))
        {
            item.Reset();
            if (null == item)
            {
                UnityEngine.Debug.LogError("pool item is added null");
            }

            available.Add(item);
        }
    }

    public void Preload(int i_count)
    {
        if (i_count < 1)
            return;

        for (int i = 0; i < i_count; i++)
        {
            if (IsFull)
                return;

            T item = new T();
            available.Add(item);
        }
    }

    #endregion
}