using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class script_EssenceDispencer_Power : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private bool isActive = false;
    [SerializeField] private Player_controle Player;
    [SerializeField] private Light2D obj_light;
    [SerializeField] private int Essence_Type;

    private void Awake() {
        obj_light = this.GetComponentInChildren<Light2D>();
    }

    private void Start() {

        Pylone_type_selector(Essence_Type);

    }
    private void Update() {

        On_use();

    }

    private void Pylone_type_selector(int set) {

        switch (set) {

            case 1:
                obj_light.color = new Color(1f, 0f, 0f);
            break;
                
            case 2:
                obj_light.color = new Color(0.19f, 0.92f, 0.82f);
            break; 

            case 3:
                obj_light.color = new Color(0.92f, 0.78f, 0.19f);
            break;

        }
    }

    private void On_use() {
        if (isActive) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (Player.Essence_Get(Essence_Type, 0)) {
                    obj_light.enabled = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        isActive = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isActive = false;
    }

    public int Get_EssenceType() {
        return Essence_Type;
    }



}
