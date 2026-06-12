using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalProcessesUI : MonoBehaviour
{
    [SerializeField] protected UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset commandTemplate;
    [SerializeField] private Entity entity;
    [SerializeField] private CompanyToLogoSO companyToLogo;

    List<RunningProcess> Processes => entity.localProcessQueue.queue;
    readonly Dictionary<RunningProcess, ProcessUI> processToUI = new();

    protected virtual Dictionary<int, string> EncryptionToStyle { get; } = new()
    {
        { 0, "none-encryption" },
        { 1, "light-encryption" },
        { 2, "medium-encryption" },
        { 3, "high-encryption" }
    };
    protected VisualElement _commands;

    protected virtual void Start()
    {
        _commands = uiDocument.rootVisualElement.Q<VisualElement>("PlayerCommands");
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
            SetEncryption(proc);
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
        public Image LogoElement;
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
    protected virtual ProcessUI CreateUI(RunningProcess proc)
    {
        TemplateContainer instance = commandTemplate.Instantiate();
        _commands.Add(instance);

        var entry = new ProcessUI
        {
            Root = instance,
            CommandNameLabel = instance.Q<Label>("CommandName"),
            LogoElement = instance.Q<Image>("Logo"),
            ProgressBar = instance.Q<ProgressBar>("ProgressBar"),
            CommandName = proc.data.processName,
            Progress = 0f
        };

        entry.LogoElement.image = companyToLogo.GetLogo(proc.data.enterprise);

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
}