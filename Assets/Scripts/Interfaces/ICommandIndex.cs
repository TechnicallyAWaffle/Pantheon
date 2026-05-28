using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandIndex
{
    private Dictionary<string, Action<string[]>> BuildHandlerMap() { return null; }
}
