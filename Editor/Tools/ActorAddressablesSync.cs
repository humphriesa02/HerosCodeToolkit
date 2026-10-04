using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.EditorTools
{
    public static class ActorAddressableSync
    {
        [MenuItem("Tools/HerosCode/Sync Actor Addressables")]
        public static void SyncActorAddressables()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("No AddressableAssetSettings found. Open the Addressables Groups window at least once to create it.");
                return;
            }

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            int synced = 0, skipped = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Actor actor = prefab != null ? prefab.GetComponent<Actor>() : null;

                if (actor == null || !actor.Persistent) continue; // only relocatable/persistent actors need this
                if (string.IsNullOrEmpty(actor.Id))
                {
                    Debug.LogWarning($"{path}: Actor has no Id assigned — skipping.", prefab);
                    skipped++;
                    continue;
                }

                AddressableAssetEntry entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                }

                if (entry.address != actor.Id)
                {
                    entry.SetAddress(actor.Id);
                    synced++;
                    Debug.Log($"Synced address for '{prefab.name}' -> '{actor.Id}'", prefab);
                }
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, null, true);
            AssetDatabase.SaveAssets();

            Debug.Log($"Addressable sync complete. {synced} entries updated, {skipped} skipped (missing id).");
        }
    }
}