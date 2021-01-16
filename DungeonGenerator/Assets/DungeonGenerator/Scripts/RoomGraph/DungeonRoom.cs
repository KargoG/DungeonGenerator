using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class DungeonRoom : ScriptableObject
    {
        [SerializeField] private List<GameplayRepresentation> _gameplayInRoom = new List<GameplayRepresentation>();
        public IReadOnlyList<GameplayRepresentation> GameplayInRoom
        {
            get { return _gameplayInRoom.AsReadOnly(); }
        }

        [SerializeField] private List<DungeonRoom> _nextRoom = new List<DungeonRoom>();
        public IReadOnlyList<DungeonRoom> NextRoom
        {
            get { return _nextRoom.AsReadOnly(); }
        }

        [SerializeField] private List<DungeonRoom> _previousRoom = new List<DungeonRoom>();
        public IReadOnlyList<DungeonRoom> PreviousRoom
        {
            get { return _previousRoom.AsReadOnly(); }
        }

        void OnEnable()
        {
            //hideFlags = HideFlags.HideAndDontSave;

            if (_gameplayInRoom == null)
                _gameplayInRoom = new List<GameplayRepresentation>();

            if (_nextRoom == null)
                _nextRoom = new List<DungeonRoom>();
        }

        public static DungeonRoom Create(GameplayRepresentation gameplayInRoom)
        {
            DungeonRoom createdRoom = CreateInstance<DungeonRoom>();

            createdRoom._gameplayInRoom.Add(gameplayInRoom);
            //AssetDatabase.AddObjectToAsset(gameplayInRoom, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + name + ".asset");

            return createdRoom;
        }

        public static DungeonRoom Create(IEnumerable<GameplayRepresentation> gameplayInRoom)
        {
            DungeonRoom createdRoom = CreateInstance<DungeonRoom>();

            createdRoom._gameplayInRoom.AddRange(gameplayInRoom);
            //foreach (GameplayRepresentation gameplayRepresentation in gameplayInRoom)
            //{
            //    AssetDatabase.AddObjectToAsset(gameplayRepresentation, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + name + ".asset");
            //}


            return createdRoom;
        }
        public void PrepareConnections()
        {
            _nextRoom.Clear();
            _previousRoom.Clear();

            foreach (GameplayRepresentation gameplayRepresentation in _gameplayInRoom)
            {
                foreach (GameplayRepresentation nextGameplay in gameplayRepresentation.NextGameplay)
                {
                    if (nextGameplay.RoomGameplayIsIn != this)
                    {
                        if (!_nextRoom.Contains(nextGameplay.RoomGameplayIsIn))
                            _nextRoom.Add(nextGameplay.RoomGameplayIsIn);
                    }
                }
            }

            foreach (GameplayRepresentation gameplayRepresentation in _gameplayInRoom)
            {
                foreach (GameplayRepresentation previousGameplay in gameplayRepresentation.PreviousGameplay)
                {
                    if (previousGameplay.RoomGameplayIsIn != this)
                    {
                        if (!_previousRoom.Contains(previousGameplay.RoomGameplayIsIn))
                            _previousRoom.Add(previousGameplay.RoomGameplayIsIn);
                    }
                }
            }
        }

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
            GUI.Box(pos, "", new GUIStyle(GUI.skin.button)); // TODO add a name here
        }

        public void DrawConnections(Vector2 nodeSize)
        {
            Color oldColor = GUI.color;
            GUI.color = Color.green;
            foreach (GameplayRepresentation gameplayRepresentation in _gameplayInRoom)
            {
                foreach (GameplayRepresentation nextGameplay in gameplayRepresentation.NextGameplay)
                {
                    Handles.DrawLine(_position + nodeSize / 2, nextGameplay.RoomGameplayIsIn.Position + nodeSize / 2);
                }
            }

            GUI.color = oldColor;
        }

#endif
    }
}

