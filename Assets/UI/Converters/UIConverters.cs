using UnityEditor;
using UnityEngine.UIElements;

// InitializeOnLoad runs this in the editor on domain reload,
// which is when the binding window needs to see converters
public static class UIConverters
{
    static UIConverters()
    {
        RegisterConverters();
    }

    // Also register at runtime if you need it during Play mode
    [UnityEngine.RuntimeInitializeOnLoadMethod]
    public static void RegisterConverters()
    {
        // The string ID here is what appears in the converter list
        var group = new ConverterGroup("Open Memory To String");

        // Source -> UI property (int -> string in this example, adjust to match your OpenMemory type)
        group.AddConverter<int, string>((ref int value) => value.ToString() + " TB");
        group.AddConverter<string, int>((ref string value) => int.TryParse(value, out int result) ? result : 0);

        ConverterGroups.RegisterConverterGroup(group);

        var modVarGroup = new ConverterGroup("Mod Var Converters");
        modVarGroup.AddConverter<ModVar, string>((ref ModVar var) => var.Value.ToString());

        ConverterGroups.RegisterConverterGroup(modVarGroup);
    }
}