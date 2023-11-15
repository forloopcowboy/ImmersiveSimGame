using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.HealthSystem
{
    public class DeathScreenUIController : MonoBehaviour
    {
        [Required]
        public Health health;

        public RectTransform root;

        private void Update()
        {
            root.gameObject.SetActive(health.isDead);
            if (health.isDead) Cursor.lockState = CursorLockMode.None;
        }
    }
}