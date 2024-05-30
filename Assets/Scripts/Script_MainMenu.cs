using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_MainMenu : MonoBehaviour
{
    [SerializeField] private Canvas Pause_Canvas;
    [SerializeField] private Canvas ATH_Canvas;

    public void Start_Level() {

        SceneManager.LoadScene("Scn_Main");
    }

    public void Quit_Game() {

        Time.timeScale = 1f;

        Application.Quit();
    }

    public void Pause_Game() {
        
        if (Pause_Canvas != null)
        {
            Pause_Canvas.gameObject.SetActive(true);

            if (ATH_Canvas != null)
            {
                ATH_Canvas.gameObject.SetActive(false);
            }

            Time.timeScale = 0f;
        }
    }


    public void Resume_Game() {

        if (Pause_Canvas != null) {
            Pause_Canvas.gameObject.SetActive(false);

            if (ATH_Canvas != null) { 
                ATH_Canvas.gameObject.SetActive(true); 
            }

            Time.timeScale = 1f;
        }                    
    }

    public void Start_Menue() {

        Time.timeScale = 1f;

        SceneManager.LoadScene("Scn_Menue");
    }
}
