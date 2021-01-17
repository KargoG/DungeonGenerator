using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class GameplayGraph : ScriptableObject
    {
        [SerializeField] private string _name;

        public string Name
        {
            get { return _name; }
        }

        [SerializeField] private List<GameplayRepresentation> _gameplayInGraph;
        public IReadOnlyList<GameplayRepresentation> GameplayInGraph
        {
            get { return _gameplayInGraph.AsReadOnly(); }
        }

        [SerializeField] private List<GameplayRepresentation> _startingGameplay;
        public IReadOnlyList<GameplayRepresentation> StartingGameplay { get{ return _startingGameplay.AsReadOnly(); } /*set { _startingGameplay = value; }*/ }

        [SerializeField] private List<GameplayRepresentation> _endGameplay;
        public IReadOnlyList<GameplayRepresentation> EndGameplay { get{ return _endGameplay.AsReadOnly(); } /*set { _endGameplay = value; }*/ }


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

        public GameplayRepresentation AddGameplay(Gameplay toAdd)
        {
            _gameplayInGraph.Add(GameplayRepresentation.Create(toAdd));
            if (_startingGameplay.Count == 0)
                _startingGameplay.Add(_gameplayInGraph[0]);

            return _gameplayInGraph[_gameplayInGraph.Count - 1];
        }

        public List<GameplayRepresentation> CreateCopyOfGameplay(out List<GameplayRepresentation> startingGameplay,
            out List<GameplayRepresentation> endGameplay)
        {
            List<GameplayRepresentation> copy = new List<GameplayRepresentation>();
            startingGameplay = new List<GameplayRepresentation>();
            endGameplay = new List<GameplayRepresentation>();

            Dictionary<GameplayRepresentation, GameplayRepresentation> listToConnect =
                new Dictionary<GameplayRepresentation, GameplayRepresentation>();

            // Add Gameplay copies
            foreach (GameplayRepresentation gameplayRepresentation in _gameplayInGraph)
            {
                GameplayRepresentation newGameplay = GameplayRepresentation.Create(gameplayRepresentation);
                copy.Add(newGameplay);
                listToConnect.Add(gameplayRepresentation, newGameplay);

                if (_startingGameplay.Contains(gameplayRepresentation))
                    startingGameplay.Add(newGameplay);
                if (_endGameplay.Contains(gameplayRepresentation))
                    endGameplay.Add(newGameplay);
            }

            // Properly Connect Copies

            foreach (KeyValuePair<GameplayRepresentation, GameplayRepresentation> pair in listToConnect)
            {
                List<GameplayRepresentation> nextGameplayList = new List<GameplayRepresentation>();
                List<GameplayRepresentation> previousGameplayList = new List<GameplayRepresentation>();

                foreach (GameplayRepresentation nextGameplay in pair.Key.NextGameplay)
                {
                    nextGameplayList.Add(listToConnect[nextGameplay]);
                }
                foreach (GameplayRepresentation previousGameplay in pair.Key.PreviousGameplay)
                {
                    previousGameplayList.Add(listToConnect[previousGameplay]);
                }

                pair.Value.SetNextGameplay(nextGameplayList);
                pair.Value.SetPreviousGameplay(previousGameplayList);
            }

            return copy;
        }

        public List<GameplayRepresentation> CreateCopyOfGameplay()
        {
            return CreateCopyOfGameplay(out List<GameplayRepresentation> unused, out List<GameplayRepresentation> unusedAsWell);
        }


        public void InsertSubgraph(GameplayGraph toReplace, GameplayGraph replacement)
        {
            KeyValuePair<GameplayRepresentation, GameplayRepresentation> replacementStart = FindSubgraph(toReplace);

            if (replacementStart.Key == null)
                return;

            List<GameplayRepresentation> replacementCopy = replacement.CreateCopyOfGameplay(out List<GameplayRepresentation> start, out List<GameplayRepresentation> end);
            _gameplayInGraph.AddRange(replacementCopy);

            foreach (GameplayRepresentation copy in replacementCopy)
            {
                AssetDatabase.AddObjectToAsset(copy, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + name + ".asset");
            }


            // Re-link connections
            foreach (GameplayRepresentation toPrepare in replacementStart.Key.PreviousGameplay)
            {
                foreach (GameplayRepresentation startingGameplay in start)
                {
                    toPrepare.AddNextGameplay(startingGameplay);
                    startingGameplay.AddPreviousGameplay(toPrepare);
                }

                toPrepare.RemoveNextGameplay(replacementStart.Key);
            }
            foreach (GameplayRepresentation toPrepare in replacementStart.Key.NextGameplay)
            {
                foreach (GameplayRepresentation startingGameplay in start)
                {
                    toPrepare.AddPreviousGameplay(startingGameplay);
                    startingGameplay.AddNextGameplay(toPrepare);
                }

                toPrepare.RemovePreviousGameplay(replacementStart.Key);
            }

            foreach (GameplayRepresentation toPrepare in replacementStart.Value.NextGameplay)
            {
                foreach (GameplayRepresentation endGameplay in end)
                {
                    toPrepare.AddPreviousGameplay(endGameplay);
                    endGameplay.AddNextGameplay(toPrepare);
                }
                toPrepare.RemovePreviousGameplay(replacementStart.Value);
            }
            foreach (GameplayRepresentation toPrepare in replacementStart.Value.PreviousGameplay)
            {
                foreach (GameplayRepresentation endGameplay in end)
                {
                    toPrepare.AddNextGameplay(endGameplay);
                    endGameplay.AddPreviousGameplay(toPrepare);
                }
                toPrepare.RemoveNextGameplay(replacementStart.Value);
            }

            if (_startingGameplay.Contains(replacementStart.Key))
                foreach (GameplayRepresentation startingGameplay in start)
                {
                    _startingGameplay.Add(startingGameplay);
                }

            if (_endGameplay.Contains(replacementStart.Value))
                foreach (GameplayRepresentation endGameplay in end)
                {
                    _endGameplay.Add(endGameplay);
                }

            
            RemoveGameplay(replacementStart.Key);
            RemoveGameplay(replacementStart.Value);
            // TODO the replaed Gameplay is not longer linked but still in memory. Delete it if you dont want graphs to turn into garbage dumps
            // TODO gameplay that comes after key and before value still needs the new start and end hooked up
        }

        private KeyValuePair<GameplayRepresentation, GameplayRepresentation> FindSubgraph(GameplayGraph toSearch)
        {
            KeyValuePair<GameplayRepresentation, GameplayRepresentation> result;
            List<Gameplay> thisGraphsString = GetStringFromPosition(_startingGameplay[0]);
            List<Gameplay> subgraphString = GetStringFromPosition(toSearch.StartingGameplay[0]);

            int startPos = -1;
            int endPos = -1;

            for (int i = 0; i < thisGraphsString.Count; i++)
            {
                if (thisGraphsString[i] == subgraphString[0])
                {
                    int patternEndPos = ComparePattern(thisGraphsString, subgraphString, i);
                    if (patternEndPos >= 0)
                    {
                        startPos = i;
                        endPos = i + patternEndPos;
                        break;
                    }
                }
            }

            if (startPos >= 0)
            {
                result = new KeyValuePair<GameplayRepresentation, GameplayRepresentation>(GetGameplayAtPosition(startPos), GetGameplayAtPosition(endPos));
            }

            return result;
        }

        private GameplayRepresentation GetGameplayAtPosition(int pos)
        {
            Stack<GameplayRepresentation> positionStack = new Stack<GameplayRepresentation>();
            Stack<int> branchingPath = new Stack<int>();

            GameplayRepresentation currentGameplay = _startingGameplay[0];
            int lastUsedPath = 0;

            for (int i = 0; i < pos; i++)
            {
                if (currentGameplay.NextGameplay.Count > 1)
                {
                    positionStack.Push(currentGameplay);
                    branchingPath.Push(lastUsedPath);
                }

                if (lastUsedPath < currentGameplay.NextGameplay.Count)
                    currentGameplay = currentGameplay.NextGameplay[lastUsedPath];
                else
                    currentGameplay = null;

                lastUsedPath = 0;

                if (currentGameplay == null)
                {
                    currentGameplay = positionStack.Pop();
                    lastUsedPath = branchingPath.Pop() + 1;
                    i--;
                }
            }

            return currentGameplay;
        }

        private List<Gameplay> GetStringFromPosition(GameplayRepresentation start)
        {
            List<Gameplay> asString = new List<Gameplay>();

            GameplayRepresentation nextLetter = start;

            asString.Add(nextLetter.Gameplay);

            if (nextLetter.NextGameplay.Count > 1)
            {
                foreach (GameplayRepresentation gameplayRepresentation in nextLetter.NextGameplay)
                {
                    asString.Add(Gameplay.Empty);
                    asString.AddRange(GetStringFromPosition(gameplayRepresentation));
                    asString.Add(null);
                }
            }
            else if(nextLetter.NextGameplay.Count == 1)
            {
                asString.AddRange(GetStringFromPosition(nextLetter.NextGameplay[0]));
            }


            return asString;
        }

        private int ComparePattern(List<Gameplay> thisString, List<Gameplay> patternString, int startPosition)
        {
            int pos = 0;
            int positionOffset = 1;

            for (int i = 0; i < patternString.Count; i++, pos++)
            {
                if (startPosition + pos >= thisString.Count)
                    return -1;
                if (thisString[startPosition + pos] != patternString[i])
                {
                    if (patternString[i] == null) // End Subgraph
                    {
                        while (thisString[startPosition + pos] != null)
                        {
                            pos++;
                            if (startPosition + pos >= thisString.Count)
                                return -1;
                        }
                    }
                    else if (thisString[startPosition + pos] == Gameplay.Empty) // Open Subgraph
                    {
                        i--;
                        positionOffset++;
                        continue;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            pos-= positionOffset;

            return pos;
        }

        public void RemoveGameplay(GameplayRepresentation gp)
        {
            if (_endGameplay.Contains(gp))
                _endGameplay.Remove(gp);
            if (_startingGameplay.Contains(gp))
                _startingGameplay.Remove(gp);
            if (_gameplayInGraph.Contains(gp))
                _gameplayInGraph.Remove(gp);

            foreach (GameplayRepresentation gameplay in _gameplayInGraph)
            {
                gameplay.RemoveNextGameplay(gp);
                gameplay.RemovePreviousGameplay(gp);
            }
        }

        public void ChangeStartingGameplay(GameplayRepresentation gp)
        {
            if (_startingGameplay.Contains(gp))
                _startingGameplay.Remove(gp);
            else
                _startingGameplay.Add(gp);
        }

        public void ChangeEndGameplay(GameplayRepresentation gp)
        {
            if (_endGameplay.Contains(gp))
                _endGameplay.Remove(gp);
            else
                _endGameplay.Add(gp);
        }
    }
}
