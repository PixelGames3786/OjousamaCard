using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    //カード番号
    [NonSerialized]
    public int CardNumber, HandNumber;

    public float DiscriptNeedTime;
    private float PointerTime;

    //選択されているかどうかのフラグ
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
        //カード番号を使ってデータベースからカードデータを取り出す
        CardData = CardDataBase.GetCardParameter(CardNumber);

        //名前・コスト・ダメージセット
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
        //既に選択済みならばとりはずす
        if (ChoicedFlag)
        {
            MyChara.Choiced.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            ChoicedFlag = false;

            MyChara.CostChange(CardData.Cost, true);

            return;
        }

        //もし選択可能ならば
        if (MyChara.Choiced.Count < 2)
        {
            //もし十分なコストを持っていたら
            if (MyChara.Cost - CardData.Cost >= 0)
            {
                MyChara.Choiced.Add(HandNumber);

                ChoicedFrame.SetActive(true);

                ChoicedFlag = true;

                MyChara.CostChange(CardData.Cost, false);
            }
            else
            {
                print("コストが足りないよ");
            }
        }
        else
        {
            print("三枚選択済みだよ");
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
