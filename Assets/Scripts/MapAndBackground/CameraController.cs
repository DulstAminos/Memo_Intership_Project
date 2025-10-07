using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera vCamera;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        vCamera = GetComponent<CinemachineVirtualCamera>();
        playerController = vCamera.Follow.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.CurrentHealth == 0)
        {
            vCamera.enabled = false;
        }
    }
}
