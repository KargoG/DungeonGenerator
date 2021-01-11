﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGraph : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name { get { return _name; } }
    [SerializeField] [HideInInspector] private List<DungeonRoom> _dungeonGraph = new List<DungeonRoom>();
    public List<DungeonRoom> DungeonGraph { get { return _dungeonGraph; } }

    [SerializeField] private DungeonRoom _firstRoom;


    public static RoomGraph CreateStartingGraph(string name, GameplayGraph gameplayGraph)
    { 
        RoomGraph newGraph = CreateInstance<RoomGraph>();

        newGraph._name = name;

        List<GameplayRepresentation> gameplayInDungeon = gameplayGraph.CreateCopyOfGameplay();

        foreach (GameplayRepresentation gameplayRepresentation in gameplayInDungeon)
        {
            newGraph.AddRoom(gameplayRepresentation);
        }

        // TODO connect all rooms based on their gameplay somehow

        return newGraph;
    }

    public void AddRoom(GameplayRepresentation gameplayInRoom)
    {
        DungeonRoom newRoom = DungeonRoom.Create(gameplayInRoom);
        _dungeonGraph.Add(newRoom);
        gameplayInRoom.RoomGameplayIsIn = newRoom;

        if (!_firstRoom)
        {
            _firstRoom = newRoom;
        }
    }
}