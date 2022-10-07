using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaParameter", menuName = "CreateAsset/CharaParameter")]

public class CharaParameter : ScriptableObject
{
    public string CharaName;

    public int MaxHP;

    public int MaxCost;

    public string DeckSeed;

    public string ScriptName;

}


