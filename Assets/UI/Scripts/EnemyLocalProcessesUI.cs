using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public class EnemyLocalProcessesUI : LocalProcessesUI
{
    protected override Dictionary<int, string> EncryptionToStyle { get; } = new()
    {
        { 0, "enemy-none-encryption" },
        { 1, "enemy-custom-light" },
        { 2, "enemy-custom-medium" },
        { 3, "enemy-custom-high" }
    };

    protected override void Start()
    {
        _commands = uiDocument.rootVisualElement.Q<VisualElement>("EnemyCommands");
        ClearCommands();
    }

    protected override ProcessUI CreateUI(RunningProcess proc)
    {
        var procUI = base.CreateUI(proc);
        procUI.Root.name = "EnemyLocalCommand";
        return procUI;
    }
}
