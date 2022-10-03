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

    public BuffUseType UseType;
    public BuffDecreaseType DecreaseType;

    [NonSerialized]
    public int BuffAddNum;

    public int BuffNumber,TurnCount,UseCount;
    public float WaitTime;

    public Sprite BuffIcon;

    abstract public void BuffProcess(BattleManager BM,bool MeOrEnemy);

}
