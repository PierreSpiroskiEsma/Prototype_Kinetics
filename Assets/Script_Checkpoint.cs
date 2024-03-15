using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Checkpoint : MonoBehaviour
{

    private BoxCollider2D CheckpointCollider;

    private void Awake() {
        CheckpointCollider = this.GetComponent<BoxCollider2D>();
    }
}
