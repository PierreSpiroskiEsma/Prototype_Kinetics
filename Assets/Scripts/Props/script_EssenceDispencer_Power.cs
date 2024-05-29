using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class script_EssenceDispencer_Power : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] private float Cooldown_Activation;
    enum Essence { Power, Speed, Range }
    [SerializeField] Essence _Essence = new Essence();

    private Light2D obj_light, obj_light2, obj_light3, obj_light4, obj_light5;
    private bool isActive;
    private int Essence_Type;


    private void Awake() {

        obj_light = this.transform.Find("PowerEssence_Light").GetComponent<Light2D>();
        obj_light2 = this.transform.Find("PowerEssence_Light2").GetComponent<Light2D>();
        obj_light3 = this.transform.Find("PowerEssence_Light3").GetComponent<Light2D>();
        obj_light4 = this.transform.Find("PowerEssence_Light4").GetComponent<Light2D>();
        obj_light5 = this.transform.Find("PowerEssence_Light5").GetComponent<Light2D>();

        isActive = true;

        switch (_Essence) {

            case Essence.Power:
                
                Essence_Type = 1;
                obj_light.color = new Color(1f, 0f, 0f);
                obj_light2.color = new Color(1f, 0f, 0f);
                obj_light3.color = new Color(1f, 0f, 0f);
                obj_light4.color = new Color(1f, 0f, 0f);
                obj_light5.color = new Color(1f, 0f, 0f);

                break;

            case Essence.Speed:
                
                Essence_Type = 2;
                obj_light.color = new Color(0.19f, 0.92f, 0.82f);
                obj_light2.color = new Color(0.19f, 0.92f, 0.82f);
                obj_light3.color = new Color(0.19f, 0.92f, 0.82f);
                obj_light4.color = new Color(0.19f, 0.92f, 0.82f);
                obj_light5.color = new Color(0.19f, 0.92f, 0.82f);

                break; 

            case Essence.Range:
                
                Essence_Type = 3;
                obj_light.color = new Color(0.92f, 0.78f, 0.19f);
                obj_light2.color = new Color(0.92f, 0.78f, 0.19f);
                obj_light3.color = new Color(0.92f, 0.78f, 0.19f);
                obj_light4.color = new Color(0.92f, 0.78f, 0.19f);
                obj_light5.color = new Color(0.92f, 0.78f, 0.19f);

                break;
        }
    }

    private void Update() {

        if (isActive) {

            obj_light.intensity = 6f;
            obj_light2.intensity = 6f;
            obj_light3.intensity = 6f;
            obj_light4.intensity = 6f;
            obj_light5.intensity = 6f;

        }
        else {

            obj_light.intensity = 0f;
            obj_light2.intensity = 0f;
            obj_light3.intensity = 0f;
            obj_light4.intensity = 0f;
            obj_light5.intensity = 0f;
        }

    }

    public int Get_EssenceType() {

        return Essence_Type;
    }

    public bool Essence_Consume() {

        if (isActive) {

            StartCoroutine(Desactivation_phase());

            return true;

        } else {

            return false;
        }
    }

    IEnumerator Desactivation_phase() {

        isActive = false;

        yield return new WaitForSeconds(Cooldown_Activation);

        isActive = true;
    }


}
