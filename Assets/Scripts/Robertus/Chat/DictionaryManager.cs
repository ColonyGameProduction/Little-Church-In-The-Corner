using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Class untuk mengatur renungan-renungan yang disimpan. Isinya function untuk save dan load renungan.
/// </summary>
public class DictionaryManager : MonoBehaviour
{
    public static DictionaryManager Instance { get; private set; }
    /// <summary>
    /// Renungan yang saat ini sedang dipilih dari list renungan yang telah disimpan
    /// </summary>
    [HideInInspector] public DialogSO SO_currDialogSelected;
    /// <summary>
    /// List semua renungan yang telah disimpan
    /// </summary>
    public List<DialogSO> List_SO_allDownloadedDialog;
    /// <summary>
    /// Biar mudah akses UI Dictionary
    /// </summary>
    public UIDictionary SCR_UIDictionary;

    /// <summary>
    /// Ini lokasi tempat penyimpanan renungan yang didownload di device pemain
    /// </summary>
    private string S_saveFilePath;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
        //Basically, nanti kalau di Windows Unity Editor, lokasinya jadi %userprofile%\AppData\LocalLow\<companyname>\<productname>\Data\a.data
        S_saveFilePath = Path.Combine(Application.persistentDataPath, "Data", "downloaded.data");
    }

    private void Start()
    {
        //Pas awal mulai, langsung ngeload renungan yang sudah disimpan.
        LoadFromDevice(S_saveFilePath);
    }

    /// <summary>
    /// Download renungan saat ini ke device pemain
    /// </summary>
    public void DownloadToDevice()
    {
        if (List_SO_allDownloadedDialog == null) List_SO_allDownloadedDialog = new List<DialogSO>();

        //TODO: ganti supaya pakai TransitionManager.
        Debug.LogError("WARNING: Ganti codingan SetupRenungan supaya memakai ruangan saat ini");
        List_SO_allDownloadedDialog.Add(ChatManager.Instance.SO_listOfDialogueSO.SO_GetDialogSO(ENM_Room.Church, ChatManager.Instance.ENM_currDialog));

        SaveToDevice();
    }

    /// <summary>
    /// Function buat menyimpan data renungan ke device pemain.
    /// Mungkin pindahin ke DataManager.
    /// </summary>
    public void SaveToDevice()
    {
        //Harus gini soalnya JsonUtility.ToJson ga bisa nerima List<>
        SaveData SCR_saveData = new SaveData(List_SO_allDownloadedDialog);

        Debug.Log($"Saving...\n{SCR_saveData}");

        DataManager.Instance.SaveXOR(S_saveFilePath, JsonUtility.ToJson(SCR_saveData));
    }

    /// <summary>
    /// Ngeload data renungan dari device pemain, kalau ada.
    /// Kalau ga ada, well ga ngapa-apain.
    /// </summary>
    /// <param name="S_filePath">Lokasi tempat penyimpanan</param>
    private void LoadFromDevice(string S_filePath)
    {
        List_SO_allDownloadedDialog = new List<DialogSO>();
        //Kalau ternyata ada filenya:
        if (File.Exists(S_filePath))
        {
            //Ambil isi save file yang telah terenkripsi sebelumnya
            string S_encryptedString = File.ReadAllText(S_filePath);

            //Diconvert ke teks yang bisa dibaca (JSON)
            string S_decryptedString = DataManager.Instance.S_EncryptDecrypt(S_encryptedString);

            //Diconvert ke class SaveData.
            SaveData SCR_loadedData = JsonUtility.FromJson<SaveData>(S_decryptedString);

            //Kalau misalnya konversinya berhasil
            if (SCR_loadedData != null)
            {
                Debug.Log(SCR_loadedData);
                //Taro hasil konversinya ke dalam List asli, yang bisa dibaca dan diakses oleh class lain
                List_SO_allDownloadedDialog = SCR_loadedData.List_SO_ConvertSaveData();
            }
            else
            {
                Debug.LogError("ERROR: Data cannot be loaded");
            }
        }
        else
        {
            Debug.LogWarning("WARNING: File doesn't exist");
        }
    }

    #region Save Data Classes
    //Yes, harus kayak gini biar bisa diserialize ke JSON. Ga bisa langsung serialize List<DialogSO> soalnya JSONUtility ga bisa serialize list for some reason. Harus ada wrapper classnya.
    [Serializable]
    public class SaveData
    {
        public List<SaveDataDialog> List_SO_allDownloadedDialog;

        public SaveData(List<DialogSO> list_SO_allDownloadedDialog)
        {
            List_SO_allDownloadedDialog = new List<SaveDataDialog>();
            foreach (DialogSO SO_dialog in list_SO_allDownloadedDialog)
            {
                List<SaveDataDialogComponent> List_SCR_components = new List<SaveDataDialogComponent>();
                foreach (DialogComponent SCR_component in SO_dialog.SCR_dialogComponent)
                {
                    List_SCR_components.Add
                    (
                        new SaveDataDialogComponent
                        (
                            SCR_component.COL_bubbleColour,
                            SCR_component.S_stringText,
                            SCR_component.ENM_charFace
                        )
                    );
                }

                List_SO_allDownloadedDialog.Add
                (
                    new SaveDataDialog
                    (
                        SO_dialog.ENM_dialogTitle,
                        List_SCR_components
                    )
                );
            }
        }

        public List<DialogSO> List_SO_ConvertSaveData()
        {
            List<DialogSO> List_SO_result = new List<DialogSO>();

            foreach (SaveDataDialog SCR_dialog in List_SO_allDownloadedDialog)
            {
                List<DialogComponent> List_SCR_components = new List<DialogComponent>();
                foreach (SaveDataDialogComponent SCR_component in SCR_dialog.SCR_dialogComponent)
                {
                    List_SCR_components.Add
                    (
                        new DialogComponent
                        (
                            SCR_component.COL_bubbleColour,
                            SCR_component.S_stringText,
                            SCR_component.ENM_charFace
                        )
                    );
                }

                DialogSO SO_newDialogSO = ScriptableObject.CreateInstance<DialogSO>();
                SO_newDialogSO.ENM_dialogTitle = SCR_dialog.ENM_dialogTitle;
                SO_newDialogSO.SCR_dialogComponent = List_SCR_components;

                List_SO_result.Add(SO_newDialogSO);
            }

            return List_SO_result;
        }

        public override string ToString()
        {
            string S_dialogs = string.Empty;

            foreach (SaveDataDialog SCR_dialog in List_SO_allDownloadedDialog)
            {
                S_dialogs += SCR_dialog.ToString() + "\n";
            }
            return S_dialogs;
        }
    }

    //Dan harus ada ini soalnya kalau pakai DialogSO biasa, di save filenya cuma muncul ID doang, ga ada isinya.
    [Serializable]
    public class SaveDataDialog
    {
        public ENM_DialogTitle ENM_dialogTitle;
        public List<SaveDataDialogComponent> SCR_dialogComponent;

        public SaveDataDialog(ENM_DialogTitle eNM_dialogTitle, List<SaveDataDialogComponent> sCR_dialogComponent)
        {
            ENM_dialogTitle = eNM_dialogTitle;
            SCR_dialogComponent = sCR_dialogComponent;
        }

        public override string ToString()
        {
            string S_dialogComponents = string.Empty;

            foreach (SaveDataDialogComponent SCR_component in SCR_dialogComponent)
            {
                S_dialogComponents += SCR_component.ToString() + "\n";
            }

            return $"Title: {ENM_dialogTitle}\nDialog components:{S_dialogComponents}";
        }
    }

    [Serializable]
    public class SaveDataDialogComponent
    {
        public Color COL_bubbleColour = Color.white;
        public string S_stringText;
        public ENM_CharFace ENM_charFace;

        public SaveDataDialogComponent(Color cOL_bubbleColour, string stringText, ENM_CharFace eNM_charFace)
        {
            COL_bubbleColour = cOL_bubbleColour;
            S_stringText = stringText;
            ENM_charFace = eNM_charFace;
        }

        public override string ToString()
        {
            return $"Color = {COL_bubbleColour}, charface = {ENM_charFace}\nText = {S_stringText}";
        }
    }

    #endregion
}
