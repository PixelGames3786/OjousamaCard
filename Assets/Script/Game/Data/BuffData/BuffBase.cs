using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BuffBase : ScriptableObject
{
    //�o�t���g����^�C�~���O
    public enum BuffUseType
    {
        OnAttack,
        OnDifence,
        OnTurnEnd
    }

    //�o�t�̃^�[���J�E���g������^�C�~���O
    public enum BuffDecreaseType
    {
        OnUse,
        OnTurnEnd,
        OnUseAndEnd
    }

    public enum BuffEffectType
    {
        AttackEnhance,
        AddBuff,
        Shield
    }

    public BuffUseType UseType;
    public BuffDecreaseType DecreaseType;
    public BuffEffectType EffectType;
    public CharaDisplayManager.MoveType MyChara, Enemy;

    public string BuffName;

    public string BuffDescription;

    [NonSerialized]
    public int BuffAddNum;

    public int BuffNumber,TurnCount,UseCount;
    public float WaitTime;

    public Sprite BuffIcon;

    abstract public void BuffProcess(BattleManager BM,bool MeOrEnemy);

    abstract public int BuffEnhanceProcess();

    abstract public int ShieldPowerChange(int MotoPower);

    public void TurnCountDecrease(CharaBase Target,bool MeOrEnemy)
    {
        TurnCount--;

        if (TurnCount<=0)
        {
            FieldManager.FM.RemoveBuff(Target.NowBuffs.IndexOf(this), MeOrEnemy);
            Target.NowBuffs.Remove(this);
        }
    }

    public void UseCountDecrease(CharaBase Target, bool MeOrEnemy)
    {
        UseCount--;

        if (UseCount <= 0)
        {
            FieldManager.FM.RemoveBuff(Target.NowBuffs.IndexOf(this), MeOrEnemy);
            Target.NowBuffs.Remove(this);
        }
    }
}
