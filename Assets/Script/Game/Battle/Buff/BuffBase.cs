using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BuffBase : ScriptableObject
{
    //�o�t���g����^�C�~���O
    public enum BuffType
    {
        OnAttack,
        OnDifence,
        OnTurnEnd
    }

    public BuffType Type;

    public int BuffNumber,TurnCount;
    public float WaitTime;

    public Sprite BuffIcon;

    abstract public void BuffProcess(BattleManager BM,bool MeOrEnemy);

}
