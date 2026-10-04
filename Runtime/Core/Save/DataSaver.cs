namespace HerosCode.Toolkit.Core
{
    /// <summary>
    /// Provides abstract concepts of saving for individual
    /// save systems to extend.
    /// </summary>
    public abstract class DataSaver
    {
        /// <summary>
        /// Initialize the save file.
        /// Create JSON file/db/cloud sync/whatever
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// Delete the save file
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// For something like a db, one time startup
        /// </summary>
        public abstract void Activate();
        /// <summary>
        /// For something like a db, one time shutdown
        /// </summary>
        public abstract void Deactivate();

        /// <summary>
        /// Per typing "save"
        /// At a base, maps an id
        /// within our "thing" to a dictionary
        /// esque "SaveData"
        /// </summary>
        public abstract void Save<T>(string type, string id, T data) where T : SaveData;

        /// <summary>
        /// Per typing "Load"
        /// At a base, given an id,
        /// retrieve the data.
        /// </summary>
        public abstract T Load<T>(string type, string id) where T : SaveData;

        /// <summary>
        /// Determine if the given data is stored
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool Has(string type, string id);
    }

}
