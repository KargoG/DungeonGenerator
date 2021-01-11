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
        [SerializeField] private Vector2 _interactionBoxHalfExtends = new Vector2();

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
            Instantiate(_attackPre, transform.position, transform.rotation);
        }
        private void Interact()
        {
            Collider[] possibleInteractions = Physics.OverlapBox(transform.position + transform.forward * 0.5f, _interactionBoxHalfExtends, transform.rotation);

            foreach (Collider possibleInteraction in possibleInteractions)
            {
                //IInteractable interactable = possibleInteraction.GetComponent<IInteractable>();

                //if (!interactable)
                //    continue;

                //interactable.Interact();
            }
        }

    }
}
