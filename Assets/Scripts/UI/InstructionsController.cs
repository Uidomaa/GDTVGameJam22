using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InstructionsController : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] instructions;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private float textFadeOutTime = 0.5f;
    [SerializeField] private float textFadeInTime = 0.5f;

    private MenuController menuController;
    private int instructionIndex = -1;

    private void Awake()
    {
        // Turn off all pages
        foreach (var canvasGroup in instructions)
        {
            canvasGroup.alpha = 0;
        }
        nextButton.SetActive(false);
    }

    public void Begin(MenuController controller, float beginDelay)
    {
        menuController = controller;
        // Fade in first instructions
        StartCoroutine(ShowNextInstruction(beginDelay));
    }

    public void NextPressed()
    {
        nextButton.SetActive(false);
        StartCoroutine(ShowNextInstruction());
    }
    
    private IEnumerator ShowNextInstruction(float waitTime = 0f)
    {
        yield return new WaitForSeconds(waitTime);
        // fade out previous instruction
        if (instructionIndex > -1)
        {
            yield return instructions[instructionIndex].DOFade(0f, textFadeOutTime).SetEase(Ease.InOutSine).WaitForCompletion();
        }
        // Increment index
        instructionIndex++;
        // Finish if instructions are done
        if (instructionIndex >= instructions.Length)
        {
            Finished();
            yield break;
        }
        // fade in next instruction
        yield return instructions[instructionIndex].DOFade(1f, textFadeInTime).SetEase(Ease.InOutSine).WaitForCompletion();
        // Fade in next button 
        nextButton.SetActive(true);
    }

    private void Finished()
    {
        StartCoroutine(menuController.LoadGameScene());
    }
}
