using System.Collections;
using UnityEngine;

public class Script_BadBoi : MonoBehaviour {

    // --- Statistic --- \\
    [Header("Enemi Statistics")]
    [SerializeField] private int Life_Point;
    [SerializeField] private char Enemeie_Type;
    [SerializeField] private float speed;
    [SerializeField] private float jump;

    [Header("Coldown")]
    [SerializeField] private float Stun_Time;
    [SerializeField] private float Attack_Time;
    [SerializeField] private float Decision_Time;

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
    private bool Can_jump, Can_think;

    // --- QOther --- \\
    private Rigidbody2D _RigideBody;
    private SpriteRenderer _SpriteRenderer;
    private Animator _Animator;

    public bool Is_On_Left { get; private set; }



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
        Can_jump = false;
        Can_think = true;

        Enemy_Hitbox_Transform = this.transform.Find("Hitbox_Position").GetComponent<Transform>();
        
    }

    private void Start() {
        
        switch (Random.Range(0, 3)) {

            case 0:
                speed = speed + 10;
            break;

            case 1:
                
            break;

            case 2:
                speed = speed - 10;
            break;
        }
    }

    // ***************************************************************************************** \\
    // Update
    // ***************************************************************************************** \\
    private void Update() {

        if (Is_Alive && !Is_Hit && !Is_Freeze) {

            if (Target_Transform != null) {

                Position_Control();
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

            if (Is_On_Left) {

                CleenSpeed = CleenSpeed * -1;
                _SpriteRenderer.flipX = true;
                Enemy_Hitbox_Transform.position = new Vector3(this.transform.position.x - 1f, this.transform.position.y, 0);
               

            } else {

                _SpriteRenderer.flipX = false;
                Enemy_Hitbox_Transform.position = new Vector3(this.transform.position.x + 1f, this.transform.position.y, 0);
            }

            Go_Jump();
            Vector2 _velocity = _RigideBody.velocity;
            _velocity.x = CleenSpeed ;
            _RigideBody.velocity = _velocity;

        } else if (Is_Close && !Is_Attack) {

            _Animator.SetTrigger("Trigger_Attack");
            _RigideBody.velocity = new Vector2 (0,0);
            StartCoroutine(Attack_Couldown());

        }
    }

    private void Position_Control() {

        if (Can_think) {

            Is_On_Left = this.transform.position.x > Target_Transform.position.x;
            StartCoroutine(Thinking_Time());

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



    // ***************************************************************************************** \\
    // Action
    // ***************************************************************************************** \\


    public void Go_Attack() {

        Collider2D[] Enemy_Hitbox = Physics2D.OverlapBoxAll(Enemy_Hitbox_Transform.position, Enemy_Hitbox_Size, Enemy_Hitbox_layermask);

        foreach (var Target in Enemy_Hitbox) {

            if (Target.tag == "Player") {

                Debug.Log(Target.name);

            }
        }


        Debug.Log("Basic attack Is Performed");

    }

    private void Go_Jump() {

        if (!Can_jump) {

            if ((Random.Range(0, 5) == 1)) {

                _RigideBody.AddForce(new Vector2(0f, jump), ForceMode2D.Impulse);
            }

            StartCoroutine(Jump_Couldown());
        }

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

    // ***************************************************************************************** \\
    // Coroutine
    // ***************************************************************************************** \\


    IEnumerator Stunlock() {

        Is_Hit = true;
        yield return new WaitForSeconds(Stun_Time);
        Is_Hit = false;
    }

    IEnumerator Attack_Couldown() {

        Is_Attack = true;
        yield return new WaitForSeconds(Attack_Time);
        Is_Attack = false;
    }

    IEnumerator Jump_Couldown() {

        Can_jump = true;
        yield return new WaitForSeconds(0.25f);
        Can_jump = false;
    }

    IEnumerator Thinking_Time() {

        Can_think = false;

        yield return new WaitForSeconds(Decision_Time);

        Can_think = true;
    }


}