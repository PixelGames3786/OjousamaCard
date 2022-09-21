using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敵や自キャラのデュエル中のステータス
public class BattleStatus
{
    public int HP=50,MaxHP=50;

    public int NowHaveCost=3;

    public List<int> Deck, HandCard, ChoicedCard;
}
