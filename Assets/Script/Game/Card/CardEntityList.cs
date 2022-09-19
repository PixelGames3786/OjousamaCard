using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEntityList", menuName = "Create CardEntityList")]

public class CardEntityList : ScriptableObject
{
    public List<CardEntity> EntityList;
}