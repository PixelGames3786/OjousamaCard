using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class AttackUpSmallCard : CardBase
{
    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        BattleStatus Target = MeOrEnemy ? BM.MyChara : BM.Enemy;
        Subject<BuffBase> Subject = FieldManager.FM.BuffSub;

        BuffBase AddBuff = (BuffBase)GameObject.Instantiate(BM.BuffDataBase.GetBuffScript("AttackUpSmall"));

        Target.Buffs.Add(AddBuff);

        FieldManager.FM.BuffChanger = !MeOrEnemy;
        Subject.OnNext(AddBuff);

        if (MeOrEnemy)
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.MyChara, Parameter.Enemy);

        }
        else
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.Enemy, Parameter.MyChara);
        }

        yield return new WaitForSeconds(0.3f);
    }
}