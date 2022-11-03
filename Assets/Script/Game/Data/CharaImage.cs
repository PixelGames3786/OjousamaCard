using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaImages", menuName = "Create CharaImage")]

public class CharaImage : ScriptableObject
{
    public Sprite NormalSprite,PATKSprite,MATKSprite,DamageSprite,DifenseSprite;

    public Vector2 NormalSize, PATKSize, MATKSize, DamageSize, DifenseSize;

    public Vector2 NormalPosi, PATKPosi, DamagePosi;
}
