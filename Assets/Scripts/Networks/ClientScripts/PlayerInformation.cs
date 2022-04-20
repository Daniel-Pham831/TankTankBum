using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation Singleton { get; private set; }

    private Player myInformation;
    public List<Player> playerList;

    void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        this.myInformation = new Player();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        this.registerToEvent(true);
    }

    private void OnDestroy()
    {
        Singleton = null;
        this.registerToEvent(false);

    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_WELCOME += this.OnWelcomeClient;
            MainMenuUI.Singleton.OnHostOrJoinRoom += this.OnHostOrJoinRoom;
        }
        else
        {
            NetUtility.C_WELCOME -= this.OnWelcomeClient;
            MainMenuUI.Singleton.OnHostOrJoinRoom -= this.OnHostOrJoinRoom;
        }
    }

    private void OnHostOrJoinRoom(string inputName)
    {
        this.myInformation.Name = inputName;
    }

    private void OnWelcomeClient(NetMessage message)
    {
        NetWelcome welcomeMessage = message as NetWelcome;

        this.myInformation.Id = welcomeMessage.AssignedId;
        this.playerList = welcomeMessage.PlayerList;

        Debug.Log("Connected To Server");
        Debug.Log($"My ID:{this.myInformation.Id}\nMy Name:{this.myInformation.Name}");

    }
}
