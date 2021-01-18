using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonGenerator
{
    [Serializable]
    public class GameplayContainer : ScriptableObject
    {
        [SerializeField] private List<Gameplay> _definedGameplay;

        void OnEnable()
        {
            if (_definedGameplay == null)
                _definedGameplay = new List<Gameplay>();
        }

        public void AddGameplay(Gameplay toAdd)
        {
            _definedGameplay.Add(toAdd);
        }

        public Gameplay GetGameplay(int i)
        {
            return _definedGameplay[i];
        }

        public int GetAmountOfGameplay()
        {
            return _definedGameplay.Count;
        }

        public void RemoveGameplay(Gameplay toRemove)
        {
            _definedGameplay.Remove(toRemove);
        }

        public bool UsesGameplayElement(GameplayElement gameplayElement, GameplayElementTypes type)
        {
            foreach (Gameplay gameplay in _definedGameplay)
            {
                switch (type)
                {
                    case GameplayElementTypes.Action:
                        if (gameplay.Action == gameplayElement)
                            return true;
                        break;
                    case GameplayElementTypes.Entity:
                        if (gameplay.Entity == gameplayElement)
                            return true;
                        break;
                    case GameplayElementTypes.Ability:
                        if (gameplay.Ability == gameplayElement)
                            return true;
                        break;
                    case GameplayElementTypes.Consumable:
                        if (gameplay.Consumable == gameplayElement)
                            return true;
                        break;
                }
            }

            return false;
        }

        public List<Gameplay> GetRandomPlacableGameplay()
        {
            return _definedGameplay;
        }

        public string[] GetGameplayNames()
        {
            string[] names = new string[_definedGameplay.Count];

            for (int i = 0; i < _definedGameplay.Count; i++)
            {
                names[i] = _definedGameplay[i].name;
            }

            return names;
        }
    }
}