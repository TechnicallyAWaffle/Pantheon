using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModVar
{
    private int baseValue;
    private Dictionary<ITargetable, int> addMods;

    public ModVar(int startingBaseValue)
    { 
        baseValue = startingBaseValue;
        addMods = new Dictionary<ITargetable, int>();
    }

    public void CreateAddMod(ITargetable source, int value)
    {
        addMods.Add(source, value);
    }

    public void ChangeAddMod(ITargetable source, int newValue)
    {
        if (addMods.ContainsKey(source))
            addMods[source] = newValue;
    }

    public void RemoveAddMod(ITargetable source)
    {
        addMods.Remove(source);
    }

    public void ClearAllMods()
    {
        addMods.Clear();
    }
    public int BaseValue
    {
        get { return baseValue; }
        set { }
    }

    public int Value
    {
        get {
            int finalValue = baseValue;
            foreach (KeyValuePair<ITargetable, int> mod in addMods)
                finalValue += mod.Value;
            return finalValue;
        }
        set {
        }
    }
}
