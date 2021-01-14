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
        public Gameplay Gameplay { get{ return _gameplay; } }
        [SerializeField] private List<GameplayRepresentation> _nextGameplay; 

        void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            if (_nextGameplay == null)
                _nextGameplay = new List<GameplayRepresentation>();
#if UNITY_EDITOR
            //if (_position == null)
            //    _position = new Vector2();
            if ((int)_nodeDimensions.width == 0)
                _nodeDimensions = new Rect(0, 0, 150, 50);
#endif
        }

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
            //mouseOffset.y += 30;

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
            newGameplayRepresentation._nodeDimensions.position = gameplay._nodeDimensions.position;

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

        [SerializeField] private List<GameplayRepresentation> _gameplayInGraph;

        public List<GameplayRepresentation> GameplayInGraph
        {
            get { return _gameplayInGraph; }
        }

        [SerializeField] private List<GameplayRepresentation> _startingGameplay;
        public List<GameplayRepresentation> StartingGameplay { get{ return _startingGameplay; } set { _startingGameplay = value; } }

        [SerializeField] private List<GameplayRepresentation> _endGameplay;
        public List<GameplayRepresentation> EndGameplay { get{ return _endGameplay; } set { _endGameplay = value; } }


        void OnEnable()
        {
            if (_gameplayInGraph == null)
                _gameplayInGraph = new List<GameplayRepresentation>();
            if (_startingGameplay == null)
                _startingGameplay = new List<GameplayRepresentation>();
            if (_endGameplay == null)
                _endGameplay = new List<GameplayRepresentation>();
        }

        public static GameplayGraph CreateGraph(string name)
        {
            GameplayGraph newGraph = CreateInstance<GameplayGraph>();
            newGraph._name = name;
            return newGraph;
        }

        public void AddGameplay(Gameplay toAdd)
        {
            _gameplayInGraph.Add(GameplayRepresentation.Create(toAdd));
            if (_startingGameplay.Count == 0)
                _startingGameplay.Add(_gameplayInGraph[0]);
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

        // TODO finish this shit
        public void InsertSubgraph(GameplayGraph toReplace, GameplayGraph replacement)
        {
            GameplayRepresentation replacementStart = FindSubgraph(toReplace);

            if (replacementStart == null)
                return;

            // TODO Replace
        }

        // TODO finish this shit
        private GameplayRepresentation FindSubgraph(GameplayGraph toSearch)
        {
            foreach (GameplayRepresentation startInSearchedPattern in toSearch.StartingGameplay)
            {
                for (int i = 1; i < _gameplayInGraph.Count; i++)
                {
                    if (startInSearchedPattern.Gameplay != _gameplayInGraph[i].Gameplay)
                        continue;

                    GameplayRepresentation lastGameplay = ComparePattern(_gameplayInGraph[i], startInSearchedPattern);


                }
            }

            return null;
        }

        // TODO this method is just slapped together. It will NOT work on complicated graphs. Replace for proper algorithm
        private GameplayRepresentation ComparePattern(GameplayRepresentation startInThisGraph, GameplayRepresentation startInSearchedPattern)
        {
            if (startInThisGraph.Gameplay != startInSearchedPattern.Gameplay)
                return null;

            GameplayRepresentation lastGameplay = null;

            foreach (GameplayRepresentation nextGameplay in startInSearchedPattern.NextGameplay)
            {
                bool foundFit = false;

                foreach (GameplayRepresentation possiblePaths in nextGameplay.NextGameplay)
                {
                    lastGameplay = ComparePattern(possiblePaths, startInSearchedPattern);
                    if (lastGameplay != null)
                    {
                        foundFit = true;
                        break;
                    }
                }

                if (!foundFit)
                    return null;
            }

            return lastGameplay;
        }
    }
}
