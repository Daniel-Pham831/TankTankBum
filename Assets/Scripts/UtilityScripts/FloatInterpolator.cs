using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class FloatInterpolator : Interpolator<float>
{
    public new float PreTargetValue;

    public FloatInterpolator(int totalStepsToTarget) : base(totalStepsToTarget) { }

    public override float Interpolate(float currentValue, float targetValue)
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
