using System;
using TMPro;
using UnityEngine;

public class GameStartTimerUI : MonoBehaviour
{
    public static event Action OnGameStartTimerFinished;

    [SerializeField] private float gameStartTimerMax = 1f;
    [SerializeField] private TMP_Text gameStartTimerText;

    private readonly Timer gameStartTimer = new(0, true, false);
    private bool timerRunning;
    
    private void Start()
    {
        gameStartTimer.SetTimerMax(gameStartTimerMax);
        
        PlayerReady.Instance.OnEveryPlayerReadyChanged += OnEveryPlayerReadyChanged;
        
        SetShow(false);
    }

    private void OnDestroy()
    {
        if (PlayerReady.Instance == null)
            return;
        
        PlayerReady.Instance.OnEveryPlayerReadyChanged -= OnEveryPlayerReadyChanged;
    }

    private void OnEveryPlayerReadyChanged(bool playersReady)
    {
        if (playersReady)
            StartTimer();
        else
            StopTimer();
    }

    private void Update()
    {
        if (!timerRunning)
            return;
        
        gameStartTimerText.text = "Game starting in " + Mathf.CeilToInt(gameStartTimer.GetTimer());
        
        if (gameStartTimer.TryFinishTimer())
        {
            OnGameStartTimerFinished?.Invoke();
            StopTimer();
        }
    }
    
    private void StartTimer()
    {
        timerRunning = true;
        gameStartTimer.Reset();
        SetShow(true);
    }
    
    private void StopTimer()
    {
        timerRunning = false;
        SetShow(false);
    }
    
    private void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
