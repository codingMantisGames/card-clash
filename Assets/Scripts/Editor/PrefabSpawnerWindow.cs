using UnityEditor;
using UnityEngine;

public class PrefabSpawnerWindow : EditorWindow
{
    #region VARIABLES
    private GameObject prefab; // Target parent object
    #endregion

    #region FUNCTIONS
    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSpawnerWindow>("Prefab Spawner");
    }

    private void OnEnable()
    {
        // Subscribe to the SceneView drawing callback
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent errors when the window is closed
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeTransform == null)
            return;

        for (int i = 0; i < Selection.activeTransform.childCount; i++)
        {
            Handles.color = Color.blue;
            if (Handles.Button(Selection.activeTransform.GetChild(i).position, Quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap))
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, Selection.activeTransform.parent);

                instance.transform.position = Selection.activeTransform.GetChild(i).position;
                instance.transform.rotation = Selection.activeTransform.GetChild(i).rotation;

                Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Hex Tile Editor", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Parent Object", prefab, typeof(GameObject), true);


    }
    #endregion
}
