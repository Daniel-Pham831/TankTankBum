using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeColorInformation", menuName = "TankTankBum/GrenadeColorInformation", order = 0)]
public class GrenadeColorInformation : ScriptableObject
{
    public Color blueGrenadeColor;
    public Color redGrenadeColor;
    public Material blueGrenadeMaterial;
    public Material redGrenadeMaterial;

    public Color GetColorBasedOnTeam(Team team) => team == Team.Blue ? blueGrenadeColor : redGrenadeColor;

    public Material GetMaterialBasedOnTeam(Team team) => team == Team.Blue ? blueGrenadeMaterial : redGrenadeMaterial;
}
