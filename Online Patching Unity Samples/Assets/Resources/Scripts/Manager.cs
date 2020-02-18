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
        AsyncOperationHandle<GameObject> asyc = Addressables.LoadAssetAsync<GameObject>(name);
        yield return asyc.IsDone;

        Debug.Log("ASSET LOADED!");
        while (asyc.PercentComplete < 1)
        {
            Debug.Log(asyc.PercentComplete);
            yield return new WaitForFixedUpdate();
        }

        if(asyc.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = asyc.Result;
            Addressables.Release(asyc);
            Debug.Log("ASSET = " + obj.name);
        }
    }

    IEnumerator PreloadDependencies()
    {
        AsyncOperationHandle asyc = Addressables.DownloadDependenciesAsync(key);
        yield return asyc.IsDone;

        Debug.Log("ASSET DOWNLOADED!");
        while (asyc.PercentComplete < 1)
        {
            Debug.Log(asyc.PercentComplete);
            yield return new WaitForFixedUpdate();
        }

        if (asyc.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(asyc);
            Debug.Log("ASSET LOADING...");
        }
    }
}
