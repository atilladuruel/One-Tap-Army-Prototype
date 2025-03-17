using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public GameObject pauseMenu;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void TogglePauseMenu(bool isActive)
        {
            pauseMenu.SetActive(isActive);
        }
    }
}