using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CharaBase
{
    public CharaParameter Para;

    public void Coroutine(int CoroutineNum)
    {
        // �g�����̗�
        CoroutineHandler.StartStaticCoroutine(DrawProcess());
    }

    //���������鑊���ݒ�
    virtual public IEnumerator DrawProcess()
    {
        yield return null;
    }
}
