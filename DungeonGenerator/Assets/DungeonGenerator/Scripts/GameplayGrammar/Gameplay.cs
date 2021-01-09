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

    private bool _randomPlacable = true; // TODO add UI to change this value
    public bool RandomPlacable
    {
        get { return _randomPlacable; }
    }

    public static Gameplay CreateGameplay(Action action, Entity entity, Ability ability, Consumable consumable)
    {
        Gameplay createdGameplay = CreateInstance<Gameplay>();
        createdGameplay._action = action;
        createdGameplay._entity = entity;
        createdGameplay._ability = ability;
        createdGameplay._consumable = consumable;

        return createdGameplay;
    }

    public string ToString()
    {
        string asString = Action.Name + " " + Entity.Name;

        if (Ability)
        {
            asString += " " + Ability.Name;
        }
        if (Consumable)
        {
            asString += " " + Consumable;
        }

        return asString;
    }

    private Gameplay()
    {

    }
}
