using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class BasicTerminalUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private ScrollView scrollView;
    private VisualElement content;
    private VisualElement inputRow;
    private TextField inputField;
    private Label inputPrompt;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        scrollView = root.Q<ScrollView>("terminal-scroll");
        content = root.Q<VisualElement>("terminal-content");
        inputRow = root.Q<VisualElement>("input-row");
        inputField = root.Q<TextField>("terminal-input");
        inputPrompt = root.Q<Label>("input-prompt");

        inputField.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

        inputField.RegisterCallback<FocusEvent>(_ =>
            inputPrompt.style.display = DisplayStyle.Flex);

        inputField.RegisterCallback<BlurEvent>(_ =>
            inputPrompt.style.display = DisplayStyle.None);

        root.RegisterCallback<ClickEvent>(_ => inputField.Focus());

        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        inputField.isDelayed = false;
        inputField.multiline = false;

        HideInputField();
    }

    void OnDisable()
    {
        inputField.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public string GetInputFieldText()
    {
        return inputField.value;
    }

    void OnKeyDown(KeyDownEvent e)
    {
        if (e.keyCode != KeyCode.Return && e.keyCode != KeyCode.KeypadEnter)
            return;

        string input = inputField.value.Trim();
        if (string.IsNullOrWhiteSpace(input)) return;

        Print("> " + input, "terminal-line-user");

        inputField.value = "";
        inputField.Focus();
        e.StopPropagation();
    }

    public void HideInputField()
    {
        // hide the whole row so prompt and field disappear together
        inputField.Blur();
        inputRow.style.display = DisplayStyle.None;
    }

    public void ShowInputField()
    {
        inputRow.style.display = DisplayStyle.Flex;
        inputField.Focus();
    }

    public void Print(string message, string additionalClass = null)
    {
        var line = new Label(message);
        line.AddToClassList("terminal-line");

        if (!string.IsNullOrEmpty(additionalClass))
            line.AddToClassList(additionalClass);

        int inputRowIndex = content.IndexOf(inputRow);
        content.Insert(inputRowIndex, line);

        StartCoroutine(ScrollAfterLayout());
    }

    IEnumerator ScrollAfterLayout()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (scrollView.contentContainer.layout.height > scrollView.layout.height)
            scrollView.verticalScroller.value = scrollView.verticalScroller.highValue;
    }

    public void PrintSystem(string message)
        => Print(message, "terminal-line-system");

    public void PrintUser(string message)
        => Print(message, "terminal-line-user");

    public void PrintError(string message)
        => Print(message, "terminal-line-error");

    public void PrintDefault(string message)
        => Print(message, "terminal-line-system");
}