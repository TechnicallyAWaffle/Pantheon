using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public UIDocument uiDocument;
    public ProcessQueue playerQueue;
    public ProcessQueue enemyQueue;

    void Start()
    {
        UIConverters.RegisterConverters();
        var root = uiDocument.rootVisualElement;

        // bind player data sources to player process queue
        root.Q<VisualElement>("PlayerComputeNumber").dataSource = playerQueue;
        root.Q<VisualElement>("PlayerMemoryNumber").dataSource = playerQueue;

        // bind enemy data sources to enemy process queue
        root.Q<VisualElement>("EnemyComputeNumber").dataSource = enemyQueue;
        root.Q<VisualElement>("EnemyMemoryNumber").dataSource = enemyQueue;
    }
}
