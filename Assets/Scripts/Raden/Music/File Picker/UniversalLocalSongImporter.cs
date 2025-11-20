using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using SFB; // only for PC
#if UNITY_ANDROID || UNITY_IOS
using NativeFilePickerNamespace;
#endif

public class UniversalLocalSongImporter : MonoBehaviour
{
    public MusicManager SCR_MM;

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

            // CEK DUPLIKAT DULU
            if (SCR_MM.IsSongAlreadyAdded(path))
            {
                Debug.LogWarning("Song already added sebelumnya!");
                return;
            }

            StartCoroutine(AddSongFromPath(path));

        }
    }

    // ANDROID
    private async void ImportAndroid()
    {
        Debug.Log("IMPORT ANDROID DIPANGGIL!");

        // minta read_media_audio (android 13)
        var p1 = await AndroidRuntimePermissions.RequestPermissionAsync("android.permission.READ_MEDIA_AUDIO");

        // minta read_external_storage (android 12 ke bawah)
        var p2 = await AndroidRuntimePermissions.RequestPermissionAsync("android.permission.READ_EXTERNAL_STORAGE");

        if (p1 != AndroidRuntimePermissions.Permission.Granted && p2 != AndroidRuntimePermissions.Permission.Granted)
        {
            Debug.LogError("Permission Denied!");
            return;
        }

        NativeFilePicker.PickFile((path) =>
        {
            Debug.Log("PATH SELECTED: " + path);

            if (!string.IsNullOrEmpty(path))
            {
                // CEK DUPLIKAT DULU
                if (SCR_MM.IsSongAlreadyAdded(path))
                {
                    Debug.LogWarning("Song already added sebelumnya!");
                    return;
                }

                StartCoroutine(AddSongFromPath(path));
            }
        }, new string[] { "audio/*" });
    }

    // IOS
    private void ImportIOS() // riweh ini
    {
#if UNITY_IOS
        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                // CEK DUPLIKAT DULU
                if (musicManager.IsSongAlreadyAdded(path))
                {
                    Debug.LogWarning("Song already added sebelumnya!");
                    return;
                }

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

            Songs newSong = new Songs
            {
                ADO_music = clip,
                S_titleAndAuthor = title,
                ENM_musicCode = ENM_MusicCode.SongLocal1
            };

            SCR_MM.AddLocalSong(clip, title, path);

            Debug.Log("Song imported successfully: " + title);
        }
    }
}
