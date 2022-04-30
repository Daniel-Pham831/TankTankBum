using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public abstract class Interpolator<T>
{
    public int TotalStepsToTarget;
    public int Counter;
    protected T PreTargetValue;

    public Interpolator(int totalStepsToTarget)
    {
        TotalStepsToTarget = SetTotal(totalStepsToTarget);
        Counter = SetCounter(0);
    }

    protected virtual int SetCounter(int value)
    {
        if (value > TotalStepsToTarget || value <= 0)
            return 0;
        else
            return value;
    }

    protected virtual int SetTotal(int totalStepsToTarget)
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

    public abstract T Interpolate(T currentValue, T targetValue);
}
