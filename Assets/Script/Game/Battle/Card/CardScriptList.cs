using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "CardScriptList", menuName = "Create CardScriptList")]
public class CardScriptList : ScriptableObject
{
    public List<CardBase> ScriptList;
    public CardBase GetCardScript(int Num)
    {
        return ScriptList.Find(Card => Card.CardID == Num);
    }
}