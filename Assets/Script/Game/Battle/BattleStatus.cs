using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�G�⎩�L�����̃f���G�����̃X�e�[�^�X
public class BattleStatus
{
    public int HP=20,MaxHP=20;

    public int NowHaveCost=3;

    public List<int> Deck, HandCard, ChoicedCard;

    //�����݂������Ă���o�t
    public List<int> BuffNumbers,BuffCounts;
}
