using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInputSource : InputSource
{

    public float inputOverlappingPreventTime = 0.5f;
    private Dictionary<Button, float> lastInputTimes;
    private Dictionary<Button, bool> isDowns;
    private float horizontalAxisLeftStick = 0f;
    private float verticalAxisLeftStick = 0f;

    private void Awake()
    {
        lastInputTimes = new Dictionary<Button, float>
        {
            { Button.Cross, 0f },
            { Button.Circle, 0f },
            { Button.Square, 0f },
            { Button.Triangle, 0f }
        };

        isDowns = new Dictionary<Button, bool>
        {
            { Button.Cross, false },
            { Button.Circle, false },
            { Button.Square, false },
            { Button.Triangle, false }
        };
    }

    public void SetButtonDown(Button button)
    {
        if(Time.unscaledTime - lastInputTimes[button] > inputOverlappingPreventTime)
        {
            isDowns[button] = true;
            lastInputTimes[button] = Time.unscaledTime;
        }
    }
    public override bool GetButtonDown(Button button)
    {
        bool retVal = isDowns[button];
        isDowns[button] = false;
        return retVal;
    }

    public void SetHorizontalAxisLeftStick(float value)
    {
        horizontalAxisLeftStick = value;
    }
    public void SetVerticalAxisLeftStick(float value)
    {
        verticalAxisLeftStick = value;
    }
    public override float GetHorizontalAxisLeftStick()
    {
        float retVal = horizontalAxisLeftStick;
        horizontalAxisLeftStick = 0f;
        return retVal;
    }
    public override float GetVerticalAxisLeftStick()
    {
        float retVal = verticalAxisLeftStick;
        verticalAxisLeftStick = 0f;
        return retVal;
    }
}
