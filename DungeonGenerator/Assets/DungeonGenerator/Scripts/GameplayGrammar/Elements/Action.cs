using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class Action : GameplayElement
    {
        [SerializeField] private List<Entity> _entitiesThisCanBePerformedOn;

        void OnEnable()
        {
            if (_entitiesThisCanBePerformedOn == null)
                _entitiesThisCanBePerformedOn = new List<Entity>();
        }

        public string[] GetEntityNames()
        {
            string[] names = new string[_entitiesThisCanBePerformedOn.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = _entitiesThisCanBePerformedOn[i].name;
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

        public Entity GetEntity(int selectedEntityIndex)
        {
            return _entitiesThisCanBePerformedOn[selectedEntityIndex];
        }

        public static Action Create(string newActionName)
        {
            Action newAction = CreateInstance<Action>();
            newAction.name = newActionName;


            return newAction;
        }
    }
}
