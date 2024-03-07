using UnityEngine;

[CreateAssetMenu(fileName ="Player_Data", menuName = "ScriptableObject", order = 1)]

/* ********************************************************************************************
 * 
 *  Remember That Player Statistics isn't multiply by detaTime, Only actual Value are (Like speed)
 * 
 * ********************************************************************************************/

public class SciptsObject_PlayerStats : ScriptableObject
{
    [SerializeField] private float PlayerStatistics_Speed = 2.0f;
    public float Get_PlayerStatistics_Speed() { return PlayerStatistics_Speed; }
    public void Set_PlayerStatistics_Speed(float speed) { PlayerStatistics_Speed = speed; }

    [SerializeField] private float Player_Speed = 0f;
    public float Get_Player_Speed() { return (Player_Speed * Time.deltaTime); }
    public void Set_Player_Speed(float speed) { Player_Speed = speed; }


    [SerializeField] private float PlayerStatistics_Jump_Speed = 10f;
    public float Get_PlayerStatistics_Jump_Speed() { return PlayerStatistics_Jump_Speed; }
    public void Set_PlayerStatistics_Jump_Speed(float speed) { PlayerStatistics_Jump_Speed = speed; }


    [SerializeField] private float PlayerStatistics_Dash_Speed = 4f;
    public float Get_PlayerStatistics_Dash_Speed() { return PlayerStatistics_Dash_Speed; }
    public void Set_PlayerStatistics_Dash_Speed(float speed) { PlayerStatistics_Dash_Speed = speed; }


    [SerializeField] private float PlayerStatistics_Dash_Duration = 1f;
    public float Get_PlayerStatistics_Dash_Duration() {  return PlayerStatistics_Dash_Duration; }
    public void Set_Dash_PlayerStatistics_Duration(float duration) { PlayerStatistics_Dash_Duration = duration; }


    [SerializeField] private float PlayerStatistics_Dash_Couldown = 10f;
    public float Get_PlayerStatistics_Dash_Couldown() {  return PlayerStatistics_Dash_Couldown; }
    public void Set_PlayerStatistics_Dash_Couldown(float duration) { PlayerStatistics_Dash_Couldown = duration; }


    [SerializeField] private float PlayerStatistics_Essence_Slot= 2;
    public float Get_PlayerStatistics_Essence_Slot() { return PlayerStatistics_Dash_Couldown; }
    public void Set_PlayerStatistics_Essence_Slot(float Quantity) { PlayerStatistics_Dash_Couldown = Quantity; }
}
