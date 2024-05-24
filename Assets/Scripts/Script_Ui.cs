using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Ui : MonoBehaviour {

    private Canvas _Canvas;
    private RawImage Powerslot_1_Red, Powerslot_1_Blue, Powerslot_1_Yew,
        Powerslot_2_Red, Powerslot_2_Blue, Powerslot_2_Yew,
        Powerslot_3_Red, Powerslot_3_Blue, Powerslot_3_Yew;


    private void Start() {
        _Canvas = GetComponent<Canvas>();

        Powerslot_1_Red = this.transform.Find("PowerSlot_Blue1").GetComponent<RawImage>();
        Powerslot_1_Blue = this.transform.Find("PowerSlot_Red1").GetComponent<RawImage>();
        Powerslot_1_Yew = this.transform.Find("PowerSlot_Yew1").GetComponent<RawImage>();

        Powerslot_2_Red = this.transform.Find("PowerSlot_Blue2").GetComponent<RawImage>();
        Powerslot_2_Blue = this.transform.Find("PowerSlot_Red2").GetComponent<RawImage>();
        Powerslot_2_Yew = this.transform.Find("PowerSlot_Yew2").GetComponent<RawImage>();

        Powerslot_3_Red = this.transform.Find("PowerSlot_Blue3").GetComponent<RawImage>();
        Powerslot_3_Blue = this.transform.Find("PowerSlot_Red3").GetComponent<RawImage>();
        Powerslot_3_Yew = this.transform.Find("PowerSlot_Yew3").GetComponent<RawImage>();
    }

    public void Power_Display(int[] Power_storage) {

        Powerslot_1_Red.color = Color.clear;
        Powerslot_1_Blue.color = Color.clear;
        Powerslot_1_Yew.color = Color.clear;

        switch (Power_storage[0]) {
            case 2:
                Powerslot_1_Red.color = Color.white;
            break;

            case 1:
                Powerslot_1_Blue.color = Color.white;
                break;

            case 3:
                Powerslot_1_Yew.color = Color.white;
                break;
        }

        Powerslot_2_Red.color = Color.clear;
        Powerslot_2_Blue.color = Color.clear;
        Powerslot_2_Yew.color = Color.clear;

        switch (Power_storage[1]) {

            case 2:
                Powerslot_2_Red.color = Color.white;
            break;

            case 1:
                Powerslot_2_Blue.color = Color.white;
            break;

            case 3:
                Powerslot_2_Yew.color = Color.white;
            break;
        }

        Powerslot_3_Red.color = Color.clear;
        Powerslot_3_Blue.color = Color.clear;
        Powerslot_3_Yew.color = Color.clear;

        switch (Power_storage[2]) {

            case 2:
                Powerslot_3_Red.color = Color.white;
            break;

            case 1:
                Powerslot_3_Blue.color = Color.white;
            break;

            case 3:
                Powerslot_3_Yew.color = Color.white;
            break;
        }
    }
}
