using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using HerosCode.Toolkit.Save;

namespace HerosCode.Toolkit.Core
{
    public enum RoomSaveType
    {
        ExternalSave, // Won't automatically save, but game supports saving
        SaveOnExit // Automatically saves on room exit
    }
    public class Room : MonoBehaviour
    {
        [SerializeField, Tooltip("Use this to partition out the current Scene into individual rooms, rather than one big Scene level room.")]
        private Transform domainRoot;
        // Only counts as live ref, when player is in room.
        // Disk is definitive state
        private List<Actor> actorsInRoom = new();

        // Set true to save on first enter
        // subsequent saves handled by roomsavetype
        [SerializeField, Tooltip("If true allows the room to be saved.")]
        private bool allowsSaving = false;

        [SerializeField, Tooltip("The type of saving the room will use")]
        private RoomSaveType roomSaveType;

        [SerializeField] private string id;
        public string Id => id;

        /// <summary>
        /// Called when the player enters the room,
        /// either via moving into the rooms bounds,
        /// or if Scene level, on SceneEntered
        /// </summary>
        public async Task EnterDomain()
        {
            actorsInRoom.Clear();

            // Save system branch
            if (allowsSaving)
            {
                // First entry of a room, creating the entry for it
                // in the save file.
                if (!SaveSystem.Instance.Has("room", id))
                {
                    // Collect the actors in the room from hierarchy
                    actorsInRoom = domainRoot.GetComponentsInChildren<Actor>().ToList();
                    // Call "Create" on each one.
                    foreach(Actor actor in actorsInRoom)
                    {
                        actor.Create();
                    }
                    Save();
                }
                else
                {
                    await Load();
                }
            }
            else
            {
                // Collect the actors in the room from hierarchy
                actorsInRoom = domainRoot.GetComponentsInChildren<Actor>().ToList();
                // Non save system branch, always a fresh room
                foreach(Actor actor in actorsInRoom)
                {
                    actor.Create();
                }
            }
            
            // Call "Spawn" on each one.
            foreach(Actor actor in actorsInRoom)
            {
                actor.Spawn();
            }
        }

        /// <summary>
        /// Called when the player exits the room,
        /// either via moving out of the rooms bounds,
        /// or if Scene level, on SceneExited
        /// </summary>
        public void ExitDomain()
        {
            // Loop thorugh domain root, identifying actors within
            Actor[] domainActors = domainRoot.GetComponentsInChildren<Actor>();

            // Save the room to disk
            if (allowsSaving && roomSaveType == RoomSaveType.SaveOnExit)
            {
                Save();
            }

            // Call "Despawn" on each one.
            foreach(Actor actor in domainActors)
            {
                actor.Despawn();
                if (!actor.ShouldSave)
                {
                    actor.Delete();
                }
            }
        }

        /// <summary>
        /// Saves the room state to memory
        /// </summary>
        public void Save()
        {
            RoomSaveData saveData = new();
            Actor[] domainActors = domainRoot.GetComponentsInChildren<Actor>();
            List<string> idList = new();
            foreach(Actor actor in domainActors)
            {
                if (!actor.ShouldSave) continue;
                actor.Save();
                idList.Add(actor.Id);
            }
            saveData.actorIds = idList;
            SaveSystem.Instance.Save<RoomSaveData>(SaveDataTypes.room.ToString(), id, saveData);
        }

        /// <summary>
        /// Loads the definitive room state from memory
        /// </summary>
        public async Task Load()
        {
            RoomSaveData roomSaveData = SaveSystem.Instance.Load<RoomSaveData>(SaveDataTypes.room.ToString(), id);

            // Map actors to their ids from the domainRoot
            Dictionary<string, Actor> existingById = domainRoot.GetComponentsInChildren<Actor>(true).ToDictionary(a => a.Id, a => a);

            foreach (string actorId in roomSaveData.actorIds)
            {
                // Scenario 1: actor id is in the room, activate it
                if (existingById.TryGetValue(actorId, out Actor actor))
                {
                    actor.gameObject.SetActive(true);
                    existingById.Remove(actorId);
                }
                else // Scenario 2: actor id not in room, spawn via Addressables
                {
                    GameObject instance = await Addressables.InstantiateAsync(actorId, domainRoot).Task;
                    instance.SetActive(false);
                    actor = instance.GetComponent<Actor>();
                }
                // Populate the actor with data from disk
                actor.Load();
                actor.gameObject.SetActive(true);
                actorsInRoom.Add(actor);
            }

            // Deactivate any actor not in the save data's id list
            foreach (Actor leftover in existingById.Values)
            {
                leftover.gameObject.SetActive(false);
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
            sb.Append("room_");
            
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
