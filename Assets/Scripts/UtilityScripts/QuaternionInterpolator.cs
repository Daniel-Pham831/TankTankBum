using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class QuaternionInterpolator : Interpolator<Quaternion>
{
    public new Quaternion PreTargetValue;

    public QuaternionInterpolator(int totalStepsToTarget) : base(totalStepsToTarget) { }

    public override Quaternion Interpolate(Quaternion currentValue, Quaternion targetValue)
    {
        if (Counter == TotalStepsToTarget || currentValue.eulerAngles == targetValue.eulerAngles)
        {
            PreTargetValue = targetValue;
            Counter = SetCounter(0);
            return targetValue;
        }

        if (!PreTargetValue.Equals(targetValue))
            Counter = SetCounter(0);

        PreTargetValue = targetValue;

        return Quaternion.Lerp(currentValue, targetValue, (float)Counter++ / TotalStepsToTarget);
    }
}
