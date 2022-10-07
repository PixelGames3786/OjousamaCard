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
        
    }

    public void CardChoiced()
    {
        //既に選択済みならばとりはずす
        if (ChoicedFlag)
        {
            MyChara.Choiced.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            ChoicedFlag = false;

            MyChara.Cost += CardData.Cost;
            BM.MyCostText.text = MyChara.Cost.ToString();

            return;
        }

        //もし選択可能ならば
        if (MyChara.Choiced.Count<2)
        {
            //もし十分なコストを持っていたら
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
                print("コストが足りないよ");
            }
        }
        else
        {
            print("三枚選択済みだよ");
        }

    }
}
