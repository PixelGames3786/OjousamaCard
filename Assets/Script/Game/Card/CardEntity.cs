using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(fileName = "CardEntity", menuName = "Create CardEntity")]

public class CardEntity : ScriptableObject
{
    public enum CardType 
    {
        Attack,
        Difence,
        MyBuff,
        EnemyBuff
    };

    public int CardID;
    public string Name;
    public int Cost;
    public int Power;
    public Sprite Icon;

    public CardType[] Types;
}