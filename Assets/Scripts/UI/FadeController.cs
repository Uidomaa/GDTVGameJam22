using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeController : MonoBehaviour
{
    [SerializeField] private bool shouldFadeInOnAwake = true;
    [SerializeField] private float fadeInTime = 1f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (shouldFadeInOnAwake)
        {
            if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); }
            fadeCoroutine = StartCoroutine(FadeOverlay(true, fadeInTime));
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    public IEnumerator FadeOverlay(bool isFadingIn, float totalFadeTime = 1f)
    {
        float curAlpha = isFadingIn ? 1f : 0f;
        float curFadeTime = 0f;

        canvasGroup.alpha = curAlpha;

        while (curFadeTime < totalFadeTime)
        {
            curFadeTime += Time.deltaTime;
            var curFade = curFadeTime / totalFadeTime;
            canvasGroup.alpha = isFadingIn ? 1 - curFade : curFade;
            yield return null;
        }

        canvasGroup.alpha = isFadingIn ? 0f : 1f;
        canvasGroup.interactable = !isFadingIn;
        canvasGroup.blocksRaycasts = !isFadingIn;
    }
}
