using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeScriptAssetGenerator", menuName = "Mazes/Script Asset Generator")]
public class MazeScriptAssetGenerator : ScriptableObject
{
    [SerializeField] private MonoScript mazeScript;
    [SerializeField] private string outputFolder = "Assets/Assets/Prefabs/Mazes";

    public bool TryCreateMazeAsset(out string message)
    {
        message = string.Empty;

        if (mazeScript == null)
        {
            message = "Assign a C# maze script first.";
            return false;
        }

        Type mazeType = mazeScript.GetClass();
        if (mazeType == null || !typeof(Maze).IsAssignableFrom(mazeType) || mazeType.IsAbstract)
        {
            message = "The assigned script must define a concrete class derived from Maze.";
            return false;
        }

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            message = "The output folder does not exist: " + outputFolder;
            return false;
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(
            outputFolder + "/" + mazeType.Name + ".asset");
        Maze maze = (Maze)CreateInstance(mazeType);
        AssetDatabase.CreateAsset(maze, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = maze;
        EditorGUIUtility.PingObject(maze);

        message = "Created " + assetPath;
        return true;
    }
}

[CustomEditor(typeof(MazeScriptAssetGenerator))]
public class MazeScriptAssetGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mazeScript"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputFolder"));
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Create Maze Asset"))
        {
            MazeScriptAssetGenerator generator = (MazeScriptAssetGenerator)target;
            if (generator.TryCreateMazeAsset(out string message))
            {
                Debug.Log(message);
            }
            else
            {
                EditorUtility.DisplayDialog("Maze Asset Generator", message, "OK");
            }
        }
    }
}