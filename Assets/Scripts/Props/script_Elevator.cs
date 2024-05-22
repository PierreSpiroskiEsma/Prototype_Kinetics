using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_Elevator : MonoBehaviour
{
    [SerializeField] private float ellevator_Speed;
    private Transform player;

    private bool Is_In;
    enum Direction { Up, Down, Left, Right }
    [SerializeField] Direction _Direction = new Direction();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Is_In = true;
        player = collision.transform;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Is_In = false;
        player = null;
    }

    private void Update()
    {
        if (Is_In)
        {
            switch (_Direction)
            {

                case Direction.Up:

                    player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, ellevator_Speed);

                    break;

                case Direction.Down:

                    player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, -ellevator_Speed);

                    break;

                case Direction.Left:

                    //player.GetComponent<Rigidbody2D>().velocity = new Vector2(-ellevator_Speed, player.GetComponent<Rigidbody2D>().velocity.y);
                    player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-ellevator_Speed*10, 0), ForceMode2D.Force);

                    break;

                case Direction.Right:

                    //player.GetComponent<Rigidbody2D>().velocity = new Vector2(ellevator_Speed, player.GetComponent<Rigidbody2D>().velocity.y);
                    player.GetComponent<Rigidbody2D>().AddForce( new Vector2 (ellevator_Speed*10, 0), ForceMode2D.Force);

                    break;
            }
        }
    }
}

