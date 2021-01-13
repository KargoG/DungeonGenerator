using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace DungeonGenerator
{
    [Serializable]
    public class GameplayRepresentation : ScriptableObject
    {
        [SerializeField] private Gameplay _gameplay;
        [SerializeField] private List<GameplayRepresentation> _nextGameplay = new List<GameplayRepresentation>();

        public List<GameplayRepresentation> NextGameplay
        {
            get { return _nextGameplay; }
        }

        private DungeonRoom _room = null;

        public DungeonRoom RoomGameplayIsIn
        {
            get { return _room; }
            set { _room = value; }
        }

#if UNITY_EDITOR
        [SerializeField] private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private bool _dragging = false;

        public void HandleInput(Event e)
        {
            Vector2 mouseOffset = GUILayoutUtility.GetLastRect().position;
            mouseOffset.y += 30;

            Vector2 relativeMousePos = e.mousePosition - mouseOffset;

            if (relativeMousePos.x > _position.x && relativeMousePos.x < _position.x + 150)
            {
                if (relativeMousePos.y > _position.y && relativeMousePos.y < _position.y + 50)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        if (!_dragging)
                            _dragging = true;
                    }
                }
            }

            if (_dragging && e.type == EventType.MouseDrag)
            {
                _position = _position + e.delta;
            }

            if (e.type == EventType.MouseUp)
            {
                _dragging = false;
            }
        }

        public void DrawNode(Vector2 size)
        {
            Rect pos = new Rect(_position, size);
            GUI.Box(pos, _gameplay.ToString(), new GUIStyle(GUI.skin.button));
        }

        public void DrawConnections(Vector2 nodeSize)
        {
            Color oldColor = GUI.color;
            GUI.color = Color.green;
            foreach (GameplayRepresentation gameplayRepresentation in _nextGameplay)
            {
                Handles.DrawLine(_position + nodeSize / 2, gameplayRepresentation._position + nodeSize / 2);
            }

            GUI.color = oldColor;
        }

#endif
        public static GameplayRepresentation Create(Gameplay gameplay)
        {
            GameplayRepresentation newGameplayRepresentation = CreateInstance<GameplayRepresentation>();
            newGameplayRepresentation._gameplay = gameplay;

            return newGameplayRepresentation;
        }

        public static GameplayRepresentation Create(GameplayRepresentation gameplay)
        {
            GameplayRepresentation newGameplayRepresentation = CreateInstance<GameplayRepresentation>();
            newGameplayRepresentation._gameplay = gameplay._gameplay;
            newGameplayRepresentation._nextGameplay = gameplay._nextGameplay;
            newGameplayRepresentation._position = gameplay._position;

            return newGameplayRepresentation;
        }

        public void SetNextGameplay(List<GameplayRepresentation> nextGameplay)
        {
            _nextGameplay = nextGameplay;
        }

        public void
            SpawnGameplay(GameObject roomRoot) // TODO change so gameplay has more information (like geometry and shit)
        {
            Instantiate(_gameplay.Entity.Representation, roomRoot.transform.position, Quaternion.identity,
                roomRoot.transform);
        }

    }

    [Serializable]
    public class GameplayGraph : ScriptableObject
    {
        [SerializeField] private string _name;

        public string Name
        {
            get { return _name; }
        }

        [SerializeField] [HideInInspector]
        private List<GameplayRepresentation> _gameplayInGraph = new List<GameplayRepresentation>();

        public List<GameplayRepresentation> GameplayInGraph
        {
            get { return _gameplayInGraph; }
        }

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

        public List<GameplayRepresentation> CreateCopyOfGameplay()
        {
            List<GameplayRepresentation> copy = new List<GameplayRepresentation>();
            Dictionary<GameplayRepresentation, GameplayRepresentation> listToConnect =
                new Dictionary<GameplayRepresentation, GameplayRepresentation>();

            // Add Gameplay copies
            foreach (GameplayRepresentation gameplayRepresentation in _gameplayInGraph)
            {
                GameplayRepresentation newGameplay = GameplayRepresentation.Create(gameplayRepresentation);
                copy.Add(newGameplay);
                listToConnect.Add(gameplayRepresentation, newGameplay);
            }

            // Properly Connect Copies

            foreach (KeyValuePair<GameplayRepresentation, GameplayRepresentation> pair in listToConnect)
            {
                List<GameplayRepresentation> nextGameplayList = new List<GameplayRepresentation>();

                foreach (GameplayRepresentation nextGameplay in pair.Key.NextGameplay)
                {
                    nextGameplayList.Add(listToConnect[nextGameplay]);
                }

                pair.Value.SetNextGameplay(nextGameplayList);
            }

            return copy;
        }
    }
}
