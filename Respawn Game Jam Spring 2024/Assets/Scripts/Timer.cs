using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Total duration of timer
    public float duration;

    // Current time left on the timer
    public float timeLeft;

    // Is the timer active
    public bool active;

    // Is the timer finished
    public bool finished = false;

    // Has the timer been started
    private bool started = false;

    public void Init(string name, float duration)
    {
        this.name = name;

        this.duration = duration;

        timeLeft = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            started = true;

            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                finished = true;

                active = false;
            }
        }
    }


    // Resets the timer and then starts it
    public void Begin()
    {
        Reboot();

        active = true;

        started = true;
    }

    // Resets the timer so that its ready to be started
    public void Reboot()
    {
        timeLeft = duration;

        finished = false;

        active = false;

        started = false;
    }

    // Pauses the timer
    public void Pause()
    {
        active = false;
    }

    // Resumes the timer
    public void Resume()
    {
        active = true;
    }

    // Checks if the timer is finished, and resets the timer if it is finish, and if the reset parameter is true
    public bool IsFinished(bool reset)
    {
        bool isFinished = finished;

        if (finished && reset)
        {
            Reboot();
        }

        return isFinished;
    }

    // WILL RETURN TRUE EVEN IF THE TIMER IS PAUSED
    public bool HasStarted()
    {
        return started;
    }

    public bool IsActive()
    {
        return active;
    }

    public float GetTime()
    {
        return timeLeft;
    }
}
