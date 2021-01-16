using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DungeonGenerator
{
    public enum GameplayElementTypes
    {
        Action,
        Entity,
        Ability,
        Consumable
    }

    [Serializable]
    public class Gameplay : ScriptableObject
    {
        private static Gameplay _empty = null;
        public static Gameplay Empty {
            get
            {
                if (_empty == null) _empty = CreateInstance<Gameplay>();
                return _empty;
            }
        }

        [SerializeField] private Action _action;
        public Action Action
        {
            get { return _action; }
        }

        [SerializeField] private Entity _entity;
        public Entity Entity
        {
            get { return _entity; }
        }

        [SerializeField] private Ability _ability;
        public Ability Ability
        {
            get { return _ability; }
        }

        [SerializeField] private Consumable _consumable;
        public Consumable Consumable
        {
            get { return _consumable; }
        }

        [SerializeField] private bool _randomPlacable = true; // TODO add UI to change this value
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

            createdGameplay.SetName();

            return createdGameplay;
        }

        public void SetName()
        {
            name = Action.name + " " + Entity.name;

            if (Ability)
            {
                name += " " + Ability.name;
            }

            if (Consumable)
            {
                name += " " + Consumable;
            }
        }

        private Gameplay()
        {

        }
    }
}
