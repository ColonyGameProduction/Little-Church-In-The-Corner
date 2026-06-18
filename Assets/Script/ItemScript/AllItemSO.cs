using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllItemSO", menuName = "Scriptable Objects/AllItemSO")]
public class AllItemSO : ScriptableObject
{
    public List<ItemSO> items;
}


