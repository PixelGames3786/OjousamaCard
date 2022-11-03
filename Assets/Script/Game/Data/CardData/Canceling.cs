using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class Canceling : CardBase
{
    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        BM.TurnInfo.CancelFlag = true;

        if (MeOrEnemy)
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.MyChara, Parameter.Enemy, Parameter.WaitTime);
        }
        else
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.Enemy, Parameter.MyChara, Parameter.WaitTime);
        }

        yield return new WaitForSeconds(0.5f);
    }

}