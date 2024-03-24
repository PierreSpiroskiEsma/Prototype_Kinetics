using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BadBoi : MonoBehaviour {

    [SerializeField] private int Life_Point;
    [SerializeField] private char Enemeie_Type;


    private void Update() {
        Is_Dead();
    }

    // Use To inflickt damlage on himself
    public void Damage_Taken (char Damage_Type, int Attack_Strengh) {

        if (Damage_Type == Enemeie_Type) {

            Life_Point = Life_Point - Attack_Strengh;
            Debug.Log("the enemie has taken " + Attack_Strengh + " damage, it now have " + Life_Point + " Life point left.");
        }
        else {

            Debug.Log("the enemie has taken " + Attack_Strengh + " damage, it now have " + Life_Point + " Life point left.");
        }
    }

    // check if ther any hp left
    private void Is_Dead() {

        if (Life_Point <= 0) {

            Destroy(gameObject, 0.5f);
        }

    }



}