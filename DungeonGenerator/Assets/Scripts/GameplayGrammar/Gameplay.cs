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
    private Action _Action;
    private Entity _Entity;
}
