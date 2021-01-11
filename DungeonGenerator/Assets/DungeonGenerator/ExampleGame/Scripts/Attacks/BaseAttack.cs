using System.Collections;
using System.Collections.Generic;
using DungeonGenerator.ExampleGame;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    private GameObject _attacker = null;
    public GameObject Attacker {
        set
        {
            if (!_attacker) _attacker = value;
        }
    }

    void Start()
    {
        Invoke("SelfDestruct", 0.5f);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _attacker)
            return;

        other.GetComponent<PlayerController>()?.GetAttacked();
        other.GetComponent<EnemyController>()?.GetAttacked();
    }
}
