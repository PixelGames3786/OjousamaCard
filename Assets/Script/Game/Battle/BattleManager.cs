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

    [NonSerialized]
    public CardParameterList CardDataBase;
    [NonSerialized]
    public BuffScriptList BuffDataBase;
    private EnemyDecks EnemyDecks;
    private BattleInfoList InfoList;
    private BattleInformation BattleInfo;
    private CharaParameterList CharaDataBase;

    private int TurnCount = 1;

    public Transform HandCardParent;

    public Transform HandField, OrderField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText, EnemyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    public CharaBase Chara, Enemykari;

    //カードを手札で管理する
    [NonSerialized]
    public List<int> Deck = new List<int>(), HandCard = new List<int>(), ChoicedCard = new List<int>();

    [NonSerialized]
    public List<int> EnemyDeck = new List<int>(), EnemyHand = new List<int>(), EnemyChoiced = new List<int>();

    private Transform[] HandCardTrans = new Transform[5];

    public bool EnemyAttackSkip;

    public bool[] MoveOrder;

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
        InfoList = (BattleInfoList)Resources.Load("BattleInfoList");
        CharaDataBase = (CharaParameterList)Resources.Load("CharaParameterList");

        BattleInfo = InfoList.GetInfo(SaveLoadManager.instance.NextBattle);

        //キャラセット
        Type type = Type.GetType(CharaDataBase.GetCharaPara(BattleInfo.MyCharaParaName).ScriptName);

        Chara = (CharaBase)Activator.CreateInstance(type);
        Chara.Para = CharaDataBase.GetCharaPara(BattleInfo.MyCharaParaName);

        Type Enemytype = Type.GetType(CharaDataBase.GetCharaPara(BattleInfo.EnemyParaName).ScriptName);

        Enemykari = (CharaBase)Activator.CreateInstance(Enemytype);
        Enemykari.Para = CharaDataBase.GetCharaPara(BattleInfo.EnemyParaName);

        Chara.Enemy = Enemykari;
        Enemykari.Enemy = Chara;

        Chara.Initialize();
        Enemykari.Initialize();

        MyChara.Name = "MyChara";
        Enemy.Name = "Enemy";

        BattleStart();
    }

    //デュエルが始まった時
    private void BattleStart()
    {
        Chara.DeckInitialize(SaveLoadManager.instance.Data.MyDecks);
        Enemykari.DeckInitialize(EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum));

        //テストのために一旦変更　後で直す
        //Deck = SaveLoadManager.instance.Data.MyDecks;
        Deck = EnemyDecks.GetDeck(1);
        EnemyDeck = EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum);

        CharaDisplayManager.CDM.CharaInstantiate(BattleInfo);

        //相手と自分のシャッフル処理
        {
            // 整数 n の初期値はデッキの枚数
            int n = 20;

            // nが1より小さくなるまで繰り返す
            while (n > 1)
            {
                n--;

                // kは 0 ～ n+1 の間のランダムな値
                int k = Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                int temp = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = temp;

                // kは 0 ～ n+1 の間のランダムな値
                k = Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                int EnemyTemp = EnemyDeck[k];
                EnemyDeck[k] = EnemyDeck[n];
                EnemyDeck[n] = EnemyTemp;
            }
        }

        Chara.Draw(5);
        Enemykari.Draw(5);

        SetOrder();

        //Draw(5);
        //EnemyDraw(5);
    }

    //行動順設定 trueだと自分の動き　falseは敵の動き
    public void SetOrder()
    {
        MoveOrder = new bool[4];

        int FirstRan = Random.Range(0, 4);

        MoveOrder[FirstRan] = true;

        while (true)
        {
            int SecondRan = Random.Range(0, 4);

            if (SecondRan != FirstRan)
            {
                MoveOrder[SecondRan] = true;

                break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (MoveOrder[i])
            {
                OrderField.GetChild(i).GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                OrderField.GetChild(i).GetComponent<Image>().color = new Color(0, 1, 0);
            }
        }
    }

    public void MakeCards(List<int> Cards)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            //カード生成
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            CreatedCard.CardNumber = Cards[i];

            CreatedCard.BM = this;

            HandCardTrans[i] = HandCardParent.GetChild(i);
            HandCardTrans[i].localPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
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
        for (int i = 0; i < 5; i++)
        {
            HandCardTrans[i] = HandCardParent.GetChild(i);
            HandCardTrans[i].localPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
    }

    //相手の手札管理
    private void EnemyDraw(int DrawNum)
    {
        for (int i = 0; i < DrawNum; i++)
        {
            EnemyHand.Add(EnemyDeck[i]);
        }

        //ドローした分のカードを削除
        EnemyDeck.RemoveRange(0, DrawNum);
    }

    //ターンエンド処理
    public void TurnEnd()
    {
        StartCoroutine("Battle");
    }

    //ターンカウント増やしたりドローしたり
    public void TurnChange()
    {
        //ドロー
        Chara.Draw(5 - Chara.HandCard.Count);
        Enemykari.Draw(5 - Enemykari.HandCard.Count);

        //ターンカウント増やす
        TurnCount++;
        TurnText.text = TurnCount.ToString();

        //相手と自分のマナ増やす
        Chara.Cost++;
        if (Chara.Cost > Chara.Para.MaxCost) Chara.Cost = Chara.Para.MaxCost;

        MyCostText.text = Chara.Cost.ToString();

        Enemykari.Cost++;
        if (Enemykari.Cost > Enemykari.Para.MaxCost) Enemykari.Cost = Enemykari.Para.MaxCost;
        EnemyCostText.text = Enemykari.Cost.ToString();

        SetOrder();
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

        CharaDisplayManager.CDM.CharaReset();

        //敵の行動に移る
        StartCoroutine("EnemyCharaMove");
    }

    //ターン終了時の相手の動き
    private IEnumerator EnemyCharaMove()
    {
        print("敵が動いた");

        //後で直す　デバッグ用
        if (EnemyAttackSkip)
        {
            //バフをする
            EndBuffProcess();

            yield break;
        }

        EnemyChoiced.Clear();
        int LoopCount = 0, CostCount = 0;

        //敵が使うカードを選択、いい感じのAIは後で実装する予定
        while (true)
        {
            //もし追加しようとしているカードのコストが上限を超えていたら追加しない
            if ((CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost + CostCount) <= Enemy.NowHaveCost)
            {
                EnemyChoiced.Add(EnemyHand[LoopCount]);

                CostCount += CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost;
            }

            if (EnemyChoiced.Count >= 3 || LoopCount >= EnemyHand.Count - 1 || CostCount >= Enemy.NowHaveCost)
            {
                break;
            }

            LoopCount++;
        }

        //選択したカードの処理
        foreach (int Num in EnemyChoiced)
        {
            Type type = Type.GetType(CardDataBase.GetCardParameter(EnemyHand[Num]).ScriptName);

            CardBase Card = (CardBase)Activator.CreateInstance(type);
            Card.Parameter = CardDataBase.GetCardParameter(EnemyHand[Num]);

            Card.Coroutine(this, false);

            yield return new WaitForSeconds(Card.Parameter.WaitTime);
        }

        //使用可能コストを減らす
        Enemy.NowHaveCost -= CostCount;
        EnemyCostText.text = Enemy.NowHaveCost.ToString();

        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaReset();

        //バフをする
        EndBuffProcess();
    }

    //戦闘
    private IEnumerator Battle()
    {
        //後で直す　デバッグ用
        if (EnemyAttackSkip)
        {
            //バフをする
            EndBuffProcess();

            yield break;
        }

        Enemykari.ChoiceUseCard();

        for (int i = 0; i < 4; i++)
        {
            //自分行動
            if (MoveOrder[i])
            {
                if (Chara.Choiced.Count != 0)
                {
                    int CardNumber = Chara.HandCard[Chara.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);
                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    Card.Coroutine(this, true);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    Destroy(HandCardTrans[Chara.Choiced[0]].gameObject);

                    Chara.Choiced.RemoveAt(0);
                }
            }
            //敵行動
            else
            {
                if (Enemykari.Choiced.Count != 0)
                {
                    int CardNumber = Enemykari.HandCard[Enemykari.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);
                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    Card.Coroutine(this, false);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    Enemykari.Choiced.RemoveAt(0);
                }
            }
        }

        //使用可能コストを減らす
        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaReset();

        EndBuffProcess();
    }

    //後々統合する予定
    private void EndBuffProcess()
    {
        List<BuffBase> Filtered;

        //まず自分のバフを行う
        Filtered = MyChara.Buffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, false));

        //次に敵のバフを行う
        Filtered = Enemy.Buffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, true));

        //自分のバフのターンカウントを減らす
        Filtered = MyChara.Buffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Chara, false);
        }

        //敵のバフのターンカウントを減らす
        Filtered = Enemy.Buffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Enemykari, true);
        }

        TurnChange();
    }

    public IEnumerator BuffProcess(List<BuffBase> Buffs, bool MeOrEnemy)
    {
        foreach (BuffBase Buff in Buffs)
        {
            Buff.BuffProcess(this, MeOrEnemy);

            yield return new WaitForSeconds(Buff.WaitTime);
        }
    }

    public void BuffTurnDecrease(List<BuffBase> Buffs, bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? Enemykari : Chara;

        foreach (BuffBase Buff in Buffs)
        {
            Buff.TurnCountDecrease(Target, MeOrEnemy);
        }
    }

    public void GameOver()
    {
        //後で直す
        SceneManager.LoadScene("Title");
    }

    public void Clear()
    {
        //後で直す
        SceneManager.LoadScene("Lounge");
    }
}
