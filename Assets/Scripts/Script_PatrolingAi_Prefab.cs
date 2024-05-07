using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Script_PatrolingAi_Prefab : MonoBehaviour
{

    // --- Statistic --- \\
    [Header("Enemi Statistics")]
    [SerializeField] private float speed;
    [SerializeField] private int Life_Point;

    // --- Patrol --- \\
    [Header("Patrol Detection Setting")]
    [SerializeField] private Transform Patrol_Detector_Position;
    [SerializeField] private Vector2 Patrol_Detector_Size;
    [SerializeField] private float Patrol_Detector_ColisionLayer;

    // --- Chase --- \\
    [Header("Chase Option")]
    [SerializeField] private Transform Target_Transform;
    [SerializeField] private float Chase_Distance;
    [SerializeField] private float actual_distance;
    [SerializeField] private float Attack_Distance;

    // --- Ai Setting --- \\
    [Header("AI Stetting")]
    [SerializeField] private bool Is_Alive;
    private Transform Patrol_Zone;
    private float Decision_Time;

    // --- State --- \\
    private bool Is_Far, Is_Chasing, Is_Close, Is_Freeze, Is_Patrol, Is_On_Left, Is_On_L_point;
    private bool Can_think;

    // --- Other --- \\
    private Rigidbody2D _RigideBody;
    private SpriteRenderer _SpriteRenderer;
    private Animator _Animator;
    private Transform _Transform;



    // ***************************************************************************************** \\
    // Setup
    // ***************************************************************************************** \\

    private void OnEnable() {

        _RigideBody = this.GetComponent<Rigidbody2D>();
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        _Animator = this.GetComponent<Animator>();
        _Transform = this.transform.Find("Hitbox_Position").GetComponent<Transform>();

        Is_Chasing = false;
        Is_Close = false;
        Is_Patrol = true;
        Is_Far = true;
        Is_Freeze = false;
    }

    // ***************************************************************************************** \\
    // Update
    // ***************************************************************************************** \\
    private void Update() {

        if (Is_Alive && !Is_Freeze)
        {

            if (Target_Transform != null)
            {

                Position_Control();
                Chase_Control();
            }
        }

        Is_Dead();
    }

    // ***************************************************************************************** \\
    // Assign Methode
    // ***************************************************************************************** \\
    public bool Get_Alive()
    {

        return Is_Alive;
    }

    public void Assign_Player(Transform _player)
    {

        Target_Transform = _player.transform;
    }

    public void Assign_Patrol(Transform _Patrol)
    {

        Patrol_Zone = _Patrol.transform;
    }

    // Call to make the ai loose the player 
    public void Lose_Target()
    {

        Target_Transform = null;
    }


    // ***************************************************************************************** \\
    // Scan Methode
    // ***************************************************************************************** \\


    private void Search_Patrol_Point()
    {

        Collider2D[] Search_Patrol = Physics2D.OverlapBoxAll(Patrol_Detector_Position.position, Patrol_Detector_Size, Patrol_Detector_ColisionLayer);

        foreach (var Object in Search_Patrol)
        {

            if (Object.tag == "Patrol")
            {

                Is_Patrol = true;
                Assign_Patrol(Object.transform);
            }
        }
    }



    // ***************************************************************************************** \\
    // Patrol Methode
    // ***************************************************************************************** \\

    private void Go_Patrol()
    {


    }

    // ***************************************************************************************** \\
    // Behaviour
    // ***************************************************************************************** \\
    private void Chase_Control()
    {

        if (Target_Transform != null)
        {

            Is_Chasing = Vector2.Distance(this.transform.position, Target_Transform.position) < Chase_Distance;
            Is_Far = Vector2.Distance(this.transform.position, Target_Transform.position) > Chase_Distance;
            Is_Close = Vector2.Distance(this.transform.position, Target_Transform.position) < Attack_Distance;

        }
        else
        {

            Is_Chasing = false;
            Is_Close = false;
            Is_Far = true;
        }


        float CleenSpeed = speed * Time.deltaTime;

        // Chase mode
        if (Is_Chasing && !Is_Close)
        {

            if (Is_On_Left)
            {

                CleenSpeed = CleenSpeed * -1;
                _SpriteRenderer.flipX = false;
                _Transform.position = new Vector3(this.transform.position.x - 1f, this.transform.position.y, 0);


            }
            else
            {

                _SpriteRenderer.flipX = true;
                _Transform.position = new Vector3(this.transform.position.x + 1f, this.transform.position.y, 0);
            }

            // Close Mode (Delay the direction change)
        }
        else if (Is_Close && Can_think)
        {

            StartCoroutine(Thinking_Time());

            // Patrol Shearching Mode (
        }
        else if (Is_Far && !Is_Patrol)
        {

            StartCoroutine(Start_Patrol_Shearch());

            // Patrol Mode    
        }
        else
        {


        }

        Vector2 _velocity = _RigideBody.velocity;
        _velocity.x = CleenSpeed;
        _RigideBody.velocity = _velocity;
    }

    private void Position_Control()
    {

        if (Can_think)
        {

            Is_On_Left = this.transform.position.x > Target_Transform.position.x;
        }
    }

    // ***************************************************************************************** \\
    // Debug
    // ***************************************************************************************** \\

    private void OnDrawGizmos()
    {

        if (Is_Far)
        {

            Gizmos.color = Color.blue;
        }

        if (Is_Chasing)
        {

            Gizmos.color = Color.yellow;
        }

        if (Is_Close)
        {

            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(this.transform.position, Chase_Distance);
        Gizmos.DrawWireSphere(this.transform.position, Attack_Distance);
    }


    // ***************************************************************************************** \\
    // Freeze Methode
    // ***************************************************************************************** \\
    public void FreezOn()
    {

        Is_Freeze = true;
    }

    public void FreezOff()
    {

        Is_Freeze = false;
    }

    // ***************************************************************************************** \\
    // Death Detection and application
    // ***************************************************************************************** \\
    private void Is_Dead()
    {

        if (Life_Point <= 0)
        {

            Destroy(gameObject, 0.5f);
        }

    }

    // ***************************************************************************************** \\
    // Coroutine
    // ***************************************************************************************** \\
    IEnumerator Thinking_Time()
    {

        Can_think = false;

        yield return new WaitForSeconds(Decision_Time);

        Can_think = true;
    }

    IEnumerator Start_Patrol_Shearch()
    {
        yield return new WaitForSeconds(Decision_Time);

        Can_think = false;
        Is_On_Left = true;

        Search_Patrol_Point();

    }

}