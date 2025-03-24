using UnityEngine;
using UnityEngine.AI;

namespace Game.Units.States
{
    public class MoveState : IUnitState
    {
        private Vector3 target;
        public MoveState(Vector3 targetPosition)
        {
            target = targetPosition;
        }

        public void EnterState(Unit unit)
        {
            Debug.Log($"{unit.name} has entered Move State.");

            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            if (!agent.enabled && unit.gameObject.activeInHierarchy)
            {
                agent.enabled = true;
            }

            if (agent.isOnNavMesh && agent.enabled)
            {
                agent.Warp(unit.transform.position); // Ensure agent is properly placed
                agent.SetDestination(target);
            }

            unit.PlayAnimation("Run");
        }

        public void UpdateState(Unit unit)
        {
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance > 0.1f && agent.remainingDistance <= agent.stoppingDistance)
            {
                unit.ChangeState(new IdleState());
            }
        }

        public void ExitState(Unit unit)
        {
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false; // Disable NavMeshAgent on exit
            }
        }
    }
}
