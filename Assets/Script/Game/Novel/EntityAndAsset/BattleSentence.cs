using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BattleSentence   //センテンスと読みます。文章という意味です。シナリオは文章で構成されているので、この名前にしました。
{
    public int id;              //検索するための一意な数値です。
    public string speaker;
    public string speakerSide;
    public string message;      //メッセージの本文です。
    public bool endOfTalk;      //対話が終了する最後の文章なのか判定するのに使います。
}