#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SODialogueSequence))]
public class DialogueSequenceEditor : Editor
{
    private SODialogueSequence so;
    private Vector2 scroll;

    // fold state per entry
    private List<bool> foldouts = new();

    // colors
    private static readonly Color userColor = new Color(0.2f, 0.6f, 1f, 0.15f);
    private static readonly Color systemColor = new Color(0.4f, 1f, 0.4f, 0.15f);
    private static readonly Color promptColor = new Color(1f, 0.8f, 0.2f, 0.15f);
    private static readonly Color functionColor = new Color(0.8f, 0.4f, 1f, 0.15f);
    private static readonly Color headerColor = new Color(0f, 0f, 0f, 0.3f);

    void OnEnable()
    {
        so = (SODialogueSequence)target;
        SyncFoldouts();
    }

    void SyncFoldouts()
    {
        while (foldouts.Count < so.entries.Count) foldouts.Add(true);
        while (foldouts.Count > so.entries.Count) foldouts.RemoveAt(foldouts.Count - 1);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SyncFoldouts();

        DrawToolbar();
        EditorGUILayout.Space(4);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < so.entries.Count; i++)
            DrawEntry(i);

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(6);
        DrawAddButtons();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(so);
    }

    // ── Toolbar ─────────────────────────────────────────────────────────────

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label($"Dialogue Sequence  ({so.entries.Count} entries)", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Expand All", EditorStyles.toolbarButton))
            for (int i = 0; i < foldouts.Count; i++) foldouts[i] = true;

        if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton))
            for (int i = 0; i < foldouts.Count; i++) foldouts[i] = false;

        EditorGUILayout.EndHorizontal();
    }

    // ── Single Entry ─────────────────────────────────────────────────────────

    void DrawEntry(int i)
    {
        var entry = so.entries[i];
        Color bg = EntryColor(entry.type);

        // colored background rect
        Rect rect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(rect, bg);

        DrawEntryHeader(i, entry);

        if (foldouts[i])
        {
            EditorGUI.indentLevel++;
            DrawEntryBody(entry);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    void DrawEntryHeader(int i, DialogueEntry entry)
    {
        Rect headerRect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(headerRect, headerColor);

        // foldout arrow
        foldouts[i] = EditorGUILayout.Foldout(foldouts[i], GUIContent.none, true);

        // index label
        GUILayout.Label($"[{i:D2}]", EditorStyles.miniLabel, GUILayout.Width(28));

        // type badge
        string badge = entry.type switch
        {
            DialogueEntryType.UserMessage => "USER",
            DialogueEntryType.SystemMessage => "SYS",
            DialogueEntryType.InputPrompt => "INPUT",
            DialogueEntryType.FunctionCall => "FUNC",
            _ => "?"
        };
        GUILayout.Label(badge, EditorStyles.miniButtonMid, GUILayout.Width(44));

        // preview
        string preview = entry.type == DialogueEntryType.UserMessage
            ? $"{entry.sender}: {entry.message}"
            : entry.message;
        if (string.IsNullOrEmpty(preview) && entry.type == DialogueEntryType.FunctionCall)
            preview = entry.dialogueManagerFunction;

        GUILayout.Label(
            string.IsNullOrEmpty(preview) ? "<empty>" : Truncate(preview, 48),
            EditorStyles.miniLabel);

        GUILayout.FlexibleSpace();

        // delay badge
        if (entry.delayBefore > 0)
            GUILayout.Label($"⏱ {entry.delayBefore:F1}s", EditorStyles.miniLabel, GUILayout.Width(50));

        // move up / down / delete
        EditorGUI.BeginDisabledGroup(i == 0);
        if (GUILayout.Button("▲", EditorStyles.miniButtonLeft, GUILayout.Width(22)))
            MoveEntry(i, i - 1);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(i == so.entries.Count - 1);
        if (GUILayout.Button("▼", EditorStyles.miniButtonMid, GUILayout.Width(22)))
            MoveEntry(i, i + 1);
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("✕", EditorStyles.miniButtonRight, GUILayout.Width(22)))
        {
            Undo.RecordObject(so, "Remove Dialogue Entry");
            so.entries.RemoveAt(i);
            foldouts.RemoveAt(i);
            return;
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawEntryBody(DialogueEntry entry)
    {
        entry.type = (DialogueEntryType)EditorGUILayout.EnumPopup("Type", entry.type);
        entry.delayBefore = EditorGUILayout.FloatField("Delay Before (sec)", entry.delayBefore);

        EditorGUILayout.Space(2);

        switch (entry.type)
        {
            case DialogueEntryType.UserMessage:
                entry.sender = EditorGUILayout.TextField("Sender", entry.sender);
                entry.message = EditorGUILayout.TextArea(entry.message, GUILayout.MinHeight(40));
                DrawFunctionField(entry);
                break;

            case DialogueEntryType.SystemMessage:
                EditorGUILayout.LabelField("Message");
                entry.message = EditorGUILayout.TextArea(entry.message, GUILayout.MinHeight(60));
                DrawFunctionField(entry);
                break;

            case DialogueEntryType.FunctionCall:
                DrawFunctionField(entry);
                break;

            case DialogueEntryType.InputPrompt:
                entry.message = EditorGUILayout.TextField("Prompt Display Text", entry.message);
                DrawInputPrompt(entry);
                DrawFunctionField(entry);
                break;
        }
    }

    void DrawFunctionField(DialogueEntry entry)
    {
        EditorGUILayout.Space(2);
        entry.dialogueManagerFunction = EditorGUILayout.TextField(
            "→ DialogueManager Function", entry.dialogueManagerFunction);
    }

    void DrawInputPrompt(DialogueEntry entry)
    {
        if (entry.inputPrompt == null)
            entry.inputPrompt = new InputPromptData();

        var prompt = entry.inputPrompt;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Input Prompt", EditorStyles.boldLabel);

        prompt.correctInput = EditorGUILayout.TextField("Correct Input", prompt.correctInput);
        prompt.promptHint = EditorGUILayout.TextField("Hint Text", prompt.promptHint);

        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField("Wrong Input Responses", EditorStyles.miniBoldLabel);

        for (int j = 0; j < prompt.wrongInputResponses.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();
            prompt.wrongInputResponses[j] = EditorGUILayout.TextField(
                $"  [{j}]", prompt.wrongInputResponses[j]);
            if (GUILayout.Button("✕", EditorStyles.miniButton, GUILayout.Width(22)))
            {
                prompt.wrongInputResponses.RemoveAt(j);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Add Wrong Response", EditorStyles.miniButton))
            prompt.wrongInputResponses.Add("");
    }

    // ── Add Buttons ──────────────────────────────────────────────────────────

    void DrawAddButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+ User Message"))
            AddEntry(DialogueEntryType.UserMessage);
        if (GUILayout.Button("+ System Message"))
            AddEntry(DialogueEntryType.SystemMessage);
        if (GUILayout.Button("+ Input Prompt"))
            AddEntry(DialogueEntryType.InputPrompt);
        if (GUILayout.Button("+ Function Call"))
            AddEntry(DialogueEntryType.FunctionCall);

        EditorGUILayout.EndHorizontal();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    void AddEntry(DialogueEntryType type)
    {
        Undo.RecordObject(so, "Add Dialogue Entry");
        so.entries.Add(new DialogueEntry { type = type });
        foldouts.Add(true);
    }

    void MoveEntry(int from, int to)
    {
        Undo.RecordObject(so, "Move Dialogue Entry");
        var entry = so.entries[from];
        so.entries.RemoveAt(from);
        so.entries.Insert(to, entry);

        bool fold = foldouts[from];
        foldouts.RemoveAt(from);
        foldouts.Insert(to, fold);
    }

    Color EntryColor(DialogueEntryType type) => type switch
    {
        DialogueEntryType.UserMessage => userColor,
        DialogueEntryType.SystemMessage => systemColor,
        DialogueEntryType.InputPrompt => promptColor,
        DialogueEntryType.FunctionCall => functionColor,
        _ => Color.clear
    };

    string Truncate(string s, int max)
        => s.Length <= max ? s : s[..max] + "…";
}
#endif