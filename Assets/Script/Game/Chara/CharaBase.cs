using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CharaBase
{
    public CharaParameter Para;

    public void Coroutine(int CoroutineNum)
    {
        // 使い方の例
        CoroutineHandler.StartStaticCoroutine(DrawProcess());
    }

    //働きかける相手を設定
    virtual public IEnumerator DrawProcess()
    {
        yield return null;
    }
}
