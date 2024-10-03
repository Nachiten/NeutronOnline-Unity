using System;
using TMPro;
using UnityEngine;

public class GameStartTimerUI : MonoBehaviour
{
    public static event Action OnGameStartTimerFinished;
    
    [SerializeField] private TMP_Text gameStartTimerText;

    private readonly Timer timer = new(7, true, false);
    private bool timerRunning;
    
    private void Start()
    {
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
        
        gameStartTimerText.text = "Game starting in " + Mathf.CeilToInt(timer.GetTimer());
        
        if (timer.TryFinishTimer())
        {
            OnGameStartTimerFinished?.Invoke();
            StopTimer();
        }
    }
    
    private void StartTimer()
    {
        timerRunning = true;
        timer.Reset();
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
