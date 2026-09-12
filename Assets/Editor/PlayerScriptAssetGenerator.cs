using System;
using UnityEditor;
using UnityEngine;
/* 
    AI DISCLAIMER: 
    The following code was generated with the assistance of an AI tool. 
    While the AI provided a starting point, the final implementation was reviewed and modified by a human developer to ensure accuracy, functionality, and adherence to project requirements. 
    The AI's contribution is acknowledged, but the responsibility for the code's correctness and suitability lies with the human developer.
    This disclaimer is also written by Generative AI.
*/
[CreateAssetMenu(fileName = "PlayerScriptAssetGenerator", menuName = "Players/Script Asset Generator")]
public class PlayerScriptAssetGenerator : ScriptableObject
{
    [SerializeField] private MonoScript playerScript;
    [SerializeField] private string outputFolder = "Assets/Assets/Prefabs/PlayerScripts";

    public bool TryCreatePlayerAsset(out string message)
    {
        message = string.Empty;

        if (playerScript == null)
        {
            message = "Assign a C# player script first.";
            return false;
        }

        Type playerType = playerScript.GetClass();
        if (playerType == null || !typeof(Player).IsAssignableFrom(playerType) || playerType.IsAbstract)
        {
            message = "The assigned script must define a concrete class derived from Player.";
            return false;
        }

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            message = "The output folder does not exist: " + outputFolder;
            return false;
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(
            outputFolder + "/" + playerType.Name + ".asset");
        Player player = (Player)CreateInstance(playerType);
        AssetDatabase.CreateAsset(player, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = player;
        EditorGUIUtility.PingObject(player);

        message = "Created " + assetPath;
        return true;
    }
}

[CustomEditor(typeof(PlayerScriptAssetGenerator))]
public class PlayerScriptAssetGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerScript"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputFolder"));
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Create Player Asset"))
        {
            PlayerScriptAssetGenerator generator = (PlayerScriptAssetGenerator)target;
            if (generator.TryCreatePlayerAsset(out string message))
            {
                Debug.Log(message);
            }
            else
            {
                EditorUtility.DisplayDialog("Player Asset Generator", message, "OK");
            }
        }
    }
}
