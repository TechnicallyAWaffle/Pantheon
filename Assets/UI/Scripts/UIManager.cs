using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public UIDocument uiDocument;
    public Entity playerEntity;
    public ProcessQueue playerQueue;
    public Entity enemyEntity;
    public ProcessQueue enemyQueue;
    public ProcessQueue serverQueue;

    void Start()
    {
        UIConverters.RegisterConverters();
        var root = uiDocument.rootVisualElement;

        // bind player data sources to player process queue
        root.Q<VisualElement>("PlayerComputeNumber").dataSource = playerQueue;
        root.Q<VisualElement>("PlayerMemoryNumber").dataSource = playerEntity;
        root.Q<VisualElement>("PlayerServerComputeNumber").dataSource = playerEntity;
        root.Q<VisualElement>("PlayerAvailableServerMemoryNumber").dataSource = playerEntity;

        //bind player process autocomplete to CommandManager


        // bind enemy data sources to enemy process queue
        root.Q<VisualElement>("EnemyComputeNumber").dataSource = enemyQueue;
        root.Q<VisualElement>("EnemyMemoryNumber").dataSource = enemyEntity;
        root.Q<VisualElement>("EnemyServerComputeNumber").dataSource = enemyEntity;
        root.Q<VisualElement>("EnemyAvailableServerMemoryNumber").dataSource = enemyEntity;

        // bind server data sources to server process queue
        root.Q<VisualElement>("ServerComputeNumber").dataSource = serverQueue;
        root.Q<VisualElement>("ServerMemoryNumber").dataSource = serverQueue;
    }
}
