using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GameplayElement : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name { get{ return _name; } }

    public GameplayElement(string name)
    {
        _name = name;
    }
}
