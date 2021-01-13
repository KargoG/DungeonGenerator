using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class Entity : GameplayElement
    {
        private GameObject _representation = null;

        public GameObject Representation
        {
            get { return _representation; }
            set { _representation = value; }
        }

        public static Entity Create(string newEntityName)
        {
            Entity newEntity = CreateInstance<Entity>();
            newEntity.name = newEntityName;

            return newEntity;
        }
    }
}
