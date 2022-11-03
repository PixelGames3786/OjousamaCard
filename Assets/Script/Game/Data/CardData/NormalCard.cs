using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class NormalCard : CardBase
{
    public override IEnumerator CardProcess(BattleManager BM, bool MeOrEnemy)
    {
        int Power = 0;

        CharaBase Target=MeOrEnemy ? BM.Enemy : BM.Chara;
        CharaBase BuffTarget=MeOrEnemy ? BM.Chara : BM.Enemy;

        if (MeOrEnemy)
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.MyChara, Parameter.Enemy,Parameter.WaitTime);
        }
        else
        {
            CharaDisplayManager.CDM.CharaMove(Parameter.Enemy, Parameter.MyChara,Parameter.WaitTime);
        }

        float EachTime = (Parameter.WaitTime-0.5f) / Parameter.MoveNum;

        for (int i=0;i<Parameter.MoveNum;i++)
        {
            switch (Parameter.Types[i])
            {
                case CardParameter.CardType.Attack:

                    {
                        List<BuffBase> RemoveBuffs = new List<BuffBase>();

                        //攻撃時発動（攻撃側）のバフの処理
                        List<BuffBase> Buffs = BuffTarget.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnAttack);

                        Power = Random.Range(Parameter.MinPowers[0], Parameter.MaxPowers[0]);

                        foreach (BuffBase Buff in Buffs)
                        {
                            if (Buff.EffectType == BuffBase.BuffEffectType.AttackEnhance)
                            {
                                Power += Buff.BuffEnhanceProcess();
                            }
                            else
                            {
                                Buff.BuffProcess(BM, MeOrEnemy);
                            }

                            //バフカウント減少
                            if (Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd)
                            {
                                Buff.UseCountDecrease(BuffTarget, !MeOrEnemy);
                            }
                        }

                        //攻撃時発動（防御側）のバフの処理
                        Buffs = Target.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnDifence);

                        foreach (BuffBase Buff in Buffs)
                        {
                            if (Buff.EffectType == BuffBase.BuffEffectType.Shield)
                            {
                                Subject<int> Subject = FieldManager.FM.ShieldSub;

                                int MotoPower = Power;

                                Power = Buff.ShieldPowerChange(Power);

                                FieldManager.FM.ShieldChanger = MeOrEnemy;
                                Subject.OnNext(Buff.UseCount);

                                if (MeOrEnemy&&CharaDisplayManager.CDM.DifenseFlag)
                                {
                                    FieldManager.FM.OnePoint("-"+(MotoPower - Power).ToString(), new Vector2(575, 120), new Vector2(625, 120), 1f);
                                }
                                else if(CharaDisplayManager.CDM.DifenseFlag)
                                {
                                    FieldManager.FM.OnePoint("-"+(MotoPower - Power).ToString(), new Vector2(-540, 120), new Vector2(-490, 120), 1f);
                                }

                                if (Buff.UseCount<=0)
                                {
                                    RemoveBuffs.Add(Buff);
                                }

                                continue;
                            }

                            //バフカウント減少
                            if (Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd)
                            {
                                Buff.UseCountDecrease(BuffTarget, !MeOrEnemy);
                            }
                        }

                        Target.HPChange(Power, false);

                        Debug.Log("ダメージ" + Power);

                        if (MeOrEnemy)
                        {
                            BM.TurnInfo.EnemyDamageSum += Power;

                            if (Power>0)
                            {
                                FieldManager.FM.OnePoint("-" + Power, new Vector2(500, 225), new Vector2(550, 225), 1f);
                            }

                        }
                        else
                        {
                            BM.TurnInfo.MyDamageSum += Power;

                            if (Power>0)
                            {
                                FieldManager.FM.OnePoint("-" + Power, new Vector2(-570, 225), new Vector2(-520, 225), 1f);

                            }
                        }
                    }

                    break;

                case CardParameter.CardType.Shield:

                    {
                        Power = Random.Range(Parameter.MinPowers[i], Parameter.MaxPowers[i]);

                        BuffTarget.AddShield(Power);

                        Debug.Log("シールド" + Power);

                        if (!MeOrEnemy)
                        {
                            if (Power > 0)
                            {
                                FieldManager.FM.OnePoint("+" + Power, new Vector2(575, 120), new Vector2(625, 120), 1f);
                            }

                        }
                        else
                        {

                            if (Power > 0)
                            {
                                FieldManager.FM.OnePoint("+" + Power, new Vector2(-540, 120), new Vector2(-490, 120), 1f);

                            }
                        }
                    }

                    break;

                case CardParameter.CardType.Draw:

                    {
                        Power = Random.Range(Parameter.MinPowers[i], Parameter.MaxPowers[i]);

                        Debug.Log("ドロー" + Power);

                        BuffTarget.Draw(Power);
                    }

                    break;

                case CardParameter.CardType.CostRecover:

                    {
                        Power = Random.Range(Parameter.MinPowers[i], Parameter.MaxPowers[i]);

                        BuffTarget.CostChange(Power,true);
                    }
                    break;

                case CardParameter.CardType.Recover:

                    {
                        Power = Random.Range(Parameter.MinPowers[i], Parameter.MaxPowers[i]);

                        BuffTarget.HPChange(Power,true);
                    }
                    break;

                case CardParameter.CardType.SelfDamage:

                    {
                        Power = Random.Range(Parameter.MinPowers[i], Parameter.MaxPowers[i]);

                        Debug.Log("自傷"+Power);

                        BuffTarget.HPChange(Power, false);

                        if (!MeOrEnemy)
                        {
                            if (Power > 0)
                            {
                                FieldManager.FM.OnePoint("-" + Power, new Vector2(500, 225), new Vector2(550, 225), 1f);
                            }

                        }
                        else
                        {
                            if (Power > 0)
                            {
                                FieldManager.FM.OnePoint("-" + Power, new Vector2(-570, 225), new Vector2(-520, 225), 1f);

                            }
                        }
                    }

                    break;
            }

            yield return new WaitForSeconds(EachTime);
        }

        yield return new WaitForSeconds(0.5f);
    }
}