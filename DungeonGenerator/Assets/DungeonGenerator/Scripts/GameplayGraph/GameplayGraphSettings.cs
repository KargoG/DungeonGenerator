using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEngine;

namespace DungeonGenerator.Editor
{
    [CreateAssetMenu(fileName = "new Graph Settings", menuName = "DungeonGenerator/GraphSettings")]
    public class GameplayGraphSettings : ScriptableObject
    {
        [SerializeField] private List<Gameplay> _startGameplay = new List<Gameplay>();

        public List<Gameplay> StartGameplay
        {
            get { return _startGameplay; }
        }

        [SerializeField] private List<Gameplay> _endGameplay = new List<Gameplay>();

        public List<Gameplay> EndGameplay
        {
            get { return _endGameplay; }
        }

        [SerializeField] private List<Gameplay> _mustIncludeGameplay = new List<Gameplay>();

        public List<Gameplay> MustIncludeGameplay
        {
            get { return _mustIncludeGameplay; }
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

    }
}
