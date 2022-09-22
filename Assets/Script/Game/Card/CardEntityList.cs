using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "CardEntityList", menuName = "Create CardEntityList")]
public class CardEntityList : ScriptableObject
{
    [SerializeField]
    private List<CardEntity> EntityList;

    public CardEntity GetCardData(int Num)
    {
        return EntityList.Find(Data=>Data.CardID==Num);
    }
}