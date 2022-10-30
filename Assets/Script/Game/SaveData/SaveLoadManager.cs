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

    //�V���O���g������
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
        //�Z�[�u�f�[�^���������烍�[�h����
        if (File.Exists(Application.persistentDataPath + "/SaveData/SaveData"))
        {
            Data = Load();
        }
        else
        {
            //�Ȃ������珉��������
            SaveDataInitialize();

            //FirstPlay = true;
        }
    }

    //�Z�[�u�f�[�^�̃��[�h
    private SaveData Load()
    {
        //��Œ���
        SaveData data = new SaveData();

        return data;
    }

    private void SaveDataInitialize()
    {
        Data = new SaveData();
    }

}
