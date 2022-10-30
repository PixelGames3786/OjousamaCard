using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public SaveLoadManager SLM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //はじめから
    public void ForBeginning()
    {
        SLM.NextNovel = 0;

        SceneManager.LoadSceneAsync("Novel");
    }

    //つづきから
    public void ForContinue()
    {
        //ここにデータがあるかチェックする

    }
}
