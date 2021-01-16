using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonGenerator
{
    [Serializable]
    public class GameplayRepresentation : ScriptableObject
    {
        [SerializeField] private Gameplay _gameplay;
        public Gameplay Gameplay { get { return _gameplay; } }

        [SerializeField] private List<GameplayRepresentation> _nextGameplay;
        public IReadOnlyList<GameplayRepresentation> NextGameplay
        {
            get { return _nextGameplay; }
        }

        [SerializeField] private List<GameplayRepresentation> _previousGameplay;
        public IReadOnlyList<GameplayRepresentation> PreviousGameplay
        {
            get { return _previousGameplay; }
        }

        void OnEnable()
        {
            if (_nextGameplay == null)
                _nextGameplay = new List<GameplayRepresentation>();
            if (_previousGameplay == null)
                _previousGameplay = new List<GameplayRepresentation>();
#if UNITY_EDITOR
            if ((int)_nodeDimensions.width == 0)
                _nodeDimensions = new Rect(0, 0, 150, 50);
#endif
        }

        [SerializeField] private DungeonRoom _room = null;
        public DungeonRoom RoomGameplayIsIn
        {
            get { return _room; }
            set { _room = value; }
        }

#if UNITY_EDITOR
        //[SerializeField] private Vector2 _position;
        [SerializeField] private Rect _nodeDimensions;
        public Vector2 Position
        {
            get { return _nodeDimensions.position; }
            set { _nodeDimensions.position = value; }
        }

        private bool _dragging = false;

        public bool HandleInput(Event e)
        {

            if (_nodeDimensions.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.button == 0)
                    {
                        if (!_dragging)
                        {
                            _dragging = true;
                            return false;
                        }
                    }
                }
            }

            if (_dragging && e.type == EventType.MouseDrag)
            {
                _nodeDimensions.position = _nodeDimensions.position + e.delta / 2;
                return true;
            }

            if (_dragging && e.type == EventType.MouseUp)
            {
                _dragging = false;
                return false;
            }

            return false;
        }

        public bool IsInRect(Vector2 mousePos)
        {
            Vector2 mouseOffset = GUILayoutUtility.GetLastRect().position;

            Vector2 relativeMousePos = mousePos - mouseOffset;

            return relativeMousePos.x > _nodeDimensions.position.x && relativeMousePos.x < _nodeDimensions.position.x + 150 && relativeMousePos.y > _nodeDimensions.position.y && relativeMousePos.y < _nodeDimensions.position.y + 50;
        }

        public void DrawNode(Vector2 offset)
        {
            Rect relativePos = _nodeDimensions;
            relativePos.position += offset;
            GUI.Box(relativePos, _gameplay.ToString(), new GUIStyle(GUI.skin.button));
        }

        public void DrawConnections(Vector2 offset)
        {
            Color oldColor = GUI.color;
            GUI.color = Color.green;
            foreach (GameplayRepresentation gameplayRepresentation in _nextGameplay)
            {
                Handles.DrawLine(_nodeDimensions.position + _nodeDimensions.size / 2 + offset, gameplayRepresentation._nodeDimensions.position + _nodeDimensions.size / 2 + offset);
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
            newGameplayRepresentation._previousGameplay = gameplay._previousGameplay;
            newGameplayRepresentation._nodeDimensions.position = gameplay._nodeDimensions.position;

            return newGameplayRepresentation;
        }

        public void SetNextGameplay(List<GameplayRepresentation> nextGameplay)
        {
            _nextGameplay = nextGameplay;
        }

        public void SetPreviousGameplay(List<GameplayRepresentation> previousGameplay)
        {
            _previousGameplay = previousGameplay;
        }

        public void SpawnGameplay(GameObject roomRoot) // TODO change so gameplay has more information (like geometry and shit)
        {
            Instantiate(_gameplay.Entity.Representation, roomRoot.transform.position, Quaternion.identity,
                roomRoot.transform);
        }

        public void AddNextGameplay(GameplayRepresentation newNextGameplay)
        {
            if (!_nextGameplay.Contains(newNextGameplay))
                _nextGameplay.Add(newNextGameplay);
        }

        public void AddPreviousGameplay(GameplayRepresentation newPreviousGameplay)
        {
            if (!_previousGameplay.Contains(newPreviousGameplay))
                _previousGameplay.Add(newPreviousGameplay);
        }

        public void RemoveNextGameplay(GameplayRepresentation toRemove)
        {
            if (_nextGameplay.Contains(toRemove))
                _nextGameplay.Remove(toRemove);
        }

        public void RemovePreviousGameplay(GameplayRepresentation toRemove)
        {
            if (_previousGameplay.Contains(toRemove))
                _previousGameplay.Remove(toRemove);
        }
    }
}
