using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private InstructionsController instructions;
    [SerializeField] private CinemachineVirtualCamera deathSourceVirtualCam;

    private void Awake()
    {
        deathSourceVirtualCam.Priority = 0;
    }

    public void BeginPressed()
    {
        Debug.Log("Begin pressed!");
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
        //TODO Fade audio if applicable
        yield return StartCoroutine(fadeController.FadeOverlay(false, 2f));
        SceneManager.LoadScene("GameScene");
    }
}
