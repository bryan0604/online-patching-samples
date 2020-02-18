using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Manager : MonoBehaviour
{
    public string key;
    public string name;
    [SerializeField]
    bool isloaded;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Load());
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCoroutine(PreloadDependencies());
        }
    }

    IEnumerator Load()
    {
        AsyncOperationHandle asyc = Addressables.LoadAssetAsync<GameObject>(name);
        yield return asyc.IsDone;

        Debug.Log("ASSET LOADED");
    }

    IEnumerator PreloadDependencies()
    {
        AsyncOperationHandle asyc = Addressables.DownloadDependenciesAsync(key);
        yield return asyc.IsDone;

        Debug.Log("ASSET DOWNLOADED!");
    }
}
