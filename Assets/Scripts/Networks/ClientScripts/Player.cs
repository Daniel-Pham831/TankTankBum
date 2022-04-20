using System;

public class Player
{
    public byte Id;
    public string Name;

    public Player()
    {
        this.Id = 0;
        this.Name = "";
    }

    public Player(byte id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}
