using System.Collections;
using UnityEngine;

public class Script_BadBoi : MonoBehaviour {

    // --- Statistic --- \\
    [Header("Enemi Statistics")]
    [SerializeField] private int Life_Point;
    [SerializeField] private char Enemeie_Type;
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private float Stun_Time;
    [SerializeField] private int Enemy_Jump_layermask;

    [Header("Attack Stetting")]
    [SerializeField] private Vector2 Enemy_Hitbox_Size;
    [SerializeField] private float Enemy_Hitbox_layermask;
    [SerializeField] private Transform Enemy_Hitbox_Transform;

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

    [Header("AI Stetting")]
    [SerializeField] private bool Is_Alive;

    // --- State --- \\
    private bool Is_Far, Is_Chasing, Is_Close, Is_Hit, Is_Attack, Is_Freeze;

    // --- QOther --- \\
    private Rigidbody2D _RigideBody;
    private SpriteRenderer _SpriteRenderer;
    private Animator _Animator;
    private bool jump_coldown;



    // ***************************************************************************************** \\
    // Setup
    // ***************************************************************************************** \\

    private void OnEnable() {

        _RigideBody = this.GetComponent<Rigidbody2D>();
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        _Animator = this.GetComponent<Animator>();

        Is_Chasing = false;
        Is_Close = false;
        Is_Far = true;
        Is_Hit = false;
        Is_Attack = false;
        Is_Freeze = false;
        jump_coldown = false;

        Enemy_Hitbox_Transform = this.transform.Find("Hitbox_Position").GetComponent<Transform>();
        
    }

    // ***************************************************************************************** \\
    // Update
    // ***************************************************************************************** \\
    private void Update() {

        if (Is_Alive && !Is_Hit && !Is_Freeze) {

            if (Target_Transform != null) {

                Chase_Control();
            }
            
        }
        
        Is_Dead();
    }

    // ***************************************************************************************** \\
    // Assign player
    // ***************************************************************************************** \\


    public bool Get_Alive() {

        return Is_Alive;
    }
    public void assign_Player(Transform _player) {

        Target_Transform = _player.transform;
    }

    // ***************************************************************************************** \\
    // Behaviour
    // ***************************************************************************************** \\
    private void Chase_Control() {

        Is_Chasing = Vector2.Distance(this.transform.position, Target_Transform.position) < Chase_Distance;
        Is_Far = Vector2.Distance(this.transform.position, Target_Transform.position) > Chase_Distance;
        Is_Close = Vector2.Distance(this.transform.position, Target_Transform.position) < Attack_Distance;

        float CleenSpeed = speed*Time.deltaTime;

        if (Is_Chasing && !Is_Close) {

            if( this.transform.position.x > Target_Transform.position.x ) {

                CleenSpeed = CleenSpeed * -1;
                _SpriteRenderer.flipX = false;
                Enemy_Hitbox_Transform.position = new Vector3(this.transform.position.x - 1f, this.transform.position.y, 0);
               

            } else {

                _SpriteRenderer.flipX = true;
                Enemy_Hitbox_Transform.position = new Vector3(this.transform.position.x + 1f, this.transform.position.y, 0);
            }

            Spring_random();
            Vector2 _velocity = _RigideBody.velocity;
            _velocity.x = CleenSpeed ;
            _RigideBody.velocity = _velocity;

        } else if (Is_Close && !Is_Attack) {

            _Animator.SetTrigger("Trigger_Attack");
            StartCoroutine(Attack_Couldown());

        }
    }

    private void Spring_random() {

        if (!jump_coldown) {

            if ((Random.Range(0, 10) == 1)) {

                _RigideBody.AddForce(new Vector2(0f, jump), ForceMode2D.Impulse);
                StartCoroutine(Couldown());

            }
        }

    }

    // ***************************************************************************************** \\
    // Debug
    // ***************************************************************************************** \\

    private void OnDrawGizmos() {

        if (Is_Alive) {
            Gizmos.DrawWireCube(Enemy_Hitbox_Transform.position, Enemy_Hitbox_Size);
        }

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
    // Attack
    // ***************************************************************************************** \\


    public void Attack_Normal() {

        Collider2D[] Enemy_Hitbox = Physics2D.OverlapBoxAll(Enemy_Hitbox_Transform.position, Enemy_Hitbox_Size, Enemy_Hitbox_layermask);

        foreach (var Target in Enemy_Hitbox) {

            if (Target.tag == "Player") {

                Debug.Log(Target.name);

            }
        }


        Debug.Log("Basic attack Is Performed");

    }
    IEnumerator Attack_Couldown() {

        Is_Attack = true;
        yield return new WaitForSeconds(Stun_Time);
        Is_Attack = false;
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
    // Freeze Methode
    // ***************************************************************************************** \\
    public void FreezOn() {

        Is_Freeze = true;
    }

    public void FreezOff() {

        Is_Freeze = false;
    }

    // ***************************************************************************************** \\
    // Death Detection and application
    // ***************************************************************************************** \\
    private void Is_Dead() {

        if (Life_Point <= 0) {

            Destroy(gameObject, 0.5f);
        }

    }

    IEnumerator Couldown()
    {

        jump_coldown = true;
        yield return new WaitForSeconds(0.5f);
        jump_coldown = false;
    }



}