using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveOrderManager : MonoBehaviour
{
    public GameObject HatenaCardPrefab,WaitCardPrefab,CardPrefab;

    private RectTransform[] Cards = new RectTransform[4];
    private List<bool> ShortOrder = new List<bool>();

    private List<Vector2> DefaultPosi = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //行動順に何らかの変更があった際に呼ぶ : カードを選んだときとか
    public void OrderChange()
    {
        int MyChoiceCount=0;

        for (int i=0;i<ShortOrder.Count;i++)
        {
            if (ShortOrder[i])
            {
                CharaBase Chara = BattleManager.BM.Chara;

                if (Chara.Choiced.Count > MyChoiceCount)
                {
                    CardController Card = Instantiate(CardPrefab, transform).GetComponent<CardController>();

                    Card.Initialize(Chara.HandCard[Chara.Choiced[MyChoiceCount]]);
                    Card.GetComponent<RectTransform>().anchoredPosition = DefaultPosi[i];
                    Card.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);

                    Card.enabled = false;

                    Destroy(Cards[i].gameObject);

                    Cards[i] = Card.GetComponent<RectTransform>();

                    MyChoiceCount++;

                }
                else
                {
                    GameObject Card = Instantiate(WaitCardPrefab, transform);

                    Card.GetComponent<RectTransform>().anchoredPosition = DefaultPosi[i];

                    Destroy(Cards[i].gameObject);

                    Cards[i] = Card.GetComponent<RectTransform>();
                    Cards[i].GetComponent<CanvasGroup>().alpha=1;

                    MyChoiceCount++;
                }
                
            }
        }
    }

    public IEnumerator OrderInitialize(bool[] Order)
    {
        //全破壊
        foreach (Transform Child in transform)
        {
            Destroy(Child.gameObject);
        }

        CharaBase Enemy = BattleManager.BM.Enemy;

        ShortOrder = new List<bool>();

        int EnemyChoiceCount = 0;
        float KanKaku = 0,Motoposi=0;

        //敵の行動だけど敵がカードを選んでいない行動を消す
        for (int i=0;i<4;i++)
        {
            if (Order[i])
            {
                ShortOrder.Add(true);
            }
            else
            {
                //敵が何かしらのカードを選択していたなら
                if (Enemy.Choiced.Count > EnemyChoiceCount)
                {
                    ShortOrder.Add(false);
                }

                EnemyChoiceCount++;
            }
        }

        KanKaku = 110f;
        Motoposi = ShortOrder.Count * -41.25f;

        for (int i=0;i<ShortOrder.Count;i++)
        {
            GameObject Instance=null;

            if (ShortOrder[i])
            {
                Instance = WaitCardPrefab;
            }
            else
            {
                Instance = HatenaCardPrefab;
            }


            Cards[i]=(Instantiate(Instance, transform).GetComponent<RectTransform>());

            Cards[i].anchoredPosition = new Vector2(Motoposi + i * KanKaku + 50, 0);

            Cards[i].DOAnchorPosX(Motoposi + i * KanKaku, 0.3f);
            Cards[i].GetComponent<CanvasGroup>().DOFade(1f,0.3f);

            DefaultPosi.Add(new Vector2(Motoposi+i*KanKaku,0));

            yield return new WaitForSeconds(0.3f);
        }
    }
}
