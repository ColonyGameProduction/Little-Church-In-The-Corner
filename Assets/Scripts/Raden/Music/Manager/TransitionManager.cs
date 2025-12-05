using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    public List<GameObject> GO_Ruangan; // sementara roomnya image dulu yaks

    private Camera cam;

    public Transform TF_parent;

    public GameObject[] List_GO_roomObjects;
    public List<Vector3> List_V3_positions;

    public float F_moveAnimationDuration = 0.5f;
    private int I_moveAnimationID;

    private Coroutine COR_swipeUpdateCoroutine;

    /// <summary>
    /// Ini seberapa jauh minimal harus swipe sebelum bisa ganti ruangan
    /// </summary>
    public float F_swipeSensitivity = 0.5f;

    /// <summary>
    /// Ruangan saat ini
    /// </summary>
    public ENM_Room ENM_room;

    /// <summary>
    /// Ini buat tau apakah menu playlist lagi dibuka atau engga
    /// </summary>
    public MusicPanelSwipe SCR_musicPanelSwipe;

    private void Awake()
    {
        Instance = this;
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


        for (int I_roomIndex = 1; I_roomIndex < List_GO_roomObjects.Length; I_roomIndex++)
        {
            //Tambah offset sebelumnya biar centered
            currentPos += centerOffset;

            List_GO_roomObjects[I_roomIndex].transform.position = Vector3.zero;
            centerOffset = new Vector3(GetBounds(List_GO_roomObjects[I_roomIndex]).center.x, 0, 0);
            //Debug.Log($"Center offset = {centerOffset}");

            Vector3 currentSize = new Vector3(GetBounds(List_GO_roomObjects[I_roomIndex]).extents.x, 0, 0);
            Vector3 prevSize = new Vector3(GetBounds(List_GO_roomObjects[I_roomIndex - 1]).extents.x / 2, 0, 0);

            currentPos += distanceFromCenter + centerOffset + currentSize + prevSize;

            Vector3 newPos = new Vector3(currentPos.x, 0f, 0f);

            List_V3_positions.Add(newPos * -1f);
            //Debug.Log(newPos);

            List_GO_roomObjects[I_roomIndex].transform.position = newPos;
        }
        //Debug.Log("done");
    }



    //Ini ditrigger di component Player Input
    private void OnSwipeStart()
    {
        //Kalau lagi dibuka menu playlist lagu, jangan bolehin swipe
        if (SCR_musicPanelSwipe.ENM_currentState == ENM_PanelState.Playlist) return;
        //Kalau lagi dragging music panel, jangan bolehin swipe ruangan
        if (SCR_musicPanelSwipe.B_isDragging) return;

        //Kalau lagi animasi pindah ruangan, animasi swipenya diberhentiin
        if (LeanTween.isTweening(I_moveAnimationID)) LeanTween.cancel(I_moveAnimationID);

        if (cam == null) cam = Camera.main;

        if (COR_swipeUpdateCoroutine != null) StopCoroutine(COR_swipeUpdateCoroutine);
        COR_swipeUpdateCoroutine = StartCoroutine(SwipeUpdate());
    }

    private IEnumerator SwipeUpdate()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Vector3 V3_originalMousePos = cam.ScreenToWorldPoint(Mouse.current.position.value);
#elif UNITY_ANDROID
        Vector3 V3_originalMousePos = cam.ScreenToWorldPoint(Touchscreen.current.position.value);
