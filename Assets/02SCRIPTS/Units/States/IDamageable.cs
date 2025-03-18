namespace Game.Units
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsAlive();
    }
}
