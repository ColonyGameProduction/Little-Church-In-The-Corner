using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicPanelSwipe : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("referensi")]
    public RectTransform RT_musicPanelRoot;
    public GameObject GO_miniMusicPanel;
    public GameObject GO_fullMusicPanel;
    public GameObject GO_playlistMusicPanel;
    public CanvasGroup CG_miniCanvasGroup;
    public CanvasGroup CG_fullCanvasGroup;
    public CanvasGroup CG_playlistCanvasGroup;
    // Ini buat ngambil Reference Resolution biar mouse position akurat dan sesuai dengan ukuran UI sebenarnya, soalnya eventPosition tergantung ukuran layar, dan ukurannya belum tentu sama dengan ukuran reference resolution di canvas UI. -Robert
    public CanvasScaler UICanvasScaler;

    [Header("Y position setiap panel")]
    public float F_miniY = -940f;
    public float F_fullY = -820f;
    public float F_playlistY = 0f;
    public float F_snapThreshold = 80f;
    public float F_animationDuration = 0.25f;

    private Vector2 V_dragStartPos;
    // Ini biar dia ngedrag dari posisi awal musik panel, bukan di posisi sebenarnya (karena kalau posisi sebenarnya, dia kan selalu update pas dragging, jadi bakal terus menerus nambah posisinya. -Robert
    private Vector2 V_musicPanelRootStartPos;
    public bool B_isDragging = false;
    public ENM_PanelState ENM_currentState = ENM_PanelState.Mini;

    void Start()
    {
        SetPanelPosition(F_miniY, instant: true);
        SetActivePanel(ENM_PanelState.Mini, instant: true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!B_isDragging)
        {
            // Cara kerja ini adalah dia bakal kaliin reference resolution dari canvas scaler UI (bagian tingginya) dengan posisi mouse saat ini, lalu dibagi dengan tinggi layar sebenarnya. Ini biar posisinya akurat dan nyesuaiin dengan reference resolution UI canvas dan bukan sesuaiin dengan ukuran layar -Robert
            V_dragStartPos = UICanvasScaler.referenceResolution.y * eventData.position / Screen.height;
            V_musicPanelRootStartPos = RT_musicPanelRoot.anchoredPosition;
            B_isDragging = true;
        }

        // Ini sama kayak di atas, tapi dia langsung ambil tingginya, lalu dikurangin dengan titik awal drag
        float F_dragDeltaY = (UICanvasScaler.referenceResolution.y * eventData.position / Screen.height).y - V_dragStartPos.y;
        float F_newY = Mathf.Clamp(V_musicPanelRootStartPos.y + F_dragDeltaY, F_miniY, F_playlistY);

        SetPanelPosition(F_newY, instant: true);
        // Ini kayaknya ga perlu lagi -Robert
        //V_dragStartPos = eventData.position;

        // fade realtime selama drag
        UpdateFade(F_newY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        B_isDragging = false;
        float F_currentY = RT_musicPanelRoot.anchoredPosition.y;

        // tentuin state terdekat
        float F_distMini = Mathf.Abs(F_currentY - F_miniY);
        float F_distFull = Mathf.Abs(F_currentY - F_fullY);
        float F_distPlaylist = Mathf.Abs(F_currentY - F_playlistY);

        ENM_PanelState targetState;

        if (F_distMini < F_distFull && F_distMini < F_distPlaylist)
        {
            targetState = ENM_PanelState.Mini;
        }
        else if (F_distFull < F_distPlaylist)
        {
            targetState = ENM_PanelState.Full;
        }
        else
        {
            targetState = ENM_PanelState.Playlist;
        }

        SnapToState(targetState);
    }

    private void SetPanelPosition(float y, bool instant = false)
    {
        if (instant)
        {
            var pos = RT_musicPanelRoot.anchoredPosition;
            pos.y = y;
            RT_musicPanelRoot.anchoredPosition = pos;
        }
        else
        {
            LeanTween.cancel(RT_musicPanelRoot.gameObject);
            LeanTween.moveY(RT_musicPanelRoot, y, F_animationDuration).setEaseOutCubic().setOnUpdate(UpdateFade); // biar fadenya ikut halus waktu nge-snap
        }
    }

    private void SnapToState(ENM_PanelState target)
    {
        ENM_currentState = target;
        float F_targetY = F_miniY;

        switch (target)
        {
            case ENM_PanelState.Mini: F_targetY = F_miniY; break;
            case ENM_PanelState.Full: F_targetY = F_fullY; break;
            case ENM_PanelState.Playlist: F_targetY = F_playlistY; break;
        }

        SetPanelPosition(F_targetY);

        AnimateFade(
            miniIn: target == ENM_PanelState.Mini,
            fullIn: target == ENM_PanelState.Full,
            playlistIn: target == ENM_PanelState.Playlist
        );
    }

    private void SetActivePanel(ENM_PanelState state, bool instant = false)
    {
        GO_miniMusicPanel.SetActive(true);
        GO_fullMusicPanel.SetActive(true);
        GO_playlistMusicPanel.SetActive(true);

        float F_miniA = (state == ENM_PanelState.Mini) ? 1f : 0f;
        float F_fullA = (state == ENM_PanelState.Full) ? 1f : 0f;
        float F_playlistA = (state == ENM_PanelState.Playlist) ? 1f : 0f;

        if (instant)
        {
            CG_miniCanvasGroup.alpha = F_miniA;
            CG_fullCanvasGroup.alpha = F_fullA;
            CG_playlistCanvasGroup.alpha = F_playlistA;

            CG_miniCanvasGroup.blocksRaycasts = (state == ENM_PanelState.Mini);
            CG_fullCanvasGroup.blocksRaycasts = (state == ENM_PanelState.Full);
            CG_playlistCanvasGroup.blocksRaycasts = (state == ENM_PanelState.Playlist);
        }
        else
        {
            AnimateFade(
                miniIn: state == ENM_PanelState.Mini,
                fullIn: state == ENM_PanelState.Full,
                playlistIn: state == ENM_PanelState.Playlist
            );
        }
    }

    // fade realtime berdasarkan posisi panel
    private void UpdateFade(float y)
    {
        float F_tFull = Mathf.InverseLerp(F_miniY, F_fullY, y);
        float F_tPlaylist = Mathf.InverseLerp(F_fullY, F_playlistY, y);

        // mini -> full
        CG_miniCanvasGroup.alpha = Mathf.Lerp(1f, 0f, F_tFull);
        CG_fullCanvasGroup.alpha = Mathf.Lerp(0f, 1f, F_tFull);

        // full -> playlist
        if (y > F_fullY)
        {
            CG_fullCanvasGroup.alpha = Mathf.Lerp(1f, 0f, F_tPlaylist);
            CG_playlistCanvasGroup.alpha = Mathf.Lerp(0f, 1f, F_tPlaylist);
        }
        else
        {
            CG_playlistCanvasGroup.alpha = 0f;
        }
    }

    private void AnimateFade(bool miniIn, bool fullIn, bool playlistIn)
    {
        AnimateCanvas(CG_miniCanvasGroup, miniIn);
        AnimateCanvas(CG_fullCanvasGroup, fullIn);
        AnimateCanvas(CG_playlistCanvasGroup, playlistIn);

        CG_miniCanvasGroup.blocksRaycasts = miniIn;
        CG_fullCanvasGroup.blocksRaycasts = fullIn;
        CG_playlistCanvasGroup.blocksRaycasts = playlistIn;
    }


    private void AnimateCanvas(CanvasGroup group, bool fadeIn)
    {
        LeanTween.cancel(group.gameObject);
        float target = fadeIn ? 1f : 0f;

        LeanTween.value(
            group.gameObject,
            group.alpha,
            target,
            F_animationDuration
        )
        .setOnUpdate((float val) => group.alpha = val)
        .setEaseOutCubic();
    }
}
