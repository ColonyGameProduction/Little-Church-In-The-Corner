using UnityEngine;
using UnityEngine.EventSystems;

public class MusicPanelSwipe : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("referensi")]
    public RectTransform RT_musicPanelRootV2;
    public GameObject GO_miniMusicPanelv2;
    public GameObject GO_fullMusicPanelV2;
    public GameObject GO_playlistMusicPanelV2;

    public CanvasGroup CG_miniCanvasGroup;
    public CanvasGroup CG_fullCanvasGroup;
    public CanvasGroup CG_playlistCanvasGroup;

    [Header("y position buat setiap panel")]
    public float F_miniY = -540f;
    public float F_fullY = -200f;
    public float F_playlistY = 0f;
    public float F_swipeThreshold = 100f;

    public float F_fadeDuration = 0.25f;

    public bool B_MiniMusicPanelActive;

    private float F_currentTargetY;

    private void Start()
    {
        // mulai dari posisi mini
        F_currentTargetY = F_miniY;
        B_MiniMusicPanelActive = true;
        SetPanel(F_miniY);
        UpdatePanelVisibilityImmediate();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float F_deltaY = eventData.delta.y;

        // biar swipe up & down ke clamp di mini panel (dibawah) dan playlist panel (diatas)
        float F_newY = RT_musicPanelRootV2.anchoredPosition.y + F_deltaY;

        F_newY = Mathf.Clamp(F_newY, F_miniY, F_playlistY);

        RT_musicPanelRootV2.anchoredPosition = new Vector2(0, F_newY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float F_endY = RT_musicPanelRootV2.anchoredPosition.y;

        if (F_endY > F_currentTargetY + F_swipeThreshold)
        {
            // swipe ke atas
            if (F_currentTargetY == F_miniY)
            {
                F_currentTargetY = F_fullY;
            }
            else if (F_currentTargetY == F_fullY) 
            {
                F_currentTargetY = F_playlistY;
            }
        }
        else if (F_endY < F_currentTargetY - F_swipeThreshold)
        {
            // swipe ke bawah
            if (F_currentTargetY == F_playlistY)
            {
                F_currentTargetY = F_fullY;
            }
            else if (F_currentTargetY == F_fullY) 
            {
                F_currentTargetY = F_miniY;
            } 
        }

        // Smooth snap anjay
        LeanTween.moveY(RT_musicPanelRootV2, F_currentTargetY, 0.25f).setEaseOutCubic().setOnComplete(UpdatePanelVisibilitySmooth);
    }

    private void SetPanel(float y)
    {
        RT_musicPanelRootV2.anchoredPosition = new Vector2(0, y);
    }

    private void UpdatePanelVisibilitySmooth()
    {
        FadePanel(CG_miniCanvasGroup, F_currentTargetY == F_miniY);
        FadePanel(CG_fullCanvasGroup, F_currentTargetY == F_fullY);
        FadePanel(CG_playlistCanvasGroup, F_currentTargetY == F_playlistY);
    }

    private void UpdatePanelVisibilityImmediate()
    {
        CG_miniCanvasGroup.alpha = F_currentTargetY == F_miniY ? 1 : 0;
        CG_fullCanvasGroup.alpha = F_currentTargetY == F_fullY ? 1 : 0;
        CG_playlistCanvasGroup.alpha = F_currentTargetY == F_playlistY ? 1 : 0;

        CG_miniCanvasGroup.interactable = F_currentTargetY == F_miniY;
        CG_fullCanvasGroup.interactable = F_currentTargetY == F_fullY;
        CG_playlistCanvasGroup.interactable = F_currentTargetY == F_playlistY;
    }

    private void FadePanel(CanvasGroup cg, bool show)
    {
        LeanTween.alphaCanvas(cg, show ? 1f : 0f, F_fadeDuration);
        cg.interactable = show;
        cg.blocksRaycasts = show;
    }
}

