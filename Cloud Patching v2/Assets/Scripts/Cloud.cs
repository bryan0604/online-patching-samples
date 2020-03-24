using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class Cloud : MonoBehaviour
{
    public AssetReference asset_background;
    public AssetReference asset_prem_content;
    public GameObject image_go;
    public Transform parent_canvas;
    public List<Button> Buttons = new List<Button>();
    public bool proceedDownloadingPatch = false;
    public bool isCheckedPatch = false;
    public bool agreedDownload = false;
    public bool pendingDownload = false;
    public bool requireDownloadPatch = false;
    public string downloadKey = "";
    [SerializeField] bool isCloudInit = false;
    [SerializeField] bool isCatalogChecked = false;
    bool isSpawning = false;


    private void Awake()
    {
        Buttons[0].onClick.AddListener(() => StartCoroutine(Process()));
    }
    void InitDone(AsyncOperationHandle<IResourceLocator> obj)
    {
        Debug.Log(obj.Status);

        if (obj.Status == AsyncOperationStatus.Succeeded)
        {

            isCloudInit = true;
        }
    }

    void OnCheckingCatalogs(AsyncOperationHandle<List<string>> cb_checkforupdates)
    {
        Debug.Log(cb_checkforupdates.Status);

        if(cb_checkforupdates.Status == AsyncOperationStatus.Succeeded)
        {
            isCatalogChecked = true;

            Debug.LogError(cb_checkforupdates.Result.Count);
            if(cb_checkforupdates.Result.Count == 0)
            {
                isCheckedPatch = true;
                requireDownloadPatch = false;
                return;
            }

            foreach (var item in Addressables.ResourceLocators)
            {
                ResourceLocationMap map = (ResourceLocationMap)item;

                Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

                foreach (var loc in locations)
                {
                    if (loc.Key.Equals("default"))
                    {
                        Debug.Log("Found the packet!");
                        Addressables.GetDownloadSizeAsync(loc.Key).Completed += OnGettingDownloadSize;
                        downloadKey = loc.Key.ToString();
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }

    void OnUpdateCatalogs(AsyncOperationHandle<List<IResourceLocator>> cb_updatecatalog)
    {
        Debug.Log(cb_updatecatalog.Status);

        foreach (var loc in cb_updatecatalog.Result)
        {
            Debug.Log(loc.Keys);
            Debug.Log(loc.LocatorId);
        }
    }

    void OnGettingDownloadSize(AsyncOperationHandle<System.Int64> cb_getdownloadsize)
    {
        Debug.Log("Download size = " + cb_getdownloadsize.Result + " status = " + cb_getdownloadsize.Status + " " + cb_getdownloadsize.IsDone);

        isCheckedPatch = true;
        requireDownloadPatch = true;
    }

    IEnumerator Process()
    {
        Addressables.InitializeAsync().Completed += InitDone;

        while (isCloudInit != true)
        {
            yield return new WaitForFixedUpdate();
        }

        Addressables.CheckForCatalogUpdates().Completed += OnCheckingCatalogs;

        while (isCheckedPatch != true)
        {
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Patch checked!");

        if(requireDownloadPatch)
        {

            Debug.Log("Pending download...");
            while (pendingDownload != true)
            {
                yield return new WaitForFixedUpdate();
                if (proceedDownloadingPatch)
                {
                    pendingDownload = proceedDownloadingPatch;
                    Debug.Log("patch responded");
                }
            }

            if(agreedDownload)
            {
                Debug.Log("Downloading patch...");

                AsyncOperationHandle cb_downloads = Addressables.DownloadDependenciesAsync(downloadKey, false);

                while (cb_downloads.PercentComplete < 1)
                {
                    yield return new WaitForFixedUpdate();

                    Debug.Log(cb_downloads.PercentComplete);
                }
                Debug.Log("Download done" + cb_downloads.Status);

                Addressables.UpdateCatalogs().Completed += OnUpdateCatalogs;
            }
            else
            {
                Debug.Log("No downloading...");
            }
        }
        else
        {
            Debug.Log("No patch to update...");

            StartCoroutine(LoadAssets(asset_background));
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(LoadAssets(asset_prem_content));
        }
    }

    IEnumerator LoadAssets(AssetReference _asset)
    {
        //if(isSpawning)
        //{
        //    while (isSpawning != false)
        //    {
        //        yield return new WaitForFixedUpdate();
        //    }
        //}
        //isSpawning = true;
        yield return new WaitForSeconds(0.25f);
        Addressables.LoadAssetAsync<GameObject>(_asset).Completed += (callback =>
        {
            if (callback.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                InstantiateAsyncFromCloud(callback);
            }
        });
    }

    void InstantiateAsyncFromCloud(AsyncOperationHandle<GameObject> _asset)
    {
        Instantiate(_asset.Result, parent_canvas);
        //isSpawning = false;
    }
}
