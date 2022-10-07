using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "AttackUpSmall", menuName = "CreateBuff/AttackBuffSmall")]

public class AttackUpSmall : BuffBase
{
    public override void BuffProcess(BattleManager BM,bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? BM.Enemykari : BM.Chara;
        Subject<int> HPSubject = FieldManager.FM.HPSub;

        Target.HPDecrease(1);

        FieldManager.FM.HPChanger = MeOrEnemy;
        HPSubject.OnNext(Target.HP);
    }

    public override int BuffEnhanceProcess()
    {
        return 1;
    }
}
