using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardParameter01", menuName = "Create CardParameter")]

public class CardParameter : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Shield,
        Draw,
        CostRecover,
        Recover,
        SelfDamage,
        BuffToMe,
        BuffToEne
    }

    public enum Rarity
    {
        Common,
        UnCommon,
        Rare,
        SuperRare
    }

    public int CardID;
    public string Name;

    [TextArea]
    public string Description;
    public Rarity Rare;
    public int Cost;

    //çUåÇâÒêî
    public int MoveNum;
    public int DisplayPower;
    public List<int> MinPowers=new List<int>(), MaxPowers=new List<int>();

    public float WaitTime;
    public Sprite Icon;
    public AudioClip UseSE;

    public List<CardType> Types=new List<CardType>();
    public CharaDisplayManager.MoveType MyChara, Enemy;

    public string ScriptName;
}
