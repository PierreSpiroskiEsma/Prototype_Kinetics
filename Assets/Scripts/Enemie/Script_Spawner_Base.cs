using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject Enemy_prefab;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Instantiate(Enemy_prefab, transform.position, Quaternion.identity);
        }
    }
}
