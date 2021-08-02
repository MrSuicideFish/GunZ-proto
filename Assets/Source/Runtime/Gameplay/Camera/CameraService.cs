using UnityEngine;
using Cinemachine;

public static class CameraService
{
    private const string CombatCameraPath = "_combat_camera";

    public static GameCamera CreateCombatCamera()
    {
        var camRes = Resources.Load<GameCamera>(CombatCameraPath);
        if(camRes != null)
        {
            return GameObject.Instantiate<GameCamera>(camRes);
        }
        return null;
    }
}