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

    public GameObject Fader;

    public MiniDiscriptionController MDC;
    public SpeechBubbleManager SBM;
    public MoveOrderManager MOM;

    [NonSerialized]
    public CharaBase Chara, Enemy;

    public Transform[] HandCardTrans = new Transform[9];

    [NonSerialized]
    public TurnInformation TurnInfo=new TurnInformation();

    private AudioSource Audio;

    [NonSerialized]
    public bool EnemyAttackSkip,ClearFlag,CanNext=true;

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

        Audio = GetComponent<AudioSource>();

        Chara.Initialize();
        Enemy.Initialize();

        FieldManager.FM.Initialize();

        CharaDisplayManager.CDM.CharaInstantiate();

        CharaMaxCost = Chara.Para.FirstMaxCost;
        EnemyMaxCost = Chara.Para.FirstMaxCost;

        Fader.GetComponent<Image>().DOFade(0f, 1f).OnComplete(() =>
        {
            Fader.SetActive(false);

            if (BattleInfo.BeforeNovel >= 0)
            {
                SBM.gameObject.SetActive(true);
                SBM.MessageStart(BattleInfo.BeforeNovel);

                return;
            }

            BGMManager.instance.BGMPlay("a",1,BattleInfo.BGM);

            BattleStart();
        });

    }

    //デュエルが始まった時
    public void BattleStart()
    {
        Chara.DeckInitialize(SaveLoadManager.instance.Data.MyDecks);
        Enemy.DeckInitialize(EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum));

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
        int TransCount = HandField.childCount;

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

            for (int u = 0; u < TransCount; u++)
            {
                HandCardTrans[u] = HandField.GetChild(u);
            }

            //既に存在しているカードを動かす
            for (int u = 0; u < HandCardTrans.Count(Trans => Trans != null); u++)
            {
                HandCardTrans[u].GetComponent<RectTransform>().DOAnchorPosX(MotoPosi + u * Kankaku, 0.3f);
                HandCardTrans[u].GetComponent<CardController>().HandNumber = u;
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
        if (ClearFlag)
        {
            Clear();

            return;
        }

        int MyDraw = Chara.Para.DrawNum, EnemyDraw = Enemy.Para.DrawNum;

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

        TurnInfo = new TurnInformation();

        for (int i = 0; i < 4; i++)
        {
            TurnInfo.NowMoveNum = i;

            if (TurnInfo.CancelFlag)
            {
                TurnInfo.CancelFlag = false;

                if (i==3)
                {
                    break;
                }

                if (MoveOrder[i+1])
                {
                    if (Chara.Choiced.Count != 0)
                    {
                        Chara.Choiced.RemoveAt(0);
                    }
                }
                else
                {
                    if (Enemy.Choiced.Count!=0)
                    {
                        Enemy.Choiced.RemoveAt(0);
                    }
                    
                }

                continue;
            }

            //自分行動
            if (MoveOrder[i])
            {
                if (Chara.Choiced.Count != 0)
                {
                    int CardNumber = Chara.HandCard[Chara.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    print(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);

                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    //音を鳴らす
                    Audio.clip = Card.Parameter.UseSE;
                    Audio.Play();

                    print(Card.Parameter.Name);

                    Card.Coroutine(this, true);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    //処理が終わった後に手札を消す
                    Destroy(HandCardTrans[Chara.Choiced[0]].gameObject);

                    if (Chara.Choiced.Count>1&&Chara.Choiced[1]>Chara.Choiced[0])
                    {

                        for (int u = Chara.Choiced[0]; u < 8; u++)
                        {
                            HandCardTrans[u] = HandCardTrans[u + 1];
                        }

                        HandCardTrans[8] = null;

                        Chara.HandCard.RemoveAt(Chara.Choiced[0]);
                        Chara.Choiced.RemoveAt(0);

                        Chara.Choiced[0]--;
                    }
                    else
                    {
                        Chara.HandCard.RemoveAt(Chara.Choiced[0]);
                        Chara.Choiced.RemoveAt(0);
                    }
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

                    //音を鳴らす
                    Audio.clip = Card.Parameter.UseSE;
                    Audio.Play();

                    print(Card.Parameter.Name);

                    Card.Coroutine(this, false);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    if (Enemy.Choiced.Count > 1 && Enemy.Choiced[1] > Enemy.Choiced[0])
                    {
                        Enemy.HandCard.RemoveAt(Enemy.Choiced[0]);
                        Enemy.Choiced.RemoveAt(0);

                        Enemy.Choiced[0]--;
                    }
                    else
                    {
                        Enemy.HandCard.RemoveAt(Enemy.Choiced[0]);
                        Enemy.Choiced.RemoveAt(0);
                    }
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
        Fader.SetActive(true);

        Fader.GetComponent<Image>().DOFade(1f, 1f).OnComplete(() =>
        {
            //後で直す
            SceneManager.LoadScene("Title");
        });

    }

    public void Clear()
    {
        Fader.SetActive(true);

        Fader.GetComponent<Image>().DOFade(1f, 1f).OnComplete(() =>
        {
            switch (BattleInfo.EndType)
            {
                case BattleInformation.BattleEndType.Battle:

                    SaveLoadManager.instance.NextBattle = BattleInfo.EndDetail;

                    SceneManager.LoadScene("Battle");

                    break;

                case BattleInformation.BattleEndType.Novel:

                    SaveLoadManager.instance.NextNovel = int.Parse(BattleInfo.EndDetail);

                    SceneManager.LoadScene("Novel");

                    break;
            }
        });


        

        //後で直す
        //SceneManager.LoadScene("Lounge");
    }
}
