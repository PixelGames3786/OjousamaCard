using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class MyOjouScript : CharaBase
{
    public override void HPChange(int Change,bool PlusMinus)
    {
        //true�Ȃ�� false�Ȃ猸��
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
        //�����f�b�L���h���[���閇���ȉ����������n�̃J�[�h���V���b�t�����ăf�b�L�ɓ����
        if (DrawNum >= Deck.Count)
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
            HandCard.Add(Deck[i]);
        }

        BattleManager.BM.MakeCards(HandCard);

        //��n�Ɏg�����J�[�h��ǉ����č폜����
        Grave.AddRange(Deck.GetRange(0, DrawNum));

        Deck.RemoveRange(0, DrawNum);
    }

    public override void ChoiceUseCard()
    {
        
    }
}
