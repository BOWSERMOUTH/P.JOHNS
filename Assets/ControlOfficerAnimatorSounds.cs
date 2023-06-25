using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlOfficerAnimatorSounds : MonoBehaviour
{
    private GameManager gamemanager;
    private void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Footstep()
    {
        GameObject.Find("ControlOfficer").GetComponent<ControlOfficer>().FootStep();
    }
    private void WalkJingle()
    {
        GameObject.Find("ControlOfficer").GetComponent<ControlOfficer>().WalkJingle();
    }
    private void SwingSound()
    {
        GameObject.Find("ControlOfficer").GetComponent<ControlOfficer>().SwingSound();
    }
    private void ControlOfficerHit()
    {
        GameObject.Find("ControlOfficer").GetComponent<ControlOfficer>().ControlOfficerHit();
    }
    private void CameraShake()
    {
        gamemanager.CameraShake();
    }
}
