using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//TODO: JANGAN LUPA HAPUS INI
[ExecuteAlways]
public class RoomPositionManager : MonoBehaviour
{
    public static RoomPositionManager Instance { get; private set; }

    private Camera cam;

    public Transform TF_parent;

    public GameObject[] List_GO_roomObjects;
    public List<Vector3> List_V3_positions;
    public int I_roomIndex;

    public float F_moveAnimationDuration = 0.5f;
    private int I_moveAnimationID;

    private Coroutine COR_swipeUpdateCoroutine;

    /// <summary>
    /// Ini seberapa jauh minimal harus swipe sebelum bisa ganti ruangan
    /// </summary>
    public float F_swipeSensitivity = 0.5f;

    // Ini bagian untuk tombol, paling pindahin ke UIHUDManager
    public RectTransform RT_highlightRect;
    public List<Image> List_IMG_roomButtonActiveIcons;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetupRoomPositions();
    }


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
        List_V3_positions.Add(currentPos * -1f);

        //Debug.Log($"Center offset = {centerOffset}");
        //Debug.Log(currentPos);


        for (int i = 1; i < List_GO_roomObjects.Length; i++)
        {
            //Tambah offset sebelumnya biar centered
            currentPos += centerOffset;

            List_GO_roomObjects[i].transform.position = Vector3.zero;
            centerOffset = new Vector3(GetBounds(List_GO_roomObjects[i]).center.x, 0, 0);
            //Debug.Log($"Center offset = {centerOffset}");

            Vector3 currentSize = new Vector3(GetBounds(List_GO_roomObjects[i]).extents.x, 0, 0);
            Vector3 prevSize = new Vector3(GetBounds(List_GO_roomObjects[i-1]).extents.x / 2, 0, 0);

            currentPos += distanceFromCenter + centerOffset + currentSize + prevSize;

            Vector3 newPos = new Vector3(currentPos.x, 0f, 0f);

            List_V3_positions.Add(newPos * -1f);
            //Debug.Log(newPos);

            List_GO_roomObjects[i].transform.position = newPos;
        }
        //Debug.Log("done");
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

    // Ini pindahin ke TransitionManager, ke Transition()
    public void GoToPosition()
    {
        I_moveAnimationID = LeanTween
            .move(
                TF_parent.gameObject,
                new Vector3(
                    List_V3_positions[I_roomIndex].x,
                    TF_parent.position.y,
                    TF_parent.position.z),
                F_moveAnimationDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .id;

        LeanTween
            .move(
                RT_highlightRect,
                new Vector2(RT_highlightRect.sizeDelta.x * I_roomIndex, 0f),
                F_moveAnimationDuration
            )
            .setEase(LeanTweenType.easeOutCubic);

        for (int i = 0; i < List_IMG_roomButtonActiveIcons.Count; i++)
        {
            float F_alpha = 0f;
            if (i == I_roomIndex) F_alpha = 1f;
            LeanTween
                .alpha(
                List_IMG_roomButtonActiveIcons[i].rectTransform,
                F_alpha,
                F_moveAnimationDuration
                )
                .setEase(LeanTweenType.easeOutCubic);
        }
    }

    //Ini ditrigger di component Player Input
    private void OnSwipeStart()
    {
        //Kalau lagi animasi pindah ruangan, animasi swipenya diberhentiin
        if (LeanTween.isTweening(I_moveAnimationID)) LeanTween.cancel(I_moveAnimationID);

        if (cam == null) cam = Camera.main;

        if (COR_swipeUpdateCoroutine != null) StopCoroutine(COR_swipeUpdateCoroutine);
        COR_swipeUpdateCoroutine = StartCoroutine(SwipeUpdate());
    }

    private IEnumerator SwipeUpdate()
    {
        Vector3 V3_originalMousePos = cam.ScreenToWorldPoint(Mouse.current.position.value);
        Vector3 V3_originalParentPos = TF_parent.position;
        Vector2 V2_minMaxPosition = new Vector2(List_V3_positions[0].x, List_V3_positions[List_V3_positions.Count - 1].x);
        while (true)
        {
            //Debug.Log($"Mouse position = {Mouse.current.position.value}");
            //Debug.Log($"Mouse position world = {cam.ScreenToWorldPoint(Mouse.current.position.value)}");
            Vector3 displacement = V3_originalParentPos + (cam.ScreenToWorldPoint(Mouse.current.position.value) - V3_originalMousePos);
            //minmaxposition.y itu nilai terkecil, makanya dia duluan. Yeah, "terkecil" karena angkanya negatif.
            TF_parent.position = new Vector3(Mathf.Clamp(displacement.x, V2_minMaxPosition.y, V2_minMaxPosition.x), TF_parent.position.y, TF_parent.position.z);
            //TF_parent.position = new Vector3(displacement.x, TF_parent.position.y, TF_parent.position.z);
            yield return null;
        }
    }

    private void OnSwipeRelease()
    {
        //Kalau lagi animasi pindah ruangan, animasi swipenya diberhentiin
        if (LeanTween.isTweening(I_moveAnimationID)) LeanTween.cancel(I_moveAnimationID);

        if (COR_swipeUpdateCoroutine != null) StopCoroutine(COR_swipeUpdateCoroutine);

        int I_nextRoomIndex = I_roomIndex;

        //Kalau misalnya udah cukup jauh swipenya, maka boleh pindah ke ruangan lain
        if (Mathf.Abs(List_V3_positions[I_roomIndex].x - TF_parent.position.x) > F_swipeSensitivity)
        {
            float smallestDifference = float.PositiveInfinity;

            for (int i = 0; i < List_V3_positions.Count; i++)
            {
                // Kalau i sama dengan ruangan saat ini, skip
                if (I_roomIndex == i) continue;

                //Debug.Log($"{i} Smallest difference {smallestDifference} vs difference {Mathf.Abs(List_V3_positions[i].x - TF_parent.position.x)}");
                if (smallestDifference > Mathf.Abs(List_V3_positions[i].x - TF_parent.position.x))
                {
                    I_nextRoomIndex = i;
                    smallestDifference = Mathf.Abs(List_V3_positions[i].x - TF_parent.position.x);
                }
            }
        }

        I_roomIndex = I_nextRoomIndex;

        GoToPosition();
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
