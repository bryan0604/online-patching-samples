using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Function01 : MonoBehaviour
{
    [SerializeField]
    Button button;
    void Start()
    {
        button.onClick.AddListener(()=>Debug.Log("function01"));    
    }
}
