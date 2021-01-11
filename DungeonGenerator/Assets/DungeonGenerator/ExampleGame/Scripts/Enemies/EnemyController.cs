using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator.ExampleGame
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour
    {
        private CharacterController _characterController;
        [SerializeField] private float _movementSpeed = 5;
        [SerializeField] private GameObject _attackPre = null;
        [SerializeField] private float _reactionDistance = 10;
        [SerializeField] private float _attackDistance = 1;
        private PlayerController _player = null;

        private GameObject _attack = null;

        void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Start()
        {
            _player = FindObjectOfType<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
            if (distanceToPlayer < _attackDistance)
            {
                if(!_attack)
                    Attack();
            }
            else if (distanceToPlayer < _reactionDistance)
            {
                _characterController.Move((_player.transform.position - transform.position).normalized * Time.deltaTime * _movementSpeed);
            }
        }

        private void Attack()
        {
            _attack = Instantiate(_attackPre, transform.position + transform.up, transform.rotation);
            _attack.GetComponent<BaseAttack>().Attacker = gameObject;
        }

        public void GetAttacked()
        {
            Destroy(gameObject);
        }
    }
}
