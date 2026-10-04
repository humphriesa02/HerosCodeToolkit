using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HerosCode.Toolkit.Core
{
    /// <summary>
    /// Saves data into a JSON file.
    /// Also provides capability of JSON parsing to parse save data.
    /// 
    /// JSON Package: Newtonsoft
    /// </summary>
    public class JSONDataSaver : DataSaver
    {
        JsonSerializerSettings settings;
        private Dictionary<string, Dictionary<string, object>> store;
        private string filePath;
        private readonly string fileName;

        public JSONDataSaver(string fileName)
        {
            this.fileName = fileName;
        }

        public override void Init()
        {
            settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            filePath = Path.Combine(Application.persistentDataPath, fileName+".json");

            if (File.Exists(filePath))
            {
                string raw = File.ReadAllText(filePath);
                store = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(raw, settings)
                         ?? new Dictionary<string, Dictionary<string, object>>();
            }
            else
            {
                store = new Dictionary<string, Dictionary<string, object>>();
            }
        }

        public override void Delete()
        {
            // Check if doc exists local
            // if it does delete the doc
            if (File.Exists(filePath)) File.Delete(filePath);
            store = new Dictionary<string, Dictionary<string, object>>();
        }

        // TBD if needed
        public override void Activate(){ return; }

        // TBD if needed
        public override void Deactivate(){ return; }

        public override void Save<T>(string type, string id, T data)
        {
            if (!store.TryGetValue(type, out var bucket))
            {
                bucket = new Dictionary<string, object>();
                store[type] = bucket;
            }
            bucket[id] = data;
            Flush();
        }  

        public override T Load<T>(string type, string id)
        {
            if (store.TryGetValue(type, out var bucket) && bucket.TryGetValue(id, out var raw))
            {
                if (raw is T direct) return direct;

                // After a real file round-trip, Newtonsoft hands back JObject
                // rather than the original concrete type — reconstruct it.
                return JObject.FromObject(raw).ToObject<T>(); 
            }
            return null;
        }

        public override bool Has(string type, string id)
        {
            return store.TryGetValue(type, out var bucket) && bucket.ContainsKey(id);
        }

        private void Flush()
        {
            string raw = JsonConvert.SerializeObject(store, settings);
            File.WriteAllText(filePath, raw);
        }
    }
}
