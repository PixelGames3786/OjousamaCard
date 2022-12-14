using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    public SaveLoadManager SLM;

    public Image Fader;

    public int NovelStartPoint;

    // Start is called before the first frame update
    void Start()
    {
        //BGMManager.instance.BGMPlay("bgm0",2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //はじめから
    public void ForBeginning()
    {
        SLM.NextNovel = NovelStartPoint;

        Fader.gameObject.SetActive(true);

        Fader.DOFade(1f, 1f).OnComplete(() => 
        {
            SceneController.instance.StartSceneLoad("Novel");
        });

    }

    //つづきから
    public void ForContinue()
    {
        //ここにデータがあるかチェックする

    }
}
