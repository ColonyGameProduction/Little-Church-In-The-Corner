using UnityEngine;
using UnityEngine.UI;

public class UIHUDManager : MonoBehaviour
{
    public TransitionManager STR_TM;

    [Header("Para gameobject Room")]
    public GameObject GO_church;
    public GameObject GO_office;
    public GameObject GO_bedRoom;
    //public GameObject GO_highlightRoomButton;

    [Header("Image buat setiap Room")]
    public Image IMG_church;
    public Image IMG_office;
    public Image IMG_bedRoom;

    [Header("Para Button buat Room")]
    public Button BTN_church;
    public Button BTN_office;
    public Button BTN_bedRoom;

    [Header("Ini Button buat yang diatas itu")]
    public Button BTN_exit;
    public Button BTN_setting;
    public Button BTN_minimizeAndMaximize;

    private void Start()
    {
        BTN_church.onClick.AddListener(() => RoomTransitionUI(ENM_Room.Church));
        BTN_office.onClick.AddListener(() => RoomTransitionUI(ENM_Room.Office));
        BTN_bedRoom.onClick.AddListener(() => RoomTransitionUI(ENM_Room.Bedroom));

        // Robert: ada ini biar kalau null reference, dia ga munculin error. Kayaknya ini ga kepake juga akhirnya?
        if(BTN_exit) BTN_exit.onClick.AddListener(Exit);
        if (BTN_setting) BTN_setting.onClick.AddListener(Settings);
        if(BTN_minimizeAndMaximize) BTN_minimizeAndMaximize.onClick.AddListener(MinimizeAndMaximize);
    }

    public void RoomTransitionUI(ENM_Room room)
    {
        //Ini nanti pindahin semua yang ada di RoomPositionManager ke sini
        //Ini cuma sementara doang gara-gara aku mager
        //- Robert
        TransitionManager.Instance.ENM_room = room;
        RoomPositionManager.Instance.GoToPosition();

        //STR_TM.Transition(room);
        //HighlightRoomButtonTransition(room);
    }

    // ini buat nge highlight buttonya tapi ini buat contoh baek
    public void HighlightRoomButtonTransition(ENM_Room room)
    {
        ResetHighlight();

        switch (room)
        {
            case ENM_Room.Church:
                IMG_church.color = Color.white;
                break;
            case ENM_Room.Office:
                IMG_office.color = Color.white;
                break;
            case ENM_Room.Bedroom:
                IMG_bedRoom.color = Color.white;
                break;
        }
    }

    // ini buat reset highlight sementara
    private void ResetHighlight()
    {
        IMG_church.color = Color.gray;
        IMG_office.color = Color.gray;
        IMG_bedRoom.color = Color.gray;
    }

    // buat masuk ke panel settings
    public void Settings()
    {
        // sementara debug duls nanti ganti ke settings panel
        Debug.Log("Open Settings");
    }

    public void Exit()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }

    // buat nge minimize atau maximize windownya nanti
    public void MinimizeAndMaximize()
    {
        // logic minimize sama maximizenya in progress lagi rnd
        Debug.Log("Toggle Window State...");
    }
}

