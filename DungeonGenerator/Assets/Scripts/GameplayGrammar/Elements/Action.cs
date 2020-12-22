using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Action : GameplayElement
{
    private List<Entity> _entitiesThisCanBePerformedOn = new List<Entity>();

    public Action(string name) : base(name)
    {
    }

    public string[] GetEntityNames()
    {
        string[] names = new string[_entitiesThisCanBePerformedOn.Count];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = _entitiesThisCanBePerformedOn[i].Name;
        }

        return names;
    }
    public void AddEntityToPerformOn(Entity entity)
    {
        if (!_entitiesThisCanBePerformedOn.Contains(entity))
            _entitiesThisCanBePerformedOn.Add(entity);
    }
    public void RemoveEntityToPerformOn(Entity entity)
    {
        _entitiesThisCanBePerformedOn.Remove(entity);
    }
    public bool ContainsEntity(Entity entity)
    {
        return _entitiesThisCanBePerformedOn.Contains(entity);
    }

    public int GetAmountOfEntities()
    {
        return _entitiesThisCanBePerformedOn.Count;
    }
}
