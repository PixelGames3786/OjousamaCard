using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnInformation : MonoBehaviour
{
    //今の行動順
    public int NowMoveNum;

    //敵と自分の受けたダメージの合計
    public int MyDamageSum, EnemyDamageSum;

    public bool CancelFlag;
}
