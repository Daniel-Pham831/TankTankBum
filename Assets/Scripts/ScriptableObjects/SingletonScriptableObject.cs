using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T singleton;
    public static T Singleton
    {
        get
        {
            if (singleton == null)
            {
                T[] assets = Resources.LoadAll<T>("");
                if (assets == null || assets.Length < 1)
                {
                    throw new System.Exception("Could not find any singleton scriptable object instances in the resources");
                }
                else if (assets.Length > 1)
                {
                    Debug.LogWarning("There are multiple instances of the singleton scriptable object found in the resources.");
                }
                singleton = assets[0];
            }
            return singleton;
        }
    }
}
