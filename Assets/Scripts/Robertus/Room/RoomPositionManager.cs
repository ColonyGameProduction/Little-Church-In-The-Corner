using System.Collections.Generic;
using UnityEngine;

//TODO: JANGAN LUPA HAPUS INI
[ExecuteAlways]
public class RoomPositionManager : MonoBehaviour
{
    private Camera cam;

    public Transform TF_parent;

    public GameObject[] List_GO_roomObjects;
    public List<Vector3> List_V3_positions;
    private int I_roomIndex;

    //private void Start()
    //{
    //    SetupRoomPositions();
    //}

#if UNITY_EDITOR
    private void OnEnable()
    {
        SetupRoomPositions();
    }
#endif

    private void SetupRoomPositions()
    {
        if (cam == null) cam = Camera.main;

        List_V3_positions = new List<Vector3>();

        //Dapatkan jarak dari tengah layar ke pinggir layar
        Vector3 distanceFromCenter = (cam.ViewportToWorldPoint(Vector3.one * 0.5f) - cam.ViewportToWorldPoint(Vector3.zero));
        distanceFromCenter = new Vector3(distanceFromCenter.x, 0f, 0f);

        //Pertama-tama, asumsikan ruangan pertama itu di paling kiri, yang aktif sekarang (artinya ada di tengah-tengah layar), dan yang lain ada di kanannya.

        List_GO_roomObjects[0].transform.position = Vector3.zero;
        Vector3 centerOffset = new Vector3(GetBounds(List_GO_roomObjects[0]).center.x, 0, 0);

        //List_GO_roomObjects[0].transform.position = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f)) - centerOffset;

        Vector3 currentPos = List_GO_roomObjects[0].transform.position;
        List_V3_positions.Add(currentPos);

        Debug.Log($"Center offset = {centerOffset}");
        Debug.Log(currentPos);


        for (int i = 1; i < List_GO_roomObjects.Length; i++)
        {
            //Tambah offset sebelumnya biar centered
            currentPos += centerOffset;

            List_GO_roomObjects[i].transform.position = Vector3.zero;
            centerOffset = new Vector3(GetBounds(List_GO_roomObjects[i]).center.x, 0, 0);
            Debug.Log($"Center offset = {centerOffset}");

            Vector3 currentSize = new Vector3(GetBounds(List_GO_roomObjects[i]).extents.x, 0, 0);
            Vector3 prevSize = new Vector3(GetBounds(List_GO_roomObjects[i-1]).extents.x / 2, 0, 0);

            currentPos += distanceFromCenter - centerOffset - currentSize - prevSize;

            Vector3 newPos = new Vector3(currentPos.x, 0f, 0f);

            List_V3_positions.Add(newPos);
            Debug.Log(newPos);

            List_GO_roomObjects[i].transform.position = newPos;
        }
        Debug.Log("done");
    }

    public void GoToNext()
    {
        I_roomIndex = Mathf.Clamp(I_roomIndex + 1, 0, List_V3_positions.Count - 1);
        GoToPosition();
    }

    public void GoToPrev()
    {
        I_roomIndex = Mathf.Clamp(I_roomIndex - 1, 0, List_V3_positions.Count - 1);
        GoToPosition();
    }

    private void GoToPosition()
    {
        TF_parent.position = new Vector3(List_V3_positions[I_roomIndex].x * -1f, TF_parent.position.y, TF_parent.position.z);
    }

    //https://discussions.unity.com/t/getting-the-bounds-of-the-group-of-objects/431270/13
    public Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        bool firstRendererFound = false;
        if (renderers.Length > 0)
        {
            //Encapsulate for all renderers
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    //Find first enabled renderer to start encapsulate from it
                    if (!firstRendererFound)
                    {
                        firstRendererFound = true;
                        bounds = renderer.bounds;
                    }

                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return bounds;
    }
}
