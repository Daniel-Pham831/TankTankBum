using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Singleton { get; private set; }
    [SerializeField] private TMP_InputField nameInputField;
    public Server server;
    public Client client;

    private Animator mainMenuAnimator;

    public Action<string> OnHostOrJoinRoom;

    private void Awake()
    {
        if (Singleton != null)
            return;

        Singleton = this;
    }

    private void Start()
    {
        mainMenuAnimator = GetComponent<Animator>();

        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            LobbyUI.Singleton.OnLobbyLeft += OnLobbyLeft;
            ClientInformation.Singleton.StartGame += StartGame;
        }
        else
        {
            LobbyUI.Singleton.OnLobbyLeft -= OnLobbyLeft;
            ClientInformation.Singleton.StartGame -= StartGame;
        }
    }

    private void StartGame()
    {
        mainMenuAnimator.SetTrigger("ToStartGame");
    }

    private void OnLobbyLeft()
    {
        mainMenuAnimator.SetTrigger("ToOnlineSettingMenu");
    }

    public void OnOnlineBtn()
    {
        mainMenuAnimator.SetTrigger("ToHostJoinMenu");
    }

    public void OnSettingBtn()
    {

    }

    /*
    Switch in to Lobby Menu
    Take the name from ipf + assign it to the player
    */
    public void OnHostBtn()
    {
        server.Init(8007, 10); //This need to change (Stop hard-coded)
        client.Init("127.0.0.1", 8007, GetPlayerName);

        OnHostOrJoinRoom?.Invoke(GetPlayerName);
        mainMenuAnimator.SetTrigger("ToLobbyMenu");
    }

    public void OnJoinBtn()
    {
        mainMenuAnimator.SetTrigger("ToConnectMenu");
    }

    public void OnConnectBtn()
    {
        client.Init("127.0.0.1", 8007, GetPlayerName);
        OnHostOrJoinRoom?.Invoke(GetPlayerName);

        mainMenuAnimator.SetTrigger("ToLobbyMenu");
    }

    private string GetPlayerName => nameInputField.text != "" ? nameInputField.text : "I forgot to name myself";
}
