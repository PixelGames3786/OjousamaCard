using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class CardTest01 : CardBase
{
    

    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        Debug.Log("実行");

        BattleStatus Target=MeOrEnemy ? BM.Enemy : BM.MyChara;
        Subject<int> HPSubject = FieldManager.FM.HPSub;

        Target.HPDecrease(Parameter.Power);

        FieldManager.FM.HPChanger = MeOrEnemy;
        HPSubject.OnNext(Target.HP);

        yield return new WaitForSeconds(0.3f);
    }
}