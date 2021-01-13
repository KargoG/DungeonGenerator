using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonGenerator
{
    [Serializable]
    public class RoomCreator : UnityEvent<DungeonRoom>
    {
        //private GameObject _roomPre;
    }

    [CreateAssetMenu(fileName = "new Room Creation Settings", menuName = "DungeonGenerator/RoomSettings")]
    public class RoomCreationSettings : ScriptableObject
    {
        [SerializeField] private RoomCreator _roomCreator = null;

        public RoomCreator RoomCreator
        {
            get { return _roomCreator; }
        }
    }
}