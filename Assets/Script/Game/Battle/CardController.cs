using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    //カード番号
    [NonSerialized]
    public int CardNum,HandNumber;

    public float DiscriptNeedTime;
    private float PointerTime;

    //選択されているかどうかのフラグ
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
        //カード番号を使ってデータベースからカードデータを取り出す
        CardData = CardDataBase.GetCardParameter(CardNumber);

        //名前・コスト・ダメージセット
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
