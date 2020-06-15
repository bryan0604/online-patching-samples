using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnPressed : MonoBehaviour
{
    public Button button_return;
    public Image image_background;
    public void Awake()
    {
        button_return.onClick.AddListener(onPressedButton);
    }

    void onPressedButton()
    {
        image_background.color = Random.ColorHSV();
        //
    }
}
public class test

{

}