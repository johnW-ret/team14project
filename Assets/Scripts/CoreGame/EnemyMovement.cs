using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] [HideInInspector] private NavMeshAgent nmAgent;

    private void Reset()
    {
        SetRefernces();
    }

    [ContextMenu("Set References")]
    private void SetRefernces()
    {
        if (!nmAgent)
            nmAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (target)
            nmAgent.destination = target.position;
    }
}
