namespace Game.Units.States
{
    // Interface for Unit States
    public interface IUnitState
    {
        void EnterState(Unit unit);
        void UpdateState(Unit unit);
        void ExitState(Unit unit);
    }
}