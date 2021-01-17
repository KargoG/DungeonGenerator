using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DungeonGenerator
{
    [Serializable][CreateAssetMenu(fileName = "new Room Creation Settings", menuName = "DungeonGenerator/RoomSettings")]
    public class RoomCreationSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private Dictionary<Tuple<Gameplay, Gameplay>, float> _connectables;
        public IReadOnlyDictionary<Tuple<Gameplay, Gameplay>, float> Connectables { get{ return _connectables; } }

        [SerializeField] private int _numberOfFields = 0;
        [SerializeField] private List<Gameplay> _firstGameplay;
        [SerializeField] private List<Gameplay> _secondGameplay;
        [SerializeField] private List<float> _likeliness;

        void OnEnable()
        {
            if (_connectables == null)
            {
                _connectables = new Dictionary<Tuple<Gameplay, Gameplay>, float>();
            }
        }

        public void OnBeforeSerialize()
        {
            if (_firstGameplay == null) _firstGameplay = new List<Gameplay>();
            if (_secondGameplay == null) _secondGameplay = new List<Gameplay>();
            if (_likeliness == null) _likeliness = new List<float>();

            _firstGameplay.Clear();
            _secondGameplay.Clear();
            _likeliness.Clear();

            foreach (KeyValuePair<Tuple<Gameplay, Gameplay>, float> keyValuePair in _connectables)
            {
                _firstGameplay.Add(keyValuePair.Key.Item1);
                _secondGameplay.Add(keyValuePair.Key.Item2);
                _likeliness.Add(keyValuePair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            if (_connectables == null) _connectables = new Dictionary<Tuple<Gameplay, Gameplay>, float>();
            _connectables.Clear();

            for (int i = 0; i < _numberOfFields; i++)
            {
                _connectables.Add(new Tuple<Gameplay, Gameplay>(
                        i < _firstGameplay.Count ? _firstGameplay[i] : null,
                        i < _secondGameplay.Count ? _secondGameplay[i] : null),
                        i < _likeliness.Count ? _likeliness[i] : 0);
            }
        }
    }
}