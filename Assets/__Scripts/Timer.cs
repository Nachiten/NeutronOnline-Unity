using System;
using UnityEngine;

[Serializable]
public class Timer
{
    private float timer;
    private float timerMax;

    private bool decreasing;
    private bool loop;

    private bool finished;
    
    public Timer(float timerMax, bool decreasing = true, bool loop = true)
    {
        this.timerMax = timerMax;
        timer = decreasing ? timerMax : 0;
        this.decreasing = decreasing;
        this.loop = loop;
    }
    
    public Timer(float timerMax, float timer, bool decreasing = true, bool loop = true)
    {
        this.timerMax = timerMax;
        this.timer = timer;
        this.decreasing = decreasing;
        this.loop = loop;
    }

    public bool TryFinishTimer()
    {
        if (finished)
            return false;
        
        if (!TryEndTimer()) 
            return false;
        
        finished = true;
        
        if (loop)
            Reset();

        return true;
    }

    private bool TryEndTimer()
    {
        if (decreasing)
        {
            timer -= Time.deltaTime;
            return timer < 0;
        }
      
        timer += Time.deltaTime;
        return timer > timerMax;
    }

    public void Reset()
    {
        timer = decreasing ? timerMax : 0;
        finished = false;
    }
    
    public void SetTimerMax(float _timerMax, bool reset = true)
    {
        timerMax = _timerMax;
        
        if (reset)
            Reset();
    }
    
    private void SetTimer(float _timer)
    {
        timer = _timer;
    }
    
    public float GetTimerNormalized()
    {
        return timer / timerMax;
    }
    
    public float GetTimer() => timer;
    public float GetTimerMax() => timerMax;
}