using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[Serializable]
public class GameplayRepresentation : ScriptableObject
{
    [SerializeField] private Gameplay _gameplay;
    [SerializeField] private List<Gameplay> _nextGameplay;

#if UNITY_EDITOR
    [SerializeField] private Vector2 _position;
    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public void DrawNode(Vector2 size)
    {
        Rect pos = new Rect(_position, size);
        GUI.Box(pos, _gameplay.ToString(), new GUIStyle(GUI.skin.button));
    }

#endif
    public static GameplayRepresentation Create(Gameplay gameplay)
    {
        GameplayRepresentation newGameplayRepresentation = CreateInstance<GameplayRepresentation>();
        newGameplayRepresentation._gameplay = gameplay;

        return newGameplayRepresentation;
    }
}

[Serializable]
public class GameplayGraph : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name { get{ return _name; } }
    [SerializeField][HideInInspector] private List<GameplayRepresentation> _gameplayInGraph = new List<GameplayRepresentation>();
    public List<GameplayRepresentation> GameplayInGraph { get{ return _gameplayInGraph; } }

    [SerializeField] private GameplayRepresentation _startingGameplay;

    public static GameplayGraph CreateGraph(string name)
    {
        GameplayGraph newGraph = CreateInstance<GameplayGraph>();
        newGraph._name = name;
        return newGraph;
    }

    public void AddGameplay(Gameplay toAdd)
    {
        _gameplayInGraph.Add(GameplayRepresentation.Create(toAdd));
    }
}
