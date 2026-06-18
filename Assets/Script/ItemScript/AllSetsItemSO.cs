using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllSetsItemSO", menuName = "Scriptable Objects/AllSetsItemSO")]
public class AllSetsItemSO : ScriptableObject
{
    public List<setItemSO> items;
}