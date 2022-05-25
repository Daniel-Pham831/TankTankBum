using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringUti", menuName = "TankTankBum/StringUti", order = 0)]
public class StringUti : SingletonScriptableObject<StringUti>
{
    [SerializeField] public string YouAreDead;
    [SerializeField] public string PingCounter;
    [SerializeField] public string FpsCounter;

    public static string Format(string format, params object[] args)
    {
        return String.Format(format, args).Replace("\\n", "\n");
    }
}

