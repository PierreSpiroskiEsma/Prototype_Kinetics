using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player_controle : MonoBehaviour {

    private Animator _Animator;
    private SpriteRenderer _SpriteRenderer;
    private Rigidbody2D _rigidbody;
    private Vector2 Mouve_Direction;

    //GroundCheck Overlap
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask CollisionsLayers;

    //WallJump Overlap
    [SerializeField] Transform WallJump_Hitbox_Location;
    [SerializeField] Vector2 WallJump_Hitbox_Size;
    [SerializeField] LayerMask WallJump_ColisionLayer;

    //Hitbox Overlap
    [SerializeField] private Transform Player_Hitbox_position;
    [SerializeField] private Vector2 Player_Hitbox_RangeSize;
    [SerializeField] private LayerMask Player_Hitbox_LayerMask;

    //is the player is Freeze in space
    [SerializeField] bool Freeze = false;

    //the Statistic sheets of the player
    [SerializeField] SciptsObject_PlayerStats Stats;

    //State Check
    [SerializeField] bool Bool_Dash_Available = true;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isWall;

    //Essence System
    [SerializeField] int[] Essence_Inventory = new int[3];
    private bool Essence_IsActive;

    //Input Manager
    private Player_Input_Manager _inputManager;
    private InputAction _moveAction, _attackAction, _blockAction, _dashAction, _essenceAction, _interactAction, _jumpAction;

    //Particle Call
    private Script_Essence_Praticle_Rotation ParticleBox;

    //Trasform Call for Movement transformation (Flip sprite ...)
    private Transform Action_Transform;

    // ***************************************************************************************** \\
    // Creation Setup
    // ***************************************************************************************** \\

    private void Awake() {

        _Animator = this.GetComponent<Animator>();
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        _rigidbody = this.GetComponent<Rigidbody2D>();

        Action_Transform = this.transform.Find("Action");

        ParticleBox = this.transform.Find("Essence_Particle_Box").GetComponent<Script_Essence_Praticle_Rotation>();

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

        Essence_IsActive = false;

        _moveAction.Enable();
        _attackAction.Enable();
        _blockAction.Enable();
        _dashAction.Enable();
        _essenceAction.Enable();
        _interactAction.Enable();
        _jumpAction.Enable();

        _jumpAction.performed += OnJump;
        _attackAction.performed += OnAttack;
        _essenceAction.performed += OnEssence;
        _blockAction.performed += OnBlock;
        _moveAction.performed += OnMouve;
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
        _attackAction.performed -= OnAttack;
        _essenceAction.performed -= OnEssence;
        _blockAction.performed -= OnBlock;
        _moveAction.performed -= OnMouve;
    }

    // ***************************************************************************************** \\
    // START
    // ***************************************************************************************** \\
    void Start()
    {

        //Initialisation of the actual player speed based on his Speed statistic
        Speed_Reset();

    }


    // ***************************************************************************************** \\
    // UPDATE
    // ***************************************************************************************** \\
    void Update() {

        // Détection des entrées utilisateur et application des commandes associer
        if (!Freeze) {
            player_mouvement();
        }

    }

    // ***************************************************************************************** \\
    // FIXED UPDATE
    // ***************************************************************************************** \\

    private void FixedUpdate() {

        // ************** Ground Detection ************** \\

        //groundCheck.position = new Vector2(this.transform.position.x + this.GetComponent<BoxCollider2D>().offset.x, groundCheck.position.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, CollisionsLayers);
        isWall = Physics2D.OverlapBox(WallJump_Hitbox_Location.position, WallJump_Hitbox_Size, WallJump_ColisionLayer);

        if (isGrounded) {
            animate_jump(false);
        } else {
            animate_jump(true);
        }

        Mouve_Direction = _moveAction.ReadValue<Vector2>();
        Vector2 Player_Velocity = _rigidbody.velocity;
        Player_Velocity.x = Stats.Get_Player_Speed() * Mouve_Direction.x;
        _rigidbody.velocity = Player_Velocity;

        animate_StopRun();
    }

    // ***************************************************************************************** \\
    // Development Tool
    // ***************************************************************************************** \\

    private void OnDrawGizmos()
    {
        // rendu et position du cercle sous le joueur 
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireCube(WallJump_Hitbox_Location.position, WallJump_Hitbox_Size);
        Gizmos.DrawWireCube(Player_Hitbox_position.position, Player_Hitbox_RangeSize);
    }

    // ***************************************************************************************** \\
    // Cinematic setting
    // ***************************************************************************************** \\

    public void FreezOn() { Freeze = true; }

    public void FreezOff() { Freeze = false; }


    // ***************************************************************************************** \\
    // fonction d'input 
    // ***************************************************************************************** \\

    // --- JUMP --- \\
   private void OnJump(InputAction.CallbackContext context) {

        Debug.Log("Jump");

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

        Essence_IsActive = true;
    }

    // --- BLOCK --- \\
    private void OnBlock(InputAction.CallbackContext context) {

        if (Essence_IsActive) {

            Essence_IsActive = false;
            Debug.Log("Essence Use is cancel + Block");

        } else {

            Debug.Log("Block");
        }

    }

    // --- MOUVE --- \\
    void OnMouve(InputAction.CallbackContext context) {

        animate_run();

    }

    // ***************************************************************************************** \\
    // Action
    // ***************************************************************************************** \\


    void Attack_Normal() {

        Collider2D[] Player_Hitbox = Physics2D.OverlapBoxAll(Player_Hitbox_position.position, Player_Hitbox_RangeSize, Player_Hitbox_LayerMask);

        foreach (var Enemy in Player_Hitbox) {

            if (Enemy.tag == "Enemy") {

                Enemy.GetComponent<Script_BadBoi>().Damage_Taken('n', 1);

                Debug.Log(Enemy.name);

            }
        }


        Debug.Log("Basic Action Is Performed");

    }

    void Dash_Accelerate() {
        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() * Stats.Get_PlayerStatistics_Dash_Speed());
    }

    void Dash_Deccelerate() {
        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() / 2);
    }

    void Speed_Reset() {
        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed());
    }

    void go_dash() {

        StartCoroutine(Fonction_Dash());            
        Animate_Dash_On();

    }

    void go_jump() {

        if (isGrounded) {

            Debug.Log("try jumping");
            Debug.Log(Stats.Get_PlayerStatistics_Jump_Speed());
            Debug.Log(Stats.Get_PlayerStatistics_Jump_Speed() * Time.deltaTime);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, (Stats.Get_PlayerStatistics_Jump_Speed() * Time.deltaTime));

        }


    }

    // ***************************************************************************************** \\
    // animation 
    // ***************************************************************************************** \\

    void animate_run() {

        if (Mouve_Direction.x < -0.2f) {
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0.08f, -0.07f);
            Action_Transform.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            _Animator.SetBool("B_Anim_Run", true);
            _SpriteRenderer.flipX = true;

        } else if (Mouve_Direction.x > 0.2f) {
            this.GetComponent<BoxCollider2D>().offset = new Vector2(-0.08f, -0.07f);
            Action_Transform.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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

        transform.GetChild(0).gameObject.SetActive(set);

        return true;
    }
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! \\


    // GET

    public bool Essence_Get (int New_Essence, int Action_Type) {

    if (Action_Type ==  1 || Action_Type == 2) {

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


// USE


bool Essence_Use(int Action_Type) {
    for (int i = 0; i < Essence_Inventory.Length; i++) {
        int Essence_Type = Essence_Inventory[Essence_Inventory.Length - (i + 1)];

        if (Essence_Type != 0) {

            switch (Action_Type) { // peut y avoirs une fonction ici

                //Attack
                case 1:

                    switch (Essence_Type) {

                        case 1 :
                            Debug.Log("Strong Attack");
                        break;

                        case 2 :
                            Debug.Log("Speed Attack");
                        break;

                        case 3 :
                            Debug.Log("Ranged Attack");
                        break;

                        default: return false;
                    }

                break; 

                //Block
                case 2:

                    Debug.Log("Block"); // cancel la magie et ne la suprime pas de la reserve

                return true;

                //Sprint
                case 3:

                    switch (Essence_Type) {

                        case 1:
                            Debug.Log("Charge");
                        break;

                        case 2:
                            Debug.Log("Super Speed");
                        break;

                        case 3:
                            Debug.Log("Teleport");
                        break;

                        default: return false;
                    }

                break;

                //Jump
                case 4:

                    switch (Essence_Type) {

                        case 1:
                            Debug.Log("Big Jump");
                        break;

                        case 2:
                            Debug.Log("Wall Jump");
                        break;

                        case 3:
                            Debug.Log("Teleport");
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

            Essence_Inventory[Essence_Inventory.Length - (i + 1)] = 0;
            Debug.Log("Essence " + Essence_Inventory[Essence_Inventory.Length - (i + 1)] + " Has Been used");
            Essence_Lit();
            return true;
        }
    }

    Debug.Log("No Essence Avaiable");
    Essence_Lit();
    return false;
}

// ***************************************************************************************** \\
// OLD fonction d'input 
// ***************************************************************************************** \\
void player_mouvement() {

    //jump
    if (Input.GetKey(KeyCode.UpArrow)) {
        go_jump();
    };

    if (Input.GetKeyDown(KeyCode.Keypad2)) {

        Essence_Use(2);
    }

    //dash
    if (Input.GetKeyDown(KeyCode.Keypad3) && Bool_Dash_Available) {
        go_dash();
        Animate_Dash_On();
    }

}

// ************** Dash ************** \\

IEnumerator Fonction_Dash()
{

    Bool_Dash_Available = false;

    //couldown
    yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());

    Bool_Dash_Available = true;

}
}
