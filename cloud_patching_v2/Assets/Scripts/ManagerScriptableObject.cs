using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Storage", menuName = "Database/Storage", order = 1)]
public class ManagerScriptableObject : ScriptableObject
{
    public List<string> items = new List<string>(22);
}
