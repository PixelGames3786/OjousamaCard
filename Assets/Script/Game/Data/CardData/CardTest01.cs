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

        BattleStatus Target=MeOrEnemy ? BM.Enemy : BM.MyChara;
        BattleStatus BuffTarget=MeOrEnemy ? BM.MyChara : BM.Enemy;
        Subject<int> HPSubject = FieldManager.FM.HPSub;

        //攻撃時発動のバフの処理
        List<BuffBase> Buffs = BuffTarget.Buffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnAttack);

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

        Target.HPDecrease(Power);

        FieldManager.FM.HPChanger = MeOrEnemy;
        HPSubject.OnNext(Target.HP);

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