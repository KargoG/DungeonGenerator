using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator.ExampleGame
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _characterController;
        [SerializeField] private float _movementSpeed = 10;
        [SerializeField] private GameObject _attackPre = null;
        [SerializeField] private Vector3 _interactionBoxHalfExtends = new Vector3(0.25f, 0.25f, 0.5f);

        private int _keys;

        // Start is called before the first frame update
        void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
            HandleInput();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + transform.forward * 0.5f + transform.up * 0.5f, _interactionBoxHalfExtends * 2);
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Interact();
            }
        }

        void HandleMovement()
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _characterController.Move(movement * Time.deltaTime * _movementSpeed);
        }

        private void Attack()
        {
            Instantiate(_attackPre, transform.position + transform.up, transform.rotation).GetComponent<BaseAttack>().Attacker = gameObject;
        }
        
        private void Interact()
        {
            Collider[] possibleInteractions = Physics.OverlapBox(transform.position + transform.forward * 0.5f + transform.up * 0.5f, _interactionBoxHalfExtends, transform.rotation);

            foreach (Collider possibleInteraction in possibleInteractions)
            {
                Key key = possibleInteraction.GetComponent<Key>();

                if (key)
                {
                    key.Pickup();
                    continue;
                }

                Door door = possibleInteraction.GetComponent<Door>();

                if (door)
                {
                    door.ExitRoom(gameObject);
                    continue;
                }
            }
        }

        public void AddKey()
        {
            _keys++;
        }

        public void GetAttacked()
        {
            print("Take Damage");
        }
    }
}
