﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;
using RestSharp;

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
    public bool downloadDone = false;
    public bool foundpacket = false;
    public bool autoUpdate = false;
    public bool isloadingAsset = false;
    public string catalogKeyname = "";
    public string downloadKey = "";
    [SerializeField] bool isCloudInit = false;
    [SerializeField] bool isCatalogChecked = false;
    bool isSpawning = false;
    UserInterface UI_MANAGER;
    WaitForSeconds frequencyshort = new WaitForSeconds(0.5f);

    private void Awake()
    {
        Buttons[0].onClick.AddListener(() => StartCoroutine(Process()));
        Buttons[1].onClick.AddListener(()=> InvokeRepeating("CheckEveryXSeconds",1f,1f));
        UI_MANAGER = GetComponent<UserInterface>();
    }

    void CheckEveryXSeconds()
    {
        if(autoUpdate == false)
        {
            Addressables.InitializeAsync().Completed += InitDone;
            autoUpdate = true;
        }

        if (foundpacket)
        {
            CancelInvoke("CheckEveryXSeconds");
            return;
        }
        
        Addressables.CheckForCatalogUpdates(false).Completed += OnCheckingCatalogs;
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
            Debug.LogError(cb_checkforupdates.Task.Result.Count );
            Debug.LogError(cb_checkforupdates.Result.Count);
       
            //Debug.LogError(Addressables.ResourceLocators.First<IResourceLocator>().Keys.First());
            
            if(cb_checkforupdates.Result.Count == 0)
            {
                isCheckedPatch = true;
                requireDownloadPatch = false;
                return;
            }
            else
            {
                for (int i = 0; i < cb_checkforupdates.Result.Count; i++)
                {
                    Debug.LogError("patch - " + cb_checkforupdates.Result[i]);
                }
            }

            foreach (var item in Addressables.ResourceLocators)
            {
                //Debug.LogWarning(item + " || " + item.Keys + " || " + item.LocatorId + " || " + item.Keys.Count());

                if(item.GetType() == typeof(ResourceLocationMap))
                {
                    ResourceLocationMap map = (ResourceLocationMap)item;
                    Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

                    foreach (var loc in locations)
                    {
                        Debug.Log(loc.Key + " || " + loc.Value + " || " + loc.Value[0].PrimaryKey + " || " + loc.Value[0].DependencyHashCode);
                        if (loc.Key.Equals("default"))
                        {
                            Debug.Log("Found the packet!");
                            foundpacket = true;

                            downloadKey = loc.Key.ToString();
                            StartCoroutine(CheckCatalogVersion(downloadKey));
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
    }

    void OnUpdateCatalogs(AsyncOperationHandle<List<IResourceLocator>> cb_updatecatalog)
    {
        Debug.Log(cb_updatecatalog.Status);
        downloadDone = true;
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


        UI_MANAGER.PopulateContent(cb_getdownloadsize.Result.ToString());
    }

    void OnDownload()
    {
        proceedDownloadingPatch = true;
    }

    void OnDownloadReject()
    {
        Debug.LogError("Sorry gotta update dude!");
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

                    UI_MANAGER.UpdateDynamicContent(cb_downloads.PercentComplete.ToString());
                }
                Debug.Log("Download done" + cb_downloads.Status);
                yield return new WaitForSeconds(5f);

                UI_MANAGER.RemoveAndClear();

                Addressables.UpdateCatalogs().Completed += OnUpdateCatalogs;

                while(downloadDone != true)
                {
                    yield return new WaitForSeconds(0.25f);
                }

                if (downloadDone)
                {
                    StartCoroutine(LoadAssets(asset_background));
                    yield return new WaitForSeconds(0.25f);
                    StartCoroutine(LoadAssets(asset_prem_content));
                }
            }
            else
            {
                Debug.Log("No downloading...");
            }
        }
        else
        {
            Debug.Log("No patch to update...");
            InitialiseAssetsSpawn();
        }
    }
    
    void  InitialiseAssetsSpawn()
    {
        StartCoroutine(LoadAssets(asset_background));
        StartCoroutine(LoadAssets(asset_prem_content));
    }

    IEnumerator LoadAssets(AssetReference _asset)
    {
        while (isloadingAsset != false)
        {
            yield return frequencyshort;
        }

        isloadingAsset = true;
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
        /*isloadingAsset =*/ Instantiate(_asset.Result, parent_canvas);
        isloadingAsset = false;
        //isSpawning = false;
    }

    IEnumerator CheckCatalogVersion(string key)
    {
        RestClient client = new RestClient("https://storage.googleapis.com/cloud_patching_sample/Android");
        RestRequest request = new RestRequest("catalog_"+ catalogKeyname +".hash", Method.GET);
        IRestResponse response = client.Execute(request);
        yield return response.StatusCode;

        Debug.Log(response.StatusCode);
        if(response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Debug.Log("Catalog exist in server");

            UI_MANAGER.Initialise(OnDownload, OnDownloadReject);
            Addressables.GetDownloadSizeAsync(key).Completed += OnGettingDownloadSize;
        }
        else
        {
            Debug.Log("Catalog not exist in the server or not tally with the server api");
            InitialiseAssetsSpawn();
        }
    }
}