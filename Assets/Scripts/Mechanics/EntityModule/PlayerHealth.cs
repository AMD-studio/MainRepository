using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Mechanics.EntityModule
{
    public class PlayerHealth : MonoBehaviour
    {
        public Slider sliderValue;

        private const float maxHealth = 100f;
        [SerializeField]
        private float currentHealth = 100f;

        public float Health
        {
            get { return currentHealth; }
            set
            {
                currentHealth = Mathf.Clamp(value, 0f, maxHealth);

                if (sliderValue != null)
                {
                    sliderValue.value = currentHealth / maxHealth;
                }

                if (currentHealth <= 0)
                {
                    HandleDeath();
                }
            }
        }

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void HandleDeath()
        {
            Debug.Log("Player is dead!");
        }
    }
}
