using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Manager : MonoBehaviour
{
    [TextArea]
    public string path;
    [TextArea]
    public string key;
    [TextArea]
    public string name;
    public AssetReference reference;
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
            Instantiate(obj);
        }
    }

    IEnumerator PreloadDependencies()
    {
        AsyncOperationHandle asyc = Addressables.LoadContentCatalogAsync(path);
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

        AsyncOperationHandle<IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>> goods = Addressables.LoadResourceLocationsAsync(key,typeof(GameObject));

        while (goods.PercentComplete < 1)
        {
            Debug.Log(goods.PercentComplete);
            yield return new WaitForFixedUpdate();
        }

        if(goods.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Asset total = " + goods.Result.Count);

            for (int i = 0; i < goods.Result.Count; i++)
            {
                Debug.Log( goods.Result[i].ResourceType);

                AsyncOperationHandle<GameObject> gO = Addressables.LoadAssetAsync<GameObject>(goods.Result[i]);

                Addressables.Release(gO);
            }

        }
        Addressables.Release(goods);
        Addressables.ClearResourceLocators();
    }
}
