using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Script_BadBoi : MonoBehaviour {

    [Header("Enemi Statistics")]
    [SerializeField] private int Life_Point;
    [SerializeField] private char Enemeie_Type;

    [Header("Knowkback Strenght")]
    [SerializeField] private Vector2 Light_knockback;
    [SerializeField] private Vector2 Heavy_knockback;

    private Rigidbody2D _Collider;

    private void Awake() {

        _Collider = this.GetComponent<Rigidbody2D>();

    }

    private void Update() {
        Is_Dead();
    }

    // Use To inflickt damlage on himself
    public void Damage_Taken (char Damage_Type, int Attack_Strengh, bool heavy, bool front) {

        if (Damage_Type == Enemeie_Type) {

            Life_Point = Life_Point - Attack_Strengh;
            Debug.Log("the enemie has taken " + Attack_Strengh + " damage, it now have " + Life_Point + " Life point left.");
            Knockback(heavy, front);
        }
        else {

            Debug.Log("the enemie has taken " + Attack_Strengh + " damage, it now have " + Life_Point + " Life point left.");
        }
    }


    public void Knockback (bool Heavy_damage, bool front) {


        if (Heavy_damage) {

            if (!front) {

                _Collider.AddForce(new Vector2(Heavy_knockback.x * (-1), Heavy_knockback.y), ForceMode2D.Impulse);

            } else {

                _Collider.AddForce(Heavy_knockback, ForceMode2D.Impulse);
            }

        } else {

            if (!front) {

                _Collider.AddForce(new Vector2(Light_knockback.x * (-1), Light_knockback.y), ForceMode2D.Impulse);

            } else {

                _Collider.AddForce(Light_knockback, ForceMode2D.Impulse);
            }
        }


    }

    // check if ther any hp left
    private void Is_Dead() {

        if (Life_Point <= 0) {

            Destroy(gameObject, 0.5f);
        }

    }



}