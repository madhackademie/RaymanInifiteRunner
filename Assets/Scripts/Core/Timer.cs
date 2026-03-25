using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic countdown or stopwatch timer. Can loop, auto-start, and notify listeners on tick and completion.
/// </summary>
public class Timer : MonoBehaviour
{
    public enum TimerMode { Countdown, Stopwatch }

    [Header("Settings")]
    [SerializeField] private TimerMode mode = TimerMode.Countdown;
    [SerializeField] private float duration = 10f;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool loop = false;

    [Header("Events")]
    public UnityEvent onCompleted;
    public UnityEvent<float> onTick;

    private float elapsedTime;
    private bool isRunning;

    public float ElapsedTime => elapsedTime;
    public float RemainingTime => Mathf.Max(0f, duration - elapsedTime);
    public float NormalizedProgress => duration > 0f ? Mathf.Clamp01(elapsedTime / duration) : 0f;
    public bool IsRunning => isRunning;

    private void Start()
    {
        if (autoStart)
            StartTimer();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        elapsedTime += Time.deltaTime;
        float currentValue = mode == TimerMode.Countdown ? RemainingTime : elapsedTime;
        onTick.Invoke(currentValue);

        if (elapsedTime >= duration)
        {
            elapsedTime = duration;
            onCompleted.Invoke();

            if (loop)
                Restart();
            else
                Stop();
        }
    }

    /// <summary>Starts the timer from its current elapsed time.</summary>
    public void StartTimer()
    {
        isRunning = true;
    }

    /// <summary>Pauses the timer without resetting it.</summary>
    public void Pause()
    {
        isRunning = false;
    }

    /// <summary>Resets elapsed time and stops the timer.</summary>
    public void Stop()
    {
        isRunning = false;
        elapsedTime = 0f;
    }

    /// <summary>Resets and immediately starts the timer.</summary>
    public void Restart()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    /// <summary>Sets a new duration and restarts the timer.</summary>
    public void SetDuration(float newDuration)
    {
        duration = Mathf.Max(0f, newDuration);
        Restart();
    }
}
