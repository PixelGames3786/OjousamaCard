using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private int TurnCount = 1;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText;

    //カードを手札で管理する
    public List<int> Deck,HandCard = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
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

        //シャッフル処理
        {
            // 整数 n の初期値はデッキの枚数
            int n = 20;

            // nが1より小さくなるまで繰り返す
            while (n > 1)
            {
                n--;

                // kは 0 ～ n+1 の間のランダムな値
                int k = UnityEngine.Random.Range(0, n + 1);

                // k番目のカードをtempに代入
                int temp = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = temp;
            }
        }

        //ドロー処理&カード生成
        for (int i=0;i<5;i++)
        {
            HandCard.Add(Deck[i]);

            Transform CreatedCard = Instantiate(CardPrefab, HandField).transform;

            CreatedCard.localPosition = new Vector3(-500+i*250,0,0);
        }
        //デッキからカードを削除
        Deck.RemoveRange(0, 5);

    }

    //ターンエンド処理
    public void TurnEnd()
    {
        StartCoroutine("MyCharaMove");
    }

    //ターンカウント増やしたりドローしたり
    public void TurnChange()
    {
        TurnCount++;

        TurnText.text = TurnCount.ToString();
    }

    private IEnumerator MyCharaMove()
    {
        print("自身が動いた");

        StartCoroutine("EnemyCharaMove");

        return null;
    }

    private IEnumerator EnemyCharaMove()
    {
        print("敵が動いた");

        TurnChange();

        return null;
    }
}
