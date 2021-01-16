using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEngine;

public class DefaultRoomCreator : MonoBehaviour, IRoomCreator
{
    private List<IRoomConnector> _roomConnectors = new List<IRoomConnector>();
    [SerializeField] private Door _doorPre = null;
    public void CreateRoom(DungeonRoom dungeonRoom)
    {
        // TODO create a room here
        print("roooom");
        foreach (DungeonRoom nextRoom in dungeonRoom.NextRoom)
        {
            _roomConnectors.Add(Instantiate(_doorPre, transform));
        }
        foreach (DungeonRoom previousRoom in dungeonRoom.PreviousRoom)
        {
            _roomConnectors.Add(Instantiate(_doorPre, transform));
        }
    }

    public void SetUpConnections(GameObject[] roomsToConnectTo)
    {
        // TODO connect Rooms
        if (roomsToConnectTo.Length != _roomConnectors.Count)
            Debug.LogError("Number of doors and number of connected Rooms needs to be the same!");
        for (int i = 0; i < _roomConnectors.Count; i++)
        {
            _roomConnectors[i].SetUpConnection(roomsToConnectTo[i]);
        }
    }
}
