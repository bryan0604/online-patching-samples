using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class Manager : MonoBehaviour
{
    public AssetReference asset;
    private void Start()
    {
        Addressables.InitializeAsync().Completed += InitialisePatchManager;
    }

    void InitialisePatchManager(AsyncOperationHandle<IResourceLocator> response)
    {
        Debug.Log(response);
        CheckPatch(Addressables.CheckForCatalogUpdates());
    }

    private void CheckPatch(AsyncOperationHandle<List<string>> callback)
    {
        List<string> tempStorage = callback.Result;
        if(tempStorage!=null)
        Debug.Log(tempStorage.Count);

        StartCoroutine(Patching());
    }

    IEnumerator Patching()
    {
        AsyncOperationHandle<GameObject> c = asset.InstantiateAsync(new Vector3(0, 0, 0), Quaternion.identity);

        yield return c.IsDone;

        Debug.LogError("Done - Instiantiate");
    }

    
    //private IEnumerable<string> Test(string callbackResult)
    //{
    //    return Addressables.ca;
    //}
}
