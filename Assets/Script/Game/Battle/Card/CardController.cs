using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    //カード番号
    [NonSerialized]
    public int CardNumber,HandNumber;

    //選択されているかどうかのフラグ
    private bool ChoicedFlag;

    private CardParameter CardData;
    public CardParameterList CardDataBase;

    public GameObject ChoicedFrame;

    public TextMeshProUGUI Name,Power,Cost;

    public BattleManager BM;


    // Start is called before the first frame update
    void Start()
    {
        //カード番号を使ってデータベースからカードデータを取り出す
        CardData = CardDataBase.GetCardParameter(CardNumber);

        //名前・コスト・ダメージセット
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
        //既に選択済みならば
        if (ChoicedFlag)
        {
            BM.ChoicedCard.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            ChoicedFlag = false;

            BM.MyChara.NowHaveCost += CardData.Cost;
            BM.MyCostText.text = BM.MyChara.NowHaveCost.ToString();

            return;
        }

        //もし選択可能ならば
        if (BM.ChoicedCard.Count<3)
        {
            //もし十分なコストを持っていたら
            if (BM.MyChara.NowHaveCost-CardData.Cost>=0)
            {
                BM.ChoicedCard.Add(HandNumber);

                ChoicedFrame.SetActive(true);

                ChoicedFlag = true;

                BM.MyChara.NowHaveCost -= CardData.Cost;
                BM.MyCostText.text = BM.MyChara.NowHaveCost.ToString();

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
}
