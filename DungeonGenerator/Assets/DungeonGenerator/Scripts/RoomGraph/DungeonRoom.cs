using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator
{
    public class DungeonRoom : ScriptableObject
    {
        private List<GameplayRepresentation> _gameplayInRoom = new List<GameplayRepresentation>();

        public List<GameplayRepresentation> GameplayInRoom
        {
            get { return _gameplayInRoom; }
        }

        public static DungeonRoom Create(GameplayRepresentation gameplayInRoom)
        {
            DungeonRoom createdRoom = CreateInstance<DungeonRoom>();

            createdRoom._gameplayInRoom.Add(gameplayInRoom);

            return createdRoom;
        }

        public static DungeonRoom Create(IEnumerable<GameplayRepresentation> gameplayInRoom)
        {
            DungeonRoom createdRoom = CreateInstance<DungeonRoom>();

            createdRoom._gameplayInRoom.AddRange(gameplayInRoom);

            return createdRoom;
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

