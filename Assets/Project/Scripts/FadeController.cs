using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Image))]
public class FadeController : MonoBehaviour
{
    public static FadeController Singleton { get; private set; }

    public float fadeDuration = 2.5f;

    private float fadeTimer;
    private float fadeTimerStart;
    private Image fadePanel;

    private Action onFadeDone;

    private void Awake()
    {
        if( Singleton == null )
            Singleton = this;

        fadePanel = GetComponent<Image>();
    }
    void Start()
    {
        FadeIn();
    }

    [Button]
    public void FadeIn( Action onComplete = null )
    {
        onFadeDone = onComplete;
        StartCoroutine( FadeInCoroutine() );
    }

    IEnumerator FadeInCoroutine()
    {
        Time.timeScale = 0f;
        fadeTimerStart = Time.unscaledTime;
        fadeTimer = Time.unscaledTime + fadeDuration;
        fadePanel.enabled = true;
        fadePanel.color = Color.black;
        while( true )
        {
            if( Time.unscaledTime > fadeTimer )
            {
                fadePanel.enabled = false;
                break;
            }
            else
            {
                float a = VectorExtras.ReverseLerp(Time.unscaledTime, fadeTimerStart, fadeTimerStart + fadeDuration);
                a = VectorExtras.MirrorValue(a, 0f, 1f);
                fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, a);
            }
            yield return null;
        }
        Time.timeScale = 1f;
        if( onFadeDone != null )
            onFadeDone.Invoke();
    }
    
    [Button]
    public void FadeOut( Action onComplete = null )
    {
        onFadeDone = onComplete;
        StartCoroutine( FadeOutCoroutine() );
    }

    IEnumerator FadeOutCoroutine()
    {
        Time.timeScale = 0f;
        fadeTimerStart = Time.unscaledTime;
        fadeTimer = Time.unscaledTime + fadeDuration;
        fadePanel.enabled = true;
        fadePanel.color = Color.clear;
        while( true )
        {
            yield return null;
            if( Time.unscaledTime > fadeTimer )
            {
                fadePanel.color = Color.black;
                break;
            }
            else
            {
                float a = VectorExtras.ReverseLerp(Time.unscaledTime, fadeTimerStart, fadeTimerStart + fadeDuration);
                fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, a);
            }
        }
        //Time.timeScale = 1f;
        if( onFadeDone != null )
            onFadeDone.Invoke();
    }

}
