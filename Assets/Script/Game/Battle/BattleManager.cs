using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using DG.Tweening;

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

    private int TurnCount = 1,CharaMaxCost,EnemyMaxCost;

    public Transform HandCardParent;

    public Transform HandField, OrderField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText;

    public MiniDiscriptionController MDC;
    public SpeechBubbleManager SBM;
    public MoveOrderManager MOM;

    [NonSerialized]
    public CharaBase Chara, Enemy;

    //カードを手札で管理する
    [NonSerialized]
    public List<int> Deck = new List<int>(), HandCard = new List<int>(), ChoicedCard = new List<int>();

    [NonSerialized]
    public List<int> EnemyDeck = new List<int>(), EnemyHand = new List<int>(), EnemyChoiced = new List<int>();

    public Transform[] HandCardTrans = new Transform[9];

    public bool EnemyAttackSkip,CanNext=true;

    private bool[] MoveOrder;

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

        Enemy = (CharaBase)Activator.CreateInstance(Enemytype);
        Enemy.Para = CharaDataBase.GetCharaPara(BattleInfo.EnemyParaName);

        Chara.Enemy = Enemy;
        Enemy.Enemy = Chara;

        Chara.Initialize();
        Enemy.Initialize();

        FieldManager.FM.Initialize();

        CharaDisplayManager.CDM.CharaInstantiate();

        CharaMaxCost = Chara.Para.FirstMaxCost;
        EnemyMaxCost = Chara.Para.FirstMaxCost;

        if (BattleInfo.BeforeNovel >= 0)
        {
            SBM.gameObject.SetActive(true);
            SBM.MessageStart(BattleInfo.BeforeNovel);

            return;
        }

        BattleStart();
    }

    //デュエルが始まった時
    public void BattleStart()
    {
        Chara.DeckInitialize(SaveLoadManager.instance.Data.MyDecks);
        Enemy.DeckInitialize(EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum));

        //テストのために一旦変更　後で直す
        //Deck = SaveLoadManager.instance.Data.MyDecks;
        Deck = EnemyDecks.GetDeck(1);
        EnemyDeck = EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum);

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

        Chara.Draw(5);
        Enemy.Draw(5);

        Enemy.ChoiceUseCard();

        SetOrder();
    }

    //行動順設定 trueだと自分の動き　falseは敵の動き
    public void SetOrder()
    {
        //trueだと自分の行動
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

        MOM.StartCoroutine("OrderInitialize", MoveOrder);

    }

    public IEnumerator MakeCards(List<int> Cards)
    {
        int TransCount = HandCardTrans.Count(Trans => Trans != null);

        for (int i = TransCount; i < TransCount+Cards.Count; i++)
        {
            int MotoPosi = i * -100;
            float Kankaku = 200;

            if (MotoPosi<-500)
            {
                MotoPosi = -500;
                Kankaku =500.0f*2f/(TransCount+Cards.Count-1);
            }

            //カード生成
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            CreatedCard.Initialize(Cards[i-TransCount]);
            CreatedCard.AppearAnimation(0.8f);

            CreatedCard.DefaultPosi= new Vector3(MotoPosi + i * Kankaku, -35, 0);

            HandCardTrans[i] = CreatedCard.transform;

            HandCardTrans[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(MotoPosi + i * Kankaku, -35, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;


            //既に存在しているカードを動かす
            for (int u = 0; u < HandCardTrans.Count(Trans => Trans != null); u++)
            {
                HandCardTrans[u].GetComponent<RectTransform>().DOAnchorPosX(MotoPosi + u * Kankaku, 0.3f);
                HandCardTrans[u].GetComponent<CardController>().HandNumber = i;

            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    //ターンエンド処理
    public void TurnEnd()
    {
        if (!CanNext) return;

        CanNext = false;

        StartCoroutine("Battle");
    }

    //ターンカウント増やしたりドローしたり
    public void TurnChange()
    {
        int MyDraw = Chara.Para.DrawNum, EnemyDraw = Enemy.Para.DrawNum;

        if (Chara.HandCard.Count>=9)
        {
            MyDraw = 0;
        }

        if (Enemy.HandCard.Count>=9)
        {
            EnemyDraw = 0;
        }

        Chara.Draw(MyDraw);
        Enemy.Draw(EnemyDraw);

        //ターンカウント増やす
        TurnCount++;
        TurnText.text = TurnCount.ToString();

        //相手と自分のマナ増やす
        Chara.Cost+=Chara.Para.CostRecover;
        if (Chara.Cost > CharaMaxCost) Chara.Cost = CharaMaxCost;

        FieldManager.FM.CostChanger = false;
        FieldManager.FM.CostSub.OnNext(Chara.Cost);

        Enemy.Cost+=Enemy.Para.CostRecover;
        if (Enemy.Cost > EnemyMaxCost) Enemy.Cost = EnemyMaxCost;

        FieldManager.FM.CostChanger = true;
        FieldManager.FM.CostSub.OnNext(Enemy.Cost);

        Enemy.ChoiceUseCard();

        SetOrder();
        CostMaxChange();
    }

    private void CostMaxChange()
    {
        bool NextSkip = false;

        if (TurnCount>CharaMaxCost && !Chara.Awaked)
        {
            CharaMaxCost++;

            if (CharaMaxCost>Chara.Para.EndMaxCost)
            {
                CharaMaxCost = Chara.Para.EndMaxCost;

                //覚醒処理
                if (Chara.Cost == CharaMaxCost) 
                {
                    StartCoroutine(Awake(true));
                    NextSkip = true;
                }
            }
        }

        if (TurnCount > EnemyMaxCost &&!Enemy.Awaked)
        {
            EnemyMaxCost++;

            if (EnemyMaxCost > Enemy.Para.EndMaxCost)
            {
                EnemyMaxCost = Enemy.Para.EndMaxCost;

                if(Enemy.Cost==EnemyMaxCost)
                {
                    StartCoroutine(Awake(false));
                    NextSkip = true;
                }
            }
        }

        if (NextSkip)
        {
            return;
        }

        CanNext = true;
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

                    Chara.HandCard.RemoveAt(Chara.Choiced[0]);
                    Chara.Choiced.RemoveAt(0);
                }
            }
            //敵行動
            else
            {
                if (Enemy.Choiced.Count != 0)
                {
                    int CardNumber = Enemy.HandCard[Enemy.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);
                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    Card.Coroutine(this, false);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    Enemy.HandCard.RemoveAt(Enemy.Choiced[0]);
                    Enemy.Choiced.RemoveAt(0);
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
        Filtered = Chara.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, false));

        //次に敵のバフを行う
        Filtered = Enemy.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, true));

        //自分のバフのターンカウントを減らす
        Filtered = Chara.NowBuffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Chara, false);
        }

        //敵のバフのターンカウントを減らす
        Filtered = Enemy.NowBuffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Enemy, true);
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
        CharaBase Target = MeOrEnemy ? Enemy : Chara;

        foreach (BuffBase Buff in Buffs)
        {
            Buff.TurnCountDecrease(Target, MeOrEnemy);
        }
    }

    public IEnumerator Awake(bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? Chara : Enemy;

        Target.Para = Target.Para.AwakedPara;

        Target.Awaked = true;

        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaAwake(MeOrEnemy);

        CanNext = true;
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
