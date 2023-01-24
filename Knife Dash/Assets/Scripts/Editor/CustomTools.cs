using UnityEditor;
using UnityEngine;

public class CustomTools : EditorWindow
{
    [MenuItem("Tools/Enable")]
    public static void Enable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    [MenuItem("Tools/Disable")]
    public static void Disable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneview)
    {
        Handles.BeginGUI();

        if (GUILayout.Button("Rotate Z 180"))
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                obj.transform.Rotate(Vector3.forward * 180);
            }
        }
        if (GUILayout.Button("Rotate Y 180"))
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                obj.transform.Rotate(Vector3.up * 180);
            }
        }
        if (GUILayout.Button("Rotate Z 90"))
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                obj.transform.Rotate(Vector3.forward * 90);
            }
        }


        Handles.EndGUI();
    }
}