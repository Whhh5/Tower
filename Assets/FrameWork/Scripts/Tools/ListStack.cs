using System.Collections;
using System.Collections.Generic;
using B1;
using UnityEngine;
using System;

public enum ESortType
{
    None = -1,
    Forward = 0,
    Back = 1,
}

[Serializable]
public class ListQueue<T> : Base
{
    private string m_Message = null;

    public ListQueue(string f_Message, int count = 10)
    {
        count = count < 10 ? 10 : count;
        m_List = new(new T[count]);
        m_AddCount = count;
        m_EndP = 0;
        m_StartP = m_List.Count - 1;
        m_Message = f_Message;
    }

    public T this[int key]
    {
        get { return m_List[key]; }
    }

    [SerializeField] private List<T> m_List;
    [SerializeField] private int m_StartP = 0;
    [SerializeField] private int m_EndP = 0;
    [SerializeField] private int m_AddCount = 0;

    private int ListCount => m_List.Count;
    public int Count => m_EndP - m_StartP < 0 ? m_List.Count + (m_EndP - m_StartP) - 1 : m_EndP - m_StartP - 1;

    

    public bool TryPop(out T value)
    {
        value = default;

        var targetP = (m_StartP + 1) % ListCount;
        if (targetP == m_EndP) return false;

        value = m_List[targetP];
        m_StartP = targetP;
        return true;
    }

    public void Push(T item)
    {
        m_List[m_EndP] = item;
        m_EndP = (m_EndP + 1) % m_List.Count;
        
        var targetP = (m_EndP + 1) % m_List.Count;
        if (targetP == m_StartP)
        {
            Extend(true);
        }
    }

    public void Extend(bool f_Force = false)
    {
        var newList = new List<T>(new T[ListCount + m_AddCount]);
        for (var i = 0; i < ListCount; i++)
        {
            var index = (m_StartP + i + 1) % ListCount;
            newList[i] = m_List[index];
        }
        m_List = newList;
    }

    public bool Contains(T f_Item)
    {
        return m_List.Contains(f_Item);
    }
}

[Serializable]
public class ListStack<T> : Base
{
    private string m_Message = null;

    public ListStack(string f_Message = "", int count = 10, Func<T, T, bool> sort = null,
        ESortType sortTyoe = ESortType.Forward)
    {
        count = count <= 0 ? 1 : count;
        m_List = new List<T>(new T[count]);
        m_AddCount = count;
        m_Pointer = 0;
        m_Message = f_Message;
        m_Sort = sort;
        m_SortType = sortTyoe;
    }

    [SerializeField] private List<T> m_List = new();
    [SerializeField] private int m_Pointer = 0;
    [SerializeField] private int m_AddCount = 0;
    Func<T, T, bool> m_Sort = null;
    ESortType m_SortType = ESortType.None;
    public int Count => m_Pointer;


    public T this[int key]
    {
        get { return m_List[key]; }
    }

