using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Script_Essence_Praticle_Rotation : MonoBehaviour{

    [SerializeField] float Rotation_Speed;
    [SerializeField] SpriteRenderer obj_Spirte;
    [SerializeField] Light2D obj_Light;

    void Update() {

        float Rotation_Speed_Clean = Rotation_Speed * Time.deltaTime;
        this.transform.Rotate(0, 0, this.transform.rotation.z + Rotation_Speed_Clean);

    }

    public void Essence_Color_Switch (int type) {

        switch (type) {

            case 1:
                obj_Spirte.color = new Color(1f, 0, 0, 1f);
                obj_Light.color = new Color(1f, 0, 0);
            break;

            case 2:
                obj_Spirte.color = new Color(0.19f, 0.92f, 0.82f, 1f);
                obj_Light.color = new Color(0.19f, 0.92f, 0.82f);
            break;

            case 3:
                obj_Spirte.color = new Color(0.92f, 0.78f, 0.19f, 1f);
                obj_Light.color = new Color(0.92f, 0.78f, 0.19f);
            break;
        }

    }

    

   
}
