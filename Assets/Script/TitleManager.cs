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

    //�͂��߂���
    public void ForBeginning()
    {
        SLM.NextNovel = 0;

        SceneManager.LoadSceneAsync("Novel");
    }

    //�Â�����
    public void ForContinue()
    {
        //�����Ƀf�[�^�����邩�`�F�b�N����

    }
}
