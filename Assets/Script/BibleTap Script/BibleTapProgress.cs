using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BibleTapProgress : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private RectTransform progressIndicator;
    [SerializeField] private RectTransform progressBarIndicatorContainer;
    [SerializeField] private float progressBarWidth;

    private void Awake()
    {
        StartCoroutine(GetProgressBarWidth());
    }

    private IEnumerator GetProgressBarWidth()
    {
        //DebugTest();
        ////LayoutRebuilder.ForceRebuildLayoutImmediate(progressBarIndicatorContainer);
        //Canvas.ForceUpdateCanvases();
        //DebugTest();
        yield return null;
        ////TODO: kalau anchor min max diset manual, maka dia bakal ngaco for some reason. Research more.

        //DebugTest();
        //////Canvas.ForceUpdateCanvases();
        ////DebugTest();
        ////progressBarIndicatorContainer.anchorMin = new Vector2(0.5f, 0.5f);
        ////progressBarIndicatorContainer.anchorMax = new Vector2(0.5f, 0.5f);
        ////DebugTest();
        //////Canvas.ForceUpdateCanvases();
        ////progressBarIndicatorContainer.anchorMin = Vector2.zero;
        ////progressBarIndicatorContainer.anchorMax = Vector2.one;
        ////Canvas.ForceUpdateCanvases();
        ////DebugTest();

        ////progressBarWidth = progressBarIndicatorContainer.
        progressBarWidth = progressBarIndicatorContainer.rect.width;
    }

    private void DebugTest()
    {
        Debug.Log("DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG");
        Debug.Log(progressBarIndicatorContainer.rect.size);
        Debug.Log($"anchorMin: {progressBarIndicatorContainer.anchorMin}");
        Debug.Log($"anchorMax: {progressBarIndicatorContainer.anchorMax}");
        Debug.Log($"offsetMin: {progressBarIndicatorContainer.offsetMin}");
        Debug.Log($"offsetMax: {progressBarIndicatorContainer.offsetMax}");
        Debug.Log($"sizeDelta: {progressBarIndicatorContainer.sizeDelta}");
        Debug.Log($"rect: {progressBarIndicatorContainer.rect}");
        Debug.Log("DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG");
    }

    private void OnEnable()
    {
        BibleTapManager.OnNewPage += UpdateProgressBar;
        BibleTapManager.OnGameSetupDone += Initialize;
    }

    private void OnDisable()
    {
        BibleTapManager.OnNewPage -= UpdateProgressBar;
        BibleTapManager.OnGameSetupDone -= Initialize;
    }

    private void Initialize(int maxPages)
    {
        progressBar.maxValue = maxPages;
        progressBar.value = 0;
        MoveIndicator();
    }

    private void UpdateProgressBar(int value)
    {
        progressBar.value = value;
        MoveIndicator();
    }

    private void MoveIndicator()
    {
        progressIndicator.anchoredPosition = new Vector2(progressBar.value / progressBar.maxValue * progressBarWidth, progressIndicator.anchoredPosition.y);
    }
}
