using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    public AudioSource BGMAudio,SEAudio;

    //ÉVÉìÉOÉãÉgÉìèàóù
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        
    }

    public void SetLoop(string Set,bool Loop)
    {
        if (Set=="BGM")
        {
            BGMAudio.loop = Loop;
        }
        else
        {
            SEAudio.loop = Loop;

        }
    }

    public void SEPlay(string Name,float FadeTime)
    {
        AudioClip Clip = Resources.Load<AudioClip>("SE/" + Name);

        SEAudio.clip = Clip;

        SEAudio.Play();
        SEAudio.DOFade(1f, FadeTime);
    }

    public void SEStop(float FadeTime)
    {
        SEAudio.DOFade(0f, FadeTime).OnComplete(()=> { SEAudio.Stop(); });
    }

    public void BGMPlay(string Name,float FadeTime,AudioClip BGM=null)
    {

        if (BGM!=null)
        {
            BGMAudio.clip = BGM;

            BGMAudio.Play();
            BGMAudio.DOFade(1f, FadeTime);

            return;
        }

        AudioClip Clip = Resources.Load<AudioClip>("BGM/"+Name);

        print("BGM/"+Name);
        print(Clip);

        BGMAudio.clip = Clip;

        BGMAudio.Play();
        BGMAudio.DOFade(1f,FadeTime);
    }

    public void BGMStop(float FadeTime)
    {
        BGMAudio.DOFade(0f, FadeTime).OnComplete(() => { BGMAudio.Stop(); });

    }
}
