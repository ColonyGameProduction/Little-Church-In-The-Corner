using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicUIBinder : MonoBehaviour
{
    [Header("referensi ui")]
    public Slider SLD_progress;
    public TextMeshProUGUI TMPUGUI_songAndAuthor;

    private void Update()
    {
        if (MusicManager.Instance == null || MusicManager.Instance.SCR_currSong == null)
        {
            return;
        }

        // update progress bar (kalo ada)
        if (SLD_progress != null)
        {
            var audioSource = MusicManager.Instance.GetComponent<AudioSource>();
            if (audioSource.clip != null)
            {
                SLD_progress.value = audioSource.time / audioSource.clip.length;
            }
        }

        // update song & author text
        if (TMPUGUI_songAndAuthor != null)
        {
            TMPUGUI_songAndAuthor.text = MusicManager.Instance.SCR_currSong.S_titleAndAuthor;
        }
    }

    public void OnSliderValueChanged()
    {
        if (SLD_progress != null)
        {
            MusicManager.Instance.OnSliderValueChanged(SLD_progress);
        }
    }
}
