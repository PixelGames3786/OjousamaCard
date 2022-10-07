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

    private CardParameter CardData;
    public CardParameterList CardDataBase;

    public GameObject ChoicedFrame;

    public TextMeshProUGUI Name,Power,Cost;

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
        
    }

    public void CardChoiced()
    {
        //���ɑI���ς݂Ȃ�΂Ƃ�͂���
        if (ChoicedFlag)
        {
            MyChara.Choiced.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            ChoicedFlag = false;

            MyChara.Cost += CardData.Cost;
            BM.MyCostText.text = MyChara.Cost.ToString();

            return;
        }

        //�����I���\�Ȃ��
        if (MyChara.Choiced.Count<2)
        {
            //�����\���ȃR�X�g�������Ă�����
            if (MyChara.Cost-CardData.Cost>=0)
            {
                MyChara.Choiced.Add(HandNumber);

                ChoicedFrame.SetActive(true);

                ChoicedFlag = true;

                MyChara.Cost -= CardData.Cost;
                BM.MyCostText.text = MyChara.Cost.ToString();

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
