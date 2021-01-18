using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonGenerator
{
    [CreateAssetMenu(fileName = "new Graph Settings", menuName = "DungeonGenerator/GraphSettings")]
    public class GameplayGraphSettings : ScriptableObject
    {
        [SerializeField] private List<Gameplay> _startGameplay = new List<Gameplay>();
        public IReadOnlyList<Gameplay> StartGameplay
        {
            get { return _startGameplay.AsReadOnly(); }
        }

        [SerializeField] private List<Gameplay> _endGameplay = new List<Gameplay>();
        public IReadOnlyList<Gameplay> EndGameplay
        {
            get { return _endGameplay.AsReadOnly(); }
        }

        [SerializeField] private List<Gameplay> _mustIncludeGameplay = new List<Gameplay>();
        public IReadOnlyList<Gameplay> MustIncludeGameplay
        {
            get { return _mustIncludeGameplay.AsReadOnly(); }
        }

        [SerializeField] private int _minStartGraphLength = 5;
        [SerializeField] private int _maxStartGraphLength = 10;

        public int MinStartGraphLength
        {
            get { return _minStartGraphLength; }
        }

        public int MaxStartGraphLength
        {
            get { return _maxStartGraphLength; }
        }

        [NonSerialized] private List<Gameplay> _randomlyPlaceableGameplay = new List<Gameplay>();
        public void SetRandomPlacableGameplay(List<Gameplay> randomGameplay)
        {
            _randomlyPlaceableGameplay = randomGameplay;
        }

        public Gameplay GetRandomPlacableGameplay()
        {
            Gameplay toReturn = null;
            do
            {
                toReturn = _randomlyPlaceableGameplay[Random.Range(0, _randomlyPlaceableGameplay.Count)];
            } while (!toReturn.RandomPlacable);

            return toReturn;
        }
    }
}
