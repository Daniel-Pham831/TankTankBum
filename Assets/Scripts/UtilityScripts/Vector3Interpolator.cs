using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Vector3Interpolator : Interpolator<Vector3>
{
    public new Vector3 PreTargetValue;

    public Vector3Interpolator(int totalStepsToTarget) : base(totalStepsToTarget) { }

    public override Vector3 Interpolate(Vector3 currentValue, Vector3 targetValue)
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

        return Vector3.Lerp(currentValue, targetValue, (float)Counter++ / TotalStepsToTarget);
    }
}
