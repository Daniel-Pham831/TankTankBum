using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformation : MonoBehaviour
{
    public static GameInformation Singleton { get; private set; }
    public int MaxPlayer = 10;
    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        DontDestroyOnLoad(this.gameObject);
    }
}
