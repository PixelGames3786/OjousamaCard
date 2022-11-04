using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    static public SceneController instance;

    public AsyncOperation SceneLoad, SceneUnLoad;

    private string SceneName;

    //ƒVƒ“ƒOƒ‹ƒgƒ“ˆ—
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneLoad!=null)
        {
            float LoadProgress = SceneLoad.progress;

            if (LoadProgress >= 0.9f)
            {
                SceneLoad.allowSceneActivation = true;

                LoadProgress = 0;

                SceneLoad = null;
            }
        }
    }

    public void StartSceneLoad(string SceneName)
    {
        SceneLoad = SceneManager.LoadSceneAsync(SceneName);

        SceneLoad.allowSceneActivation = false;
    }
}
