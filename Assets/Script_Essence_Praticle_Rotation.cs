using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Essence_Praticle_Rotation : MonoBehaviour{

    [SerializeField] float Rotation_Speed;

    void Update() {

        float Rotation_Speed_Clean = Rotation_Speed * Time.deltaTime;
        this.transform.Rotate(0, 0, this.transform.rotation.z + Rotation_Speed_Clean);

    }
}
