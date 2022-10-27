using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    //�J�[�h�ԍ�
    [NonSerialized]
    public int CardNum,HandNumber;

    public float DiscriptNeedTime;
    private float PointerTime;

    //�I������Ă��邩�ǂ����̃t���O
    private bool ChoicedFlag, PointerFlag;

    private RectTransform ThisRect;

    private CardParameter CardData;
    public CardParameterList CardDataBase;

    public GameObject ChoicedFrame;

    public TextMeshProUGUI Name, Power, Cost;

    public BattleManager BM;
    private CharaBase MyChara;

    public Vector3 DefaultPosi;

    // Start is called before the first frame update
    void Start()
    {
        ThisRect = GetComponent<RectTransform>();
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

    public void Initialize(int CardNumber)
    {
        //�J�[�h�ԍ����g���ăf�[�^�x�[�X����J�[�h�f�[�^�����o��
        CardData = CardDataBase.GetCardParameter(CardNumber);

        //���O�E�R�X�g�E�_���[�W�Z�b�g
        Name.text = CardData.Name;
        Cost.text = CardData.Cost.ToString();
        Power.text = CardData.MaxPower.ToString();

        MyChara = BattleManager.BM.Chara;
    }

    public void AppearAnimation(float Size)
    {
        GetComponent<RectTransform>().DOScaleX(Size, 0.3f);
    }

    public void CardChoiced()
    {
        //���ɑI���ς݂Ȃ�΂Ƃ�͂���
        if (ChoicedFlag)
        {
            MyChara.Choiced.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            BattleManager.BM.MOM.OrderChange();

            ChoicedFlag = false;

            MyChara.CostChange(CardData.Cost, true);

            return;
        }

        //�����I���\�Ȃ��
        if (MyChara.Choiced.Count < 2)
        {
            //�����\���ȃR�X�g�������Ă�����I������
            if (MyChara.Cost - CardData.Cost >= 0)
            {
                MyChara.Choiced.Add(HandNumber);
                MyChara.CostChange(CardData.Cost, false);

                BattleManager.BM.MOM.OrderChange();

                ChoicedFrame.SetActive(true);

                ChoicedFlag = true;

            }
            else
            {
                print("�R�X�g������Ȃ���");
            }
        }
        else
        {
            print("�񖇑I���ς݂���");
        }

    }

    public void PointerEnter()
    {
        GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.3f);
        GetComponent<RectTransform>().DOLocalMoveY(0f,0.3f);

        PointerFlag = true;
    }

    public void PointerExit()
    {
        GetComponent<RectTransform>().DOScale(new Vector3(0.8f, 0.8f, 1), 0.3f);
        GetComponent<RectTransform>().DOLocalMoveY(-35f, 0.3f);

        PointerFlag = false;
        PointerTime = 0;

        BattleManager.BM.MDC.Close();
    }

    public void MakeCardDiscription()
    {
        PointerFlag = false;

        PointerTime = 0;

        BattleManager.BM.MDC.Initialize(CardData);
    }
}
