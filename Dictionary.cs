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
            None,               //한번도 사용된적 없음
            Using,              //사용 중
            Deleted             //지워진 곳
        }
        public State state;
        public Tkey key;
        public Tvalue value;
    }
    private const int defualtCapacity = 100;
    private Entry[] tables;                         //table 배열 생성
    private int count;                              //갯수

    public Tvalue this[Tkey key]
    {
        get
        {
            if (Find(key, out int index))
                return tables[index].value;
            else
                throw new KeyNotFoundException();  //없으니까 에러임
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
            //Dictionary는 key의 중복이 없어야 함
            throw new InvalidOperationException("이미 존재하는 key");
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
            return false;   //key가 없으므로
        }
    }
    public bool ContainsKey(Tkey key)   //key가 있는지 확인
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
            if (tables[index].state == Entry.State.None)    //인덱스가 안쓰는 곳이라면
            {
                return false;
            }
            else if (tables[index].state == Entry.State.Using && key.Equals(tables[index].key)) //인덱스가 사용중이고, 키 값이 일치하면
            {
                return true;
            }
            else
            {
                index = Hash2(index);
            }
        }
        index = -1;
        throw new Exception();  //해당하지 않으므로 찾을 수 없음
    }
    int Hash(Tkey key)      //key값으로 hashing을 통해 절대값의 index로 치환
    {
        //나눗셈인데 key값을 hashing한 hash코드를 tables의 길이로 나눔
        return Math.Abs(key.GetHashCode() % tables.Length);
    }
    int Hash2(int index)    //다음 위치로 이동
    {
        return (index+1) % tables.Length;
    }
    void ReHashing()    //성능 저하 방지용 함수, 사용율 70%이상 시 진행
    {
        Entry[] oldTables = tables;
        tables = new Entry[tables.Length * 2];
        count = 0;
        for(int i=0; i<oldTables.Length; i++)
        {
            Entry entry = oldTables[i];
            if(entry.state == Entry.State.Using)    //사용중인 값은 이사
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
