using System;
using System.Collections;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get; private set; }

    public ListOfDialogSO SO_listOfDialogueSO;
    public ListOfFace SCR_listOfFace;
    public DialogSO SO_currDialog;
    public float F_interval;
    [HideInInspector] public int I_currDialogComponentIndex;
    public static event Action<int> ACT_PlayDialogue;

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
    }

    public IEnumerator DialogueSequence(float f_interval)
    {
        while (I_currDialogComponentIndex < SO_currDialog.SCR_dialogComponent.Count)
        {
            ACT_PlayDialogue?.Invoke(I_currDialogComponentIndex);
            yield return new WaitForSeconds(f_interval);
            I_currDialogComponentIndex++;
        }
        Debug.Log("All dialogues done!");
    }

    public void PlayDialogue()
    {
        StartCoroutine(DialogueSequence(F_interval));
    }

    public void StopDialogue()
    {
        StopAllCoroutines();
    }

    public void SetupRenungan()
    {
        //Terpanggil dari RenunganIsReady

        SO_currDialog = SO_listOfDialogueSO.SO_GetRandomDialogSO();
        I_currDialogComponentIndex = 0;
    }
}
