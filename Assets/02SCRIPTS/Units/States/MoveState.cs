using TMPro;
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
            unit.PlayAnimation("Run"); 
            unit.GetComponent<NavMeshAgent>().SetDestination(target);
        }
        public void UpdateState(Unit unit)
        {
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                unit.ChangeState(new IdleState());
            }
        }
        public void ExitState() { }
    }
}
