using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
    }
}