    public bool TryValue(out T f_Value)
    {
        f_Value = default(T);
        if (m_Pointer > 0)
        {
            f_Value = m_List[m_Pointer - 1];
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryPop(out T value)
    {
        bool isTry = false;
        value = default(T);
        if (m_Pointer > 0)
        {
            value = m_List[(int)--m_Pointer];
            m_List[(int)m_Pointer] = default(T); //.Remove(value);
            isTry = true;
        }

        return isTry;
    }

    public void Push(T item)
    {
        var type = m_SortType == ESortType.Back;
        var insertIndex = m_Pointer;
        if (m_Sort != null)
        {
            insertIndex = type ? 0 : insertIndex;
            for (int i = 0; i < m_Pointer; i++)
            {
                int index = 0;
                bool condition = false;
                switch (m_SortType)
                {
                    case ESortType.None:
                        break;
                    case ESortType.Forward:
                    {
                        index = i;
                        condition = m_Sort.Invoke(item, m_List[index]);
                    }
                        break;
                    case ESortType.Back:
                    {
                        index = m_Pointer - i - 1;
                        condition = !m_Sort.Invoke(item, m_List[index]);
                        index++;
                    }
                        break;
                    default:
                        break;
                }

                if (condition)
                {
                    insertIndex = index;
                    break;
                }
            }
        }

        Insert(insertIndex, item);
    }

    public void Extend(bool f_Froce = false)
    {
        if (m_Pointer < m_List.Count || f_Froce) return;
        var newList = new List<T>(new T[(uint)m_List.Count + m_AddCount]);
        for (int i = 0; i < m_List.Count; i++)
        {
            newList[i] = m_List[i];
        }

        m_List = newList;
    }

    public bool Contains(T f_Item)
    {
        return m_List.Contains(f_Item);
    }

    public int IndexOf(T f_Item)
    {
        return m_List.IndexOf(f_Item);
    }

    public int IndexOf(Func<T, bool> f_Condition)
    {
        for (int i = 0; i < m_Pointer; i++)
        {
            var item = m_List[i];
            if (f_Condition.Invoke(item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int f_Index, T f_Item)
    {
        var index = Mathf.Min(m_Pointer, f_Index);
        m_List[m_Pointer++] = default;
        for (int i = m_Pointer - 1; i > index; i--)
        {
            m_List[i] = m_List[i - 1];
        }

        m_List[index] = f_Item;
        Extend();
    }

    public void LogData()
    {
        string str = $"{m_Message}";
        uint index = 0;
        foreach (var item in m_List)
        {
            str += $"\n[ {index++} ] = {item}";
        }

        Log(str);
    }

    public IEnumerable<KeyValuePair<int, T>> GetEnumerator()
    {
        for (int i = 0; i < m_Pointer; i++)
        {
            var index = i;
            var tempItem = m_List[index];
            yield return new KeyValuePair<int, T>(index, tempItem);
        }
    }
    // 将最后一个元素覆盖需要删除的元素，数组将变为无序，删除速度快
    public bool Remove(T f_Target)
    {
        var index = m_List.IndexOf(f_Target);
        if (index < 0)
        {
            return false;
        }
        else
        {
            if (index != m_Pointer - 1)
            {
                if (TryPop(out var element))
                {
                    m_List[index] = element;
                }
            }
            else
            {
                TryPop(out var element);
            }

            return true;
        }
    }

    // f_Target: 删除对象      f_Callback：移动对象，旧的索引，新的索引， 删除之后数据依旧有序
    public bool Remove2(T f_Target, Action<T, int, int> f_Callback = null)
    {
        var index = m_List.IndexOf(f_Target);
        if (index < 0)
        {
            return false;
        }
        else
        {
            for (int i = index; i < m_Pointer; i++)
            {
                m_List[i] = m_List[i + 1];
                f_Callback?.Invoke(m_List[i], i + 1, i);
            }

            m_Pointer--;

            return true;
        }
    }

    public static List<T> ListStackAdd(ListStack<T> f_List1, ListStack<T> f_List2)
    {
        List<T> newList = new(new T[f_List1.Count + f_List2.Count]);

        for (int i = 0; i < f_List1.Count; i++)
        {
            newList[i] = f_List1[i];
        }

        for (int i = 0; i < f_List2.Count; i++)
        {
            newList[(int)(f_List1.Count + i)] = f_List2[i];
        }

        return newList;
    }
}

public class DicStack<TKey, TValue> : Base
{
    public TValue this[TKey key]
    {
        get { return m_Dic[key]; }
    }


    private string m_Message = null;

    public DicStack(string f_Message, int count = 10)
    {
        count = count <= 0 ? 1 : count;
        m_KeyStack = new ListStack<TKey>(f_Message, count);
        m_Dic = new();
        m_Message = f_Message;
    }

    private Dictionary<TKey, TValue> m_Dic = null;
    private ListStack<TKey> m_KeyStack = null;
    public int Count => m_KeyStack.Count;

    public bool TryPop(out TValue f_Value)
    {
        bool isTry = false;
        f_Value = default(TValue);
        if (m_KeyStack.TryPop(out var key))
        {
            f_Value = m_Dic[key];
            m_Dic.Remove(key);
            isTry = true;
        }

        return isTry;
    }

    public void Push(TKey f_Key, TValue f_Value)
    {
        if (!m_KeyStack.Contains(f_Key) || m_Dic.ContainsKey(f_Key))
        {
            m_KeyStack.Push(f_Key);
            m_Dic.Add(f_Key, f_Value);
        }
        else
        {
            Log($"字典列表添加失败 已经存在 键值对  key = {f_Key}   " +
                $"key stack = {m_KeyStack.Contains(f_Key)}  " +
                $"dic data = {m_Dic.ContainsKey(f_Key)}");
        }
    }

    public bool TryGetValue(TKey f_Key, out TValue f_Value)
    {
        bool isTry = false;
        f_Value = default(TValue);
        if (m_KeyStack.Contains(f_Key))
        {
            f_Value = m_Dic[f_Key];
            isTry = true;
        }

        return isTry;
    }

    public bool TryGetTopValue(out TValue f_Value)
    {
        var tryValue = false;
        f_Value = default;
        if (m_KeyStack.TryValue(out var key))
        {
            f_Value = m_Dic[key];
            tryValue = true;
        }

        return tryValue;
    }

    public void LogData()
    {
        string str = $"{m_Message}";
        uint index = 0;
        foreach (var item in m_Dic)
        {
            str += $"\n[ {index++} ] = " +
                   $"\n{{" +
                   $"\n\tkey \t= {item.Key}" +
                   $"\n\tvalue \t= {item.Value}" +
                   $"\n}}";
        }

        Log(str);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var item in m_Dic)
        {
            yield return item;
        }
    }
}