using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleinfoList", menuName = "Create BattleInfoList")]
public class BattleInfoList : ScriptableObject
{
    public List<BattleInformation> InfoList = new List<BattleInformation>();

    public BattleInformation GetInfo(string InfoName)
    {
        return InfoList.Find(Info=>Info.BattleName==InfoName);
    }
}
