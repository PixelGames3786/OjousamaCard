using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敵や自キャラのデュエル中のステータス
public class BattleStatus
{
    public int HP=20,MaxHP=20;

    public int NowHaveCost=3;

    public List<int> Deck, HandCard, ChoicedCard;

    //今現在かかっているバフ
    public List<BuffBase> Buffs=new List<BuffBase>();

    public void HPDecrease(int Minus)
    {
        HP-=Minus;

        if (HP<=0)
        {
            BattleManager.BM.GameOver();
        }
    }
}
