using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator.ExampleGame
{
    public class Key : MonoBehaviour
    {
        private PlayerController _player;

        void Start()
        {
            _player = FindObjectOfType<PlayerController>();
        }

        public void Pickup()
        {
            _player.AddKey();
        }
    }
}
