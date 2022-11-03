using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public bool WaitFlag;

    private CardParameter CardData;
    public CardParameterList CardDataBase;

    public GameObject ChoicedFrame;

    public TextMeshProUGUI Name, Power, Cost;
    public Image Image;

    public BattleManager BM;
    private CharaBase MyChara;

    [NonSerialized]
    public Vector3 DefaultPosi;

    public AudioClip ChoiceClip;

    private AudioSource Audio;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioSource>();
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

        if (CardData.Name.Length>=7)
        {
            Name.fontSize = 23;
        }
        if (CardData.Name.Length >= 8)
        {
            Name.fontSize = 20;
        }

        Cost.text = CardData.Cost.ToString();
        Power.text = CardData.DisplayPower.ToString();

        Image.sprite = CardData.Icon;

        MyChara = BattleManager.BM.Chara;
    }

    public void AppearAnimation(float Size)
    {
        GetComponent<RectTransform>().DOScaleX(Size, 0.3f);
    }

    public void CardChoiced()
    {
        if (WaitFlag)
        {
            return;
        }

        //既に選択済みならばとりはずす
        if (ChoicedFlag)
        {
            MyChara.Choiced.Remove(HandNumber);

            ChoicedFrame.SetActive(false);

            BattleManager.BM.MOM.OrderChange();

            ChoicedFlag = false;

            MyChara.CostChange(CardData.Cost, true);

            return;
        }

        //もし選択可能ならば
        if (MyChara.Choiced.Count < 2)
        {
            //もし十分なコストを持っていたら選択する
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
                print("コストが足りないよ");
            }
        }
        else
        {
            print("二枚選択済みだよ");
        }

    }

    public void PointerEnter()
    {
        if (WaitFlag)
        {
            PointerFlag = true;

        }
        else
        {
            GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.3f);
            GetComponent<RectTransform>().DOLocalMoveY(0f, 0.3f);

            PointerFlag = true;

            Audio.clip = ChoiceClip;
            Audio.Play();
        }

        
    }

    public void PointerExit()
    {
        if (WaitFlag)
        {
            PointerFlag = false;
            PointerTime = 0;

        }
        else
        {
            GetComponent<RectTransform>().DOScale(new Vector3(0.8f, 0.8f, 1), 0.3f);
            GetComponent<RectTransform>().DOLocalMoveY(-35f, 0.3f);

            PointerFlag = false;
            PointerTime = 0;

            BattleManager.BM.MDC.Close();
        }

        
    }

    public void MakeCardDiscription()
    {
        PointerFlag = false;

        PointerTime = 0;

        BattleManager.BM.MDC.Initialize(CardData);
    }
}
