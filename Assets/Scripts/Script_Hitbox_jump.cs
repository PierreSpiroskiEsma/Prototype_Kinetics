using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Hitbox_jump : MonoBehaviour {

    [SerializeField] Player_controle Player;
    [SerializeField] Collider2D Collider;
    [SerializeField] bool ColiderCheck;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("World") || other.gameObject.CompareTag("MovingObject"))
        {
            Player.IsFalling(false);
            ColiderCheck = true;
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("World") || other.gameObject.CompareTag("MovingObject"))
        {
            Player.IsFalling(true);
            ColiderCheck = false;
        }

        
    }
}
