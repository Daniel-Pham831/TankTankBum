using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCamera : MonoBehaviour
{
    [SerializeField] private Vector3 attackerPositionView;
    [SerializeField] private Vector3 attackerRotationEulerView;
    [SerializeField] private Vector3 defenderPositionView;
    [SerializeField] private Vector3 defenderRotationEulerView;

    public Camera ActualTankCamera;
    public Quaternion GetActualCameraRot => ActualTankCamera.transform.rotation;

    private GameObject tank;

    private void LateUpdate()
    {
        if (tank != null)
        {
            transform.position = tank.transform.position;
        }
    }

    public void SetupTankCamera(GameObject tankToFollow, Role tankRole)
    {
        tank = tankToFollow;
        SetCameraBasedOnRole(tankRole);
    }

    public void SetCameraBasedOnRole(Role role)
    {
        Vector3 targetPositionView = role == Role.Attacker ? attackerPositionView : defenderPositionView;
        Vector3 targetRotationEulerView = role == Role.Attacker ? attackerRotationEulerView : defenderRotationEulerView;

        ActualTankCamera.transform.localPosition = targetPositionView;
        ActualTankCamera.transform.localRotation = Quaternion.Euler(targetRotationEulerView);
    }
}
