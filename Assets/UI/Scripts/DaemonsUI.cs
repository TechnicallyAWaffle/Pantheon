using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;


public class DaemonsUI : MonoBehaviour
{
    [SerializeField] protected UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset daemonTemplate;
    [SerializeField] private CompanyToLogoSO companyToLogo;
    [SerializeField] private Entity entity;

    List<DaemonBase> Daemons => entity.daemons;
    readonly Dictionary<DaemonBase, DaemonUI> daemonToUI = new();

    protected VisualElement _daemonContainer;

    protected virtual void Start()
    {
        _daemonContainer = uiDocument.rootVisualElement.Q<VisualElement>("Daemons");
        ClearDaemonContainer();
    }

    protected void ClearDaemonContainer()
    {
        var toRemove = new List<VisualElement>();

        foreach (var child in _daemonContainer.Children())
        {
            if (child.name != "Empty")
            {
                toRemove.Add(child);
            }
        }

        foreach (var child in toRemove)
        {
            _daemonContainer.Remove(child);
        }
    }

    void Update()
    {
        // Add UI for any new processes
        foreach (var daemon in Daemons)
        {
            if (!daemonToUI.ContainsKey(daemon))
            {
                daemonToUI.Add(daemon, CreateUI(daemon));
            }

            // TODO: Update progress
            SetProgress(daemon, 0);

            // TODO: check suspension
        }

        // Remove UI for any processes that have left the queue
        // Build removal list first — can't modify dictionary while iterating it
        var toRemove = new List<DaemonBase>();
        foreach (var key in daemonToUI.Keys)
        {
            if (!Daemons.Contains(key))
            {
                toRemove.Add(key);
            }
        }

        foreach (var daemon in toRemove)
        {
            RemoveUI(daemon);
        }
    }

    // Creates a new UI box for a daemon
    protected virtual DaemonUI CreateUI(DaemonBase daemon)
    {
        TemplateContainer instance = daemonTemplate.Instantiate();
        _daemonContainer.Add(instance);

        var entry = new DaemonUI
        {
            Root = instance,
            DaemonNameLabel = instance.Q<Label>("DaemonName"),
            DaemonDescLabel = instance.Q<Label>("DaemonDescription"),
            IconImage = instance.Q<Image>("Icon"),
            ProgressBar = instance.Q<ProgressBar>("ProgressBar"),
            DaemonName = $"{daemon.daemonName}",
            Progress = 0f
        };

        // TODO: set icon of daemon
        // entry.LogoElement.image = companyToLogo.GetLogo(daemon.);

        return entry;
    }

    // Removes the UI box for a daemon and cleans up the dictionary entry
    private void RemoveUI(DaemonBase daemon)
    {
        if (!daemonToUI.TryGetValue(daemon, out var entry))
        {
            return;
        }

        _daemonContainer.Remove(entry.Root);
        daemonToUI.Remove(daemon);
    }

    // Updates progress bar for a daemon by its DaemonBase key
    private void SetProgress(DaemonBase daemon, float value)
    {
        if (!daemonToUI.TryGetValue(daemon, out var entry))
        {
            return;
        }

        entry.Progress = value;
    }

    // Wraps one instantiated template and its tracked data
    public class DaemonUI
    {
        // UI references
        public TemplateContainer Root;
        public Label DaemonNameLabel;
        public Label DaemonDescLabel;
        public Image IconImage;
        public ProgressBar ProgressBar;

        // Tracked data — use properties to keep UI in sync
        private string _daemonName;
        private string _daemonDesc;
        private float _progress;

        public string DaemonName
        {
            get => _daemonName;
            set
            {
                _daemonName = value;
                if (DaemonNameLabel != null)
                {
                    DaemonNameLabel.text = value;
                }
            }
        }

        public string DaemonDesc
        {
            get => _daemonDesc;
            set
            {
                _daemonDesc = value;
                if (DaemonDescLabel != null)
                {
                    DaemonDescLabel.text = value;
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
}