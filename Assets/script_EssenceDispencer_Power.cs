using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class script_EssenceDispencer_Power : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private bool isActive = false;
    [SerializeField] private Player_controle Player;
    [SerializeField] private Light2D light;
    

    private void OnTriggerEnter2D(Collider2D collision) {
        isActive = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isActive = false;
    }

    private void Update() {
        if (isActive) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (Player.Essence_Get(1, 0)) {
                    light.enabled = false;
                }
            }
        }
    }
}
