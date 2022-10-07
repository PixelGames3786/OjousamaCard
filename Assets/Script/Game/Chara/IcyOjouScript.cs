using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IcyOjouScript : CharaBase
{
    public override void HPDecrease(int Minus)
    {
        HP -= Minus;
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
        //�h���[����&�J�[�h����
        for (int i = 0; i < DrawNum; i++)
        {
            HandCard.Add(Deck[i]);
        }
        //�f�b�L����h���[�������̃J�[�h���폜
        Deck.RemoveRange(0, DrawNum);
    }

    public override void ChoiceUseCard()
    {
        CardParameterList DataBase = BattleManager.BM.CardDataBase;

        int LoopCount = 0, CostCount = 0;

        Choiced.Clear();

        //�G���g���J�[�h��I���A����������AI�͌�Ŏ�������\��
        while (true)
        {
            //�����ǉ����悤�Ƃ��Ă���J�[�h�̃R�X�g������𒴂��Ă�����ǉ����Ȃ�
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
        BattleManager.BM.EnemyCostText.text = Cost.ToString();

    }
}