using HerosCode.Toolkit.Core;
using UnityEngine;

namespace HerosCode.Toolkit.Gameplay.Combat
{
    public class HealthSaveData : SaveData
    {
        public float maxHealth;
        public float currentHealth;
    }

    /// <summary>
    /// Basic health system. When currentHealth falls
    /// below 0, despawn the actor.
    /// </summary>
    [RequireComponent(typeof(Actor))]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth;
        private Actor actor;

        void Awake()
        {
            actor = GetComponent<Actor>();
            actor.OnSpawned += OnSpawn;
        }

        public void TakeDamage(float amount)
        {
            if (!actor.Alive) return;

            currentHealth = Mathf.Clamp(currentHealth - amount, 0.0f, maxHealth);
            if (currentHealth <= 0.0f)
            {
                actor.Despawn();
            }
        }

        public void Heal(float amount)
        {
            if (!actor.Alive) return;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0.0f, maxHealth);
        }

        private void OnSpawn()
        {
            currentHealth = maxHealth;
        }

        /// --------------- Saving -------------------
        public string SaveKey => "health";

        public SaveData Save()
        {
            HealthSaveData saveData = new()
            {
                currentHealth = currentHealth,
                maxHealth = maxHealth,
            };
            return saveData;
        }
        
        public void Load(SaveData data)
        {
            HealthSaveData healthSaveData = (HealthSaveData)data;
            currentHealth = healthSaveData.currentHealth;
            maxHealth = healthSaveData.maxHealth;
        }
    }
}
