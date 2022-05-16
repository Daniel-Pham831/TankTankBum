using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeColor : MonoBehaviour
{
    [SerializeField] private GrenadeColorInformation grenadeColorInformation;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem outLineGlow;
    [SerializeField] private MeshRenderer grenadeModelMeshRenderer;
    public void SetupGrenadeColor(Team team)
    {
        Color desiredColor = grenadeColorInformation.GetColorBasedOnTeam(team);
        Material desiredMaterial = grenadeColorInformation.GetMaterialBasedOnTeam(team);

        trail.startColor = desiredColor;

        var main = outLineGlow.main;
        main.startColor = desiredColor;

        grenadeModelMeshRenderer.material = desiredMaterial;
    }




}
