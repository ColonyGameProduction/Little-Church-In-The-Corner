using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB; // Only for PC
#if UNITY_ANDROID || UNITY_IOS
using NativeFilePickerNamespace;
#endif

public class UniversalLocalSongImporter : MonoBehaviour
{
    public MusicManager musicManager;

    public void ImportLocalSong()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        ImportPC();
#elif UNITY_ANDROID
        ImportAndroid();
#elif UNITY_IOS
        ImportIOS();
#else
        Debug.LogWarning("Platform not supported for file import.");
#endif
    }

    // PC ato EDITOR
    private void ImportPC()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Select Music",
            "",
            new[] { new ExtensionFilter("Audio Files", "mp3", "wav", "ogg") },
            false
        );

        if (paths.Length > 0)
        {
            string path = paths[0];
            StartCoroutine(AddSongFromPath(path));
        }
    }

    // ANDROID
    private void ImportAndroid()
    {
#if UNITY_ANDROID
        // Request permission
        var permission = AndroidRuntimePermissions.RequestPermission("android.permission.READ_EXTERNAL_STORAGE");

        if (permission == AndroidRuntimePermissions.Permission.Denied)
        {
            Debug.LogError("READ_EXTERNAL_STORAGE permission denied.");
            return;
        }

        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                StartCoroutine(AddSongFromPath(path));
            }
        }, new string[] { "audio/*" });
#endif
    }

    // IOS
    private void ImportIOS()
    {
#if UNITY_IOS
        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                StartCoroutine(AddSongFromPath(path));
            }
        }, new string[] { "public.audio" }); // iOS audio UTI
#endif
    }

    // ini fungsi addsong yang baru
    private IEnumerator AddSongFromPath(string path)
    {
        Debug.Log("Loading audio from: " + path);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio: " + www.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            string title = Path.GetFileNameWithoutExtension(path);

            // === Tambah ke MusicManager kamu ===
            Songs newSong = new Songs
            {
                ADO_music = clip,
                S_titleAndAuthor = title,
                ENM_musicCode = ENM_MusicCode.SongLocal1
            };

            musicManager.AddLocalSong(clip, title, path);

            Debug.Log("Song imported successfully: " + title);
        }
    }
}
