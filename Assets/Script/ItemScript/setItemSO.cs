using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "setItemSO", menuName = "Scriptable Objects/setItemSO")]
public class setItemSO : ScriptableObject
{
    public string setID;
    public string setName;
    public List<ItemSO> items = new List<ItemSO>();
}
