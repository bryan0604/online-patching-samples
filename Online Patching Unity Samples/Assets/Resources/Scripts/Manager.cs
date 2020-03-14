using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Manager : MonoBehaviour
{
    public AssetReference asset;
    private void Start()
    {
        asset.InstantiateAsync(new Vector3(0,0,0),Quaternion.identity);
    }
}
