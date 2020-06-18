using UnityEngine.AddressableAssets;
using RestSharp;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DownloadScriptsManager : MonoBehaviour
{
    public List<bucketurladdress> urls = new List<bucketurladdress>();
    [SerializeField]
    bool downloadPatchScripts = false;
    [SerializeField]
    Button button_downloadpatchScripts;
    [SerializeField]
    bucketurladdress.url_type url_selection = bucketurladdress.url_type.url_1;
    public void OnValidate()
    {
        if (downloadPatchScripts)
        {
            DownloadZipFile();
            downloadPatchScripts = false;
        }
    }

    public void Awake()
    {
        button_downloadpatchScripts.onClick.AddListener(DownloadZipFile);
    }
    public void DownloadZipFile()
    {
        StartCoroutine(AttemptDownloading());
    }

    IEnumerator AttemptDownloading()
    {
        yield return "";
        Debug.Log("Downloading patchscripts.zip file");
        Debug.Log(GetURL(url_selection));

        RestClient client = new RestClient(GetURL(url_selection));
        RestRequest request = new RestRequest("patchscripts.zip", Method.GET);

        //client.DownloadData(request);
        //request.AddHeader("Accept;", "application/zip");

        // execute the request
        //IRestResponse response = client.Execute(request);
        //yield return response.StatusCode;
        Debug.Log(Application.streamingAssetsPath);
        //string[] files = Directory.GetFiles(Application.dataPath);
        //Debug.Log("Files amount = "+files.Length);

        //if(File.Exists(Application.streamingAssetsPath))
        //{
        //    Debug.Log("A");
        //    if (File.Exists(Application.streamingAssetsPath+"/assets"))
        //    {
        //        Debug.Log("B");
        //        if (File.Exists(Application.streamingAssetsPath+"/assets/scripts"))
        //        {
        //            Debug.Log("C");

        //        }
        //    }
        //}

        //File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath+"/assets/scripts", "patchscripts.zip"), client.DownloadData(request));
        //#endif

        Debug.Log("Done");
    }


    public string GetURL(bucketurladdress.url_type _type)
    {
        for (int i = 0; i < urls.Count; i++)
        {
            if (_type == urls[i].url_choice)
            {
                return urls[i].url;
            }
        }

        return null;
    }

    [System.Serializable]
    public class bucketurladdress
    {
        public string url;
        public url_type url_choice = url_type.url_1;
        public enum url_type
        {
            url_1,
            url_2,
        }
    }
}
