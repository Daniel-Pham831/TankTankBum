using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    public TMP_Text display_Text;
    private float timeBetweenEachShowFps = 1f;
    private float nextShowFps = 0;


    public void Update()
    {
        if (Time.time >= nextShowFps)
        {
            nextShowFps += timeBetweenEachShowFps;
            display_Text.SetText(StringUti.Format(StringUti.Singleton.FpsCounter, (int)(1f / Time.deltaTime)));
        }
    }
}
