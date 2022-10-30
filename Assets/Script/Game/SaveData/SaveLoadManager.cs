using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    static public SaveLoadManager instance;

    public SaveData Data;

    public string NextBattle;

    public int NextNovel;

    //シングルトン処理
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AllLoad();
    }

    private void AllLoad()
    {
        //セーブデータがあったらロードする
        if (File.Exists(Application.persistentDataPath + "/SaveData/SaveData"))
        {
            Data = Load();
        }
        else
        {
            //なかったら初期化する
            SaveDataInitialize();

            //FirstPlay = true;
        }
    }

    //セーブデータのロード
    private SaveData Load()
    {
        //後で直す
        SaveData data = new SaveData();

        return data;
    }

    private void SaveDataInitialize()
    {
        Data = new SaveData();
    }

}
