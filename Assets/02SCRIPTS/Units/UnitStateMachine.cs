using Game.Units.States;

namespace Game.Units
{
    public class UnitStateMachine
    {
        private IUnitState currentState;
        private Unit unit;

        public UnitStateMachine(Unit unit)
        {
            this.unit = unit;
        }

        public void ChangeState(IUnitState newState)
        {
            currentState?.ExitState(unit);
            currentState = newState;
            currentState.EnterState(unit);
        }

        public void Update()
        {
            currentState?.UpdateState(unit);
        }
    }
}
