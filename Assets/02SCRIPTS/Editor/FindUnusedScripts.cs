using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class FindUnusedScripts : EditorWindow
{
    private List<MonoScript> unusedScripts = new List<MonoScript>();
    private bool isSearching = false;

    [MenuItem("Tools/Find Unused Scripts")]
    public static void ShowWindow()
    {
        GetWindow<FindUnusedScripts>("Find Unused Scripts");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Unused Scripts") && !isSearching)
        {
            FindScripts();
        }

        if (isSearching)
        {
            EditorGUILayout.LabelField("Searching... Please wait.", EditorStyles.boldLabel);
        }
        else if (unusedScripts.Count > 0)
        {
            EditorGUILayout.LabelField("Unused Scripts:", EditorStyles.boldLabel);
            foreach (var script in unusedScripts)
            {
                EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
            }

            if (GUILayout.Button("Delete All Unused Scripts"))
            {
                DeleteUnusedScripts();
            }
        }
    }

    private void FindScripts()
    {
        isSearching = true;
        unusedScripts.Clear();

        // Find all script files in the project
        string[] allScriptGUIDs = AssetDatabase.FindAssets("t:Script");
        List<string> scriptPaths = allScriptGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToList();

        // Find all used scripts in scenes, prefabs, and assets
        string[] usedGUIDs = AssetDatabase.FindAssets("t:GameObject t:Prefab t:Scene");
        HashSet<string> usedScripts = new HashSet<string>();

        foreach (string guid in usedGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileContent = System.IO.File.ReadAllText(path);

            foreach (string scriptPath in scriptPaths)
            {
                string scriptName = System.IO.Path.GetFileNameWithoutExtension(scriptPath);
                if (fileContent.Contains(scriptName))
                {
                    usedScripts.Add(scriptPath);
                }
            }
        }

        // Find scripts that are NOT used anywhere
        foreach (string scriptPath in scriptPaths)
        {
            if (!usedScripts.Contains(scriptPath))
            {
                MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                if (scriptAsset != null)
                    unusedScripts.Add(scriptAsset);
            }
        }

        Debug.Log($"Found {unusedScripts.Count} unused scripts.");
        isSearching = false;
        Repaint();
    }

    /// <summary>
    /// Deletes all unused scripts from the project.
    /// </summary>
    private void DeleteUnusedScripts()
    {
        foreach (MonoScript script in unusedScripts)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(script));
        }

        unusedScripts.Clear();
        AssetDatabase.Refresh();
        Debug.Log("All unused scripts have been deleted.");
    }
}
