using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_controle : MonoBehaviour

//definition des champ de regalge de la vitesse
{
    [SerializeField] Animator Animator_player;
    [SerializeField] SpriteRenderer sprite_renderer;
    [SerializeField] Rigidbody2D rigidbody;

    [SerializeField] float groundCheckRadius;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask CollisionsLayers;


    [SerializeField] bool Freeze = false;

    [SerializeField] SciptsObject_PlayerStats Stats;

    [SerializeField] bool Bool_Dash_Available = true;
    [SerializeField] bool isGrounded;

    [SerializeField] int[] Essence_Inventory = new int[3];

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

        if (isGrounded) {
            animate_jump(false);
        } else {
            animate_jump(true);
        }
    }


    private void OnDrawGizmos()
    {
        // rendu et position du cercle sous le joueur 
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    // ***************************************************************************************** \\
    // Cinematic setting
    // ***************************************************************************************** \\

    public void FreezOn() { Freeze = true; }

    public void FreezOff() { Freeze = false; }

    // ***************************************************************************************** \\
    // Ground détection
    // ***************************************************************************************** \\


    public void IsFalling(bool set)
    {
        if (set)
        {
            animate_jump(true);
        }
        else
        {
            animate_jump(false);
        }
    }

    //void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("World") || other.gameObject.CompareTag("MovingObject"))
    //    {
    //        isGrounded = true;
    //        animate_jump(false);
    //    }
    //}
    //void OnCollisionExit2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("World") || other.gameObject.CompareTag("MovingObject"))
    //    {
    //        isGrounded = false;
    //        animate_jump(true);
    //    }
    //}

    // ***************************************************************************************** \\
    // fonction d'action 
    // ***************************************************************************************** \\
    void go_left() {

        //Transform
        transform.Translate(-Stats.Get_Player_Speed(), 0, 0, Space.World);
        //BoxCollider2D
        this.GetComponent<BoxCollider2D>().offset = new Vector2(0.08f, -0.07f);
    }

    void go_right() {

        //Transform
        transform.Translate(Stats.Get_Player_Speed(), 0, 0, Space.World);
        //BoxCollider2D
        this.GetComponent<BoxCollider2D>().offset = new Vector2(-0.08f, -0.07f);
        
    }

    void go_attack() {
 
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

    //void go_slide(bool set)
    //{

    //    if (set)
    //    {
    //        if (Input.GetKey(KeyCode.LeftArrow))
    //        {

    //            this.GetComponent<BoxCollider2D>().offset = new Vector2(0.08f, -0.19f);
    //        }
    //        else
    //        {

    //            this.GetComponent<BoxCollider2D>().offset = new Vector2(-0.08f, -0.19f);
    //        }

    //        this.GetComponent<BoxCollider2D>().size = new Vector2(0.45f, 0.3f);
    //    }
    //    else
    //    {
    //        this.GetComponent<BoxCollider2D>().size = new Vector2(0.30f, 0.52f);
    //    }

    //}

    void go_dash() {

        StartCoroutine(Fonction_Dash());            
        Animate_Dash_On();

    }

    void go_jump() {

        if (isGrounded) {

            Debug.Log("try jumping");
            Debug.Log(Stats.Get_PlayerStatistics_Jump_Speed());
            Debug.Log(Stats.Get_PlayerStatistics_Jump_Speed() * Time.deltaTime);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, (Stats.Get_PlayerStatistics_Jump_Speed() * Time.deltaTime));

        }


    }

    // ***************************************************************************************** \\
    // fonction d'animation 
    // ***************************************************************************************** \\
    void animate_left() {

        //animation
        Animator_player.SetBool("B_Anim_Run", true);
        sprite_renderer.flipX = true;

    }
    void animate_right() {

        //animation
        Animator_player.SetBool("B_Anim_Run", true);
        sprite_renderer.flipX = false;

    }

    void animate_run_stop() {

        Animator_player.SetBool("B_Anim_Run", false);

    }

    void Animate_Attaque_On() {

        //animation
        Animator_player.SetBool("B_Anim_Attack", true);

    }

    void Animate_Attaque_Off() {

        //animation
        Animator_player.SetBool("B_Anim_Attack", false);

    }

    //void animate_slide(bool set) {
    //    //animation
    //    Animator_player.SetBool("Bool_Slide", set);
    //}

    void Animate_Dash_On() {
        Animator_player.SetBool("B_Anim_Dash", true);
    }

    void Animate_Dash_Off() {

        Animator_player.SetBool("B_Anim_Dash", false);
    }

    void animate_jump(bool set) {

        //animation
        Animator_player.SetBool("B_Anim_Jump", set);

    }

    // ***************************************************************************************** \\
    // Action d'animation 
    // ***************************************************************************************** \\


    // ***************************************************************************************** \\
    // essence
    // ***************************************************************************************** \\



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
                return true;
            }
        }

        Debug.Log("No Essence Avaiable");
        return false;
    }

    // ***************************************************************************************** \\
    // fonction d'appelle 
    // ***************************************************************************************** \\
    void player_mouvement() {

        //run
        if (Input.GetKey(KeyCode.LeftArrow)) {
            go_left();
            animate_left();
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            go_right();
            animate_right();
        }

        //jump
        if (Input.GetKey(KeyCode.UpArrow)) {
            go_jump();
        };

        //runStop
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) {
            animate_run_stop();
        }

        //attaque
        if (Input.GetKey(KeyCode.Keypad1)) {

            go_attack();
            Animate_Attaque_On();
            Essence_Use(1);

        }

        if (Input.GetKeyDown(KeyCode.Keypad2)) {

            Essence_Use(2);
        }

        ////slide
        //if (Input.GetKey(KeyCode.Keypad2)) {

        //    go_slide(true);
        //    animate_slide(true);
        //}
        //else if (Input.GetKeyUp(KeyCode.Keypad2)) {

        //    go_slide(false);
        //    animate_slide(false);
        //};

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
