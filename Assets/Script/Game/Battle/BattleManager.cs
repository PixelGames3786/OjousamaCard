using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager BM;

    private int TurnCount = 1;

    public Transform HandCardParent;

    private CardParameterList CardDataBase;
    [NonSerialized]
    public BuffScriptList BuffDataBase;
    private EnemyDecks EnemyDecks;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText,EnemyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    //カードを手札で管理する
    public List<int> Deck, HandCard, ChoicedCard = new List<int>();

    public List<int> EnemyDeck,EnemyHand,EnemyChoiced = new List<int>();

    private Transform[] HandCardTrans=new Transform[5];

    private void Awake()
    {
        BM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ランダムのシード値設定
        Random.InitState(System.DateTime.Now.Millisecond);

        CardDataBase = (CardParameterList)Resources.Load("CardScriptList");
        BuffDataBase = (BuffScriptList)Resources.Load("BuffScriptList");
        EnemyDecks = (EnemyDecks)Resources.Load("EnemyDecks");

        BattleStart();

    }

    // Update is called once per frame
    void Update()
    {

    }

    //デュエルが始まった時
    private void BattleStart()
    {
        Deck = SaveLoadManager.instance.Data.MyDecks;
        EnemyDeck= EnemyDecks.GetDeck(0);

        //相手と自分のシャッフル処理
        {
            // 整数 n の初期値はデッキの枚数
            int n = 20;

            // nが1より小さくなるまで繰り返す
            while (n > 1)
            {
                n--;

                // kは 0 〜 n+1 の間のランダムな値
                int k = Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                int temp = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = temp;

                // kは 0 〜 n+1 の間のランダムな値
                k = Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                int EnemyTemp = EnemyDeck[k];
                EnemyDeck[k] = EnemyDeck[n];
                EnemyDeck[n] = EnemyTemp;
            }
        }

        Draw(5);
        EnemyDraw(5);
    }

    //ドロー処理&カード生成
    private void Draw(int DrawNum)
    {
        //ドロー処理&カード生成
        for (int i = 0; i < DrawNum; i++)
        {
            HandCard.Add(Deck[i]);

            //カード生成
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            //カード番号入力
            CreatedCard.CardNumber = Deck[i];

            CreatedCard.BM = this;
        }
        //デッキからドローした分のカードを削除
        Deck.RemoveRange(0, DrawNum);

        //カードナンバー振り直し
        for (int i=0;i<5;i++)
        {
            HandCardTrans[i] = HandCardParent.GetChild(i);
            HandCardTrans[i].localPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
    }

    //相手の手札管理
    private void EnemyDraw(int DrawNum)
    {
        for (int i =0; i < DrawNum; i++)
        {
            EnemyHand.Add(EnemyDeck[i]);
        }

        //ドローした分のカードを削除
        EnemyDeck.RemoveRange(0,DrawNum);
    }

    //ターンエンド処理
    public void TurnEnd()
    {
        StartCoroutine("MyCharaMove");
    }

    //ターンカウント増やしたりドローしたり
    public void TurnChange()
    {
        //ドロー
        Draw(5-HandCard.Count);
        EnemyDraw(5-EnemyHand.Count);

        //ターンカウント増やす
        TurnCount++;
        TurnText.text = TurnCount.ToString();

        //相手と自分のマナ増やす
        MyChara.NowHaveCost++;
        MyCostText.text = MyChara.NowHaveCost.ToString();

        Enemy.NowHaveCost++;
        EnemyCostText.text = Enemy.NowHaveCost.ToString();
    }
    
    //ターン終了時の自分の動き
    private IEnumerator MyCharaMove()
    {
        print("自身が動いた");

        //選択したカードの処理
        foreach (int Num in ChoicedCard)
        {
            Type type = Type.GetType(CardDataBase.GetCardParameter(HandCard[Num]).ScriptName);

            CardBase Card = (CardBase)Activator.CreateInstance(type);
            Card.Parameter = CardDataBase.GetCardParameter(HandCard[Num]);

            Card.Coroutine(this, true);

            yield return new WaitForSeconds(Card.Parameter.WaitTime);
        }

        ChoicedCard.Sort((a, b) => b - a);

        //手札から削除
        foreach (int Num in ChoicedCard)
        {
            HandCard.RemoveAt(Num);
            Destroy(HandCardTrans[Num].gameObject);
        }

        ChoicedCard.Clear();

        yield return new WaitForSeconds(1f);

        //敵の行動に移る
        StartCoroutine("EnemyCharaMove");
    }

    //ターン終了時の相手の動き
    private IEnumerator EnemyCharaMove()
    {
        print("敵が動いた");

        EnemyChoiced.Clear();
        int LoopCount = 0,CostCount = 0;

        //敵が使うカードを選択、いい感じのAIは後で実装する予定
        while (true)
        {
            //もし追加しようとしているカードのコストが上限を超えていたら追加しない
            if ((CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost+CostCount)<=Enemy.NowHaveCost)
            {
                EnemyChoiced.Add(EnemyHand[LoopCount]);

                CostCount += CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost;
            }

            if (EnemyChoiced.Count>=3 || LoopCount>EnemyHand.Count || CostCount>=Enemy.NowHaveCost)
            {
                break;
            }

            LoopCount++;
        }

        //選択したカードの処理
        foreach (int Num in ChoicedCard)
        {
            Type type = Type.GetType(CardDataBase.GetCardParameter(EnemyHand[Num]).ScriptName);

            CardBase Card = (CardBase)Activator.CreateInstance(type);
            Card.Parameter = CardDataBase.GetCardParameter(EnemyHand[Num]);

            Card.Coroutine(this, false);

            yield return new WaitForSeconds(Card.Parameter.WaitTime);
        }

        //もし自身のHPがなくなったらゲームオーバー
        if (MyChara.HP<=0)
        {
            //後で直す
            SceneManager.LoadScene("Title");
        }

        //使用可能コストを減らす
        Enemy.NowHaveCost -= CostCount;
        EnemyCostText.text = Enemy.NowHaveCost.ToString();

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(EndBuffProcess());

        //TurnChange();
    }

    private IEnumerator BuffProcess(BuffBase.BuffType Type,bool MeOrEnemy)
    {
        //三項演算子という書き方　行を節約できて楽
        List<BuffBase> AllBuff = MeOrEnemy ? Enemy.Buffs : MyChara.Buffs;

        //発動タイミングによってバフを絞り込む
        List<BuffBase> FilteredBuff = AllBuff.FindAll(FindBuff=>FindBuff.Type==Type);

        foreach (BuffBase Buff in FilteredBuff)
        {
            Buff.BuffProcess(this,MeOrEnemy);

            yield return new WaitForSeconds(Buff.WaitTime);
        }

        switch (Type)
        {
            case BuffBase.BuffType.OnTurnEnd:

                TurnChange();

                break;
        }

        yield return null;
    }

    //後々統合する予定
    private IEnumerator EndBuffProcess()
    {
        List<BuffBase> FilteredBuff;

        //まず自分のバフを行う
        FilteredBuff = MyChara.Buffs.FindAll(FindBuff=>FindBuff.Type==BuffBase.BuffType.OnTurnEnd);

        foreach (BuffBase Buff in FilteredBuff)
        {
            Debug.Log("実行");

            Buff.BuffProcess(this, false);

            yield return new WaitForSeconds(Buff.WaitTime);
        }

        //次に敵のバフを行う
        FilteredBuff = Enemy.Buffs.FindAll(FindBuff => FindBuff.Type == BuffBase.BuffType.OnTurnEnd);

        foreach (BuffBase Buff in FilteredBuff)
        {
            Debug.Log("敵実行");

            Buff.BuffProcess(this, true);

            yield return new WaitForSeconds(Buff.WaitTime);
        }

        TurnChange();

        yield return null;
    }

    /*

    //カード使用時の挙動
    private IEnumerator CardProcess(bool MeOrEnemy,List<int> Cards,List<int> Choices)
    {
        //MeOrEnemy trueだと自分 falseだと敵

        foreach (int Num in Choices)
        {
            Type type = Type.GetType(CardDataBase.GetCardParameter(Cards[Num]).ScriptName);

            CardBase Card = (CardBase)Activator.CreateInstance(type);
            Card.Parameter = CardDataBase.GetCardParameter(Cards[Num]);

            Card.Coroutine(this,MeOrEnemy);

            yield return new WaitForSeconds(Card.Parameter.WaitTime);
        }

        yield return null;
    }

    */
}
