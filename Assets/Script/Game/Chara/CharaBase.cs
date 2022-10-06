using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CharaBase
{
    public CharaParameter Para;

    public void Coroutine(int CoroutineNum)
    {
        // g‚¢•û‚Ì—á
        CoroutineHandler.StartStaticCoroutine(DrawProcess());
    }

    //“­‚«‚©‚¯‚é‘Šè‚ğİ’è
    virtual public IEnumerator DrawProcess()
    {
        yield return null;
    }
}
