using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BuffBase : ScriptableObject
{
    //バフが使われるタイミング
    public enum BuffType
    {
        OnAttack,
        OnDifence,
        OnTurnEnd
    }

    public BuffType Type;

    [NonSerialized]
    public int BuffAddNum;

    public int BuffNumber,TurnCount;
    public float WaitTime;

    public Sprite BuffIcon;

    abstract public void BuffProcess(BattleManager BM,bool MeOrEnemy);

}
