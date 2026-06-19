using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BibleTapManager : MonoBehaviour
{
    public static BibleTapManager instance;

    public PassageManager passageManager;

    public List<Verse> allVerses;
    public List<PageSet> pageSets;
    public List<string> words;
    public TextMeshProUGUI verseText;
    public TextMeshProUGUI verseTextHighlight;
    // https://discussions.unity.com/t/how-to-set-text-background-color/718556/7
    public string highlightMarkdown = "<mark=#FCFFB2 padding=“10, 10, 10, 10”>";
    public GameObject continueButton;

    public int wordsIdx;
    public int pageIdx;
    public int verseIdx;

    public InputSystem_Actions inputAction;
    public InputAction interactAction;

    [SerializeField] private GameObject introTutorial;
    public static event System.Action<int> OnNewPage;
    public static event System.Action<int> OnGameSetupDone;
    [SerializeField] private Image backgroundDarken;
    private const float DARKEN_ALPHA_VALUE = 0.9f;
    private const float DARKEN_DURATION = 1f;

    private Vector2 verseTextOriginalAnchorMin;
    private Vector2 verseTextOriginalAnchorMax;
    private Vector2 verseTextOriginalPivot;
    private Vector2 verseTextOriginalAnchoredPosition;
    private Vector2 verseTextOriginalRectSize;
    private Vector2 verseTextOriginalSizeDelta;

    private Vector2 verseTextCenterAnchorMin = new Vector2(0.5f, 0.5f);
    private Vector2 verseTextCenterAnchorMax = new Vector2(0.5f, 0.5f);
    private Vector2 verseTextCenterPivot = new Vector2(0.5f, 0.5f);
    private Vector2 verseTextCenterAnchoredPosition = Vector2.zero;

    private const float CONTINUE_BUTTON_MARGIN_FROM_TEXT = 100f;

    [SerializeField] private GameObject headerContainer;
    [SerializeField] private GameObject wordParticleEffect;
    [SerializeField] private Transform particleParent;

    private void Awake()
    {
        instance = this;

        inputAction = new InputSystem_Actions();
        interactAction = inputAction.UI.Click;

        interactAction.Enable();
        interactAction.performed += InteractAction_performed;

    }

    void Start()
    {
        passageManager = PassageManager.instance;
    }

    public void UpdateVerseTextOriginalData()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        verseTextOriginalAnchorMin = verseText.rectTransform.anchorMin;
        verseTextOriginalAnchorMax = verseText.rectTransform.anchorMax;
        verseTextOriginalAnchoredPosition = verseText.rectTransform.anchoredPosition;
        verseTextOriginalPivot = verseText.rectTransform.pivot;
        verseTextOriginalRectSize = verseText.rectTransform.rect.size;
        verseTextOriginalSizeDelta = verseText.rectTransform.sizeDelta;
        Debug.Log(verseTextOriginalRectSize);
        Debug.Log(verseTextOriginalSizeDelta);
    }


    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.gameMode == E_GameMode.BibleTap)
        {

            if (obj.phase != InputActionPhase.Performed) return;
            if (obj.ReadValue<float>() == 0) return;

            introTutorial.SetActive(false);

            if (wordsIdx == words.Count - 1)
            {
                //audio dibacakan terlebih dahulu nanti baru setup next verse dipanggil

                if (verseIdx <= allVerses.Count && wordsIdx < words.Count)
                {
                    verseText.text += words[wordsIdx++] + " ";
                    // + 1 dari spasi
                    CreateParticle(words[wordsIdx - 1].Length + 1);
                    GameManager.instance.AddMoney(1);
                }

                SetupNextVerse();





                return;
            }

            verseText.text += words[wordsIdx++] + " ";
            // + 1 dari spasi
            CreateParticle(words[wordsIdx - 1].Length + 1);
            GameManager.instance.AddMoney(1);
        }
    }

    private void CreateParticle(int amountOfCharactersInNewWord)
    {
        verseText.ForceMeshUpdate();
        GameObject newParticle = Instantiate(wordParticleEffect, particleParent);
        
        (newParticle.transform as RectTransform).anchoredPosition = verseText.textInfo.characterInfo[verseText.textInfo.characterCount - 1 - (amountOfCharactersInNewWord / 2)].bottomLeft + verseText.rectTransform.anchoredPosition3D;
        
        // Tambah -30f biar ga z fighting dan kelihatan particlenya
        (newParticle.transform as RectTransform).anchoredPosition3D = new Vector3(
            (newParticle.transform as RectTransform).anchoredPosition3D.x,
            (newParticle.transform as RectTransform).anchoredPosition3D.y,
            -30f
            );
    }


    public void SetupBibleTapGame(Passage passage)
    {
        StartCoroutine(SetupBibleTapGameDelayed(passage));
    }

    private IEnumerator SetupBibleTapGameDelayed(Passage passage)
    {
        yield return StartCoroutine(DelayedStart());

        GameManager.instance.gameMode = E_GameMode.BibleTap;


        pageIdx = 0;
        wordsIdx = 0;
        verseIdx = 0;
        verseText.text = "";
        verseTextHighlight.gameObject.SetActive(false);
        headerContainer.SetActive(true);
        backgroundDarken.color = new Color(0, 0, 0, 0f);

        MoveTextToCenter(false);

        pageSets = passage.pages;
        allVerses = pageSets[pageIdx].verses;

        introTutorial.SetActive(true);
        OnGameSetupDone?.Invoke(pageSets.Count);


        SplitVerseIntoWords(allVerses[0]);

        Debug.Log("Setup done");
    }

    public void SetupNextVerse()
    {
        verseIdx++;

        if (verseIdx >= allVerses.Count)
        {
            Button button = continueButton.GetComponent<Button>();
            RectTransform continueButtonRT = continueButton.GetComponent<RectTransform>();
            if (pageIdx >= pageSets.Count - 1)
            {

                // pembagian hadiah, dan kembali ke menu awal
                GameManager.instance.gameMode = E_GameMode.None;

                LeanTween.delayedCall(DARKEN_DURATION, ShowContinueButton).setOnCompleteParam(continueButtonRT);
                ShowContinueButton(continueButtonRT);
                button.onClick.AddListener(() => passageManager.SettingUpUIAfterTapGameplay());
                button.onClick.AddListener(() => continueButton.SetActive(false));

                Debug.Log("All verses finished");
                // Ada ini biar progress barnya penuh
                OnNewPage?.Invoke(pageSets.Count);

                HighlightText();
                DarkenBackground(true);
                MoveTextToCenter(true);
                headerContainer.SetActive(false);
                return;
            }

            Debug.Log("Button Continue nongol, kasih on click nya, narator ngomong");

            HighlightText();
            DarkenBackground(true);
            MoveTextToCenter(true);
            headerContainer.SetActive(false);

            LeanTween.delayedCall(DARKEN_DURATION, ShowContinueButton).setOnCompleteParam(continueButtonRT);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ContinuePassage());
            button.onClick.AddListener(() => continueButton.SetActive(false));
            GameManager.instance.gameMode = E_GameMode.None;
            return;

        }
        //Debug.Log("Kok Masuk");
        wordsIdx = 0;
        SplitVerseIntoWords(allVerses[verseIdx]);
    }

    private void ShowContinueButton(object obj)
    {
        RectTransform continueButtonRT;

        Debug.Log($"Show continue button?");
        if (obj is RectTransform) continueButtonRT = obj as RectTransform;
        else return;

        Debug.Log($"Yes. Move to {((verseText.preferredHeight / 2) + CONTINUE_BUTTON_MARGIN_FROM_TEXT) * -1f}");
        continueButtonRT.anchoredPosition = new Vector2(continueButtonRT.anchoredPosition.x, ((verseText.preferredHeight / 2) + CONTINUE_BUTTON_MARGIN_FROM_TEXT) * -1f);
        continueButton.SetActive(true);
    }

    private void HighlightText()
    {
        Debug.Log("Highlight!");
        verseTextHighlight.text = highlightMarkdown;
        verseTextHighlight.text += verseText.text;
        verseTextHighlight.gameObject.SetActive(true);
    }

    private void DarkenBackground(bool darken)
    {
        LeanTween.cancel(backgroundDarken.rectTransform);
        if (darken) LeanTween.alpha(backgroundDarken.rectTransform, DARKEN_ALPHA_VALUE, DARKEN_DURATION);
        else LeanTween.alpha(backgroundDarken.rectTransform, 0f, DARKEN_DURATION);
    }

    private void MoveTextToCenter(bool moveToCenter)
    {
        if (moveToCenter)
        {
            LeanTween.value(gameObject, UpdateVerseTextAnchorMin, verseTextOriginalAnchorMin, verseTextCenterAnchorMin, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextAnchorMax, verseTextOriginalAnchorMax, verseTextCenterAnchorMax, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextPivot, verseTextOriginalPivot, verseTextCenterPivot, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextAnchoredPosition, verseTextOriginalAnchoredPosition, verseTextCenterAnchoredPosition, DARKEN_DURATION);
            Vector2 targetSize = new Vector2(verseTextOriginalRectSize.x, verseText.preferredHeight);
            LeanTween.value(gameObject, UpdateVerseTextSize, verseTextOriginalSizeDelta, targetSize, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextHighlightAnchorMin, verseTextOriginalAnchorMin, verseTextCenterAnchorMin, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextHighlightAnchorMax, verseTextOriginalAnchorMax, verseTextCenterAnchorMax, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextHighlightPivot, verseTextOriginalPivot, verseTextCenterPivot, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextHighlightAnchoredPosition, verseTextOriginalAnchoredPosition, verseTextCenterAnchoredPosition, DARKEN_DURATION);
            LeanTween.value(gameObject, UpdateVerseTextHighlightSize, verseTextOriginalSizeDelta, targetSize, DARKEN_DURATION);
        }
        else
        {
            LeanTween.cancel(gameObject);
            // Balikin ke normalnya instant aja, soalnya ga keliatan juga teksnya pas mau balikin
            LeanTween.value(gameObject, UpdateVerseTextAnchorMin, verseTextCenterAnchorMin, verseTextOriginalAnchorMin, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextAnchorMax, verseTextCenterAnchorMax, verseTextOriginalAnchorMax, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextPivot, verseTextCenterPivot, verseTextOriginalPivot, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextAnchoredPosition, verseTextCenterAnchoredPosition, verseTextOriginalAnchoredPosition, 0.01f);
            Vector2 previousSize = new Vector2(verseTextOriginalRectSize.x, verseText.preferredHeight);
            LeanTween.value(gameObject, UpdateVerseTextSize, previousSize, verseTextOriginalSizeDelta, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextHighlightAnchorMin, verseTextCenterAnchorMin, verseTextOriginalAnchorMin, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextHighlightAnchorMax, verseTextCenterAnchorMax, verseTextOriginalAnchorMax, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextHighlightPivot, verseTextCenterPivot, verseTextOriginalPivot, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextHighlightAnchoredPosition, verseTextCenterAnchoredPosition, verseTextOriginalAnchoredPosition, 0.01f);
            LeanTween.value(gameObject, UpdateVerseTextHighlightSize, previousSize, verseTextOriginalSizeDelta, 0.01f);
        }
    }

    private void UpdateVerseTextAnchorMin(Vector2 newAnchorMin)
    {
        verseText.rectTransform.anchorMin = newAnchorMin;
    }

    private void UpdateVerseTextAnchorMax(Vector2 newAnchorMax)
    {
        verseText.rectTransform.anchorMax = newAnchorMax;
    }

    private void UpdateVerseTextPivot(Vector2 newPivot)
    {
        verseText.rectTransform.pivot = newPivot;
    }

    private void UpdateVerseTextAnchoredPosition(Vector2 newAnchoredPosition)
    {
        verseText.rectTransform.anchoredPosition = newAnchoredPosition;
    }

    private void UpdateVerseTextSize(Vector2 size)
    {
        verseText.rectTransform.sizeDelta = new Vector2(size.x, size.y);
    }

    //private void UpdateVerseTextHeight(float newHeight)
    //{
    //    Debug.Log($"Before = {verseText.rectTransform.sizeDelta}");
    //    verseText.rectTransform.sizeDelta = new Vector2(verseTextOriginalWidth, newHeight);
    //    Debug.Log($"After = {verseText.rectTransform.sizeDelta}");
    //}

    //private void UpdateVerseTextHeightSizeDelta(float newHeight)
    //{
    //    Debug.Log($"Before = {verseText.rectTransform.sizeDelta}");
    //    verseText.rectTransform.sizeDelta = new Vector2(verseTextOriginalSizeDeltaX, newHeight);
    //    Debug.Log($"After = {verseText.rectTransform.sizeDelta}");
    //}

    //TODO: update verse text width as well, from size delta to rect width (on move to center) or vice versa on move to original

    private void UpdateVerseTextHighlightAnchorMin(Vector2 newAnchorMin)
    {
        verseTextHighlight.rectTransform.anchorMin = newAnchorMin;
    }

    private void UpdateVerseTextHighlightAnchorMax(Vector2 newAnchorMax)
    {
        verseTextHighlight.rectTransform.anchorMax = newAnchorMax;
    }

    private void UpdateVerseTextHighlightPivot(Vector2 newPivot)
    {
        verseTextHighlight.rectTransform.pivot = newPivot;
    }

    private void UpdateVerseTextHighlightAnchoredPosition(Vector2 newAnchoredPosition)
    {
        verseTextHighlight.rectTransform.anchoredPosition = newAnchoredPosition;
    }

    //private void UpdateVerseTextHighlightHeight(float newHeight)
    //{
    //    verseTextHighlight.rectTransform.sizeDelta = new Vector2(verseTextOriginalWidth, newHeight);
    //}

    //private void UpdateVerseTextHighlightHeightSizeDelta(float newHeight)
    //{
    //    verseTextHighlight.rectTransform.sizeDelta = new Vector2(verseTextOriginalSizeDeltaX, newHeight);
    //}

    private void UpdateVerseTextHighlightSize(Vector2 size)
    {
        verseTextHighlight.rectTransform.sizeDelta = new Vector2(size.x, size.y);
    }

    public void ContinuePassage()
    {
        //save passage as a checkpoint
        //continue here
        if(pageIdx == pageSets.Count - 1)
        {
            return;
        }

        headerContainer.SetActive(true);

        pageIdx++;
        Debug.Log(pageIdx);
        OnNewPage?.Invoke(pageIdx);
        wordsIdx = 0;
        verseIdx = 0;
        verseText.text = "";
        DarkenBackground(false);
        MoveTextToCenter(false);
        verseTextHighlight.gameObject.SetActive(false);
        allVerses = pageSets[pageIdx].verses;
        GameManager.instance.gameMode = E_GameMode.BibleTap;
        SplitVerseIntoWords(allVerses[verseIdx]);

    }

    public void SplitVerseIntoWords(Verse verse)
    {
        words.Clear();

        string[] splitedWords = verse.verse.Split(' ');

        words = splitedWords.ToList();
    }



}
