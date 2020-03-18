//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using UnityEngine.ResourceManagement.ResourceProviders;

//public class Manager : MonoBehaviour
//{
//    [SerializeField]
//    string filehash;
//    [SerializeField]
//    string fileKey;
//    public AssetReference asset;
//    Task<List<GameObject>> gameObjectsss;
//    [SerializeField]
//    IList<IResourceLocation> assetsToLoad_shaders = new List<IResourceLocation>();
//    private void Awake()
//    {
//        //Addressables.InitializeAsync();
//    }

//    //public async Task<List<T>> GetAllAssetsInFolderAsync<T>(string folderPath, Action<List<T>> onComplete)
//    //{
//    //    IList<IResourceLocation> assetsToLoad = new List<IResourceLocation>();

//    //    foreach (var locs in Addressables.ResourceLocators)
//    //    {
//    //        ResourceLocationMap map = (ResourceLocationMap)locs;

//    //        Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

//    //        foreach (var loc in locations)
//    //        {
//    //            if (loc.Key.ToString().Contains(folderPath))
//    //            {
//    //                Debug.Log($"{loc.Key} contains filepath {folderPath}");

//    //                assetsToLoad.Add(loc.Value[0]);
//    //            }
//    //        }
//    //    }

//    //    AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(assetsToLoad, null);

//    //    await handle.Task;

//    //    if (handle.Status != AsyncOperationStatus.Succeeded)
//    //    {
//    //        return null;
//    //    }

//    //    onComplete?.Invoke((List<T>)handle.Result);
//    //    return (List<T>)handle.Result;
//    //}

//    void LoadSpecificFile(string _path = null, string _key = null)
//    {
//        StartCoroutine(loadingAsset<object>(_path, _key));
//    }

//    IEnumerator loadingAsset<T>(string _path = null, string _key = null)
//    {
//        IList<IResourceLocation> assetsToLoad = new List<IResourceLocation>();
//        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(_key);

//        yield return handle.Status;

//        if (handle.Status == AsyncOperationStatus.Succeeded)
//        {
//            Debug.Log("Load succeeded! " + handle.Result + " count = " + handle.Result[0].DependencyHashCode);

//            AsyncOperationHandle<IResourceLocator> handler = Addressables.LoadContentCatalogAsync(Addressables.RuntimePath + "/"+ filehash + ".json");

//            yield return handler.Status;
//            Addressables.ClearResourceLocators();
//            Addressables.AddResourceLocator(handler.Result);

//            Debug.Log("Resourcec locator = " + Addressables.ResourceLocators);
//            foreach (var locs in Addressables.ResourceLocators)
//            {
//                ResourceLocationMap map = (ResourceLocationMap)locs;

//                Debug.Log("Map location = " + map.Locations);
//                Dictionary<object, IList<IResourceLocation>> locations = map.Locations;

//                foreach (var loc in locations)
//                {
//                    Debug.Log("Respective key is... " + loc.Key.ToString() + " ||| " + loc.Value.GetType());
//                    if (loc.Key.ToString().Contains(_path))
//                    {
//                        Debug.LogWarning($"{loc.Key} contains filepath {_path}");
                        
//                        assetsToLoad.Add(loc.Value[0]);
//                    }
//                    //else if(loc.Key.ToString().Contains("shaders"))
//                    //{
//                    //    Debug.LogWarning("Loading shaders");

//                    //    assetsToLoad_shaders.Add(loc.Value[0]);
//                    //}
//                }
//                break;
//            }
//        }

//        AsyncOperationHandle<IList<T>> ops = Addressables.LoadAssetsAsync<T>(assetsToLoad, null);

//        Debug.Log("Asset loaded in session with amount of " + assetsToLoad.Count);
//    }

//    IEnumerator loadShaders<T>() where T : AssetBundle
//    {
//        AsyncOperationHandle<IList<T>> ops = Addressables.LoadAssetsAsync<T>(assetsToLoad_shaders, null);

//        yield return new WaitForSeconds(1f);
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.LeftControl))
//            LoadSpecificFile("Assets/Resources_moved/Prefabs/", fileKey);
//        else if (Input.GetKeyDown(KeyCode.LeftCommand))
//            asset.InstantiateAsync(new Vector3(0, 0, 0), Quaternion.identity);

//    }

//    public void LoadGameAssets()
//    {
//        LoadSpecificFile("Assets/Resources_moved/Prefabs/", fileKey);
//    }

//    public void InstantiatePatchedAsset()
//    {
//        asset.InstantiateAsync(new Vector3(0, 0, 0), Quaternion.identity);
//    }
//}
