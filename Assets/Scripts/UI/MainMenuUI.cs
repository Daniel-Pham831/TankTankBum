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

    public void OnOnlineBtn()
    {

    }

    public void OnSettingBtn()
    {

    }

    public void OnHostBtn()
    {

    }

    public void OnJoinBtn()
    {

    }

    public void OnConnectBtn()
    {

    }

    public void OnStartBtn()
    {

    }

    public void OnReadyBtn()
    {

    }

    public void OnLeaveBtn()
    {

    }
}
