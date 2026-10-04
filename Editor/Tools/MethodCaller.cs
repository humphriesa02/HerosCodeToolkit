using UnityEngine;
using UnityEditor;

namespace HerosCode.Toolkit.EditorTools
{
    public class MethodCallerWindow : EditorWindow
    {
        private enum ParamType { None, String, Int, Float, Bool }

        private GameObject target;
        private string methodName = "";
        private ParamType paramType = ParamType.None;
        private string paramValue = "";

        [MenuItem("Tools/HerosCode/Method Caller")]
        public static void ShowWindow()
        {
            GetWindow<MethodCallerWindow>("Method Caller");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Call a method on a GameObject", EditorStyles.boldLabel);

            target = (GameObject)EditorGUILayout.ObjectField("Target", target, typeof(GameObject), true);
            methodName = EditorGUILayout.TextField("Method Name", methodName);
            paramType = (ParamType)EditorGUILayout.EnumPopup("Parameter Type", paramType);

            if (paramType != ParamType.None)
            {
                paramValue = EditorGUILayout.TextField("Value", paramValue);
            }

            EditorGUI.BeginDisabledGroup(target == null || string.IsNullOrEmpty(methodName));
            if (GUILayout.Button("Call"))
            {
                object arg = ParseArg();
                if (arg != null)
                {
                    target.SendMessage(methodName, arg, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                }
                Debug.Log($"Called '{methodName}' on '{target.name}' with arg: {arg ?? "(none)"}");
            }
            EditorGUI.EndDisabledGroup();
        }

        private object ParseArg()
        {
            switch (paramType)
            {
                case ParamType.String: return paramValue;
                case ParamType.Int: return int.TryParse(paramValue, out int i) ? i : (object)null;
                case ParamType.Float: return float.TryParse(paramValue, out float f) ? f : (object)null;
                case ParamType.Bool: return bool.TryParse(paramValue, out bool b) ? b : (object)null;
                default: return null;
            }
        }
    }
}