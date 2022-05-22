using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringUti", menuName = "TankTankBum/StringUti", order = 0)]
public class StringUti : SingletonScriptableObject<StringUti>
{
    [SerializeField] public string YouAreDead;

    public static string Format(string format, params object[] args)
    {
        return String.Format(StringUti.Singleton.YouAreDead, args).Replace("\\n", "\n");
    }
}

