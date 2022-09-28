using System.Collections;
using UnityEngine;

abstract public class CardBase : ScriptableObject
{
    public int CardID;
    public string Name;
    public int Cost;
    public int Power;
    public Sprite Icon;

    abstract public IEnumerator CardProcess();
}