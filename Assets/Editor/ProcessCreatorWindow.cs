#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static SOProcessData;

public class ProcessCreatorWindow : EditorWindow
{
    // SO fields
    private string processName = "";
    private string description = "";
    private float baseExecutionTime = 1f;
    private int memoryUsage = 0;
    private int encryption = 0;
    private bool removedWhenExecuted;
    private SOProcessData.Enterprise enterprise;

    // Prefab fields
    private GameObject basePrefab;

    // Save locations
    private string soSavePath = "Assets/ScriptableObjects/Processes";
    private string prefabSavePath = "Assets/Prefabs/Processes";
    private string scriptSavePath = "Assets/Scripts/ProcessScripts";

    private Vector2 scroll;

    [MenuItem("Editor Tools/Process Creator")]
    public static void Open()
    {
        GetWindow<ProcessCreatorWindow>("Process Creator");
    }

    void OnEnable()
    {
        basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Processes/processPrefab.prefab");
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
        removedWhenExecuted = EditorGUILayout.Toggle("Removed When Executed", removedWhenExecuted);
        enterprise = (Enterprise)EditorGUILayout.EnumPopup("Enterprise", enterprise);
    }

    void DrawPrefabFields()
    {
        basePrefab = (GameObject)EditorGUILayout.ObjectField(
            "Base Process Prefab", basePrefab, typeof(GameObject), false);

        // show the script name that will be generated
        if (!string.IsNullOrWhiteSpace(processName))
        {
            EditorGUILayout.HelpBox(
                $"Script to generate: {GenerateClassName(processName)}.cs",
                MessageType.Info);
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

        EditorGUILayout.BeginHorizontal();
        scriptSavePath = EditorGUILayout.TextField("Script Save Path", scriptSavePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
            scriptSavePath = BrowseFolder(scriptSavePath);
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

        error = "";
        return true;
    }

    void CreateProcess()
    {
        EnsureDirectory(soSavePath);
        EnsureDirectory(prefabSavePath);
        EnsureDirectory(scriptSavePath);

        string className = GenerateClassName(processName);

        // 1. create the SO
        SOProcessData so = ScriptableObject.CreateInstance<SOProcessData>();
        so.processName = processName;
        so.description = description;
        so.baseExecutionTime = baseExecutionTime;
        so.memoryUsage = memoryUsage;
        so.encryption = encryption;
        so.enterprise = enterprise;
        so.removedWhenExecuted = removedWhenExecuted;
        so.name = processName + "process";

        string soPath = $"{soSavePath}/{processName}process.asset";
        AssetDatabase.CreateAsset(so, soPath);

        // 2. generate the script
        string scriptPath = $"{scriptSavePath}/{className}.cs";
        string scriptContent =
$@"using UnityEngine;

public class {className} : ProcessBase
{{
    public override void Execute(Entity owner, string[] arguments)
    {{
        // {processName} logic here
        base.Execute(owner, arguments);
    }}

    public override void OnKilled()
    {{
        base.OnKilled();
    }}
}}";

        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.ImportAsset(scriptPath);

        // 3. save state to EditorPrefs so we can finish after recompile
        EditorPrefs.SetString("PendingProcessName", processName);
        EditorPrefs.SetString("PendingProcessSOPath", soPath);
        EditorPrefs.SetString("PendingProcessScript", scriptPath);
        EditorPrefs.SetString("PendingPrefabPath", $"{prefabSavePath}/{processName}process.prefab");
        EditorPrefs.SetString("PendingBasePrefabPath", AssetDatabase.GetAssetPath(basePrefab));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); // triggers recompile — OnRecompile finishes the rest
    }

    // runs after every recompile, checks if there's a pending process to finish
    [InitializeOnLoadMethod]
    static void OnRecompile()
    {
        if (!EditorPrefs.HasKey("PendingProcessName")) return;

        // defer until AssetDatabase has finished importing
        EditorApplication.delayCall += FinishProcessCreation;
    }

    static void FinishProcessCreation()
    {
        if (!EditorPrefs.HasKey("PendingProcessName")) return;

        string pendingName = EditorPrefs.GetString("PendingProcessName");
        string soPath = EditorPrefs.GetString("PendingProcessSOPath");
        string prefabPath = EditorPrefs.GetString("PendingPrefabPath");
        string basePrefabPath = EditorPrefs.GetString("PendingBasePrefabPath");

        EditorPrefs.DeleteKey("PendingProcessName");
        EditorPrefs.DeleteKey("PendingProcessSOPath");
        EditorPrefs.DeleteKey("PendingProcessScript");
        EditorPrefs.DeleteKey("PendingPrefabPath");
        EditorPrefs.DeleteKey("PendingBasePrefabPath");

        string className = GenerateClassName(pendingName);
        System.Type type = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == className);

        if (type == null)
        {
            Debug.LogWarning($"Process Creator: could not find type '{className}' after recompile.");
            return;
        }

        GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(basePrefabPath);
        if (basePrefab == null)
        {
            Debug.LogWarning($"Process Creator: could not load base prefab at '{basePrefabPath}'.");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        instance.name = pendingName + "process";
        instance.AddComponent(type);

        GameObject variant = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        DestroyImmediate(instance);

        SOProcessData so = AssetDatabase.LoadAssetAtPath<SOProcessData>(soPath);
        if (so != null)
        {
            so.processObject = variant;
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(so);
            Debug.Log($"Process Creator: '{pendingName}' fully created.");
        }
        else
        {
            Debug.LogWarning($"Process Creator: could not load SO at '{soPath}'.");
        }
    }

    // capitalizes first character and appends "Script"
    static string GenerateClassName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "Script";
        return char.ToUpper(name[0]) + name[1..] + "Script";
    }

    string BrowseFolder(string current)
    {
        string result = EditorUtility.OpenFolderPanel("Select Folder", current, "");
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