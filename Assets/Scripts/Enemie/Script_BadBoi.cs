using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Script_BadBoi : MonoBehaviour {

    // --- Statistic --- \\
    [Header("Enemi Statistics")]
    [SerializeField] private int Life_Point;
    [SerializeField] private char Enemeie_Type;
    [SerializeField] private float speed;
    [SerializeField] private float Stun_Time;

    // --- Knockback --- \\
    [Header("Knowkback Strenght")]
    [SerializeField] private Vector2 Light_knockback;
    [SerializeField] private Vector2 Heavy_knockback;

    // --- Chase --- \\
    [Header("Chase Option")]
    [SerializeField] private Transform Target_Transform;
    [SerializeField] private float Chase_Distance;
    [SerializeField] private float actual_distance;
    [SerializeField] private float Attack_Distance;

    [SerializeField] private bool Is_Alive;

    // --- State --- \\
    private bool Is_Far, Is_Chasing, Is_Close, Is_Hit;

    // --- QOther --- \\
    private Rigidbody2D _RigideBody;
    private SpriteRenderer _SpriteRenderer;

    // ***************************************************************************************** \\
    // Setup
    // ***************************************************************************************** \\

    private void Awake() {

        _RigideBody = this.GetComponent<Rigidbody2D>();
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();

        Is_Chasing = false;
        Is_Close = false;
        Is_Far = true;
        Is_Hit = false;

    }

    // ***************************************************************************************** \\
    // Update
    // ***************************************************************************************** \\
    private void Update() {

        if (Is_Alive && !Is_Hit) {
            Chase_Control();
            actual_distance = Vector2.Distance(this.transform.position, Target_Transform.position);
        }
        
        Is_Dead();
    }

    private void Chase_Control() {

        Is_Chasing = Vector2.Distance(this.transform.position, Target_Transform.position) < Chase_Distance;
        Is_Far = Vector2.Distance(this.transform.position, Target_Transform.position) > Chase_Distance;
        Is_Close = Vector2.Distance(this.transform.position, Target_Transform.position) < Attack_Distance;

        float CleenSpeed = speed*Time.deltaTime;

        if (Is_Chasing && !Is_Close) {

            if( this.transform.position.x > Target_Transform.position.x ) {

                CleenSpeed = CleenSpeed * -1;
                _SpriteRenderer.flipX = false;

            } else {

                _SpriteRenderer.flipX = true;
            }

            Vector2 _velocity = _RigideBody.velocity;
            _velocity.x = CleenSpeed ;
            _RigideBody.velocity = _velocity;

        } else if (Is_Close) {

            // On attack

        }
    }


    private void OnDrawGizmos() {

        if (Is_Far) {

            Gizmos.color = Color.blue;

        }

        if (Is_Chasing) {

            Gizmos.color = Color.yellow;

        } 
        
        if (Is_Close) {

            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(this.transform.position, Chase_Distance);
        Gizmos.DrawWireSphere(this.transform.position, Attack_Distance);

    }

    // ***************************************************************************************** \\
    // Damage Application
    // ***************************************************************************************** \\
    public void Damage_Taken (char Damage_Type, int Attack_Strengh, bool heavy, bool front) {

        if (Damage_Type == Enemeie_Type) {

            Life_Point = Life_Point - Attack_Strengh;
            Debug.Log("the enemie has taken " + Attack_Strengh + " damage, it now have " + Life_Point + " Life point left.");
            Knockback(heavy, front);
            StartCoroutine(Stunlock());
        }
        else {

            Debug.Log("the enemie has taken " + Attack_Strengh + " damage, it now have " + Life_Point + " Life point left.");
        }
    }

    IEnumerator Stunlock() {

        Is_Hit = true;
        yield return new WaitForSeconds(Stun_Time);
        Is_Hit = false;
    }

    // ***************************************************************************************** \\
    // KnockBack Application
    // ***************************************************************************************** \\
    public void Knockback (bool Heavy_damage, bool front) {


        if (Heavy_damage) {

            if (!front) {

                _RigideBody.AddForce(new Vector2(Heavy_knockback.x * (-1), Heavy_knockback.y), ForceMode2D.Impulse);

            } else {

                _RigideBody.AddForce(Heavy_knockback, ForceMode2D.Impulse);
            }

        } else {

            if (!front) {

                _RigideBody.AddForce(new Vector2(Light_knockback.x * (-1), Light_knockback.y), ForceMode2D.Impulse);

            } else {

                _RigideBody.AddForce(Light_knockback, ForceMode2D.Impulse);
            }
        }


    }

    // ***************************************************************************************** \\
    // Death Detection and application
    // ***************************************************************************************** \\
    private void Is_Dead() {

        if (Life_Point <= 0) {

            Destroy(gameObject, 0.5f);
        }

    }



}