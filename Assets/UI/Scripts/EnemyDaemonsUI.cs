using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;


public class EnemyDaemonsUI : DaemonsUI
{
    protected override void Start()
    {
        _daemonContainer = uiDocument.rootVisualElement.Q<VisualElement>("EnemyDaemons");
        ClearDaemonContainer();
    }
}