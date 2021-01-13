using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public class Door : MonoBehaviour
    {
        private Door _otherSide = null;
        public Door OtherSide { set { if(!_otherSide) _otherSide = value; } }

        [SerializeField] private float _exitOffset = 0.5f;

        public void GoThroughDoor(GameObject objectToGoThroughDoor)
        {
            if (!_otherSide)
            {
                Debug.LogError("The dor you are trying to go through leads nowhere");
            }

            objectToGoThroughDoor.transform.position =
                _otherSide.transform.position + _otherSide.transform.forward * _otherSide._exitOffset;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.forward * _exitOffset, 0.1f);
        }
    }
}
