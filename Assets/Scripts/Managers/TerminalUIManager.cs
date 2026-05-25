using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalUIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private ScrollRect scrollRect;

    public void Print(string output)
    {
        outputText.text += output + "\n";
    }
}
