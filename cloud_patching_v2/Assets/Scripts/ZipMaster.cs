//using System.IO;
//using UnityEngine;
//using System.Collections;
//using ICSharpCode.SharpZipLib.Core;
//using ICSharpCode.SharpZipLib.Zip;
//using UnityEngine.UI;

//public class ZipMaster : MonoBehaviour
//{
//    [SerializeField]
//    string filePath;
//    [SerializeField]
//    bool unzipNow = false;
//    [SerializeField]
//    Button button_unzip;
//    [SerializeField]
//    Text stats_amountfiles_unzip;
//    int unzippedFiles_amount = 0;

//    public void OnValidate()
//    {
//        //Debug.Log(Application.dataPath);
//        if(unzipNow)
//        {
//            UnzipFile();
//            unzipNow = false;
//        }
//    }

//    public void Awake()
//    {
//        button_unzip.onClick.AddListener(UnzipFile);
//    }

//    public void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//            UnzipFile();

//        if (Input.GetKeyDown(KeyCode.LeftShift))
//            ValidateStreamAssets();
//    }
//    public void ValidateStreamAssets()
//    {
//        string[] tracker_files = Directory.GetFiles(Application.dataPath + "/Scripts");
//        for (int i = 0; i < tracker_files.Length; i++)
//        {
//            Debug.Log("File found " + i + "." + tracker_files[i]);
//        }
//    }

//    public void UnzipFile()
//    {
//        //if (!Directory.Exists(Application.dataPath + "/scripts"))
//        //{
//        //    Directory.CreateDirectory(Application.dataPath + "/scripts");
//        //}

//#if UNITY_EDITOR
//        StartCoroutine(LoadZipFile(Path.Combine(Application.dataPath + "/Scripts", filePath)));
//#elif UNITY_ANDROID
//        StartCoroutine(LoadZipFile(Path.Combine(Application.dataPath + "/scripts", filePath)));
//#endif
//    }


//    IEnumerator LoadZipFile(string FilePath)
//    {
//        Debug.LogError("Run unzipping file...at path = " + FilePath);
//        if (File.Exists(FilePath) == false)
//        {
//            Debug.LogError("Not found!");
//            yield break;
//        }
//        else
//        {
//            Debug.LogError("Found");
//        }
//        //Read file
//        FileStream fs = null;
//        try
//        {
//            fs = new FileStream(FilePath, FileMode.Open);
//        }
//        catch
//        {
//            Debug.Log("GameData file open exception: " + FilePath);
//        }

//        if (fs != null)
//        {
//            //Read zip file
//            ZipFile zf = new ZipFile(fs);
//            int numFiles = 0;

//            if (zf.TestArchive(true) == false)
//            {
//                Debug.Log("Zip file failed integrity check!");
//                zf.IsStreamOwner = false;
//                zf.Close();
//                fs.Close();
//            }
//            else
//            {
//                foreach (ZipEntry zipEntry in zf)
//                {
//                    //Ignore directories

//                    if (!zipEntry.IsFile)
//                        continue;

//                    string entryFileName = zipEntry.Name;

//                    //Skip.DS_Store files(these appear on OSX)
//                    if (entryFileName.Contains("DS_Store"))
//                        continue;

//                    Debug.Log("Unpacking zip file entry: " + entryFileName);
//                    unzippedFiles_amount++;

//                    byte[] buffer = new byte[4096];     // 4K is optimum
//                    Stream zipStream = zf.GetInputStream(zipEntry);

//                    //Manipulate the output filename here as desired.
//                    string fullZipToPath = Application.dataPath + "/scripts/" + Path.GetFileName(entryFileName);

//                    //Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
//                    //of the file, but does not waste memory.
//                    //The "using" will close the stream even if an exception occurs.
//                    using (FileStream streamWriter = File.Create(fullZipToPath))
//                    {
//                        StreamUtils.Copy(zipStream, streamWriter, buffer);
//                    }
//                }

//                numFiles++;

//                zf.IsStreamOwner = false;
//                zf.Close();
//                fs.Close();
//            }

//            stats_amountfiles_unzip.text = "Amount file(s) unzipped = " + unzippedFiles_amount;
//        }
//    }
//}
