using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CloudManager : MonoBehaviour
{
    public ImportAssets importAssetScript;
    [Header("UI")]
    public GameObject downloadIndicator;
    public GameObject loadingIndicator;
    public GameObject image;
    [SerializeField]
    private string webPath = null; 
    ulong downloadedFile;
    public IEnumerator DownloadImage(System.Action callback)
    {
        loadingIndicator.SetActive(true);
        using (UnityWebRequest www = UnityWebRequest.Get(webPath))
        {
            Debug.Log(www.downloadProgress);
            string path = Path.Combine(Application.dataPath, "testsubject00.jpeg");
            www.downloadHandler = new DownloadHandlerFile(path);
       
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadProgress);
                downloadedFile =  www.downloadedBytes;
                Debug.Log(downloadedFile);

                if (callback != null)
                    callback.Invoke();

                StartCoroutine(IndicatorExpiryTimer(2f, loadingIndicator));
            }
        }
    }

    void IndiatorDownload(string _string)
    {
        downloadIndicator.SetActive(true);

        Debug.Log("Receiving..." + _string);

        importAssetScript.LoadAssets("testsubject00",image);

        StartCoroutine(IndicatorExpiryTimer(3f, downloadIndicator));
    }

    IEnumerator IndicatorExpiryTimer(float timer = 2f , GameObject _gameobjectUI = null)
    {
        yield return new WaitForSeconds(timer);

        _gameobjectUI.SetActive(false);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(DownloadImage(() => { IndiatorDownload(downloadedFile.ToString()); })) ;
        }
    }
}
