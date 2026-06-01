#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ProcessCreatorWindow : EditorWindow
{
    // SO fields
    private string processName = "";
    private string description = "";
    private float baseExecutionTime = 1f;
    private int memoryUsage = 0;
    private int encryption = 0;

    // Prefab fields
    private GameObject basePrefab;
    private MonoScript childScript;

    // Save locations
    private string soSavePath = "Assets/Data/Processes";
    private string prefabSavePath = "Assets/Prefabs/Processes";

    private Vector2 scroll;

    [MenuItem("Editor Tools/Process Creator")]
    public static void Open()
    {
        GetWindow<ProcessCreatorWindow>("Process Creator");
    }

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawHeader("Process Data");
        DrawSOFields();

        EditorGUILayout.Space(10);

        DrawHeader("Prefab Setup");
        DrawPrefabFields();

        EditorGUILayout.Space(10);

        DrawHeader("Save Locations");
        DrawPathFields();

        EditorGUILayout.Space(20);

        DrawCreateButton();

        EditorGUILayout.EndScrollView();
    }

    void DrawHeader(string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    void DrawSOFields()
    {
        processName = EditorGUILayout.TextField("Process Name", processName);
        description = EditorGUILayout.TextField("Description", description);
        baseExecutionTime = EditorGUILayout.FloatField("Base Execution Time", baseExecutionTime);
        memoryUsage = EditorGUILayout.IntField("Memory Usage", memoryUsage);
        encryption = EditorGUILayout.IntField("Encryption", encryption);
    }

    void DrawPrefabFields()
    {
        basePrefab = (GameObject)EditorGUILayout.ObjectField(
            "Base Process Prefab", basePrefab, typeof(GameObject), false);

        childScript = (MonoScript)EditorGUILayout.ObjectField(
            "Child Script", childScript, typeof(MonoScript), false);

        // warn if the selected script doesn't inherit from ProcessBase
        if (childScript != null)
        {
            System.Type type = childScript.GetClass();
            if (type == null || !type.IsSubclassOf(typeof(ProcessBase)))
            {
                EditorGUILayout.HelpBox(
                    "Selected script does not inherit from ProcessBase.",
                    MessageType.Warning);
            }
        }
    }

    void DrawPathFields()
    {
        EditorGUILayout.BeginHorizontal();
        soSavePath = EditorGUILayout.TextField("SO Save Path", soSavePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
            soSavePath = BrowseFolder(soSavePath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        prefabSavePath = EditorGUILayout.TextField("Prefab Save Path", prefabSavePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
            prefabSavePath = BrowseFolder(prefabSavePath);
        EditorGUILayout.EndHorizontal();
    }

    void DrawCreateButton()
    {
        bool valid = Validate(out string error);

        if (!valid)
            EditorGUILayout.HelpBox(error, MessageType.Error);

        EditorGUI.BeginDisabledGroup(!valid);
        if (GUILayout.Button("Create Process", GUILayout.Height(40)))
            CreateProcess();
        EditorGUI.EndDisabledGroup();
    }

    bool Validate(out string error)
    {
        if (string.IsNullOrWhiteSpace(processName))
        { error = "Process name is required."; return false; }

        if (basePrefab == null)
        { error = "Base prefab is required."; return false; }

        if (childScript == null)
        { error = "Child script is required."; return false; }

        System.Type type = childScript.GetClass();
        if (type == null || !type.IsSubclassOf(typeof(ProcessBase)))
        { error = "Child script must inherit from ProcessBase."; return false; }

        error = "";
        return true;
    }

    void CreateProcess()
    {
        // ensure save directories exist
        EnsureDirectory(soSavePath);
        EnsureDirectory(prefabSavePath);

        // 1. create the SO
        SOProcessData so = ScriptableObject.CreateInstance<SOProcessData>();
        so.processName = processName;
        so.description = description;
        so.baseExecutionTime = baseExecutionTime;
        so.memoryUsage = memoryUsage;
        so.encryption = encryption;

        string soPath = $"{soSavePath}/{processName}_Data.asset";
        AssetDatabase.CreateAsset(so, soPath);

        // 2. create the prefab variant
        string prefabPath = $"{prefabSavePath}/{processName}_Prefab.prefab";

        // instantiate base prefab, add child script, save as variant
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        instance.name = processName;
        instance.AddComponent(childScript.GetClass());

        GameObject variant = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        DestroyImmediate(instance);

        // 3. assign the variant to the SO
        so.processObject = variant;
        EditorUtility.SetDirty(so);

        // save everything
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // ping both new assets in the project window
        EditorGUIUtility.PingObject(so);

        Debug.Log($"Created process: {processName}\n  SO: {soPath}\n  Prefab: {prefabPath}");
    }

    string BrowseFolder(string current)
    {
        string result = EditorUtility.OpenFolderPanel("Select Folder", current, "");
        // convert absolute path to relative
        if (result.StartsWith(Application.dataPath))
            return "Assets" + result.Substring(Application.dataPath.Length);
        return current;
    }

    void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
#endif