using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PassageManager : MonoBehaviour
{
    public static PassageManager instance;

    public AllVerseSO holyBible;

    public GameObject booksContainer;

    public GameObject bibleButtonPrefab;
    public GameObject passageButtonPrefab;
    public GameObject passagesContainerPrefab;

    public GameObject bibleVerseWindow;
    public GameObject bibleTapWindow;
    public GameObject rewardWindow;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SettingUpTheBible();

    }

    public void SettingUpTheBible()
    {
        foreach(Books book in holyBible.books)
        {
            Button booksButton  = Instantiate(bibleButtonPrefab, booksContainer.transform).GetComponent<Button>();
            TextMeshProUGUI booksButtonText = booksButton.GetComponentInChildren<TextMeshProUGUI>();


            booksButtonText.text = book.book;
            GameObject passageCanvas = Instantiate(passagesContainerPrefab, booksContainer.transform);
            RectTransform passageCanvasRect = passageCanvas.GetComponent<RectTransform>();
            //passageCanvasRect.sizeDelta = new Vector2(passageCanvasRect.sizeDelta.x, booksButton.GetComponent<RectTransform>().sizeDelta.y * book.passages.Count);
            passageCanvas.SetActive(false);


            booksButton.onClick.AddListener(() => ShowingPassage(passageCanvas));

            foreach(Passage passage in book.passages)
            {
                Button passageButton = Instantiate(passageButtonPrefab, passageCanvas.transform).GetComponent<Button>();
                TextMeshProUGUI passageButtonText = passageButton.GetComponentInChildren<TextMeshProUGUI>();

                passageButtonText.text = passage.title;
                passageButton.onClick.AddListener(() => SettingUpUIForTap());
                passageButton.onClick.AddListener(() => BibleTapManager.instance.SetupBibleTapGame(passage));
            }


        }
    }

    public void ShowingPassage(GameObject passageCanvas)
    {
        if (passageCanvas == null)
        {
            return;
        }

        if(passageCanvas.activeSelf)
        {
            passageCanvas.SetActive(false);
        }
        else
        {
            passageCanvas.SetActive(true);
        }
    }

    public void SettingUpUIForTap()
    {
        bibleVerseWindow.SetActive(false);
        bibleTapWindow.SetActive(true);
    }

    public void SettingUpUIAfterTapGameplay()
    {
        rewardWindow.SetActive(true);
        bibleTapWindow.SetActive(false);
    }
}
