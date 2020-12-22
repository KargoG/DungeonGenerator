using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameplayElementTypes
{
    Action,
    Entity,
    Ability,
    Consumable
}

public class Gameplay : ScriptableObject
{
    private Action _action;
    public Action Action { get{ return _action; } }

    private Entity _entity;
    public Entity Entity { get { return _entity; } }

    private Ability _ability;
    public Ability Ability { get { return _ability; } }

    private Consumable _consumable;
    public Consumable Consumable { get { return _consumable; } }
}
