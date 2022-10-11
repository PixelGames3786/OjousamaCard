using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "BuffTest01", menuName = "CreateBuff/Test01")]

public class TestBuffOverride : BuffBase
{
    public override void BuffProcess(BattleManager BM,bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? BM.Enemy : BM.Chara;

        Target.HPChange(1,false);
    }

    public override int BuffEnhanceProcess()
    {
        throw new System.NotImplementedException();
    }
}
