using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation Singleton { get; private set; }

    private Player myInformation;
    public List<Player> playerList;
    public List<byte> idList;
    public List<string> nameList;

    public Action<Player> OnNewJoinedPlayer;
    void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        this.myInformation = new Player();

        this.idList = new List<byte>();
        this.nameList = new List<string>();

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
            NetUtility.C_JOIN += this.OnNewJoinClient;
            MainMenuUI.Singleton.OnHostOrJoinRoom += this.OnHostOrJoinRoom;
        }
        else
        {
            NetUtility.C_WELCOME -= this.OnWelcomeClient;
            NetUtility.C_JOIN -= this.OnNewJoinClient;
            MainMenuUI.Singleton.OnHostOrJoinRoom -= this.OnHostOrJoinRoom;
        }
    }

    private void OnHostOrJoinRoom(string inputName)
    {
        this.myInformation.Name = inputName;
    }

    private void OnNewJoinClient(NetMessage message)
    {
        NetJoin joinMessage = message as NetJoin;

        this.playerList.Add(joinMessage.JoinedPlayer);

        Debug.Log($"{joinMessage.JoinedPlayer.Name} just joined");

        this.idList.Add(joinMessage.JoinedPlayer.Id);
        this.nameList.Add(joinMessage.JoinedPlayer.Name);

        this.OnNewJoinedPlayer?.Invoke(joinMessage.JoinedPlayer);
    }

    private void OnWelcomeClient(NetMessage message)
    {
        NetWelcome welcomeMessage = message as NetWelcome;

        this.myInformation.Id = welcomeMessage.AssignedId;
        this.myInformation.Team = welcomeMessage.Team;
        this.myInformation.SlotIndex = welcomeMessage.SlotIndex;

        this.playerList = welcomeMessage.PlayerList;

        foreach (Player player in this.playerList)
        {
            this.idList.Add(player.Id);
            this.nameList.Add(player.Name);
            this.OnNewJoinedPlayer?.Invoke(player);
        }

        Debug.Log("Connected To Server");
        Debug.Log($"My ID:{this.myInformation.Id}\nMy Name:{this.myInformation.Name}");


        this.OnNewJoinedPlayer?.Invoke(this.myInformation);
    }
}
