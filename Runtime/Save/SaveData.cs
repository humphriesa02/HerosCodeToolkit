using System;
using System.Collections.Generic;
using UnityEngine;

namespace HerosCode.Toolkit.Save
{
    /// <summary>
    /// The actual typing we'll save to in our
    /// save store
    /// </summary>
    public enum SaveDataTypes
    {
        actor,
        room
    }

    /// <summary>
    /// Our own version of Vec3, to ignore unneeded
    /// data that gets piled on the original (normalized, magnitude, etc)
    /// </summary>
    [Serializable]
    public struct SerializableVector3
    {
        public float x, y, z;

        public SerializableVector3(Vector3 v)
        {
            x = v.x; y = v.y; z = v.z;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
    }

    /// <summary>
    /// Our own version of Quat, to ignore unneeded
    /// data that gets piled on the original
    /// </summary>
    [Serializable]
    public struct SerializableQuaternion
    {
        public float x, y, z, w;

        public SerializableQuaternion(Quaternion q)
        {
            x = q.x; y = q.y; z = q.z; w = q.w;
        }

        public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
    }

    [Serializable]
    public class ActorSaveData : SaveData
    {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public SerializableVector3 scale;
        public bool alive;
        public Dictionary<string, SaveData> components;
    }

    public class RoomSaveData : SaveData
    {
        public List<string> actorIds;
    }
}
