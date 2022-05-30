using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private Light sunlight;
    [SerializeField] private Transform lifeBall;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private ParticleSystem lifePS;
    [SerializeField] private GameObject pauseCanvas;
    
    private DeathSourceController deathSource;
    private GameState gameState = GameState.Playing;
    private bool isPaused = false;
    
    public enum GameState
    {
        Playing,
        Outro
    }
    
    private void Awake()
    {
        Instance = this;
        sunlight.intensity = 1f;
        lifeBall.localScale = Vector3.zero;
        SetPauseCanvas(false);
    }

    private void Update()
    {
        if (gameState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Pause) || Input.GetKeyDown(KeyCode.Tab) ||
                Input.GetKeyDown(KeyCode.Space))
            {
                TogglePauseCanvas();
            }
        }
    }

    public bool IsInPlayingState()
    {
        return gameState == GameState.Playing;
    }
    
    public GameState GetGameState()
    {
        return gameState;
    }
    
    public void SetGameState(GameState newGameState)
    {
        gameState = newGameState;
    }

    private void TogglePauseCanvas()
    {
        isPaused = !isPaused;
        SetPauseCanvas(isPaused);
    }

    private void SetPauseCanvas(bool newPauseState)
    {
        pauseCanvas.SetActive(newPauseState);
    }
    
    public void RegisterDeathSource(DeathSourceController newDS)
    {
        deathSource = newDS;
    }
    
    public void TreeGrown(int treeID)
    {
        deathSource.IgniteTorch(treeID);
        sunlight.intensity += 0.25f;
    }

    public void TriggerWin()
    {
        StartCoroutine(PlayOutroSequence());
    }

    private IEnumerator PlayOutroSequence()
    {
        SetPauseCanvas(false);
        // Impulse
        AudioController.Instance.PlayOutroBoom();
        lifePS.Play();
        yield return new WaitForSeconds(0.1f);
        impulseSource.GenerateImpulse(1f);
        //TODO Change Wall material to green?
        // Flip all remaining tiles to life
        lifeBall.DOScale(60f, 5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);
        yield return fadeController.FadeOverlay(false, 3f, false);
        LoadWinScreen();
    }

    private void LoadWinScreen()
    {
        SceneManager.LoadScene("WinScene");
    }
    
    public void QuitGame()
    {
        SceneManager.LoadScene("HomeMenu");
    }
}
