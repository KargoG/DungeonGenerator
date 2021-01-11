using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoomCreator : ScriptableObject
{
    [SerializeField] private static GameObject _roomPrefab = null;
    public static void CreateRoom(DungeonRoom dungeonRoom)
    {
        Instantiate(_roomPrefab, dungeonRoom.Position, Quaternion.identity);
    }
}
