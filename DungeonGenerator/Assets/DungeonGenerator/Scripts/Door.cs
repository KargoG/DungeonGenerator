using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public class Door : MonoBehaviour, IRoomConnector
    {
        [SerializeField] private Door _otherSide;

        [SerializeField] private float _exitOffset = 0.5f;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.forward * _exitOffset, 0.1f);
        }

        public void SetUpConnection(GameObject exit)
        {
            if (!_otherSide)
            {
                _otherSide = exit.GetComponentInChildren<Door>();
                if (!_otherSide)
                    Debug.LogError("A door has to lead to another door!");
            }
        }

        public void ExitRoom(GameObject exitingObject)
        {
            if (!_otherSide)
            {
                Debug.LogError("The d0or you are trying to go through leads nowhere");
            }

            exitingObject.transform.position =
                _otherSide.transform.position + _otherSide.transform.forward * _otherSide._exitOffset;
        }
    }
}
