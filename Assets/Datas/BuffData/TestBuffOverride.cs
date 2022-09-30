using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffTest01", menuName = "CreateBuff/Test01")]

public class TestBuffOverride : BuffBase
{
    public override void BuffProcess(BattleManager BM,bool MeOrEnemy)
    {
        BattleStatus Target = MeOrEnemy ? BM.MyChara : BM.Enemy;

        Target.HP -= 1;

        TurnCount--;

        if (TurnCount==0)
        {
            Target.Buffs.Remove(this);
        }
    }
}
