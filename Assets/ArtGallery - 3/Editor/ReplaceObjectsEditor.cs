using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReplaceObjects))]
public class ReplaceObjectsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ReplaceObjects myScript = (ReplaceObjects)target;
        if(GUILayout.Button("Replace Child Objects"))
        {
            myScript.ReplaceChildObjects();
        }
    }
}
