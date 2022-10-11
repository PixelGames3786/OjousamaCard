using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class CardTest01 : CardBase
{
    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        int Power = Parameter.Power;

        CharaBase Target=MeOrEnemy ? BM.Enemy : BM.Chara;
        CharaBase BuffTarget=MeOrEnemy ? BM.Chara : BM.Enemy;

        //攻撃時発動のバフの処理
        List<BuffBase> Buffs = BuffTarget.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnAttack);

        foreach (BuffBase Buff in Buffs)
        {
            if (Buff.EffectType==BuffBase.BuffEffectType.AttackEnhance)
            {
                Power += Buff.BuffEnhanceProcess();
            }
            else
            {
                Buff.BuffProcess(BM,MeOrEnemy);
            }

            //バフカウント減少
            if (Buff.DecreaseType==BuffBase.BuffDecreaseType.OnTurnEnd||Buff.DecreaseType==BuffBase.BuffDecreaseType.OnUseAndEnd)
            {
                Buff.UseCountDecrease(BuffTarget,!MeOrEnemy);
            }
        }

        Target.HPChange(Power,false);

        if (MeOrEnemy)
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.MyChara, Parameter.Enemy);
        }
        else
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.Enemy, Parameter.MyChara);
        }


        yield return new WaitForSeconds(0.3f);
    }
}