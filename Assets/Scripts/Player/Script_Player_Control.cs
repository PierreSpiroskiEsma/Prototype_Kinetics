using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Player_controle : MonoBehaviour {


    private Animator _Animator;
    private SpriteRenderer _SpriteRenderer;
    private Rigidbody2D _rigidbody;
    private Vector2 Input_Direction;

    //GroundCheck Overlap
    [Header("GroundCheck")]
    [SerializeField] Transform Ground_Check_Position;
    [SerializeField] float Ground_Check_Radius;
    [SerializeField] LayerMask Ground_Check_Layer;
    private float Coyote_Counter;

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
    [SerializeField] Canvas _Canvas;
    [SerializeField] Transform Pause_Manager;

    //Ability Check
    bool Can_WallJump, Can_Dash, Can_Charge;

    //State check
    bool Is_Grounded, Is_Wall , Is_Essence_Active, Is_Attack_Default, Is_Front, Is_Charge, Is_Dash;
    int powerlevel;

    //Essence System
    [SerializeField] int[] Essence_Inventory = new int[3];
    private Light2D Light_Essence_Use;

    //Input Manager
    private Player_Input_Manager _inputManager;
    private InputAction _moveAction, _attackAction, _blockAction, _dashAction, _essenceAction, _interactAction, _jumpAction, _pauseAction;

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
        //_attackAction = _inputManager.Player.Attack;
        _blockAction = _inputManager.Player.Block;
        _dashAction = _inputManager.Player.Dash;
        _essenceAction = _inputManager.Player.Essence;
        _interactAction = _inputManager.Player.Interact;
        _jumpAction = _inputManager.Player.Jump;
        _pauseAction = _inputManager.Player.Pause;

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

        Is_Essence_Active = false;
        Can_Dash = true;
        Can_WallJump = false;
        Can_Charge = false;
        Is_Charge = false;

        powerlevel = 0;

        _moveAction.Enable();
        //_attackAction.Enable();
        _blockAction.Enable();
        _dashAction.Enable();
        _essenceAction.Enable();
        _interactAction.Enable();
        _jumpAction.Enable();
        _pauseAction.Enable();

        _jumpAction.performed += OnJump;
        _jumpAction.canceled += OffJump;
        //_attackAction.performed += OnAttack;
        _essenceAction.performed += OnEssence;
        _blockAction.performed += OnBlock;
        _moveAction.performed += OnMouve;
        _dashAction.performed += OnDash;
        _interactAction.performed += OnInteract;
        _pauseAction.performed += OnPause;
    }

    private void OnDisable() {

        _moveAction.Disable();
        //_attackAction.Disable();
        _blockAction.Disable();
        _dashAction.Disable();
        _essenceAction.Disable();
        _interactAction.Disable();
        _jumpAction.Disable();

        _jumpAction.performed -= OnJump;
        _jumpAction.canceled -= OffJump;
        //_attackAction.performed -= OnAttack;
        _essenceAction.performed -= OnEssence;
        _blockAction.performed -= OnBlock;
        _moveAction.performed -= OnMouve;
        _dashAction.performed -= OnDash;
        _interactAction.performed -= OnInteract;
        _pauseAction.performed -= OnPause;
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

        Ground_Detection();
        Wall_Detection();
        Fall_detection();
        Enemy_Awakening_Box();

        if (!Freeze) { 

            Mouvement_Update_function(); 
        }

        //if (Is_Charge) {

        //    Wall_Destroyeur();
        //}

        animate_StopRun();

        _Canvas.GetComponent<Script_Ui>().Power_Display(Essence_Inventory);

        if (Is_Essence_Active)
        {
            _Canvas.GetComponent<Script_Ui>().BarrDisplay(Essence_Inventory);
            this.GetComponent<Script_LightAnnimator>().setLight(Essence_Type_Check());
        } else
        {
            _Canvas.GetComponent<Script_Ui>().ClearDisplay();
            this.GetComponent<Script_LightAnnimator>().setLight(0);
        }

        


    }

    // ***************************************************************************************** \\
    // Development Tool
    // ***************************************************************************************** \\

    private void OnDrawGizmos() {
        // rendu et position du cercle sous le joueur 
        if (Is_Grounded) {

            Gizmos.color = Color.yellow;

        } else {

            Gizmos.color = Color.blue;
        }

        Gizmos.DrawWireSphere(Ground_Check_Position.position, Ground_Check_Radius);

        if (Is_Wall) {

            Gizmos.color = Color.yellow;

        } else {

            Gizmos.color = Color.blue;
        }

        Gizmos.DrawWireCube(WallJump_Hitbox_Position.position, WallJump_Hitbox_Size);

        if (Is_Attack_Default) {

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Player_Hitbox_position.position, Player_Hitbox_Size);
        }


        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Player_Usebox_position.position, Player_Usebox_Size);
    }

    // ***************************************************************************************** \\
    // Environement detection
    // ***************************************************************************************** \\

    // --- GROUND --- \\
    private void Ground_Detection() {

        Collider2D[] Ground_Detection = Physics2D.OverlapCircleAll(Ground_Check_Position.position, Ground_Check_Radius, Ground_Check_Layer);

        Is_Grounded = false;

        foreach (var Object in Ground_Detection) {

            if (Object.tag == "World") {

                Is_Grounded = true;
            }
        }
    }

    // --- WALL --- \\
    private void Wall_Detection() {


        Collider2D[] Wall_Detection = Physics2D.OverlapBoxAll(WallJump_Hitbox_Position.position, WallJump_Hitbox_Size, WallJump_ColisionLayer);

        Is_Wall = false;

        foreach (var Object in Wall_Detection) {

            if (Object.tag == "World") {

                Is_Wall = true;
            }
        }
    }

    // --- FALL --- \\
    private void Fall_detection() {

        if (Is_Grounded) {

            animate_jump(false);

        } else {

            animate_jump(true);
        }
    }

    // --- Enemy --- \\
    private void Enemy_Awakening_Box() {

        Collider2D[] Enemey_found = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(15,5), Player_Hitbox_LayerMask);

        foreach (var Object in Enemey_found) {

            if (Object.tag == "Enemy") {

                if (Object.GetComponent<Script_BadBoi>().Get_Alive()) {

                    Object.GetComponent<Script_BadBoi>().assign_Player(this.transform);
                }
            } 
            
            if (Object.tag == "Prefab_ia") {

                if (Object.GetComponent<Script_PatrolingAi_Prefab>().Get_Alive()) {

                    Object.GetComponent<Script_PatrolingAi_Prefab>().Assign_Player(this.transform);
                }
            }
        }
    }

    // --- ESSENCE SOURCE --- \\

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.CompareTag("Collectible")) {

            powerlevel++;

            other.gameObject.SetActive(false);
        }

    }


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

        if (Is_Essence_Active) {

            if (true) {

            }

            Essence_Use(4);

        } else if (Is_Grounded || (Is_Wall && Can_WallJump) || (Coyote_Counter > 0 && _rigidbody.velocity.y < 0.05f)) {

            Can_Dash = true;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Stats.Get_PlayerStatistics_Jump_Speed());
        }


    }

    private void OffJump(InputAction.CallbackContext context) {

        if (_rigidbody.velocity.y > 0) {

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    // --- ATTACK --- \\
    //private void OnAttack(InputAction.CallbackContext context) {

        

    //    if (Is_Essence_Active) {

    //        Essence_Use(1);

    //    } else {

    //        //Animate_Attaque_On();
    //    }

    //}

    // --- ESSENCE --- \\
    private void OnEssence(InputAction.CallbackContext context) {

        if (Essence_Type_Check() != 0) {
            Is_Essence_Active = true;
            Essence_use_Light_control();
        }
    }

    // --- BLOCK --- \\
    private void OnBlock(InputAction.CallbackContext context) {

        if (Is_Essence_Active && (Essence_Type_Check() != 0)) {

            Essence_Use(2);

        } else {

            Debug.Log("Block");
            Is_Essence_Active = false;
        }

    }

    // --- MOUVE --- \\
    void OnMouve(InputAction.CallbackContext context) {

        animate_run();

    }

    void Mouvement_Update_function() {

        Input_Direction = _moveAction.ReadValue<Vector2>();
        Vector2 Player_Velocity = _rigidbody.velocity;


        if (Is_Dash) {

            Player_Velocity.y = 0;
        }

        Player_Velocity.x = (Stats.Get_Player_Speed() * Input_Direction.x) ;
        _rigidbody.velocity = Player_Velocity;

        if (_rigidbody.velocity.x > 0.01) {

            Is_Front = true;
        
        } else if (_rigidbody.velocity.x < -0.01) {

            Is_Front = false;
        }

        if (Is_Grounded) {

            Coyote_Counter = Stats.Get_PlayerStatistics_Coyote_time();

        } else {
            
            Coyote_Counter -= Time.deltaTime;

        }


        if (_rigidbody.velocity.y < 0) {

            //_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 1.2f);

            if (_rigidbody.velocity.y < -15f) {

                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -15f);
            }
        }

    }

    // --- DASH --- \\
    private void OnDash(InputAction.CallbackContext context) {

        if ((Can_Dash || Is_Essence_Active) && powerlevel >= 1) {

            if (Is_Essence_Active) {

                Essence_Use(3);

            } else {

                StartCoroutine(Routine_Default_Dash());
            }
        }
    }

    // --- INTERACT --- \\
    private void OnInteract(InputAction.CallbackContext context) {

        Collider2D[] UseBox_Overlap = Physics2D.OverlapBoxAll(Player_Usebox_position.position, Player_Usebox_Size, Player_Usebox_LayerMask);

        foreach (var Object in UseBox_Overlap) {

            if (Object.tag == "Essence Source") {

                if (Object.GetComponent<script_EssenceDispencer_Power>().Essence_Consume()) {

                    Essence_Get(Object.GetComponent<script_EssenceDispencer_Power>().Get_EssenceType(), 2);

                } else {

                    Debug.Log("Pillier Desactiver");
                }
            }
        }
    }

    // --- PAUSE --- \\
    private void OnPause(InputAction.CallbackContext context)
    {
        Pause_Manager.GetComponent<Script_MainMenu>().Pause_Game();
    }


    // ***************************************************************************************** \\
    // Action
    // ***************************************************************************************** \\


    public void Attack_Normal() { 

        Collider2D[] Player_Hitbox = Physics2D.OverlapBoxAll(Player_Hitbox_position.position, Player_Hitbox_Size, Player_Hitbox_LayerMask);

        foreach (var Enemy in Player_Hitbox) {

            if (Enemy.tag == "Enemy") {

                Enemy.GetComponent<Script_BadBoi>().Damage_Taken('n', 1, false, Is_Front);

                Debug.Log(Enemy.name);

            }
        }


        Debug.Log("Basic attack Is Performed");

    }

    public void Attack_Heavy() {

        Collider2D[] Player_Hitbox = Physics2D.OverlapBoxAll(Player_Hitbox_position.position, Player_Hitbox_Size, Player_Hitbox_LayerMask);

        foreach (var Enemy in Player_Hitbox) {

            if (Enemy.tag == "Enemy") {

                Enemy.GetComponent<Script_BadBoi>().Damage_Taken('n', 1, true, Is_Front);

                Debug.Log(Enemy.name);

            }
        }


        Debug.Log("Basic Heavy Is Performed");

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
            

        if (_rigidbody.velocity.x > 0.05f || Input_Direction.x > 0.05f) {

            //this.GetComponent<CapsuleCollider2D>().offset = new Vector2(-0.08f, -0.07f);

            Action_Transform.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Wall_Transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            _Animator.SetBool("Bool_Run", true);

            _SpriteRenderer.flipX = false;

        } else if (_rigidbody.velocity.x < -0.05f || Input_Direction.x < -0.05f) {

            //this.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.08f, -0.07f);

            Action_Transform.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Wall_Transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            _Animator.SetBool("Bool_Run", true);

            _SpriteRenderer.flipX = true;

        } else {

            _Animator.SetBool("Bool_Run", false);

        }

    }

    void animate_StopRun() {

        if (_rigidbody.velocity.x == 0) {
            _Animator.SetBool("Bool_Run", false);
        }

    }


    //void Animate_Attaque_On() {

    //    _Animator.SetBool("B_Anim_Attack", true);
    //    Is_Attack_Default = true;

    //}

    //void Animate_Attaque_Off() {

    //    _Animator.SetBool("B_Anim_Attack", false);
    //    Is_Attack_Default = false;
    //}

    void Animate_Dash_On() {
        //_Animator.SetBool("B_Anim_Dash", true);
    }

    void Animate_Dash_Off() {

        //_Animator.SetBool("B_Anim_Dash", false);
    }

    void animate_jump(bool set) {

        //animation
        _Animator.SetBool("Bool_Jump", set);

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

                    Essence_light_off();

                    Is_Essence_Active = false;

                return true;

                //Sprint
                case 3:

                    Light_Essence_Use.intensity = 1.5f;

                    switch (Essence_Type) {

                        case 1:

                        return false;

                        
                        case 2:
                                
                            StartCoroutine(Routine_Speed_Dash());

                        break;

                        case 3:

                            if (powerlevel >= 3) {
                                this.transform.position = transform.Find("Action").transform.Find("Teleport_dash").GetComponent<Transform>().position;

                                Essence_light_off();

                            } else {

                                return false;
                            }

                            break;

                        default: return false;
                    }

                break;

                //Jump
                case 4:

                    Light_Essence_Use.intensity = 1.5f;

                    switch (Essence_Type) {

                        case 1:

                            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Stats.Get_PlayerStatistics_Jump_Speed() * 1.5f);

                            Essence_light_off();

                            break;

                        case 2:

                            

                            if (Can_WallJump == false && powerlevel >=2) {

                                Can_Dash = true;
                                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Stats.Get_PlayerStatistics_Jump_Speed());

                                StopCoroutine(Fonction_Walljump());
                                StartCoroutine(Fonction_Walljump());

                            } else {

                                return false;
                            }

                            break;

                        case 3:

                            if (powerlevel >= 3) {

                                this.transform.position = transform.Find("Teleport_jump").GetComponent<Transform>().position;

                            } else {

                                return false;
                            }                           

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

            Is_Essence_Active = false;

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
                Essence_light_off();

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

    void Essence_light_off() {

        if (!Can_WallJump && !Can_Charge) {
            Light_Essence_Use.intensity = 0f;
        }
    }

    // ***************************************************************************************** \\
    // Dash
    // ***************************************************************************************** \\


    // --- DEFAULT DASH --- \\
    IEnumerator Routine_Default_Dash() {

        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() * Stats.Get_PlayerStatistics_Dash_Speed());

        Is_Dash = true;
        Can_Dash = false;

        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Duration());

        Is_Dash = false;
        
        Speed_Reset();
        Essence_light_off();

        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());

        Can_Dash = true;
    }


    // --- SPEED DASH --- \\
    IEnumerator Routine_Speed_Dash() {

        Stats.Set_Player_Speed(Stats.Get_PlayerStatistics_Speed() * (Stats.Get_PlayerStatistics_Dash_Speed() * 2));

        Is_Dash = true;
        Can_Dash = false;

        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Duration());

        Is_Dash = false;

        Speed_Reset();
        Essence_light_off();

        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());

        Can_Dash = true;
    }


    // --- POWER DASH --- \\
    IEnumerator Routine_Power_Dash() {

        Debug.Log("Power Dash on");
        Is_Charge = true;
        //couldown
        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Walljump_duration());

        Is_Charge = false;
        Essence_light_off();
        Debug.Log("Power Dash off");

        yield return new WaitForSeconds(Stats.Get_PlayerStatistics_Dash_Couldown());
        Can_Dash = true;
    }

    // ***************************************************************************************** \\
    // Charge
    // ***************************************************************************************** \\

    //private void Wall_Destroyeur() {

    //    Collider2D[] UseBox_Overlap = Physics2D.OverlapBoxAll(Player_Usebox_position.position, Player_Usebox_Size, Player_Usebox_LayerMask);

    //    foreach (var _object in UseBox_Overlap) {

    //        if (_object.tag == "Destructible") {

    //            _object.GetComponent<Script_Desctructible>().OnDestroy();
    //        }
    //    }

    //}


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

        Essence_light_off();

    }

}
