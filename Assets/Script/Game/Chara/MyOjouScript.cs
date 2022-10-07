using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class MyOjouScript : CharaBase
{
    public override void HPDecrease(int Minus)
    {
        HP -= Minus;
    }

    public override void DeckInitialize(List<int> deck = null)
    {
        Deck = deck;

        // ���� n �̏����l�̓f�b�L�̖���
        int n = Deck.Count;

        // n��1��菬�����Ȃ�܂ŌJ��Ԃ�
        while (n > 1)
        {
            n--;

            // k�� 0 �` n+1 �̊Ԃ̃����_���Ȓl
            int k = Random.Range(0, n + 1);

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

        BattleManager.BM.MakeCards(HandCard);

        //�f�b�L����h���[�������̃J�[�h���폜
        Deck.RemoveRange(0, DrawNum);
    }

    public override void ChoiceUseCard()
    {
        
    }
}