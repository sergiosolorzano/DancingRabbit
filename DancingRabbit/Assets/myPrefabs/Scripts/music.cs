using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour {
    float timeLeft = 3.0f;
    AudioSource bgMusic;
    float startVolume;
    float tmpVol;
    public float fadeInDuration;
    public float fadeOutDuration;

    // Use this for initialization
    void Start() {
        bgMusic = GetComponent<AudioSource>();
        StartCoroutine(FadeIn(bgMusic, fadeInDuration));
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            //StartCoroutine(FadeOut(bgMusic, fadeTime));
        }

        if (Input.GetKeyDown("return"))
        {
            StartCoroutine(FadeOut(bgMusic,fadeOutDuration));
            Debug.Log("Pressed Return");
        }
    }

    public IEnumerator FadeOut(AudioSource audiosource, float fadeTime)
    {
        float startVolume = audiosource.volume;
        while (audiosource.volume > 0)
        {
            tmpVol -= startVolume * (Time.deltaTime / fadeTime);
            audiosource.volume = Mathf.Max(tmpVol, 0);
            yield return null;
        }
    }

    public IEnumerator FadeIn(AudioSource audiosource, float fadeTime)
    {
        startVolume = tmpVol= 0.4f;
        audiosource.Play();
        bgMusic.volume = 0.1f;

        while(audiosource.volume<0.99f)
        {
            tmpVol += startVolume * (Time.deltaTime / fadeTime);
            audiosource.volume = Mathf.Min(tmpVol, 1);
            yield return null;
        }
        
    }
}