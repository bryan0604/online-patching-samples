using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField]
    GameObject Content;
    [SerializeField]
    Text downloadSizeText;
    [SerializeField]
    Button ButtonAgree;
    [SerializeField]
    Button ButtonDisagree;
    string tempStorage;

    public void Initialise(System.Action agreedAction, System.Action disagreedAction)
    {
        ButtonAgree.onClick.AddListener(()=>Agreed(agreedAction));
        ButtonDisagree.onClick.AddListener(()=>Disagreed(disagreedAction));

        PopUpContent();
    }

    public void PopUpContent()
    {
        Content.SetActive(true);
    }

    public void PopulateContent(string contents)
    {
        tempStorage = contents;
        downloadSizeText.text = "0 / " + contents;
    }

    public void UpdateDynamicContent(string contents)
    {
        downloadSizeText.text = (float.Parse(tempStorage) / float.Parse(contents)) + " / " + tempStorage;
    }

    public void Agreed(System.Action cb)
    {
        cb.Invoke();
    }

    public void Disagreed(System.Action cb)
    {
        cb.Invoke();
    }

    public void RemoveAndClear()
    {
        Content.SetActive(false);
        tempStorage = null;
        ButtonAgree.onClick.RemoveAllListeners();
        ButtonDisagree.onClick.RemoveAllListeners();
    }
}
