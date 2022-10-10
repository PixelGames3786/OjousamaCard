using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class MyOjouScript : CharaBase
{
    public override void HPChange(int Change,bool PlusMinus)
    {
        //trueなら回復 falseなら減少
        if (PlusMinus)
        {
            HP +=Change;

        }
        else
        {
            HP -= Change;
        }

        FieldManager.FM.HPChanger = false;
        FieldManager.FM.HPSub.OnNext(HP);
    }

    public override void CostChange(int Change, bool PlusMinus)
    {
        if (PlusMinus)
        {
            Cost += Change;
        }
        else
        {
            Cost -= Change;
        }

        FieldManager.FM.CostChanger = false;
        FieldManager.FM.CostSub.OnNext(Cost);
    }

    public override void DeckInitialize(List<int> deck = null)
    {
        Deck = deck;

        // 整数 n の初期値はデッキの枚数
        int n = Deck.Count;

        // nが1より小さくなるまで繰り返す
        while (n > 1)
        {
            n--;

            // kは 0 ～ n+1 の間のランダムな値
            int k = Random.Range(0, n + 1);

            // k番目のカードをtempに代入
            int temp = Deck[k];
            Deck[k] = Deck[n];
            Deck[n] = temp;
        }
    }

    public override void Draw(int DrawNum)
    {
        //ドロー処理&カード生成
        for (int i = 0; i < DrawNum; i++)
        {
            HandCard.Add(Deck[i]);
        }

        BattleManager.BM.MakeCards(HandCard);

        //デッキからドローした分のカードを削除
        Deck.RemoveRange(0, DrawNum);
    }

    public override void ChoiceUseCard()
    {
        
    }
}
