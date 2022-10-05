using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�G�⎩�L�����̃f���G�����̃X�e�[�^�X
public class BattleStatus
{
    public string Name;

    public int HP=20,MaxHP=20;

    public int NowHaveCost=3;

    public List<int> Deck, HandCard, ChoicedCard;

    //�����݂������Ă���o�t
    public List<BuffBase> Buffs=new List<BuffBase>();

    public void HPDecrease(int Minus)
    {
        HP-=Minus;

        if (HP<=0&&Name=="MyChara")
        {
            BattleManager.BM.GameOver();
        }
        else if (HP<=0&&Name=="Enemy")
        {
            BattleManager.BM.GameOver();

        }
    }
}
