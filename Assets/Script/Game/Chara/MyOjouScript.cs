using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
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

            if (HP<=0)
            {
                HP = 0;

                BattleManager.BM.GameOver();
            }
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

    public override void AddBuff(BuffBase Buff)
    {
        Subject<BuffBase> Subject = FieldManager.FM.BuffSub;

        NowBuffs.Add(Buff);

        FieldManager.FM.BuffChanger = false;
        Subject.OnNext(Buff);
    }

    public override void AddShield(int Num)
    {
        Subject<int> Subject = FieldManager.FM.ShieldSub;

        if (NowBuffs.Find(Buff =>Buff.BuffName=="シールド"))
        {
            BuffBase Shield = NowBuffs.Find(Buff => Buff.BuffName == "シールド");

            Shield.UseCount += Num;

            FieldManager.FM.ShieldChanger = false;
            Subject.OnNext(Shield.UseCount);
        }
        else
        {
            BuffBase AddBuff = (BuffBase)GameObject.Instantiate(BattleManager.BM.BuffDataBase.GetBuffScript("Shield"));

            AddBuff.UseCount = Num;

            NowBuffs.Add(AddBuff);

            FieldManager.FM.ShieldChanger = false;
            Subject.OnNext(AddBuff.UseCount);
        }
    }

    public override void DeckInitialize(List<int> deck = null)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Deck.Add(deck[i]);
        }

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
        List<int> AddCard=new List<int>();

        //もしデッキがドローする枚数以下だったら墓地のカードをシャッフルしてデッキに入れる
        if (DrawNum >= Deck.Count)
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
                break;
            }

            HandCard.Add(Deck[i]);
            AddCard.Add(Deck[i]);
        }

        BattleManager.BM.StartCoroutine("MakeCards",AddCard);

        //墓地に使ったカードを追加して削除する
        Grave.AddRange(Deck.GetRange(0, DrawNum));

        Deck.RemoveRange(0, DrawNum);

    }

    public override void ChoiceUseCard()
    {
        
    }
}
