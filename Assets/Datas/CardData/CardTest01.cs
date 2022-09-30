using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardTest01 : CardBase
{
    

    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        Debug.Log("実行");

        BattleStatus Target=MeOrEnemy ? BM.Enemy : BM.MyChara;
        TextMeshProUGUI TargetText=MeOrEnemy ? BM.EnemyHPText : BM.MyHPText;

        Target.HP -= Parameter.Power;

        TargetText.text = Target.HP.ToString() + "<size=45>/" + Target.MaxHP.ToString() + "</size>";

        yield return new WaitForSeconds(0.3f);
    }
}