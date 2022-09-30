using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoisonTestCard : CardBase
{

    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        BattleStatus Target = MeOrEnemy ? BM.Enemy : BM.MyChara;

        BuffBase AddBuff = BM.BuffDataBase.GetBuffScript("Poison");

        Target.Buffs.Add(AddBuff);

        yield return new WaitForSeconds(0.3f);
    }
}