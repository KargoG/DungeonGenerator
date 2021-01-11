using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Entity : GameplayElement
{
    public Entity(string name) : base(name)
    {
    }

    private GameObject _representation = null;
    public GameObject Representation
    {
        get { return _representation; }
        set { _representation = value; }
    }

    //public string[] GetActionNames()
    //{
    //    string[] names = new string[_actionsThatCanBePerformedOnThis.Count];
    //    for (int i = 0; i < names.Length; i++)
    //    {
    //        names[i] = _actionsThatCanBePerformedOnThis[i].Name;
    //    }

    //    return names;
    //}
    //public void AddActionThatCanPerformOnThis(Action action)
    //{
    //    if (!_actionsThatCanBePerformedOnThis.Contains(action))
    //        _actionsThatCanBePerformedOnThis.Add(action);
    //}
    //public void RemoveActionThatCanPerformOnThis(Action action)
    //{
    //    _actionsThatCanBePerformedOnThis.Remove(action);
    //}
    //public bool ContainsAction(Action action)
    //{
    //    return _actionsThatCanBePerformedOnThis.Contains(action);
    //}
}
