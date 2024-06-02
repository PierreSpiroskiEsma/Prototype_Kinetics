using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Script_LightAnnimator : MonoBehaviour {

    private Transform Run_light_box;
    [SerializeField] private Transform run_1, run_2, run_3, run_4;
    private Transform Dash_light_box;
    [SerializeField] private Transform dash_1, dash_2, dash_3, dash_4;
    private Transform Idl_light_box;
    [SerializeField] private Transform Idl_1, Idl_2, Idl_3;
    private Transform Jump_light_box;
    [SerializeField] private Transform Jump_1, Jump_2, Jump_3, Jump_4;

    private void Awake() {
        Run_light_box = this.transform.Find("Light_Box_Run");
        Dash_light_box = this.transform.Find("Light_Box_Dash");
        Idl_light_box = this.transform.Find("Light_Box_Idle");
        Jump_light_box = this.transform.Find("Light_Box_Jump");
    }



    public void setLight(int Power) {

        Color _color = new Color(1f, 1f, 1f); 
    
        switch (Power) {

            case 0:
                _color = new Color(1f, 1f, 1f);
            break;

            case 1:
                _color = new Color(1f, 0f, 0f);
            break;

            case 2:
                _color = new Color(0.19f, 0.92f, 0.82f);
            break;

            case 3:
                _color = new Color(0.92f, 0.78f, 0.19f);
            break;
        }

        run_1.GetComponent<Light2D>().color = _color;
        run_2.GetComponent<Light2D>().color = _color;
        run_3.GetComponent<Light2D>().color = _color;
        run_4.GetComponent<Light2D>().color = _color;
        dash_1.GetComponent<Light2D>().color = _color;
        dash_2.GetComponent<Light2D>().color = _color;
        dash_3.GetComponent<Light2D>().color = _color;
        dash_4.GetComponent<Light2D>().color = _color;
        Idl_1.GetComponent<Light2D>().color = _color;
        Idl_2.GetComponent<Light2D>().color = _color;
        Idl_3.GetComponent<Light2D>().color = _color;
        Jump_1.GetComponent<Light2D>().color = _color;
        Jump_2.GetComponent<Light2D>().color = _color;
        Jump_3.GetComponent<Light2D>().color = _color;
        Jump_4.GetComponent<Light2D>().color = _color;

    }
}
