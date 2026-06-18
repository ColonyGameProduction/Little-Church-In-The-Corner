using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuSwipe : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    private Vector2 screenRatio;

    public RectTransform menuParent;
    public RectTransform[] menuObjectsList;
    private int currentMenuIndex;

    private Camera cam;
    public float moveAnimationDuration = 0.5f;
    private int moveAnimationID;
    private Coroutine swipeUpdateCoroutine;

    public float swipeSensitivity = 0.5f;



    [SerializeField] private float selectedPageSize = 40f;
    [SerializeField] private float unselectedPageSize = 25f;
    [SerializeField] private Transform pageIndicatorParent;
    [SerializeField] private GameObject pageIndicatorPrefab;
    private List<RectTransform> pageIndicators;

    private void Awake()
    {
        cam = Camera.main;
        screenRatio = new Vector2(Screen.width, Screen.height) / canvasScaler.referenceResolution;
        SetupMenuPositions();
        currentMenuIndex = 0;
    }

    private void SetupMenuPositions()
    {
        // Asumsikan ukuran setiap menu itu sama seperti ukuran layar
        Vector2 anchoredPos = new Vector2(canvasScaler.referenceResolution.x, 0f);
        int index = 0;

        pageIndicators = new List<RectTransform>();

        foreach (RectTransform menu in menuObjectsList)
        {
            menu.anchoredPosition = anchoredPos * index;
            pageIndicators.Add((RectTransform)Instantiate(pageIndicatorPrefab, pageIndicatorParent).transform);
            pageIndicators[index].sizeDelta = Vector2.one * unselectedPageSize;

            index++;
        }

        if (pageIndicators.Count > 0) pageIndicators[0].sizeDelta = Vector2.one * selectedPageSize;
    }

    private void OnSwipeStart()
    {
        //Kalau lagi animasi pindah menu, animasi swipenya diberhentiin
        if (LeanTween.isTweening(moveAnimationID)) LeanTween.cancel(moveAnimationID);

        if (swipeUpdateCoroutine != null) StopCoroutine(swipeUpdateCoroutine);
        swipeUpdateCoroutine = StartCoroutine(SwipeUpdate());
    }

    private IEnumerator SwipeUpdate()
    {
        if (cam == null) cam = Camera.main;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Vector2 originalMousePos = Mouse.current.position.value / screenRatio;
#elif UNITY_ANDROID
        Vector2 originalMousePos = Touchscreen.current.position.value / screenRatio;
#endif
        Vector2 originalParentPos = menuParent.anchoredPosition;
        // * -1f karena kalau swiping, harus kebalik dengan posisi sebenarnya
        Vector2 minMaxPosition = new Vector2(menuObjectsList[0].anchoredPosition.x * -1f, menuObjectsList[menuObjectsList.Length - 1].anchoredPosition.x * -1f);
        while (true)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            Vector2 displacement = originalParentPos + ((Mouse.current.position.value / screenRatio) - originalMousePos);
#elif UNITY_ANDROID
            Vector2 displacement = originalParentPos + (Touchscreen.current.position.value - originalMousePos);
#endif
            //Debug.Log("ZERO=============ZERO");
            ////Debug.Log(originalParentPos);
            ////Debug.Log(Mouse.current.position.value);
            ////Debug.Log(originalMousePos);
            //Debug.Log("ONE=============ONE");
            //Debug.Log(displacement.x);
            ////Debug.Log(minMaxPosition.x);
            ////Debug.Log(minMaxPosition.y);
            //Debug.Log("TWO=============TWO");
            menuParent.anchoredPosition = new Vector2(Mathf.Clamp(displacement.x, minMaxPosition.y, minMaxPosition.x), menuParent.anchoredPosition.y);
            yield return null;
        }
    }

    private void OnSwipeRelease()
    {
        //Kalau lagi animasi pindah menu, animasi swipenya diberhentiin
        if (LeanTween.isTweening(moveAnimationID)) LeanTween.cancel(moveAnimationID);

        if (swipeUpdateCoroutine != null) StopCoroutine(swipeUpdateCoroutine);

        int nextMenuIndex = currentMenuIndex;

        float sensitivity = canvasScaler.referenceResolution.x * swipeSensitivity;

        //Debug.Log($"Sensitivity check: {menuObjectsList[currentMenuIndex].anchoredPosition.x - menuParent.anchoredPosition.x} vs {sensitivity}");
        //Debug.Log($"More details: {menuObjectsList[currentMenuIndex].anchoredPosition.x} - {menuParent.anchoredPosition.x}");

        //Kalau misalnya udah cukup jauh swipenya, maka boleh pindah ke menu lain
        if (Mathf.Abs(Mathf.Abs(menuObjectsList[currentMenuIndex].anchoredPosition.x) - Mathf.Abs(menuParent.anchoredPosition.x)) > sensitivity)
        {
            //Debug.Log("Pindah ke menu lain");
            float smallestDifference = float.PositiveInfinity;

            for (int positionIndex = 0; positionIndex < menuObjectsList.Length; positionIndex++)
            {
                // Kalau positionIndex sama dengan menu saat ini, skip
                //Debug.Log($"{currentMenuIndex} == {positionIndex}?");
                if (currentMenuIndex == positionIndex) continue;

                //Debug.Log($"Smallest Difference check: {menuObjectsList[positionIndex].anchoredPosition.x - menuParent.anchoredPosition.x} vs {smallestDifference}");
                //Debug.Log($"More details: {menuObjectsList[positionIndex].anchoredPosition.x} - {menuParent.anchoredPosition.x}");
                if (smallestDifference > Mathf.Abs(Mathf.Abs(menuObjectsList[positionIndex].anchoredPosition.x) - Mathf.Abs(menuParent.anchoredPosition.x)))
                {
                    nextMenuIndex = positionIndex;
                    smallestDifference = Mathf.Abs(Mathf.Abs(menuObjectsList[positionIndex].anchoredPosition.x) - Mathf.Abs(menuParent.anchoredPosition.x));
                    //Debug.Log($"Smallest difference changed! next menu is {nextMenuIndex}");
                }
            }
        }

        currentMenuIndex = nextMenuIndex;

        GoToPosition();
    }

    private void GoToPosition()
    {
        if (LeanTween.isTweening(moveAnimationID)) LeanTween.cancel(moveAnimationID);

        moveAnimationID = LeanTween
            .move(
                menuParent,
                new Vector2(
                    menuObjectsList[currentMenuIndex].anchoredPosition.x * -1f,
                    menuParent.anchoredPosition.y),
                moveAnimationDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .id;

        LeanTween.size(pageIndicators[currentMenuIndex], Vector2.one * selectedPageSize, moveAnimationDuration);

        for (int i = 0; i < pageIndicators.Count; i++)
        {
            if (i == currentMenuIndex) continue;

            LeanTween.size(pageIndicators[i], Vector2.one * unselectedPageSize, moveAnimationDuration);
        }
    }
}
