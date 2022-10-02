using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class PoisonTestCard : CardBase
{

    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        BattleStatus Target = MeOrEnemy ? BM.Enemy : BM.MyChara;
        Subject<BuffBase> Subject = FieldManager.FM.BuffSub;

        BuffBase AddBuff = (BuffBase)GameObject.Instantiate(BM.BuffDataBase.GetBuffScript("Poison"));

        Target.Buffs.Add(AddBuff);

        FieldManager.FM.BuffChanger = MeOrEnemy;
        Subject.OnNext(AddBuff);


        yield return new WaitForSeconds(0.3f);
    }
}