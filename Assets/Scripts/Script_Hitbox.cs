using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Hitbox : MonoBehaviour
{
    [SerializeField] GameObject User;
    [SerializeField] GameObject target;

    BoxCollider2D User_BoxCollider;
    BoxCollider2D Target_BoxCollider;

    private void Awake() {

        User_BoxCollider = User.GetComponent<BoxCollider2D>();
        Target_BoxCollider = target.GetComponent<BoxCollider2D>();

    }
}
