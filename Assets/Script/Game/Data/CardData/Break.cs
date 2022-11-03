using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class Break : CardBase
{
    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {

        CharaBase Target = MeOrEnemy ? BM.Enemy : BM.Chara;
        CharaBase BuffTarget = MeOrEnemy ? BM.Chara : BM.Enemy;
        int Power = MeOrEnemy ? BM.TurnInfo.EnemyDamageSum : BM.TurnInfo.MyDamageSum;

        BuffTarget.AddShield(Power);

        if (MeOrEnemy)
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.MyChara, Parameter.Enemy,Parameter.WaitTime);

        }
        else
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.Enemy, Parameter.MyChara,Parameter.WaitTime);
        }

        yield return new WaitForSeconds(0.5f);
    }
}