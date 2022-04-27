using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for storing ONE TankInformation
*/
public class TankInformation : MonoBehaviour
{
    public byte ID;
    public Team Team;
    public bool IsLocalPlayer;
    public bool IsHost;
}
