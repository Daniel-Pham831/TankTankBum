using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class FloatInterpolator
{
    public int TotalStepsToTarget;
    public int Counter;
    public float PreTargetValue;

    public FloatInterpolator(int totalStepsToTarget)
    {
        TotalStepsToTarget = SetTotal(totalStepsToTarget);
        Counter = SetCounter(0);
    }

    private int SetCounter(int value)
    {
        if (value > TotalStepsToTarget || value <= 0)
            return 0;
        else
            return value;
    }

    private int SetTotal(int totalStepsToTarget)
    {
        if (totalStepsToTarget <= 0)
        {
            return 1;
        }
        else
        {
            return totalStepsToTarget;
        }
    }

    public float Interpolate(float currentValue, float targetValue)
    {
        if (Counter == TotalStepsToTarget || currentValue == targetValue)
        {
            PreTargetValue = targetValue;
            Counter = SetCounter(0);
            return targetValue;
        }

        if (!PreTargetValue.Equals(targetValue))
            Counter = SetCounter(0);

        PreTargetValue = targetValue;

        return Mathf.Lerp(currentValue, targetValue, (float)Counter++ / TotalStepsToTarget);
    }
}
