using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    public List<GameObject> GO_Ruangan; // sementara roomnya image dulu yaks

    private void Awake()
    {
        Instance = this;
    }

    public void Transition(ENM_Room targetRoom)
    {
        foreach(var room in GO_Ruangan)
        {
            room.SetActive(false);
        }

        int I_index = (int)targetRoom;

        if (I_index >= 0 && I_index < GO_Ruangan.Count)
        {
            GO_Ruangan[I_index].SetActive(true);
        }
    }
}
