using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Job : MonoBehaviour
{
    public enum CHARACTER_JOB {boxer, swordsman, archer, wizard, none}
    public CHARACTER_JOB job = CHARACTER_JOB.none;
    public List<Color> job_indicator = new List<Color>();
    public MeshRenderer meshrenderer;
    void Start()
    {
        meshrenderer = GetComponent<MeshRenderer>();
        meshrenderer.material.color = job_indicator[(int)job];
    }
}
