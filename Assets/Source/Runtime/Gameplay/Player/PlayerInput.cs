using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController _playerController;

    private void Update()
    {
        if(_playerController == null)
        {
            _playerController = this.GetComponent<PlayerController>();
            return;
        }
    }
}