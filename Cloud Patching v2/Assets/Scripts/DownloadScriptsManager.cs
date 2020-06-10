using ICSharpCode.SharpZipLib.Zip;
using RestSharp;
using RestSharp.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;

public class DownloadScriptsManager : MonoBehaviour
{
    [SerializeField]
    bool downloadPatchScripts = false;
    [SerializeField]
    Button button_downloadpatchScripts;
    [SerializeField]
    string url_patchscript_zipfile;
    public void OnValidate()
    {
        if(downloadPatchScripts)
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
        Debug.Log("Downloading patchscripts.zip file");
        Debug.Log(url_patchscript_zipfile);

        RestClient client = new RestClient(url_patchscript_zipfile);
        RestRequest request = new RestRequest("#", Method.GET);

        byte[] fileBytes = client.DownloadData(request);
        request.AddHeader("Accept;", "application/zip");

        // execute the request
        IRestResponse response = client.Execute(request);
        yield return response.StatusCode;
        //yield return request;
        Debug.LogError("Status = " + response.StatusCode + " Response = " + response.Content.GetType());

        File.WriteAllBytes(Path.Combine(Application.dataPath+ "/Scripts ", "patchscript.html"), fileBytes);
        File.WriteAllBytes(Path.Combine(Application.dataPath + "/Scripts ", "patchscript.zip"), fileBytes);

        Debug.Log("Done");
    }

}
