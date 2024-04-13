using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Desctructible : MonoBehaviour
{
    public void OnDestroy() {
        this.gameObject.SetActive(false);
    }
}
