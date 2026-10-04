using HerosCode.Toolkit.Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomCustomEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        //Called whenever the inspector is drawn for this object.
        DrawDefaultInspector();

        var room = (Room) target;

        if(GUILayout.Button("Enter Domain")) {
            EnterDomainAsync(room);
        }
        if(GUILayout.Button("Exit Domain")) {
            Debug.Log("Exited domain!");
            Undo.RecordObject(room, "ExitDomain");
            room.ExitDomain();
            EditorUtility.SetDirty(room);
        }
    }

    private async void EnterDomainAsync(Room room)
    {
        Debug.Log("Entered domain!");
        Undo.RecordObject(room, "EnterDomain");
        await room.EnterDomain();
        EditorUtility.SetDirty(room);
    }
}
