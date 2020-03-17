using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.Linq;

public class ZipMaster : MonoBehaviour
{
    [SerializeField]
    string filePath;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            LoadZipFile(Application.streamingAssetsPath+filePath);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            ValidateStreamAssets();
    }

    public void UnzipFile()
    {
        LoadZipFile(Application.streamingAssetsPath + filePath);
    }

    public void ValidateStreamAssets()
    {
        string[] tracker_files = Directory.GetFiles(Application.streamingAssetsPath);
        for (int i = 0; i < tracker_files.Length; i++)
        {
            Debug.Log("File found " + i + "." + tracker_files[i]);
        }
    }

    void LoadZipFile(string FilePath)
    {
        Debug.LogError("Run unzipping file...");
        if (System.IO.File.Exists(FilePath) == false)
        {
            Debug.LogError("Not found!");
            return;
        }

        // Read file
        FileStream fs = null;
        try
        {
            fs = new FileStream(FilePath, FileMode.Open);
        }
        catch
        {
            Debug.Log("GameData file open exception: " + FilePath);
        }

        if (fs != null)
        {
            try
            {
                // Read zip file
                ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(fs);
                int numFiles = 0;

                if (zf.TestArchive(true) == false)
                {
                    Debug.Log("Zip file failed integrity check!");
                    zf.IsStreamOwner = false;
                    zf.Close();
                    fs.Close();
                }
                else
                {
                    foreach (ZipEntry zipEntry in zf)
                    {
                        // Ignore directories
                        if (!zipEntry.IsFile)
                            continue;

                        String entryFileName = zipEntry.Name;

                        // Skip .DS_Store files (these appear on OSX)
                        if (entryFileName.Contains("DS_Store"))
                            continue;

                        Debug.Log("Unpacking zip file entry: " + entryFileName);

                        byte[] buffer = new byte[4096];     // 4K is optimum
                        Stream zipStream = zf.GetInputStream(zipEntry);

                        // Manipulate the output filename here as desired.
                        string fullZipToPath = Application.streamingAssetsPath +"/" +Path.GetFileName(entryFileName);

                        // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                        // of the file, but does not waste memory.
                        // The "using" will close the stream even if an exception occurs.
                        using (FileStream streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                        numFiles++;
                    }

                    zf.IsStreamOwner = false;
                    zf.Close();
                    fs.Close();
                }
            }
            catch
            {
                Debug.Log("Zip file error!");
            }
        }
    }
}
