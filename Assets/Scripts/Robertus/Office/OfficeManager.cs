using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Instance;

    public GameObject GO_NPC;
    public OfficeInteract SCR_officeInteract;

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

    private void OnEnable()
    {
        TimeManager.ACT_interactQnAIsReady += SetupInteract;
        UIChatManager.ACT_NoCurrentSermonAvailable += FinishInteract;
    }

    private void OnDisable()
    {
        TimeManager.ACT_interactQnAIsReady -= SetupInteract;
        UIChatManager.ACT_NoCurrentSermonAvailable -= FinishInteract;
    }

    public void SetupInteract()
    {
        //Kalau masih ada renungan yang jalan, jangan interact lagi
        if (ChatManager.Instance.ENM_currDialog != ENM_DialogTitle.None)
            return;

        //Kalau ga ada renungan dalam queue, jangan tampilin interact button.
        if (TimeManager.Instance.I_queuedQnA <= 0)
            return;

        SCR_officeInteract.gameObject.SetActive(true);
    }

    public void FinishInteract()
    {
        GO_NPC.SetActive(false);
        if (TimeManager.Instance.I_queuedQnA <= 0) SCR_officeInteract.gameObject.SetActive(false);
        else SetupInteract();
    }

    public void SetupNPC()
    {
        GO_NPC.SetActive(true);
    }
}
