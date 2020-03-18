using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Cloud : MonoBehaviour
{
    public AssetReference image_ar;
    public GameObject image_go;
    public Transform parent_canvas;

    void InstantiateAsyncFromCloud()
    {
        image_ar.InstantiateAsync(new Vector3(), Quaternion.identity, parent_canvas);
    }

    void InstantiateFromLocalClient()
    {
        Instantiate(image_go , parent_canvas);
    }

    IEnumerator CheckForUpdates()
    {
        yield return new WaitForSeconds(1f);
        Addressables.InitializeAsync().Completed += (callback_init =>
        {
            Addressables.CheckForCatalogUpdates().Completed += (callback_update => 
            {
                Debug.LogError(callback_update.Result.Count);
                if(callback_update.Result.Count> 0)
                {
                    Addressables.UpdateCatalogs().Completed += (callback_patch =>
                    {

                    });
                }
                else
                {
                    //
                }
            });
        });
    }

    void LoadAssets()
    {
        image_ar.LoadAssetAsync<GameObject>().Completed += (callback =>
        {
            Debug.Log(callback.Status);
            if (callback.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                //image_ar.InstantiateAsync(new Vector3(), Quaternion.identity, parent_canvas).Completed += (state =>
                //{
                //    Debug.Log(state.Result);
                //});
            }
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            InstantiateAsyncFromCloud();
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            InstantiateFromLocalClient();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(CheckForUpdates());
        }
        else if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            LoadAssets();
        }
    }
}
