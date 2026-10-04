using HerosCode.Toolkit.Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Actor))]
public class ActorCustomEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        //Called whenever the inspector is drawn for this object.
        DrawDefaultInspector();

        var actor = (Actor) target;

        if(GUILayout.Button("Spawn")) {
            Debug.Log("Actor spawned!");
            Undo.RecordObject(actor, "Spawn");
            actor.Spawn();
            EditorUtility.SetDirty(actor);
        }
        if(GUILayout.Button("Despawn")) {
            Debug.Log("Actor despawned!");
            Undo.RecordObject(actor, "Despawn");
            actor.Despawn();
            EditorUtility.SetDirty(actor);
        }
        if(GUILayout.Button("Save")) {
            Debug.Log("Actor saved!");
            Undo.RecordObject(actor, "Save");
            actor.Save();
            EditorUtility.SetDirty(actor);
        }
        if(GUILayout.Button("Load")) {
            Debug.Log("Actor loaded!");
            Undo.RecordObject(actor, "Load");
            actor.Load();
            EditorUtility.SetDirty(actor);
        }
    }
}
