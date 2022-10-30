using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DummyScript : CharaBase
{
    public override void HPChange(int Change, bool PlusMinus)
    {
        //trueなら回復 falseなら減少
        if (PlusMinus)
        {
            HP += Change;

        }
        else
        {
            HP -= Change;

            if (HP<=0)
            {
                HP = 0;

                BattleManager.BM.Clear();
            }
        }

        FieldManager.FM.HPChanger = true;
        FieldManager.FM.HPSub.OnNext(HP);
    }

    public override void CostChange(int Change, bool PlusMinus)
    {
        //trueなら回復 falseなら減少
        if (PlusMinus)
        {
            Cost += Change;

        }
        else
        {
            Cost -= Change;
        }

        FieldManager.FM.CostChanger = true;
        FieldManager.FM.CostSub.OnNext(Cost);
    }

    public override void DeckInitialize(List<int> deck = null)
    {
        //デッキセット
        string[] SplitDeck = Para.DeckSeed.Split(',');

        for (int i = 0; i < 9; i++)
        {
            Deck.Add(int.Parse(SplitDeck[i]));
        }

        // 整数 n の初期値はデッキの枚数
        int n = Deck.Count;

        // nが1より小さくなるまで繰り返す
        while (n > 1)
        {
            n--;

            // kは 0 ～ n+1 の間のランダムな値
            int k = UnityEngine.Random.Range(0, n + 1);

            // k番目のカードをtempに代入
            int temp = Deck[k];
            Deck[k] = Deck[n];
            Deck[n] = temp;
        }

    }

    public override void Draw(int DrawNum)
    {
        //もしデッキがドローする枚数以下だったら墓地のカードをシャッフルしてデッキに入れる
        if (DrawNum>=Deck.Count)
        {
            // 整数 n の初期値はデッキの枚数
            int n = Grave.Count;

            // nが1より小さくなるまで繰り返す
            while (n > 1)
            {
                n--;

                // kは 0 ～ n+1 の間のランダムな値
                int k = UnityEngine.Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                int temp = Grave[k];
                Grave[k] = Grave[n];
                Grave[n] = temp;
            }

            Deck.AddRange(Grave);
            Grave.Clear();
        }

        //ドロー処理&カード生成
        for (int i = 0; i < DrawNum; i++)
        {
            if (HandCard.Count >= 9)
            {
                return;
            }
            HandCard.Add(Deck[i]);
        }

        //墓地に使ったカードを追加して削除する
        Grave.AddRange(Deck.GetRange(0,DrawNum));

        Deck.RemoveRange(0, DrawNum);
    }

    public override void ChoiceUseCard()
    {
        CardParameterList DataBase = BattleManager.BM.CardDataBase;

        int LoopCount = 0, CostCount = 0;

        Choiced.Clear();

        //敵が使うカードを選択、いい感じのAIは後で実装する予定
        while (true)
        {
            //もし追加しようとしているカードのコストが上限を超えていたら追加しない
            if ((DataBase.GetCardParameter(HandCard[LoopCount]).Cost + CostCount) <= Cost)
            {
                Choiced.Add(HandCard[LoopCount]);

                CostCount += DataBase.GetCardParameter(HandCard[LoopCount]).Cost;
            }

            if (Choiced.Count >= 2 || LoopCount >= HandCard.Count - 1 || CostCount >= Cost)
            {
                break;
            }

            LoopCount++;
        }

        Cost -= CostCount;
        FieldManager.FM.CostChanger=true;

    }
}