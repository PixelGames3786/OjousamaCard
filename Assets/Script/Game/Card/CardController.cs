using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    //�J�[�h�ԍ�
    [NonSerialized]
    public int CardNumber,HandNumber;

    //�I������Ă��邩�ǂ����̃t���O
    private bool ChoicedFlag;

    private CardEntity CardData;
    public CardEntityList CardDataBase;

    public GameObject ChoicedFrame;

    public TextMeshProUGUI Name,Power,Cost;

    public BattleManager BM;


    // Start is called before the first frame update
    void Start()
    {
        //�J�[�h�ԍ����g���ăf�[�^�x�[�X����J�[�h�f�[�^�����o��
        CardData = CardDataBase.GetCardData(CardNumber);

        //���O�E�R�X�g�E�_���[�W�Z�b�g
        Name.text = CardData.Name;
        Cost.text = CardData.Cost.ToString();
        Power.text = CardData.Power.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CardChoiced()
    {
        //���ɑI���ς݂Ȃ��
        if (ChoicedFlag)
        {
            BM.ChoicedCard.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            ChoicedFlag = false;

            BM.HavingCost += CardData.Cost;
            BM.MyCostText.text = BM.HavingCost.ToString();

            return;
        }

        //�����I���\�Ȃ��
        if (BM.ChoicedCard.Count<=3)
        {
            //�����\���ȃR�X�g�������Ă�����
            if (BM.HavingCost-CardData.Cost>=0)
            {
                BM.ChoicedCard.Add(HandNumber);

                ChoicedFrame.SetActive(true);

                ChoicedFlag = true;

                BM.HavingCost -= CardData.Cost;
                BM.MyCostText.text = BM.HavingCost.ToString();

            }
            else
            {
                print("�R�X�g������Ȃ���");
            }
            
        }
        else
        {
            print("�O���I���ς݂���");
        }

    }
}