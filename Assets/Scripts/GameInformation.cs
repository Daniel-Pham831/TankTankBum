using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformation : MonoBehaviour
{
    public static GameInformation Singleton { get; private set; }
    public int MaxPlayer = 10;
    public byte HostId = 0;
    public Color BlueTeamColor;
    public Color RedTeamColor;
    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }
}
