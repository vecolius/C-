using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictionarys<Tkey, Tvalue> where Tkey : IEquatable<Tkey>
{
    private struct Entry
    {
        public enum State 
        { 
            None,               //�ѹ��� ������ ����
            Using,              //��� ��
            Deleted             //������ ��
        }
        public State state;
        public Tkey key;
        public Tvalue value;
    }
    private const int defualtCapacity = 100;
    private Entry[] tables;                         //table �迭 ����
    private int count;                              //����

    public Tvalue this[Tkey key]
    {
        get
        {
            if (Find(key, out int index))
                return tables[index].value;
            else
                throw new KeyNotFoundException();  //�����ϱ� ������
        }
        set
        {
            if(Find(key, out int index))
            {
                tables.[index].value = value;
            }
            else
            {
                if(count > tables.Length * 0.7f)
                {
                    ReHashing();
                }
                tables[index].key = key;
                tables[index].value = value;
                tables[index].state = Entry.State.Using;
                count++;
            }

        }
    }

    public Dictionarys()
    {
        tables = new Entry[defualtCapacity];
        count = 0;
    }
    public void Add(Tkey key, Tvalue value)
    {
        if(Find(key, out int index))
        {
            //Dictionary�� key�� �ߺ��� ����� ��
            throw new InvalidOperationException("�̹� �����ϴ� key");
        }
        else
        {
            tables[index].key = key;
            tables[index].value = value;
            tables[index].state = Entry.State.Using;
            count++;
        }
    }
    public bool Remove(Tkey key)
    {
        if(Find(key, out int index))
        {
            tables[index].state = Entry.State.Deleted;
            return true;
        }
        else
        {
            return false;   //key�� �����Ƿ�
        }
    }
    public bool ContainsKey(Tkey key)   //key�� �ִ��� Ȯ��
    {
        if (Find(key, out int index))
        {
            return true;
        }
        else
            return false;
    }
    bool Find(Tkey key, out int index)
    {
        index = Hash(key);
        for(int i=0; i<tables.Length; i++)
        {
            if (tables[index].state == Entry.State.None)    //�ε����� �Ⱦ��� ���̶��
            {
                return false;
            }
            else if (tables[index].state == Entry.State.Using && key.Equals(tables[index].key)) //�ε����� ������̰�, Ű ���� ��ġ�ϸ�
            {
                return true;
            }
            else
            {
                index = Hash2(index);
            }
        }
        index = -1;
        throw new Exception();  //�ش����� �����Ƿ� ã�� �� ����
    }
    int Hash(Tkey key)      //key������ hashing�� ���� ���밪�� index�� ġȯ
    {
        //�������ε� key���� hashing�� hash�ڵ带 tables�� ���̷� ����
        return Math.Abs(key.GetHashCode() % tables.Length);
    }
    int Hash2(int index)    //���� ��ġ�� �̵�
    {
        return (index+1) % tables.Length;
    }
    void ReHashing()    //���� ���� ������ �Լ�, ����� 70%�̻� �� ����
    {
        Entry[] oldTables = tables;
        tables = new Entry[tables.Length * 2];
        count = 0;
        for(int i=0; i<oldTables.Length; i++)
        {
            Entry entry = oldTables[i];
            if(entry.state == Entry.State.Using)    //������� ���� �̻�
            {
                Add(entry.key, entry.value);
            }
        }
    }
}

public class Dictionary : MonoBehaviour
{
    void Start()
    {
        
    }

}
