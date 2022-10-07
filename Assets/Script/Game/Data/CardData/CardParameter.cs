using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardParameter01", menuName = "Create CardParameter")]

public class CardParameter : ScriptableObject
{
    public enum CardType
    {
        Attack,
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
    public string Name,Description;
    public Rarity Rare;
    public int Cost;
    public int Power;
    public float WaitTime;
    public Sprite Icon;

    public CardType Type;
    public CharaDisplayManager.MoveType MyChara, Enemy;

    public string ScriptName;
}
