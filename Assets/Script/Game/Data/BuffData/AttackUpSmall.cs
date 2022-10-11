using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "AttackUpSmall", menuName = "CreateBuff/AttackBuffSmall")]

public class AttackUpSmall : BuffBase
{
    public override void BuffProcess(BattleManager BM,bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? BM.Enemy : BM.Chara;

        Target.HPChange(1,false);

    }

    public override int BuffEnhanceProcess()
    {
        return 1;
    }
}
