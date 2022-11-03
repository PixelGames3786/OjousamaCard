using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class DummyScript : CharaBase
{
    public override void HPChange(int Change, bool PlusMinus)
    {
        //true�Ȃ�� false�Ȃ猸��
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

                BattleManager.BM.ClearFlag=true;
            }
        }

        FieldManager.FM.HPChanger = true;
        FieldManager.FM.HPSub.OnNext(HP);
    }

    public override void CostChange(int Change, bool PlusMinus)
    {
        //true�Ȃ�� false�Ȃ猸��
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

        if (NowBuffs.Find(Buff => Buff.BuffName == "�V�[���h"))
        {
            BuffBase Shield = NowBuffs.Find(Buff => Buff.BuffName == "�V�[���h");

            Shield.UseCount += Num;

            FieldManager.FM.ShieldChanger = true;
            Subject.OnNext(Shield.UseCount);
        }
        else
        {
            BuffBase AddBuff = (BuffBase)GameObject.Instantiate(BattleManager.BM.BuffDataBase.GetBuffScript("Shield"));

            AddBuff.UseCount = Num;

            NowBuffs.Add(AddBuff);

            FieldManager.FM.ShieldChanger = true;
            Subject.OnNext(AddBuff.UseCount);
        }
    }

    public override void DeckInitialize(List<int> deck = null)
    {
        //�f�b�L�Z�b�g
        string[] SplitDeck = Para.DeckSeed.Split(',');

        for (int i = 0; i < 9; i++)
        {
            Deck.Add(int.Parse(SplitDeck[i]));
        }

        // ���� n �̏����l�̓f�b�L�̖���
        int n = Deck.Count;

        // n��1��菬�����Ȃ�܂ŌJ��Ԃ�
        while (n > 1)
        {
            n--;

            // k�� 0 �` n+1 �̊Ԃ̃����_���Ȓl
            int k = UnityEngine.Random.Range(0, n + 1);

            // k�Ԗڂ̃J�[�h��temp�ɑ��
            int temp = Deck[k];
            Deck[k] = Deck[n];
            Deck[n] = temp;
        }

    }

    public override void Draw(int DrawNum)
    {
        //�����f�b�L���h���[���閇���ȉ����������n�̃J�[�h���V���b�t�����ăf�b�L�ɓ����
        if (DrawNum>=Deck.Count)
        {
            // ���� n �̏����l�̓f�b�L�̖���
            int n = Grave.Count;

            // n��1��菬�����Ȃ�܂ŌJ��Ԃ�
            while (n > 1)
            {
                n--;

                // k�� 0 �` n+1 �̊Ԃ̃����_���Ȓl
                int k = UnityEngine.Random.Range(0, n + 1);

                // k�Ԗڂ̃J�[�h��temp�ɑ��
                int temp = Grave[k];
                Grave[k] = Grave[n];
                Grave[n] = temp;
            }

            Deck.AddRange(Grave);
            Grave.Clear();
        }

        //�h���[����&�J�[�h����
        for (int i = 0; i < DrawNum; i++)
        {
            if (HandCard.Count >= 9)
            {
                return;
            }
            HandCard.Add(Deck[i]);
        }

        //��n�Ɏg�����J�[�h��ǉ����č폜����
        Grave.AddRange(Deck.GetRange(0,DrawNum));

        Deck.RemoveRange(0, DrawNum);
    }

    public override void ChoiceUseCard()
    {
        /*
        CardParameterList DataBase = BattleManager.BM.CardDataBase;

        int LoopCount = 0, CostCount = 0;

        Choiced.Clear();

        //�G���g���J�[�h��I���A����������AI�͌�Ŏ�������\��
        while (true)
        {
            //�����ǉ����悤�Ƃ��Ă���J�[�h�̃R�X�g������𒴂��Ă�����ǉ����Ȃ�
            if ((DataBase.GetCardParameter(HandCard[LoopCount]).Cost + CostCount) <= Cost)
            {
                Choiced.Add(LoopCount);

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

        */

    }
}