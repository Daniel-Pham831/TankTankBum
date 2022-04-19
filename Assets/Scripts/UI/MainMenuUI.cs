using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public MainMenuUI Singleton { get; private set; }

    private Animator mainMenuAnimator;

    private void Awake()
    {
        if (Singleton != null)
            return;

        Singleton = this;
    }

    private void Start()
    {
        mainMenuAnimator = GetComponent<Animator>();

    }

    public void OnOnlineBtn()
    {
        mainMenuAnimator.SetTrigger("ToHostJoinMenu");
    }

    public void OnSettingBtn()
    {

    }

    public void OnHostBtn()
    {
        mainMenuAnimator.SetTrigger("ToLobbyMenu");
    }

    public void OnJoinBtn()
    {
        mainMenuAnimator.SetTrigger("ToConnectMenu");
    }

    public void OnConnectBtn()
    {
        mainMenuAnimator.SetTrigger("ToLobbyMenu");
    }

    public void OnStartBtn()
    {

    }

    public void OnReadyBtn()
    {

    }

    public void OnLeaveBtn()
    {
        mainMenuAnimator.SetTrigger("ToOnlineSettingMenu");
    }
}
