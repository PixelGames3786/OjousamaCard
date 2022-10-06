using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckBuildManager : MonoBehaviour
{
    public GameObject CardPrefab;

    public Transform DeckParent, HaveCardParent;

    public List<int> Deck,HaveCards;

    private List<Transform> DeckObjects=new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        gameObject.SetActive(true);

        Initialize();
    }

    //初期セット
    private void Initialize()
    {
        print("あ");

        Deck = SaveLoadManager.instance.Data.MyDecks;
        HaveCards = SaveLoadManager.instance.Data.HavingCards;

        //デッキ画面のセット
        Dictionary<int, int> DeckStuck = new Dictionary<int, int>();

        foreach (int Card in Deck)
        {
            if (!DeckStuck.ContainsKey(Card))
            {
                DeckStuck.Add(Card,1);
            }
            else
            {
                DeckStuck[Card]++;
            }
        }

        int[] Keys = new int[DeckStuck.Keys.Count];
        DeckStuck.Keys.CopyTo(Keys,0);

        int LineCount = DeckStuck.Keys.Count / 4;
        int LastCount = DeckStuck.Keys.Count % 4;

        for (int i=0;i<LineCount;i++)
        {
            for (int u=0;u<4;u++)
            {
                if (i==LineCount&&u>LastCount)
                {
                    break;
                }

                int NowNum = i * 4+u;
                DeckObjects.Add(Instantiate(CardPrefab,DeckParent).transform);

                DeckObjects[NowNum].GetComponent<RectTransform>().anchoredPosition = new Vector3(-290 + (u * 190),470-(i*230),0);

                //一枚以上スタックされていたら
                if (DeckStuck[Keys[NowNum]]>1)
                {
                    DeckObjects[NowNum].GetChild(1).gameObject.SetActive(true);
                    DeckObjects[NowNum].GetChild(1).GetComponent<TextMeshProUGUI>().text = "×" + DeckStuck[Keys[NowNum]];
                }
            }
        }

        //所持カード画面のセット

    }
}
