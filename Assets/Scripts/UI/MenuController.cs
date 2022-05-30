using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private bool isHomeScene = true;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private InstructionsController instructions;
    [SerializeField] private CinemachineVirtualCamera deathSourceVirtualCam;

    private void Awake()
    {
        if (deathSourceVirtualCam != null)
        {
            deathSourceVirtualCam.Priority = 0;
        }
        if (!isHomeScene)
        {
            instructions.Begin(this, 2f);
        }
    }

    public void BeginPressed()
    {
        // Enable DeathSource Virtual Cam
        deathSourceVirtualCam.Priority = 10;
        // Fade out title
        titleCanvasGroup.interactable = false;
        titleCanvasGroup.blocksRaycasts = false;
        titleCanvasGroup.DOFade(0f, 0.1f);
        // Enable Instructions Canvas (Show text instructions with NEXT button)
        instructions.Begin(this, 3f);
    }

    public IEnumerator LoadGameScene()
    {
        AudioController.Instance.StopBGM(2f);
        AudioController.Instance.FadeMenuRumble(2f);
        yield return StartCoroutine(fadeController.FadeOverlay(false, 2f));
        SceneManager.LoadScene(isHomeScene ? "GameScene" : "HomeMenu");
    }
}
