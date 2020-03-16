using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class Manager : MonoBehaviour
{
    [SerializeField]
    string filePath;
    [SerializeField]
    string fileKey;
    public AssetReference asset;
    private void Start()
    {
        Addressables.InitializeAsync();
    }

    void LoadSpecificFile(string _path = null, string _key = null)
    {
        StartCoroutine(loadingAsset(_path, _key));
    }

    IEnumerator loadingAsset(string _path = null, string _key = null)
    {
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(_key);

        yield return handle.Status;

        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Load succeeded! " + handle.Result + " count = " + handle.Result.Count);

            AsyncOperationHandle<IResourceLocator> handler = Addressables.LoadContentCatalogAsync(Application.streamingAssetsPath+ "/catalog_2020.03.15.05.57.56.json");

            yield return handler.Status;
        }

        Debug.Log("Asset loaded in session");

        asset.InstantiateAsync(new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            LoadSpecificFile(Application.streamingAssetsPath, fileKey);
    }
}
