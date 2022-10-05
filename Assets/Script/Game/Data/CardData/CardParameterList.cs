using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "CardScriptList", menuName = "Create CardScriptList")]
public class CardParameterList : ScriptableObject
{
    public List<CardParameter> ParameterList;
    public CardParameter GetCardParameter(int Num)
    {
        return ParameterList.Find(Card => Card.CardID == Num);
    }
}