using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HerosCode.Toolkit.Save;

namespace HerosCode.Toolkit.Core
{
    /// <summary>
    /// An extrapolation layer above a Unity GameObject
    /// that works within our own system.
    /// </summary>
    [DisallowMultipleComponent]
    public class Actor : MonoBehaviour
    {
        [SerializeField, Tooltip("The sole identifier for an actor. Used in saving, container membership, etc.")] 
        private string id;
        [SerializeField, Tooltip("Marking an actor persistant means it will become an addressable, and can be stored in different rooms")]
        private bool persistent;
        public string Id => id;
        /// <summary>
        /// "live" means within the actor system context.
        /// Make alive false when an actor persists in the system
        /// But is not available in the scene
        /// </summary>
        public bool Alive {get; private set;} = false;
        public bool Persistent => persistent;

        /// <summary>
        /// Temp actors should set this to false to get cleaned
        /// up when leaving a room.
        /// Or just in general things that won't get saved to disk
        /// </summary>
        public bool ShouldSave {get; private set;} = true;

        public event Action OnCreated;
        public event Action OnDeleted;
        public event Action OnSpawned;
        public event Action OnDespawned;
        
        // Called to spawn the actor the first time,
        // assigns id, etc.
        public void Create()
        {
            OnCreated?.Invoke();
        }

        // Fully getting rid of the actor, id gets taken out of
        // registry, etc.
        public void Delete()
        {
            if (Alive) Despawn();
            OnDeleted?.Invoke();
        }

        // bring the actor back after removal
        public void Spawn()
        {
            if (Alive) return;
            Alive = true;
            gameObject.SetActive(true);
            OnSpawned?.Invoke();
        }
        // Momentarily remove the actor
        public void Despawn()
        {
            if (!Alive) return;
            Alive = false;
            gameObject.SetActive(false);
            OnDespawned?.Invoke();
        }

        /// <summary>
        /// Serialize the Actor
        /// </summary>
        public void Save()
        {
            ActorSaveData saveData = new()
            {
                // Actor level data
                alive = Alive,
                position = new SerializableVector3(transform.position),
                rotation = new SerializableQuaternion(transform.rotation),
                scale = new SerializableVector3(transform.localScale)
            };

            // Component level data
            ISaveable[] saveables = GetComponents<ISaveable>();
            Dictionary<string, SaveData> componentData = new();
            foreach(ISaveable save in saveables)
            {
                componentData[save.SaveKey] = save.Save();
            }
            saveData.components = componentData;

            SaveSystem.Instance.Save<ActorSaveData>(SaveDataTypes.actor.ToString(), id, saveData);
        }

        /// <summary>
        /// Deserialize the Actor
        /// </summary>
        public void Load()
        {
            ActorSaveData saveData = SaveSystem.Instance.Load<ActorSaveData>(SaveDataTypes.actor.ToString(), id) ?? throw new Exception("Tried to load an actor that wasn't saved!");

            // Actor level stuff
            Alive = saveData.alive;
            transform.SetPositionAndRotation(saveData.position.ToVector3(), saveData.rotation.ToQuaternion());
            transform.localScale = saveData.scale.ToVector3();

            // Component state
            ISaveable[] saveables = GetComponents<ISaveable>();
            if (saveables.Length == 0) return;
            foreach(ISaveable save in saveables)
            {
                save.Load(saveData.components[save.SaveKey]);
            }
        }
        
        #if UNITY_EDITOR
        private void Reset()
        {
            id = GenerateSnakeCaseIdFromCamelCaseName();
        }
        #endif

        [ContextMenu("Regenerate Id")]
        private void RegenerateId()
        {
            id = GenerateSnakeCaseIdFromCamelCaseName();
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        /// <summary>
        /// Requires GameObject names to be written in CamelCase
        /// </summary>
        /// <returns></returns>
        private string GenerateSnakeCaseIdFromCamelCaseName()
        {
            var objectName = gameObject.name;
            if (string.IsNullOrEmpty(objectName)) return objectName;

            if (objectName.Length < 2) return objectName.ToLowerInvariant();

            var sb = new StringBuilder();
            // Actor specific
            sb.Append("actor_");

            sb.Append(char.ToLowerInvariant(objectName[0]));

            for (var i = 1; i < objectName.Length; ++i)
            {
                char c = objectName[i];
                if (char.IsUpper(c))
                {
                    if (objectName[i-1] != '_')
                    {
                        sb.Append('_');
                    }
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}