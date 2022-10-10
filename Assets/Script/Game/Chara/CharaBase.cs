using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

abstract public class CharaBase
{
    public CharaParameter Para;

    public int HP, Cost;

    [NonSerialized]
    public List<int> Deck = new List<int>(), HandCard = new List<int>(), Choiced = new List<int>();

    [NonSerialized]
    public List<BuffBase> NowBuffs = new List<BuffBase>();

    [NonSerialized]
    public CharaBase Enemy;

    public void Initialize()
    {
        HP = Para.MaxHP;
        Cost = 1;
    }

    public void Coroutine(string Name,CardBase Card=null)
    {
        switch (Name)
        {
            case "BattleMove":

                //CoroutineHandler.StartStaticCoroutine(BattleMove(Card));

                break;
        }

        // 使い方の例
        //CoroutineHandler.StartStaticCoroutine(DrawProcess());
    }

    //働きかける相手を設定
    virtual public IEnumerator DrawProcess()
    {
        yield return null;
    }

    abstract public void HPChange(int Change,bool PlusMinus);

    abstract public void CostChange(int Change,bool PlusMinus);

    //デッキセット＆シャッフル
    abstract public void DeckInitialize(List<int> Deck = null);

    abstract public void Draw(int DrawNum);

    abstract public void ChoiceUseCard();
}
