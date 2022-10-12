using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    //�J�[�h�ԍ�
    [NonSerialized]
    public int CardNumber, HandNumber;

    public float DiscriptNeedTime;
    private float PointerTime;

    //�I������Ă��邩�ǂ����̃t���O
    private bool ChoicedFlag, PointerFlag;

    private CardParameter CardData;
    public CardParameterList CardDataBase;

    public GameObject ChoicedFrame;

    public TextMeshProUGUI Name, Power, Cost;

    public BattleManager BM;
    private CharaBase MyChara;

    // Start is called before the first frame update
    void Start()
    {
        //�J�[�h�ԍ����g���ăf�[�^�x�[�X����J�[�h�f�[�^�����o��
        CardData = CardDataBase.GetCardParameter(CardNumber);

        //���O�E�R�X�g�E�_���[�W�Z�b�g
        Name.text = CardData.Name;
        Cost.text = CardData.Cost.ToString();
        Power.text = CardData.Power.ToString();

        MyChara = BM.Chara;
    }

    // Update is called once per frame
    void Update()
    {
        if (PointerFlag)
        {
            PointerTime += Time.deltaTime;

            if (PointerTime>DiscriptNeedTime)
            {
                MakeCardDiscription();
            }
        }
    }

    public void CardChoiced()
    {
        //���ɑI���ς݂Ȃ�΂Ƃ�͂���
        if (ChoicedFlag)
        {
            MyChara.Choiced.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            ChoicedFlag = false;

            MyChara.CostChange(CardData.Cost, true);

            return;
        }

        //�����I���\�Ȃ��
        if (MyChara.Choiced.Count < 2)
        {
            //�����\���ȃR�X�g�������Ă�����
            if (MyChara.Cost - CardData.Cost >= 0)
            {
                MyChara.Choiced.Add(HandNumber);

                ChoicedFrame.SetActive(true);

                ChoicedFlag = true;

                MyChara.CostChange(CardData.Cost, false);
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

    public void PointerDown()
    {
        PointerFlag = true;
    }

    public void PointerUp()
    {
        if (PointerFlag)
        {
            PointerFlag = false;

            PointerTime = 0;

            CardChoiced();
        }
    }

    public void MakeCardDiscription()
    {
        PointerFlag = false;

        PointerTime = 0;
    }
}
