using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaParameter", menuName = "CreateAsset/CharaParameter")]

public class CharaParameter : ScriptableObject
{
    public string Name;

    public int HP,MaxHP;

    public int Cost, MaxCost;

    public string DeckSeed;

    public string ScriptName;

    [NonSerialized]
    public List<int> Deck, HandCard, Choiced;

    [NonSerialized]
    public List<BuffBase> NowBuffs=new List<BuffBase>();

    [NonSerialized]
    public CharaParameter Enemy;
}
