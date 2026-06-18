using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(GameObject bibleVerseWindow)
    {
        bibleVerseWindow.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
