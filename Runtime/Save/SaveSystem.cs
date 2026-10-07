using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HerosCode.Toolkit.Save
{
    /// <summary>
    /// A base SaveData class, for ease of use in
    /// containers and things.
    /// Data to be saved will extend off of this.
    /// </summary>
    [Serializable]
    public abstract class SaveData { }

    /// <summary>
    /// Each component that extends this will get picked up
    /// by its owning actor when it goes to save
    /// </summary>
    public interface ISaveable
    {
        string SaveKey {get;} // The key to save this component in the data

        SaveData Save();
        void Load(SaveData data);
    }

    /// <summary>
    /// Each one has to correlate to a file in
    /// ./Savers/...
    /// </summary>
    public enum DataSaverType
    {
        JSON, // Default
        Database,
        Cloud
    }

    /// <summary>
    /// A per *Unity.Scene* SaveSystem. Creates and manages a save instance
    /// (dependent on DataSaverType, a .json file for example).
    /// 
    /// It is per Scene due to our use of stack-based memory stores to speed
    /// up loading; once a Scene loads we unpack its individual save instance,
    /// pulling that data into memory. We then clear that memory when leaving each
    /// scene. Scenes can be as basic or as advanced as needed.
    /// 
    /// Note - this class is a singleton so any code can access it quickly.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        // Singleton, each scene registers itself as the active singleton
        // when it loads up
        public static SaveSystem Instance;
        [SerializeField] private DataSaverType dataSaverType = DataSaverType.JSON;

        // Name of the save. If not set directly will be set based on
        // the name of the scene.
        [SerializeField] private string saveName;
        private DataSaver saver;

        private void Awake()
        {
            Instance = this;

            switch (dataSaverType)
            {
                case DataSaverType.JSON:
                    saver = new JSONDataSaver(saveName);
                    break;
                default:
                    saver = new JSONDataSaver(saveName);
                    break;
            }
            saver.Init();
        }

        /// <summary>
        /// Saves a given entity's ISaveable data to its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="save"></param>
        public void Save<T>(string type, string id, T data) where T : SaveData => saver.Save(type, id, data);


        /// <summary>
        /// Load a given entity's ISaveable data via its id
        /// </summary>
        public T Load<T>(string type, string id) where T : SaveData => saver.Load<T>(type, id);


        /// <summary>
        /// True if the entity exists in our save file
        /// otherwise it needs to get saved away
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Has(string type, string id) => saver.Has(type, id);

        #if UNITY_EDITOR
        private void Reset()
        {
            saveName = GenerateSnakeCaseSaveNameFromCamelCaseName();
        }
        #endif

        private string GenerateSnakeCaseSaveNameFromCamelCaseName()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            if (string.IsNullOrEmpty(sceneName)) return sceneName;

            if (sceneName.Length < 2) return sceneName.ToLowerInvariant();

            var sb = new StringBuilder();
            // Actor specific
            sb.Append("save_");

            sb.Append(char.ToLowerInvariant(sceneName[0]));

            for (var i = 1; i < sceneName.Length; ++i)
            {
                char c = sceneName[i];
                if (char.IsUpper(c))
                {
                    if (sceneName[i-1] != '_')
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

