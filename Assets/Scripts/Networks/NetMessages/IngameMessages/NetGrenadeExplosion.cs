using Unity.Networking.Transport;
using UnityEngine;

public class NetGrenadeExplosion : NetMessage
{
    public Vector3 ExplosionPosition { get; set; }
    public byte ID { get; set; }
    public Team Team { get; set; }

    public NetGrenadeExplosion(Vector3 explosionPosition, GrenadeInformation grenadeInformation)
    {
        Code = OpCode.GRENADE_EXPLOSION;
        ExplosionPosition = explosionPosition;
        ID = grenadeInformation.ID;
        Team = grenadeInformation.Team;
    }

    public NetGrenadeExplosion(ref DataStreamReader reader)
    {
        Code = OpCode.GRENADE_EXPLOSION;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteFloat(ExplosionPosition.x);
        writer.WriteFloat(ExplosionPosition.y);
        writer.WriteFloat(ExplosionPosition.z);

        writer.WriteByte(ID);
        writer.WriteByte((byte)Team);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        ExplosionPosition = new Vector3(x, y, z);

        ID = reader.ReadByte();
        Team = (Team)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_GRENADE_EXPLOSION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_GRENADE_EXPLOSION?.Invoke(this, cnn);
    }
}
