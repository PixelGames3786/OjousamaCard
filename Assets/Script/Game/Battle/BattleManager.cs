using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private int TurnCount = 1;

    public Transform HandCardParent;

    private CardEntityList CardDataBase;
    private EnemyDecks EnemyDecks;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText,EnemyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    //カードを手札で管理する
    public List<int> Deck, HandCard, ChoicedCard = new List<int>();
    private List<int> EnemyDeck,EnemyHand = new List<int>();

    private Transform[] HandCardTrans=new Transform[5];


    // Start is called before the first frame update
    void Start()
    {
        //ランダムのシード値設定
        Random.InitState(System.DateTime.Now.Millisecond);

        CardDataBase = (CardEntityList)Resources.Load("CardEntityList");
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

        //シャッフル処理
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
            }
        }

        //ドロー処理&カード生成
        for (int i = 0; i < 5; i++)
        {
            HandCard.Add(Deck[i]);

            //カード生成
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            HandCardTrans[i]=CreatedCard.transform;

            //位置設定＆カード番号入力
            CreatedCard.CardNumber = Deck[i];
            CreatedCard.HandNumber = i;

            CreatedCard.transform.localPosition = new Vector3(-500 + i * 250, 0, 0);

            CreatedCard.BM = this;
        }
        //デッキからドローした分のカードを削除
        Deck.RemoveRange(0, 5);

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
    private void EnemyHandDraw(int DrawNum)
    {
        //ドロー処理
        for (int i = 0; i < DrawNum; i++)
        {
            EnemyHand.Add(EnemyDeck[i]);
        }
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
            //カードデータ取得
            CardEntity Card = CardDataBase.GetCardData(HandCard[Num]);

            //とりあえずダメージ与える処理のみ
            Enemy.HP -= Card.Power;

            EnemyHPText.text = Enemy.HP.ToString()+"<size=45>/"+Enemy.MaxHP.ToString()+"</size>";

            yield return new WaitForSeconds(0.3f);
        }

        //手札から削除
        foreach (int Num in ChoicedCard)
        {
            HandCard.RemoveAt(Num);
            Destroy(HandCardTrans[Num].gameObject);
        }
        ChoicedCard.Clear();

        yield return new WaitForSeconds(0.3f);

        //敵の行動に移る
        StartCoroutine("EnemyCharaMove");

        yield return null;
    }

    //ターン終了時の相手の動き
    private IEnumerator EnemyCharaMove()
    {
        print("敵が動いた");

        TurnChange();

        yield return null;
    }
}
