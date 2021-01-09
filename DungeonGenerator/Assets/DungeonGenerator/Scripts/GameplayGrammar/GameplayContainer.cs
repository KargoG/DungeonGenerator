using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContainer : ScriptableObject
{
    [SerializeField] private List<Gameplay> _definedGameplay = new List<Gameplay>();

    public void AddGameplay( Gameplay toAdd )
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

    public Gameplay GetRandomPlacableGameplay()
    {
        Gameplay toReturn = null;
        do
        {
            toReturn = _definedGameplay[Random.Range(0, _definedGameplay.Count)];
        } while (!toReturn.RandomPlacable);

        return toReturn;
    }
}
