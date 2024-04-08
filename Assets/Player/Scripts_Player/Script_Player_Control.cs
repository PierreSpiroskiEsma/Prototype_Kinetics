using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Player_controle : MonoBehaviour {


    private Animator _Animator;
    private SpriteRenderer _SpriteRenderer;
    private Rigidbody2D _rigidbody;
    private Vector2 Mouve_Direction;

    //GroundCheck Overlap
    [Header("GroundCheck")]
    [SerializeField] Transform Ground_Check_Position;
    [SerializeField] float Ground_Check_Radius;
    [SerializeField] LayerMask Ground_Check_Layer;

    //WallJump Overlap
    [Header("WallJump")]
    [SerializeField] Transform WallJump_Hitbox_Position;
    [SerializeField] Vector2 WallJump_Hitbox_Size;
    [SerializeField] LayerMask WallJump_ColisionLayer;

    //Hitbox Overlap
    [Header("Hitbox")]
    [SerializeField] private Transform Player_Hitbox_position;
    [SerializeField] private Vector2 Player_Hitbox_Size;
    [SerializeField] private LayerMask Player_Hitbox_LayerMask;

    //Use Range Overlap
    [Header("Use Range")]
    [SerializeField] private Transform Player_Usebox_position;
    [SerializeField] private Vector2 Player_Usebox_Size;
    [SerializeField] private LayerMask Player_Usebox_LayerMask;

    //is the player is Freeze in space
    bool Freeze = false;

    //the Statistic sheets of the player
    [Header("Player Statistics Sheets")]
    [SerializeField] SciptsObject_PlayerStats Stats;

    //State Check
    bool Bool_Dash_Available = true;
    bool Can_WallJump = false;

    bool isGrounded, isWall, Can_Dash, Essence_IsActive;

    //Essence System
    [SerializeField] int[] Essence_Inventory = new int[3];
    private Light2D Light_Essence_Use;

    //Input Manager
    private Player_Input_Manager _inputManager;
    private InputAction _moveAction, _attackAction, _blockAction, _dashAction, _essenceAction, _interactAction, _jumpAction;

    //Particle Call
    private Script_Essence_Praticle_Rotation ParticleBox;

    //Trasform Call for Movement transformation (Flip sprite ...)
    private Transform Action_Transform;
    private Transform Wall_Transform;

    // ***************************************************************************************** \\
    // Creation Setup
    // ***************************************************************************************** \\

    private void Awake() {

        _inputManager = new Player_Input_Manager();

        _moveAction = _inputManager.Player.Move;
        _attackAction = _inputManager.Player.Attack;
        _blockAction = _inputManager.Player.Block;
        _dashAction = _inputManager.Player.Dash;
        _essenceAction = _inputManager.Player.Essence;
        _interactAction = _inputManager.Player.Interact;
        _jumpAction = _inputManager.Player.Jump;

    }

    private void OnEnable() {

        _Animator = this.GetComponent<Animator>();
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        _rigidbody = this.GetComponent<Rigidbody2D>();

        Ground_Check_Position = transform.Find("Overlap_Jump").GetComponent<Transform>();
        Player_Usebox_position = transform.Find("Action").transform.Find("UseBox").GetComponent<Transform>();
        Player_Hitbox_position = transform.Find("Action").transform.Find("Attack").transform.Find("Hitbox_Position").GetComponent<Transform>();
        WallJump_Hitbox_Position = transform.Find("WallJump").transform.Find("Overlap_WallJump").GetComponent<Transform>();

        Light_Essence_Use = transform.Find("Action").transform.Find("UseBox").GetComponentInChildren<Light2D>();

        Action_Transform = this.transform.Find("Action");
        Wall_Transform = this.transform.Find("WallJump");

        ParticleBox = this.transform.Find("Essence_Particle_Box").GetComponent<Script_Essence_Praticle_Rotation>();

        Essence_IsActive = false;
        Can_Dash = true;

        _moveAction.Enable();
        _attackAction.Enable();
        _blockAction.Enable();
        _dashAction.Enable();
        _essenceAction.Enable();
        _interactAction.Enable();
        _jumpAction.Enable();

        _jumpAction.performed += OnJump;
        _jumpAction.canceled += OffJump;
        _attackAction.performed += OnAttack;
        _essenceAction.performed += OnEssence;
        _blockAction.performed += OnBlock;
        _moveAction.performed += OnMouve;
        _dashAction.performed += OnDash;
        _interactAction.performed += OnInteract;
    }

    private void OnDisable() {

        _moveAction.Disable();
        _attackAction.Disable();
        _blockAction.Disable();
        _dashAction.Disable();
        _essenceAction.Disable();
        _interactAction.Disable();
        _jumpAction.Disable();

        _jumpAction.performed -= OnJump;
        _jumpAction.canceled -= OffJump;
        _attackAction.performed -= OnAttack;
        _essenceAction.performed -= OnEssence;
        _blockAction.performed -= OnBlock;
        _moveAction.performed -= OnMouve;
        _dashAction.performed -= OnDash;
        _interactAction.performed -= OnInteract;
    }



    // ***************************************************************************************** \\
    // START
    // ***************************************************************************************** \\
    void Start() {

        //Initialisation of the actual player speed based on his Speed statistic
        Speed_Reset();

    }

    // ***************************************************************************************** \\
    // FIXED UPDATE
    // ***************************************************************************************** \\

    private void FixedUpdate() {

        // ************** Ground Detection ************** \\

        Ground_Detection();
        Wall_Detection();
        Fall_detection();
        Mouvement_Update_function();
        animate_StopRun();
    }



    // ***************************************************************************************** \\
    // Development Tool
    // ***************************************************************************************** \\

    private void OnDrawGizmos() {
        // rendu et position du cercle sous le joueur 
        if (isGrounded) {
            Gizmos.color = Color.yellow;
        } else {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawWireSphere(Ground_Check_Position.position, Ground_Check_Radius);

        if (isWall) {
            Gizmos.color = Color.yellow;
        } else {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawWireCube(WallJump_Hitbox_Position.position, WallJump_Hitbox_Size);


        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Player_Hitbox_position.position, Player_Hitbox_Size);
    }

    // ***************************************************************************************** \\
    // Environement detection
    // ***************************************************************************************** \\

    // --- GROUND --- \\
    private void Ground_Detection() {

        Collider2D[] Ground_Detection = Physics2D.OverlapCircleAll(Ground_Check_Position.position, Ground_Check_Radius, Ground_Check_Layer);

        isGrounded = false;

        foreach (var Object in Ground_Detection) {

            if (Object.tag == "World") {

                isGrounded = true;
            }
        }
    }

    // --- WALL --- \\
    private void Wall_Detection() {


        Collider2D[] Wall_Detection = Physics2D.OverlapBoxAll(WallJump_Hitbox_Position.position, WallJump_Hitbox_Size, WallJump_ColisionLayer);

        isWall = false;

        foreach (var Object in Wall_Detection) {

            if (Object.tag == "World") {

                isWall = true;
            }
        }
    }

    // --- FALL --- \\
    private void Fall_detection() {
        if (isGrounded) {

            animate_jump(false);

        } else {

            animate_jump(true);
        }
    }

    // --- ESSENCE SOURCE --- \\


 

    // ***************************************************************************************** \\
    // Cinematic setting
    // ***************************************************************************************** \\

    public void FreezOn() {

        Freeze = true; 
    }

    public void FreezOff() { 

        Freeze = false; 
    }


    // ***************************************************************************************** \\
    // fonction d'input 
    // ***************************************************************************************** \\

    // --- JUMP --- \\
    private void OnJump(InputAction.CallbackContext context) {

        if (Essence_IsActive && isGrounded) {

            Essence_Use(4);

        } else if (isGrounded || (isWall && Can_WallJump)) {

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Stats.Get_PlayerStatistics_Jump_Speed());
        }


    }

    private void OffJump(InputAction.CallbackContext context) {

        if (_rigidbody.velocity.y > 0) {

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    // --- ATTACK --- \\
    private void OnAttack(InputAction.CallbackContext context) {

        Animate_Attaque_On();

        if (Essence_IsActive) {

            Essence_Use(1);

        } else {

            Attack_Normal();

        }

    }

    // --- ESSENCE --- \\
    private void OnEssence(InputAction.CallbackContext context) {

        if (Essence_Type_Check() != 0) {
            Essence_IsActive = true;
            Essence_use_Light_control();
        }
    }

    // --- BLOCK --- \\
    private void OnBlock(InputAction.CallbackContext context) {

        if (Essence_IsActive && (Essence_Type_Check() != 0)) {

            Essence_Use(2);

        } else {

            Debug.Log("Block");
            Essence_IsActive = false;
        }

    }

    // --- MOUVE --- \\
    void OnMouve(InputAction.CallbackContext context) {

        animate_run();

    }

    void Mouvement_Update_function() {

        Mouve_Direction = _moveAction.ReadValue<Vector2>();
        Vector2 Player_Velocity = _rigidbody.velocity;

        Player_Velocity.x = (Stats.Get_Player_Speed() * Mouve_Direction.x) ;
        _rigidbody.velocity = Player_Velocity;
    }

    // --- DASH --- \\
    private void OnDash(InputAction.CallbackContext context) {
        if (Can_Dash) {
            if (Essence_IsActive) {

                Essence_Use(3);

            } else {

                StartCoroutine(Routine_Default_Dash());
            }
        }
    }

    // --- INTERACT --- \\
    private void OnInteract(InputAction.CallbackContext context) {

        Collider2D[] Essence_Source_Detection = Physics2D.OverlapBoxAll(WallJump_Hitbox_Position.position, WallJump_Hitbox_Size, WallJump_ColisionLayer);

        foreach (var Object in Essence_Source_Detection) {

            if (Object.tag == "Essence Source") {

                if (Object.GetComponent<script_EssenceDispencer_Power>().Essence_Consume()) {

                    Essence_Get(Object.GetComponent<script_EssenceDispencer_Power>().Get_EssenceType(), 2);

                } else {

                    Debug.Log("Pillier Desactiver");
                }
            }
        }
    }


    // ***************************************************************************************** \\
    // Action
    // ***************************************************************************************** \\


    void Attack_Normal() {

        Collider2D[] Player_Hitbox = Physics2D.OverlapBoxAll(Player_Hitbox_position.position, Player_Hitbox_Size, Player_Hitbox_LayerMask);

        foreach (var Enemy in Player_Hitbox) {

            if (Enemy.tag == "Enemy") {

                Enemy.GetComponent<Script_BadBoi>().Damage_Taken('n', 1);

                Debug.Log(Enemy.name);

            }
        }


        Debug.Log("Basic Action Is Performed");

    }

    void Speed_Reset() {
        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed());
    }

    //void Dash_Accelerate() {
    //    Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() * Stats.Get_PlayerStatistics_Dash_Speed());
    //}

    //void Dash_Deccelerate() {
    //    Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() / 2);
    //}

    // ***************************************************************************************** \\
    // animation 
    // ***************************************************************************************** \\

    void animate_run() {

        if (Mouve_Direction.x < -0.2f) {

            //this.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.08f, -0.07f);

            Action_Transform.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Wall_Transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            _Animator.SetBool("B_Anim_Run", true);

            _SpriteRenderer.flipX = true;

        } else if (Mouve_Direction.x > 0.2f) {

            //this.GetComponent<CapsuleCollider2D>().offset = new Vector2(-0.08f, -0.07f);

            Action_Transform.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Wall_Transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            _Animator.SetBool("B_Anim_Run", true);

            _SpriteRenderer.flipX = false;

        } else {

            _Animator.SetBool("B_Anim_Run", false);
        }

    }

    void animate_StopRun() {

        if (_rigidbody.velocity.x == 0) {
            _Animator.SetBool("B_Anim_Run", false);
        }

    }


    void Animate_Attaque_On() {

        _Animator.SetBool("B_Anim_Attack", true);

    }

    void Animate_Attaque_Off() {

        _Animator.SetBool("B_Anim_Attack", false);

    }

    void Animate_Dash_On() {
        _Animator.SetBool("B_Anim_Dash", true);
    }

    void Animate_Dash_Off() {

        _Animator.SetBool("B_Anim_Dash", false);
    }

    void animate_jump(bool set) {

        //animation
        _Animator.SetBool("B_Anim_Jump", set);

    }

    public void IsFalling(bool set) {
        if (set) {
            animate_jump(true);
        } else {
            animate_jump(false);
        }
    }


    // ***************************************************************************************** \\
    // essence
    // ***************************************************************************************** \\


    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! A DEPLACER DANS LES PILONE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! \\
    private bool Essence_Lit() {

        bool set = false;

        for (int i = 0; i < Essence_Inventory.Length; i++) {

            if (Essence_Inventory[i] != 0) {

                ParticleBox.Essence_Color_Switch(Essence_Inventory[i]);
                set = true;
            }
        }

        transform.Find("Essence_Particle_Box").gameObject.SetActive(set);

        return true;
    }
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! \\


    // --- GET --- \\
    public bool Essence_Get(int New_Essence, int Action_Type) {

        if (Action_Type == 1 || Action_Type == 2) {

            if (Essence_Inventory[Essence_Inventory.Length - 1] != 0) {

                Debug.Log("Essence Pool is full, Damage taken");
                return false;

            }
        }

        for (int i = 0; i < Essence_Inventory.Length; i++) {

            if (Essence_Inventory[i] == 0) {

                Essence_Inventory[i] = New_Essence;
                Debug.Log("Essence " + New_Essence + " Has been added to the slots " + (i));
                Essence_Lit();
                return true;

            }
        }

        Debug.Log("Essence Pool is full, No Essense has been add");
        return false;
    }


    // --- USE --- \\
    bool Essence_Use(int Action_Type) {
            int Essence_Type = Essence_Type_Check();

        if (Essence_Type != 0) {

            switch (Action_Type) { // peut y avoirs une fonction ici

                //Attack
                case 1:

                    switch (Essence_Type) {

                        case 1:

                            Debug.Log("Strong Attack");

                        break;

                        case 2:

                            Debug.Log("Speed Attack");

                        break;

                        case 3:

                            Debug.Log("Ranged Attack");

                        break;

                        default: return false;
                    }

                break;

                //Block
                case 2:

                    Debug.Log("Essence Cancel"); // cancel la magie et ne la suprime pas de la reserve

                    Light_Essence_Use.intensity = 0f;

                    Essence_IsActive = false;

                return true;

                //Sprint
                case 3:

                    Light_Essence_Use.intensity = 1.5f;

                    switch (Essence_Type) {

                        case 1:
                                
                            StartCoroutine(Routine_Power_Dash());

                        break;

                        case 2:
                                
                            StartCoroutine(Routine_Speed_Dash());

                        break;

                        case 3:

                            this.transform.position = transform.Find("Action").transform.Find("Teleport_dash").GetComponent<Transform>().position;

                            Light_Essence_Use.intensity = 0f;

                        break;

                        default: return false;
                    }

                break;

                //Jump
                case 4:

                    Light_Essence_Use.intensity = 1.5f;

                    switch (Essence_Type) {

                        case 1:

                            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Stats.Get_PlayerStatistics_Jump_Speed() * 3);

                            Light_Essence_Use.intensity = 0f;

                        break;

                        case 2:

                            StartCoroutine(Fonction_Walljump());

                        break;

                        case 3:

                            this.transform.position = transform.Find("Teleport_jump").GetComponent<Transform>().position;

                            Light_Essence_Use.intensity = 0f;

                            break;

                        default: return false;
                    }

                break;

                //Taunt
                default:

                    switch (Essence_Type) {

                        case 1:

                            Debug.Log("Power Taunt");

                        break;

                        case 2:

                            Debug.Log("Speed Taunt");

                        break;

                        case 3:

                            Debug.Log("Range Taunt");

                        break;

                        default: return false;
                    }

                break;
            }


            Debug.Log("Essence " + Essence_Type_Check() + " Has Been used");
            Essence_Use_Clean();

            Essence_IsActive = false;

            Essence_Lit();
            return true;
            
        }

        Debug.Log("No Essence Avaiable");
        Essence_Lit();
        return false;
    }

    // --- VANITY --- \\
    void Essence_use_Light_control() {

        Light_Essence_Use.intensity = 0.5f;

        switch (Essence_Type_Check()) {

            case 1:

                Light_Essence_Use.color = new Color(1f, 0f, 0f, 1f);

            break;

            case 2:

                Light_Essence_Use.color = new Color(0f, 0f, 1f, 1f);

            break;

            case 3:

                Light_Essence_Use.color = new Color(1f, 1f, 0f, 1f);

            break;

            default:

                Light_Essence_Use.color = new Color(0f, 0f, 0f, 1f);
                Light_Essence_Use.intensity = 0f;

            break;
        }

    }

    // --- TYPE CHECK --- \\
    int Essence_Type_Check() {

        for (int i = Essence_Inventory.Length-1; i >= 0; i--)
        {
            if (Essence_Inventory[i] != 0)
            {
                return Essence_Inventory[i];
            }
        }

        return 0;

    }

    void Essence_Use_Clean() {

        for (int i = Essence_Inventory.Length - 1; i >= 0; i--) {

            if (Essence_Inventory[i] != 0) {

                Essence_Inventory[i] = 0;
                break;
            }
        }
    }

    // ***************************************************************************************** \\
    // Dash
    // ***************************************************************************************** \\


    // --- DEFAULT DASH --- \\
    IEnumerator Routine_Default_Dash() {

        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() * Stats.Get_PlayerStatistics_Dash_Speed());

        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Duration());

        Speed_Reset();
        Light_Essence_Use.intensity = 0f;

        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());
        Can_Dash = true;
    }


    // --- SPEED DASH --- \\
    IEnumerator Routine_Speed_Dash() {

        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() * (Stats.Get_PlayerStatistics_Dash_Speed() * 2));

        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Duration());

        Speed_Reset();
        Light_Essence_Use.intensity = 0f;

        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());
        Can_Dash = true;
    }


    // --- POWER DASH --- \\
    IEnumerator Routine_Power_Dash() {

        Debug.Log("Power Dash on");

        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Walljump_duration());

        Speed_Reset();
        Light_Essence_Use.intensity = 0f;
        Debug.Log("Power Dash off");

        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());
        Can_Dash = true;
    }

    // ***************************************************************************************** \\
    // WallJump
    // ***************************************************************************************** \\


    IEnumerator Fonction_Walljump() {

        Can_WallJump = true;
        Debug.Log("wall start");

        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Walljump_duration());

        Can_WallJump = false;
        Debug.Log("wall finished");

        Light_Essence_Use.intensity = 0f;

    }

}
