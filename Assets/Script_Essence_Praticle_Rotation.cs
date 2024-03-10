using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Script_Essence_Praticle_Rotation : MonoBehaviour{

    [SerializeField] float Rotation_Speed;

    void Update() {

        float Rotation_Speed_Clean = Rotation_Speed * Time.deltaTime;
        this.transform.Rotate(0, 0, this.transform.rotation.z + Rotation_Speed_Clean);

    }

    void Essence_Color_Switch (int type) {

        switch (type) {

            case 1:
                this.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                this.GetComponent<Light2D>().color = new Color(255, 0, 0);
            break;

            case 2:
                this.GetComponent<SpriteRenderer>().color = new Color(50, 235, 210);
                this.GetComponent<Light2D>().color = new Color(50, 235, 210);
            break;

            case 3:
                this.GetComponent<SpriteRenderer>().color = new Color(235, 200, 50);
                this.GetComponent<Light2D>().color = new Color(235, 200, 50);
            break;
        }







    }

    

   
}
