using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDecks", menuName = "Create EnemyDecks")]
public class EnemyDecks : ScriptableObject
{
    [SerializeField]
    private List<string> EnemyDeck = new List<string>();

    public List<int> GetDeck(int Num)
    {
        List<int> Deck = new List<int>();

        string[] SplitDeck=EnemyDeck[Num].Split(',');

        for (int i=0;i<20;i++)
        {
            Deck.Add(int.Parse(SplitDeck[i]));
        }

        return Deck;
    }
}
