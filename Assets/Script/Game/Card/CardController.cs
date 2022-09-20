using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    //�J�[�h�ԍ�
    [NonSerialized]
    public int CardNumber;

    private CardEntity CardData;
    public CardEntityList CardDataBase;

    public TextMeshProUGUI Name,Power,Cost;

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
}
