using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerProcessesUI : MonoBehaviour
{
    protected Dictionary<int, string> EnemyEncryptionToStyle { get; } = new()
    {
        { 0, "enemy-none-encryption" },
        { 1, "enemy-custom-light" },
        { 2, "enemy-custom-medium" },
        { 3, "enemy-custom-high" }
    };
    [SerializeField] protected UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset playerCommandTemplate;
    [SerializeField] private VisualTreeAsset enemyCommandTemplate;

    [SerializeField] private ProcessQueue serverQueue;
    [SerializeField] private ReferenceManager referenceManager;

    List<RunningProcess> Processes => serverQueue.queue;
    readonly Dictionary<RunningProcess, ProcessUI> processToUI = new();

    protected virtual Dictionary<int, string> EncryptionToStyle { get; } = new()
    {
        { 0, "none-encryption" },
        { 1, "light-encryption" },
        { 2, "medium-encryption" },
        { 3, "high-encryption" }
    };
    
    protected VisualElement _commands;
    protected void Start()
    {
        _commands = uiDocument.rootVisualElement.Q<VisualElement>("ServerProcessQueue");
        ClearCommands();
    }

    protected void ClearCommands()
    {
        var toRemove = new List<VisualElement>();

        foreach (var child in _commands.Children())
        {
            if (child.name != "Empty")
            {
                toRemove.Add(child);
            }
        }

        foreach (var child in toRemove)
        {
            _commands.Remove(child);
        }
    }

    void Update()
    {
        // Add UI for any new processes
        foreach (var proc in Processes)
        {
            if (!processToUI.ContainsKey(proc))
            {
                processToUI.Add(proc, CreateUI(proc));
            }

            // Update progress
            var ratio = proc.timeRemaining / proc.baseTime;
            var progress = (1f - ratio) * 100f; // fills as time runs out
            SetProgress(proc, progress);

            // TODO: check suspension
            // TODO: check encryption
            if (proc.owner == referenceManager.player)
            {
                SetEncryption(proc);
            }
            else
            {
                SetEnemyEncryption(proc);
            }
        }

        // Remove UI for any processes that have left the queue
        // Build removal list first — can't modify dictionary while iterating it
        var toRemove = new List<RunningProcess>();
        foreach (var key in processToUI.Keys)
        {
            if (!Processes.Contains(key))
            {
                toRemove.Add(key);
            }
        }

        foreach (var proc in toRemove)
        {
            RemoveUI(proc);
        }
    }

    // Wraps one instantiated template and its tracked data
    public class ProcessUI
    {
        // UI references
        public TemplateContainer Root;
        public Label CommandNameLabel;
        public VisualElement LogoElement;
        public ProgressBar ProgressBar;

        // Tracked data — use properties to keep UI in sync
        private string _commandName;
        private float _progress;

        public string CommandName
        {
            get => _commandName;
            set
            {
                _commandName = value;
                if (CommandNameLabel != null)
                {
                    CommandNameLabel.text = value;
                }
            }
        }

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = Mathf.Clamp(value, 0f, 100f);
                if (ProgressBar != null)
                {
                    ProgressBar.value = _progress;
                }
            }
        }
    }

    // Creates a new UI box for a process
    private ProcessUI CreateUI(RunningProcess proc)
    {
        TemplateContainer instance = null;
        if (proc.owner == referenceManager.player)
        {
            instance = playerCommandTemplate.Instantiate();
        }
        else
        {
            instance = enemyCommandTemplate.Instantiate();
        }
        _commands.Add(instance);

        var entry = new ProcessUI
        {
            Root = instance,
            CommandNameLabel = instance.Q<Label>("CommandName"),
            LogoElement = instance.Q<VisualElement>("Logo"),
            ProgressBar = instance.Q<ProgressBar>("ProgressBar"),
            // Use the properties so the UI reflects the initial values
            CommandName = proc.data.processName,
            // TODO: set logo for proc
            Progress = 0f
        };

        return entry;
    }

    // Removes the UI box for a process and cleans up the dictionary entry
    private void RemoveUI(RunningProcess proc)
    {
        if (!processToUI.TryGetValue(proc, out var entry))
        {
            return;
        }

        _commands.Remove(entry.Root);
        processToUI.Remove(proc);
    }

    // Updates progress bar for a process by its RunningProcess key
    private void SetProgress(RunningProcess proc, float value)
    {
        if (!processToUI.TryGetValue(proc, out var entry))
        {
            return;
        }

        entry.Progress = value;
    }

    private void SetEncryption(RunningProcess proc)
    {
        if (!processToUI.TryGetValue(proc, out var entry))
        {
            return;
        }

        int encryption = EncryptionManager.GetEncryption(proc);
        string encryptionStyle = EncryptionToStyle[encryption];

        foreach (var kvp in EncryptionToStyle)
        {
            if (kvp.Value == encryptionStyle)
            {
                entry.Root.EnableInClassList(encryptionStyle, true);
            }
            else
            {
                entry.Root.EnableInClassList(kvp.Value, false);
            }
        }
    }

    private void SetEnemyEncryption(RunningProcess proc)
    {
        if (!processToUI.TryGetValue(proc, out var entry))
        {
            return;
        }

        int encryption = EncryptionManager.GetEncryption(proc);
        string encryptionStyle = EnemyEncryptionToStyle[encryption];

        foreach (var kvp in EnemyEncryptionToStyle)
        {
            if (kvp.Value == encryptionStyle)
            {
                entry.Root.EnableInClassList(encryptionStyle, true);
            }
            else
            {
                entry.Root.EnableInClassList(kvp.Value, false);
            }
        }
    }
}
