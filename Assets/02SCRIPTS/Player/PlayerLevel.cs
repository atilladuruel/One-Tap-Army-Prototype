using System;
namespace Game.Player
{
    public class PlayerLevel
    {
        public int level = 1;
        public int experience = 0;
        public int expThreshold = 100;
        private PlayerController player;

        public event Action<int> OnLevelUp;

        public PlayerLevel(PlayerController player)
        {
            this.player = player;
        }

        public void AddExperience(int amount)
        {
            experience += amount;
            if (experience >= expThreshold)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            experience = 0;
            expThreshold += 50;
            OnLevelUp?.Invoke(level);
        }
    }
}
