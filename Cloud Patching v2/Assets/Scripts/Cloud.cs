using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        Buttons[0].onClick.AddListener(() => StartCoroutine(CheckForUpdates()));
        Buttons[1].onClick.AddListener(()=>InstantiateFromLocalClient());
        Buttons[2].onClick.AddListener(InstantiateAsyncFromCloud);
        Buttons[3].onClick.AddListener(LoadAssets);
        StartCoroutine(InitializationProgress());
    }

    IEnumerator InitializationProgress()
    {
        AsyncOperationHandle init_op = Addressables.InitializeAsync();

        while (init_op.PercentComplete >= 1)
        {
            yield return new WaitForFixedUpdate();

            Debug.Log(init_op.PercentComplete);
        }

        Debug.LogWarning("Initialization done");
    }

    IEnumerator CheckForUpdates()
    {
        //yield return new WaitForSeconds(1f);
        //Addressables.InitializeAsync().Completed += (callback_init =>
        //{
        //    Addressables.CheckForCatalogUpdates(false).Completed += (callback_update =>
        //    {
        //        Debug.LogError(callback_update.Result.Count);
        //        if (callback_update.Result.Count > 0)
        //        {
        //            Addressables.UpdateCatalogs(null, false).Completed += (callback_patch =>
        //            {
        //                Debug.Log(callback_patch.Result[0].LocatorId + "|" + callback_patch.Result[0].Keys);


        //                //Debug.Log(Addressables.BuildPath);
        //                //Debug.Log(Addressables.PlayerBuildDataPath);
        //                //Debug.Log(Addressables.RuntimePath);
        //                //Debug.Log(Addressables.ResourceLocators);

        //                foreach (var item in Addressables.ResourceLocators)
        //                {
        //                    ResourceLocationMap map = (ResourceLocationMap)item;

        //                    Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

        //                    foreach (var loc in locations)
        //                    {
        //                        if (loc.Value[0].ToString().Contains("https://storage.googleapis.com/cloud_patching_sample/"))
        //                        {
        //                            Debug.Log(loc.Key);
        //                            Debug.Log(loc.Value[0]);
        //                            Debug.Log(loc.Value[0].DependencyHashCode);
        //                            Debug.Log(loc.Value[0].Dependencies);
        //                            Debug.Log(loc.Value[0].PrimaryKey);
        //                            Addressables.GetDownloadSizeAsync(loc.Key).Completed += (cb_getsize =>
        //                            {
        //                                while (cb_getsize.PercentComplete >= 1)
        //                                {
        //                                    yield return new WaitForFixedUpdate();
        //                                }
        //                            });

        //                            Addressables.DownloadDependenciesAsync(loc.Key, false).Completed += (cb_download =>
        //                            {
        //                                Debug.Log("Download " + cb_download.Status);
        //                            });

        //                            break;
        //                        }
        //                    }

        //                    continue;
        //                }
        //            });
        //        }
        //    });
        //});

        AsyncOperationHandle<List<string>> cb_checkforupdates = Addressables.CheckForCatalogUpdates(false);
        yield return cb_checkforupdates.Result;

        if(cb_checkforupdates.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(cb_checkforupdates.Result.Count);
            if(cb_checkforupdates.Result.Count > 0)
            {
                Debug.LogError("There is an update that you can download");
            }
            else
            {
                Debug.LogError("There isn't any update...");
            }
        }
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
