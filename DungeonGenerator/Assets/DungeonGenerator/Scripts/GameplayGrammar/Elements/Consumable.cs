using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class Consumable : GameplayElement
    {
        public static Consumable Create(string newConsumableName)
        {
            Consumable newConsumable = CreateInstance<Consumable>();
            newConsumable.name = newConsumableName;

            return newConsumable;
        }
    }
}
