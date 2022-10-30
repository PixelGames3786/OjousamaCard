using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleInfo", menuName = "Create BattleInfo")]

public class BattleInformation : ScriptableObject
{
    public enum BattleEndType
    {
        Battle,
        Novel,
        Lounge
    }

    public string BattleName,MyCharaImageName,EnemyImageName,MyCharaParaName,EnemyParaName;

    public int EnemyDeckNum;

    public int BeforeNovel;

    public BattleEndType EndType;

    public string EndDetail;
}
