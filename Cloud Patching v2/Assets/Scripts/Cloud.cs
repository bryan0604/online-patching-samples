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
    public AssetReference image_ar;
    public GameObject image_go;
    public Transform parent_canvas;
    public List<Button> Buttons = new List<Button>();
  
    [SerializeField] bool isCloudInit = false;
    [SerializeField] bool isCatalogChecked = false;


    private void Awake()
    {
        Buttons[0].onClick.AddListener(() => StartCoroutine(Process()));
        Buttons[1].onClick.AddListener(() => InstantiateFromLocalClient());
        Buttons[2].onClick.AddListener(InstantiateAsyncFromCloud);
        Buttons[3].onClick.AddListener(LoadAssets);

        Addressables.InitializeAsync().Completed += InitDone;
        StartCoroutine(Process());
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

            foreach (var item in Addressables.ResourceLocators)
            {
                ResourceLocationMap map = (ResourceLocationMap)item;

                Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

                foreach (var loc in locations)
                {
                    Debug.Log(loc.Value[0]);
                    if (loc.Value[0].ToString().Contains("https://storage.googleapis.com/cloud_patching_sample/"))
                    {
                        //Debug.Log(loc.Value[0]);
                        Debug.Log(loc.Key + " | " + loc.Value[0].PrimaryKey );
                        Addressables.GetDownloadSizeAsync("default").Completed += OnGettingDownloadSize;

                        break;
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


    }

    IEnumerator Process()
    {
        while (isCloudInit != true)
        {
            yield return new WaitForFixedUpdate();
        }

        Addressables.CheckForCatalogUpdates().Completed += OnCheckingCatalogs;

        while (isCatalogChecked != true)
        {
            yield return new WaitForFixedUpdate();
        }


    }

    IEnumerator CheckForUpdates()
    {
        AsyncOperationHandle<List<string>> cb_checkforupdates = Addressables.CheckForCatalogUpdates();
        yield return cb_checkforupdates.Result;

        if(cb_checkforupdates.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(cb_checkforupdates.Result.Count);
            if(cb_checkforupdates.Result.Count > 0)
            {
                Debug.LogError("There is an update that you can download = " + cb_checkforupdates.Result[0]);
                AsyncOperationHandle<List<IResourceLocator>> cb_updatecatalog = Addressables.UpdateCatalogs(cb_checkforupdates.Result, false);

                yield return cb_updatecatalog.Result;

                Debug.Log("Catalog updates status = " + cb_updatecatalog.Status);
                //StartCoroutine(DownloadUpdates());
            }
            else
            {
                Debug.LogError("There isn't any update...");
            }
        }
        else
        {
            Debug.LogError("Failed to locate");
        }
    }

    IEnumerator DownloadUpdates()
    {
        foreach (var item in Addressables.ResourceLocators)
        {
            ResourceLocationMap map = (ResourceLocationMap)item;

            Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

            foreach (var loc in locations)
            {
                //Debug.Log(loc.Key);
                //Debug.Log(loc.Value[0]);
                //Debug.Log(loc.Value[0].DependencyHashCode);
                //Debug.Log(loc.Value[0].Dependencies);
                //Debug.Log(loc.Value[0].PrimaryKey);
                if (loc.Value[0].ToString().Contains("https://storage.googleapis.com/cloud_patching_sample/"))
                {
                    //Debug.Log(loc.Key);
                    //Debug.Log(loc.Value[0]);
                    //Debug.Log(loc.Value[0].DependencyHashCode);
                    //Debug.Log(loc.Value[0].Dependencies);
                    //Debug.Log(loc.Value[0].PrimaryKey);

                    AsyncOperationHandle<long> cb_getdownloadsize = Addressables.GetDownloadSizeAsync(loc.Key);

                    yield return cb_getdownloadsize.Result;

                    Debug.Log("Download size = " + cb_getdownloadsize.Result + " status = " + cb_getdownloadsize.Status);

                    AsyncOperationHandle cb_downloaddep = Addressables.DownloadDependenciesAsync(loc.Key, false);

                    while (cb_downloaddep.PercentComplete < 1)
                    {
                        Debug.Log(cb_downloaddep.PercentComplete);
                        yield return new WaitForFixedUpdate();
                    }

                    yield return cb_downloaddep.Result;

                    Debug.Log("Download status = " + cb_downloaddep.Status);

                    //AsyncOperationHandle<List<IResourceLocator>> cb_updatecatalog = Addressables.UpdateCatalogs(_cb_checkforUpdates.Result, false);

                    //yield return cb_updatecatalog.Result;

                    //Debug.Log("Catalog updates status = " + cb_updatecatalog.Status);

                    break;
                }
            }
        }
    }

    public static AsyncOperationHandle<List<IResourceLocator>> UpdateCatalogs(IEnumerable<string> catalog = null, bool autoReleaseHandle = true)
    {
        AsyncOperationHandle<List<IResourceLocator>> final = new AsyncOperationHandle<List<IResourceLocator>>();

        Debug.Log("H "+catalog );
        return final;
    }

    void LoadAssets()
    {
        Addressables.LoadAssetAsync<GameObject>(image_ar).Completed += (callback =>
        {
            Debug.Log(callback.Status);
            if (callback.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                InstantiateFromLocalClient(callback.Result);
            }
            else
            {
                InstantiateFromLocalClient();
            }
        });
    }

    void InstantiateAsyncFromCloud()
    {
        image_ar.InstantiateAsync(new Vector3(), Quaternion.identity, parent_canvas).Completed += (callback =>
        {
            Debug.Log("Spawn " + callback.Status);
        }); // this is 100% loaded from cloud bundle
        //Addressables.InstantiateAsync(image_ar, parent_canvas);
    }

    void InstantiateFromLocalClient(GameObject _go = null)
    {
        Instantiate(_go, parent_canvas);
    }
}
