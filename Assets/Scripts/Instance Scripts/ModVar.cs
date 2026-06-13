using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ModVar
{
    public int BaseValue { get; set; }
    private Dictionary<ITargetable, int> addMods;

    public ModVar(int startingBaseValue)
    { 
        BaseValue = startingBaseValue;
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
    public int Value
    {
        get {
            int finalValue = BaseValue;
            foreach (KeyValuePair<ITargetable, int> mod in addMods)
                finalValue += mod.Value;
            return finalValue;
        }
        set {
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
