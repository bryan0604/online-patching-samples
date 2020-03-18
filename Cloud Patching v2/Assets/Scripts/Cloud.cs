using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Cloud : MonoBehaviour
{
    public AssetReference image_ar;
    public GameObject image_go;
    public Transform parent_canvas;

    void InstantiateAsyncFromCloud()
    {
        image_ar.InstantiateAsync(new Vector3(), Quaternion.identity, parent_canvas);    
    }

    void InstantiateFromLocalClient()
    {
        Instantiate(image_go , parent_canvas);
    }

    void CheckForUpdates()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            InstantiateAsyncFromCloud();
        }
        else if(Input.GetKeyDown(KeyCode.LeftCommand))
        {
            InstantiateFromLocalClient();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }
}
