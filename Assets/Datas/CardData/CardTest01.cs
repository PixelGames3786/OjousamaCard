using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "CardTest01", menuName = "CreateCard/Test01")]
public class CardTest01 : CardBase
{

    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        BattleStatus Target;
        TextMeshProUGUI TargetText;

        if (MeOrEnemy)
        {
            Target = BM.Enemy;
            TargetText = BM.EnemyHPText;
        }
        else
        {
            Target = BM.MyChara;
            TargetText = BM.MyHPText;
        }

        Target.HP -= Power;

        TargetText.text = Target.HP.ToString() + "<size=45>/" + Target.MaxHP.ToString() + "</size>";

        yield return new WaitForSeconds(0.3f);
    }
}