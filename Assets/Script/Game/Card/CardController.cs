using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    //カード番号
    [NonSerialized]
    public int CardNumber;

    private CardEntity CardData;
    public CardEntityList CardDataBase;

    public TextMeshProUGUI Name,Power,Cost;

    // Start is called before the first frame update
    void Start()
    {
        //カード番号を使ってデータベースからカードデータを取り出す
        CardData = CardDataBase.GetCardData(CardNumber);

        //名前・コスト・ダメージセット
        Name.text = CardData.Name;
        Cost.text = CardData.Cost.ToString();
        Power.text = CardData.Power.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
