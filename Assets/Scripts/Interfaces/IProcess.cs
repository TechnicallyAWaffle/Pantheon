using System;
using UnityEngine;

public interface IProcess
{
    //This is literally just so that we can find Processes somehow without doing exhaustive searches. If we need to add anything else in here feel free -meowlvin
    //Arguments is left blank if the process has no additional arguments. The unique logic of the execute function can just simply not care that they're there
    void Execute(Entity owner, string[] arguments) { }
}
