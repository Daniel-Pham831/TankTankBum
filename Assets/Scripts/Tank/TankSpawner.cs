using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    public static TankSpawner Singleton { get; private set; }
    [SerializeField] private TankSpawnerData tankSpawnerData;
    public Dictionary<byte, GameObject> Tanks { get; set; }

    private void Awake()
    {
        Singleton = this;
        Tanks = new Dictionary<byte, GameObject>();

        DontDestroyOnLoad(gameObject);
    }

}
