using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "BuffTest01", menuName = "CreateBuff/Test01")]

public class TestBuffOverride : BuffBase
{
    public override void BuffProcess(BattleManager BM,bool MeOrEnemy)
    {
        BattleStatus Target = MeOrEnemy ? BM.Enemy : BM.MyChara;
        Subject<int> HPSubject = FieldManager.FM.HPSub;

        Target.HP -= 1;

        FieldManager.FM.HPChanger = MeOrEnemy;
        HPSubject.OnNext(Target.HP);
    }
}
