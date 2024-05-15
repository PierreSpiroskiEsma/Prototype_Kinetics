using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Script_Camera_Switch : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera Activ_Cam;
    [SerializeField] private CinemachineVirtualCamera Passiv_Cam;
    private void OnTriggerEnter2D(Collider2D collision) {

        Activ_Cam.Priority = 10;
        Passiv_Cam.Priority = 0;
    }
}
