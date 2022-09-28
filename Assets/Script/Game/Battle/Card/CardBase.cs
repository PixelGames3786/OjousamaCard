using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CardBase : ScriptableObject
{
    public int CardID;
    public string Name;
    public int Cost;
    public int Power;
    public float WaitTime;
    public Sprite Icon;

    //働きかける相手を設定
    virtual public IEnumerator CardProcess(BattleManager BM,bool MeOrEnemy)
    {
        yield return null;
    }
}