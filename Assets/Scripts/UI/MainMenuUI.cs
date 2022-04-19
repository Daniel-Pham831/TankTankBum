using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public MainMenuUI Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null)
            return;

        Singleton = this;
    }
}
