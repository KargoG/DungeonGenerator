using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContainer : ScriptableObject
{
    private List<Gameplay> _definedGameplay = new List<Gameplay>();

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
}
