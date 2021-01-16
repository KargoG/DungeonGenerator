using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEngine;

public interface IRoomCreator
{
    void CreateRoom(DungeonRoom dungeonRoom);
    void SetUpConnections(GameObject[] roomsToConnectTo);
}
