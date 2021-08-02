using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class GameCamera : MonoBehaviour
{
    protected CinemachineVirtualCamera _virtualCam;
    protected CinemachineVirtualCamera virtualCam
    {
        get
        {
            if(_virtualCam == null)
            {
                _virtualCam = this.GetComponent<CinemachineVirtualCamera>();
            }
            return _virtualCam;
        }
    }

    public void SetFollowTarget(Transform target)
    {
        virtualCam.Follow = target;
    }

    public void Toggle(bool enable)
    {
        this.gameObject.SetActive(enable);
    }
}