#endif
        Vector3 V3_originalParentPos = TF_parent.position;
        Vector2 V2_minMaxPosition = new Vector2(List_V3_positions[0].x, List_V3_positions[List_V3_positions.Count - 1].x);
        while (true)
        {
            //Kalau lagi dragging music panel, jangan bolehin swipe ruangan
            if (SCR_musicPanelSwipe.B_isDragging) yield break;

            //Debug.Log($"Mouse position = {Mouse.current.position.value}");
            //Debug.Log($"Mouse position world = {cam.ScreenToWorldPoint(Mouse.current.position.value)}");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            Vector3 displacement = V3_originalParentPos + (cam.ScreenToWorldPoint(Mouse.current.position.value) - V3_originalMousePos);
#elif UNITY_ANDROID
            Vector3 displacement = V3_originalParentPos + (cam.ScreenToWorldPoint(Touchscreen.current.position.value) - V3_originalMousePos);
#endif
            //minmaxposition.y itu nilai terkecil, makanya dia duluan. Yeah, "terkecil" karena angkanya negatif.
            TF_parent.position = new Vector3(Mathf.Clamp(displacement.x, V2_minMaxPosition.y, V2_minMaxPosition.x), TF_parent.position.y, TF_parent.position.z);
            //TF_parent.position = new Vector3(displacement.x, TF_parent.position.y, TF_parent.position.z);
            yield return null;
        }
    }

    private void OnSwipeRelease()
    {
        //Kalau lagi dibuka menu playlist lagu, jangan bolehin swipe
        if (SCR_musicPanelSwipe.ENM_currentState == ENM_PanelState.Playlist) return;
        //Kalau lagi dragging music panel, jangan bolehin swipe ruangan
        if (SCR_musicPanelSwipe.B_isDragging) return;

        //Kalau lagi animasi pindah ruangan, animasi swipenya diberhentiin
        if (LeanTween.isTweening(I_moveAnimationID)) LeanTween.cancel(I_moveAnimationID);

        if (COR_swipeUpdateCoroutine != null) StopCoroutine(COR_swipeUpdateCoroutine);

        int I_nextRoomIndex = (int)ENM_room;

        //Kalau misalnya udah cukup jauh swipenya, maka boleh pindah ke ruangan lain
        if (Mathf.Abs(List_V3_positions[(int)ENM_room].x - TF_parent.position.x) > F_swipeSensitivity)
        {
            float F_smallestDifference = float.PositiveInfinity;

            for (int I_positionIndex = 0; I_positionIndex < List_V3_positions.Count; I_positionIndex++)
            {
                // Kalau I_roomIndex sama dengan ruangan saat ini, skip
                if ((int)ENM_room == I_positionIndex) continue;

                //Debug.Log($"{I_roomIndex} Smallest difference {F_smallestDifference} vs difference {Mathf.Abs(List_V3_positions[I_roomIndex].x - TF_parent.position.x)}");
                if (F_smallestDifference > Mathf.Abs(List_V3_positions[I_positionIndex].x - TF_parent.position.x))
                {
                    I_nextRoomIndex = I_positionIndex;
                    F_smallestDifference = Mathf.Abs(List_V3_positions[I_positionIndex].x - TF_parent.position.x);
                }
            }
        }

        ENM_room = (ENM_Room)I_nextRoomIndex;

        GoToPosition();
    }

    //https://discussions.unity.com/t/getting-the-bounds-of-the-group-of-objects/431270/13
    public Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        bool B_firstRendererFound = false;
        if (renderers.Length > 0)
        {
            //Encapsulate for all renderers
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    //Find first enabled renderer to start encapsulate from it
                    if (!B_firstRendererFound)
                    {
                        B_firstRendererFound = true;
                        bounds = renderer.bounds;
                    }

                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return bounds;
    }

    // Ini pindahin ke TransitionManager, ke Transition()
    public void GoToPosition()
    {
        if (LeanTween.isTweening(I_moveAnimationID)) LeanTween.cancel(I_moveAnimationID);

        I_moveAnimationID = LeanTween
            .move(
                TF_parent.gameObject,
                new Vector3(
                    List_V3_positions[(int)ENM_room].x,
                    TF_parent.position.y,
                    TF_parent.position.z),
                F_moveAnimationDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .id;

        UIHUDManager.Instance.HighlightRoomButtonTransition(ENM_room);
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
