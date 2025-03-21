// Unit AI for handling movement and state transitions
using UnityEngine;
using Game.Units;

namespace Game.AI
{
    public class UnitAI : MonoBehaviour
    {
        private Unit unit;
        private void Awake()
        {
            unit = GetComponent<Unit>();
        }
        public void SetDestination(Vector3 targetPosition)
        {
            if (unit != null)
            {
                unit.MoveTo(targetPosition);
            }
        }
    }
}
