using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Simple pool with events used for certain components
/// </summary>
/// <typeparam name="T">Type of the pooled object</typeparam>
public class ObjectPool<T> where T : new()
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }

    public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = new T();
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}

/// <summary>
/// Pooled list based on ObjectPool
/// </summary>
/// <typeparam name="T"></typeparam>
static class ListPool<T>
{
    // Object pool to avoid allocations.
    private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }
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