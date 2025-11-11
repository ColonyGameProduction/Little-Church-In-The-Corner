using System.Collections;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class RoomPosition : MonoBehaviour
{
    private Camera cam;

    //public Vector3 V3_referenceAnchor;
    public Vector3 V3_centerPointAnchor;
    public GameObject GO_roomObject;
    public int I_roomNumber;

    //public TextMeshProUGUI viewportToWorldpointText;
    //public TextMeshProUGUI boundsBeforeText;
    //public TextMeshProUGUI boundsAfterText;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        if (cam == null) cam = Camera.main;
        SetupViewportPointUsingAnchor();
    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        if (cam == null) cam = Camera.main;
        SetupViewportPointUsingAnchor();
    }

    //private void OnRenderObject()
    //{
    //    if (cam == null) cam = Camera.main;
    //    SetupViewportPointUsingAnchor();
    //}
#endif

    private void SetupViewportPointUsingAnchor()
    {
        GO_roomObject.transform.position = Vector3.zero;
        //if (boundsBeforeText) boundsBeforeText.text = $"Bounds before = {GetBounds(GO_roomObject)}";
        Vector3 centerOffset = new Vector3(GetBounds(GO_roomObject).center.x, 0, 0);

        Vector3 distanceFromCenter = (cam.ViewportToWorldPoint(V3_centerPointAnchor) - cam.ViewportToWorldPoint(Vector3.zero));
        distanceFromCenter = new Vector3(distanceFromCenter.x, 0f, 0f);

        Vector3 sizeOffset = new Vector3(GetBounds(GO_roomObject).extents.x * I_roomNumber, 0, 0);

        GO_roomObject.transform.position = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.transform.position.z)) - centerOffset + (I_roomNumber * distanceFromCenter) - sizeOffset;



        //sizeOffset = new Vector3(GetBounds(GO_roomObject).extents.x * 0, 0, 0);
        //Vector3 distanceOne = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.transform.position.z)) - centerOffset + (0 * distanceFromCenter) - sizeOffset;
        //sizeOffset = new Vector3(GetBounds(GO_roomObject).extents.x * 1, 0, 0);
        //Vector3 distanceTwo = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.transform.position.z)) - centerOffset + (1 * distanceFromCenter) - sizeOffset;
        //Vector3 finalDistance = distanceTwo - distanceOne;
        //Vector3 finalDistance = distanceFromCenter - new Vector3(GetBounds(GO_roomObject).extents.x, 0, 0);
        //Debug.Log($"Final distance = {finalDistance}");

        //Ini bakal bikin supaya objectnya ada di luar camera setiap saat, ga peduli screen resolutionnya berapa
        //V3_referenceAnchor, itu cara kerjanya mirip anchor di UI RectTransform.
        //Vector3,
        //x menandakan titik horizontal, 0 itu paling kiri layar, 1 itu paling kanan.
        //y menandakan titik vertikal, 0 itu paling bawah layar, 1 itu paling atas
        //z menandakan jauh benda dari kamera.
        //jadi, kalau misalnya mau pastiin kalau objectnya ada di paling kiri setiap saat, set V3_referenceAnchor jadi Vector3(0, Y, Z). Kalau paling kanan, Vector3(1, Y, Z)

        //Ada new Vector3(objectRenderer.bounds.extents.x, 0, 0), itu sebagai offset yang menyesuaikan dengan ukuran object. bounds.extents itu panjang lebar dari object kalau dilihat dari kamera. Aku tambahin gini biar objectnya selalu berada di luar kamera, karena dia menggeser lokasi object sebanyak panjang (bounds.extents.x) dari object itu sendiri.
        //Also, ada - centerOffset karena terkadang objectnya ga balanced, kayak panjang sebelah. Ini supaya perhitungannya bener aja.
        //GO_roomObject.transform.position = cam.ViewportToWorldPoint(V3_referenceAnchor) + new Vector3(GetBounds(GO_roomObject).extents.x, 0, 0) - centerOffset;

        //if (viewportToWorldpointText) viewportToWorldpointText.text = $"Viewport to worldpoint = {cam.ViewportToWorldPoint(V3_referenceAnchor)}";
        //if (boundsAfterText) boundsAfterText.text = $"Bounds after = {GetBounds(GO_roomObject)}";

        //Debug.Log($"ViewportToWorldPoint: {cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)) - cam.ViewportToWorldPoint(Vector3.zero)}\nScreen width in half = {cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f))}");
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
