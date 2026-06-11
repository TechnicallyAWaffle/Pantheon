using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalProcessesUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset commandTemplate;
    [SerializeField] private Entity entity;

    List<RunningProcess> Processes => entity.localProcessQueue.queue;
    readonly Dictionary<RunningProcess, ProcessUI> processToUI = new();

    readonly Dictionary<int, string> encryptionToStyle = new()
    {
        {0, "none-encryption"},
        {1, "light-encryption"},
        {2, "medium-encryption"},
        {3, "high-encryption"}
    };

    private VisualElement _playerCommands;

    private void Start()
    {
        _playerCommands = uiDocument.rootVisualElement.Q<VisualElement>("PlayerCommands");
        ClearCommands();
    }

    private void ClearCommands()
    {
        var toRemove = new List<VisualElement>();

        foreach (var child in _playerCommands.Children())
        {
            if (child.name != "Empty")
            {
                toRemove.Add(child);
            }
        }

        foreach (var child in toRemove)
        {
            _playerCommands.Remove(child);
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
        TemplateContainer instance = commandTemplate.Instantiate();
        _playerCommands.Add(instance);

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

        _playerCommands.Remove(entry.Root);
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
        string encryptionStyle = encryptionToStyle[encryption];

        foreach (var kvp in encryptionToStyle)
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