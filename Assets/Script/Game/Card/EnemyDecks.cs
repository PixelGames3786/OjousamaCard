using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDecks", menuName = "Create EnemyDecks")]
public class EnemyDecks : ScriptableObject
{
    [SerializeField]
    private List<string> EnemyDeck = new List<string>();

    public int[] ReturnDeck(int Num)
    {
        int[] Deck = new int[20];

        string[] SplitDeck=EnemyDeck[Num].Split(',');

        for (int i=0;i<20;i++)
        {
            Deck[i] = int.Parse(SplitDeck[i]);
        }

        return Deck;
    }
}
