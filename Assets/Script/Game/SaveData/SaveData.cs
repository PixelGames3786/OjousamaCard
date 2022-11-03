using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public enum SceneType
    {
        Novel,
        Adventure,
    }

    public SceneType Scene;

    //ノベルシーンでセーブした場合の行数
    public int CurrentNovelId;

    //初期設定
    [SerializeField]
    public List<int> HavingCards=new List<int> { 1, 2, 2, 3, 4, 4, 5, 6, 7, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, 
                     MyDecks=new List<int>() { 1, 2, 2, 3, 4, 4, 5, 6, 6};

}
