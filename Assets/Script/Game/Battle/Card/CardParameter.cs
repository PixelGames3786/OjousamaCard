using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardParameter01", menuName = "Create CardParameter")]

public class CardParameter : ScriptableObject
{
    public int CardID;
    public string Name;
    public int Cost;
    public int Power;
    public float WaitTime;
    public Sprite Icon;

    public string ScriptName;
}
