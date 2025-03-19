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
            
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            if (!agent.enabled)
            {
                agent.enabled = true; 
            }

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(target);
            }

            unit.PlayAnimation("Run");
        }

        public void UpdateState(Unit unit)
        {
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                unit.ChangeState(new IdleState());
            }
        }

        public void ExitState()
        {
        }
    }
}
