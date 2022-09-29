using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CardBase
{
    public CardParameter Parameter;

    public void Coroutine(BattleManager BM,bool MeorEnemy)
    {
        // 使い方の例
        CoroutineHandler.StartStaticCoroutine(CardProcess(BM,MeorEnemy));
    }

    //働きかける相手を設定
    virtual public IEnumerator CardProcess(BattleManager BM,bool MeOrEnemy)
    {
        yield return null;
    }
}