using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

// AssetDatabaseを使用しているため、ビルド時には含めないようにしないとビルドエラーが起きる
#if UNITY_EDITOR
public class CreateCardsFromCsv : MonoBehaviour
{
    private const string AssetPath = "Assets/Datas/CardData/";

    // MenuItem属性を付けることでEditorの上部メニューに`ScriptableObjects > CreateEnemyParamAsset`が表示されます
    // 押下すると`CreateEnemyParamDataAsset()`が実行されます
    [MenuItem("ScriptableObjects/CreateCardAsset")]
    static void CreateCardDataAsset()
    {
        TextAsset csvFile= Resources.Load("CSV/CardParameterList") as TextAsset;

        //読み込んだCSVファイルを格納
        List<string[]> csvDatas = new List<string[]>();

        //CSVファイルの行数を格納
        int height = 0;

        //読み込んだテキストをString型にして格納
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            // ,で区切ってCSVに格納
            csvDatas.Add(line.Split(','));
            height++; // 行数加算
        }

        CardParameterList ParaList = ScriptableObject.CreateInstance<CardParameterList>();

        for (int i = 1; i < height; i++)
        {
            CardParameter CardAsset = ScriptableObject.CreateInstance<CardParameter>();

            ParaList.Cards.Add(CardAsset);

            //[i]は行数。[0]~[6]は列数
            //csvDatasはString型なのでそのまま格納できる
            CardAsset.CardID = i;
            CardAsset.Name = csvDatas[i][1];

            CardAsset.Description = csvDatas[i][7];

            //レアリティの変更をここにいれる
            CardAsset.Cost = int.Parse(csvDatas[i][2]);

            string[] Moves = csvDatas[i][4].Split('-');

            CardAsset.MoveNum = Moves.Length;

            for (int u=0;u<Moves.Length;u++)
            {
                if (Moves[u]=="")
                {
                    continue;
                }

                //タイプを入れる
                if (Moves[u].Contains("シールド"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.Shield);

                    Moves[u] = Moves[u].Replace("シールド","");
                }
                else if (Moves[u].Contains("ドロー"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.Draw);

                    Moves[u] = Moves[u].Replace("ドロー", "");

                }
                else if (Moves[u].Contains("コスト回復"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.CostRecover);

                    Moves[u]=Moves[u].Replace("コスト回復", "");
                }
                else if (Moves[u].Contains("回復"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.Recover);

                    Moves[u] = Moves[u].Replace("回復", "");

                }
                else if (Moves[u].Contains("自傷"))
                {
                    CardAsset.Types.Add(CardParameter.CardType.SelfDamage);

                    Moves[u] = Moves[u].Replace("自傷", "");

                }
                else
                {
                    CardAsset.Types.Add(CardParameter.CardType.Attack);
                }

                string[] Dameges = Moves[u].Split('~');

                CardAsset.MinPowers.Add(int.Parse(Dameges[0]));
                CardAsset.MaxPowers.Add(int.Parse(Dameges[1])+1);
            }

            CardAsset.ScriptName = csvDatas[i][9];

            //画像ロード
            if (AssetDatabase.LoadAssetAtPath("Assets/Arts/CardSprites/" + csvDatas[i][8] + ".png", typeof(Sprite)) != null)
            {
                CardAsset.Icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Arts/CardSprites/" + csvDatas[i][8] + ".png", typeof(Sprite));
            }

            //SEロード
            if (AssetDatabase.LoadAssetAtPath("Assets/Arts/Musics/SE/Cards/" + csvDatas[i][8] + ".mp3", typeof(AudioClip)) != null)
            {
                CardAsset.UseSE = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Arts/Musics/SE/Cards/" + csvDatas[i][8] + ".mp3", typeof(AudioClip));
            }

            CardAsset.WaitTime = Mathf.Clamp(float.Parse(csvDatas[i][10]), 1.5f, 999f);

            AssetDatabase.CreateAsset(CardAsset, AssetPath + csvDatas[i][8] + ".asset");

            AssetDatabase.Refresh();

        }

        AssetDatabase.CreateAsset(ParaList,"Assets/Resources/CardScriptList.asset");

        AssetDatabase.Refresh();


        //例えばenemyParamAsset.EnemyParamList.Add(hogeParam); 的な

        // 流し込んだ後は実際に作成します
        // ここで作ったアセットの置き場所であるパスの指定もできます

        // Asset作成後、反映させるために必要なメソッド
    }
}
# endif
