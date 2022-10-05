using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "BuffScriptList", menuName = "Create BuffScriptList")]

public class BuffScriptList : ScriptableObject
{
    public List<BuffBase> ScriptList;

    public BuffBase GetBuffScript(int Num)
    {
        return ScriptList.Find(Buff=>Buff.BuffNumber==Num);
    }

    public BuffBase GetBuffScript(string Name)
    {
        return ScriptList.Find(Buff=>Buff.name==Name);
    }
}
