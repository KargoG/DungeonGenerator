using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonGenerator
{
    [Serializable]
    public class Ability : GameplayElement
    {
        public static Ability Create(string newAbilityName)
        {
            Ability newAbility = CreateInstance<Ability>();
            newAbility.name = newAbilityName;

            return newAbility;
        }
    }
}
