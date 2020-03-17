using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    Manager patchManager;
    ZipMaster zipManager;

    public List<Button> buttons = new List<Button>();

    private void Awake()
    {
        patchManager = GetComponent<Manager>();
        zipManager = GetComponent<ZipMaster>();

        buttons[0].onClick.AddListener(zipManager.UnzipFile);
        buttons[1].onClick.AddListener(zipManager.ValidateStreamAssets);
        buttons[2].onClick.AddListener(patchManager.LoadGameAssets);
        buttons[3].onClick.AddListener(patchManager.InstantiatePatchedAsset);
    }
}
