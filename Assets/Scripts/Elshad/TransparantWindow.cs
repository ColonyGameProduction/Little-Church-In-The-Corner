using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

// Script untuk membuat window game nya transparant
// Taruh script ini di game object di scene manapun

public class TransparantWindow : MonoBehaviour
{

    //Region ini untuk me-refrence function/method yang digunakan di API windows
    //Jika ingin menggunakan OS lain, maka gunakan API OS tersebut
    #region Import From Windows API

    //mengimport API function dari user32.dll
    [DllImport("user32.dll")]
    
    //Function/method ini dipakai untuk me-refrence window game yang lagi aktif
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]

    //function ini digunakan untuk nge-set window nya
    //hWnd, itu adalah window yang di dapatkan lewat function GetActiveWindow()
    //nIndex itu adalah code yang dipakai untuk window style nya kode nya ada di "REGION ALL CONSTAN" dan dapat dicari di windows API
    //dwNewLong itu adalah kode konstan juga untuk mengatur window logic, dapat dicari di windows API

    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]

    //function ini digunakan untuk mendapatkan style window nya seperti apa
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]

    //function ini digunakan untuk menset posisi windows ada di mana. x,y itu untuk posisi nya. cx, cy untuk tinggi dan lebar in pixel
    //uFlags itu kode yang ada di enum API windows
    //hWndInsertAfter dipakai untuk menset depan atau belakang

    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]

    //untuk menset atribut di window nya
    //crKey untuk transparant nya
    //bAlpha untuk opaque window nya
    //konstan yang ada di Window API

    static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    #endregion


    //semua konstan disini HARUS MEMILIKI:
    //string yang sama dengan windows API
    //value yang sama dengan yang ada di windows API
    #region ALL Constant

    const int GWL_EXSTYLE = -20;
    const int WS_EX_LAYERED = 0x80000;
    const int LWA_COLORKEY = 0x1;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    #endregion

    //refrence untuk windows game nya
    private IntPtr hWnd;

    private void Start()
    {

        //untuk mensetup windows infisible dan menset windows tersebut untuk selalu didepan
#if !UNITY_EDITOR_
        hWnd = GetActiveWindow();

        //setup window style nya
        SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_LAYERED);
        //setup window infisible
        SetLayeredWindowAttributes(hWnd, 0, 255, LWA_COLORKEY);
        //setup supaya didepan terus
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif

        Application.runInBackground = true;

    }

    private void Update()
    {
        //ngecek untuk interact sama object di game nya
        SetClickThrough(IsSomethingBehindCursor());
    }

    private void SetClickThrough(bool clickThrough)
    {
        if (clickThrough)
        {
            //kalo gaada object di game nya, bisa di klik window belakang nya
            SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_LAYERED);
        }
        else
        {
            //kalo ada object di game nya, gabisa di klik window belakang nya
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }

    }

    //ngecek apakah dibelakang mouse ada object (UI, 3D object, 2D object
    public static bool IsSomethingBehindCursor()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit3D))
            return true;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit2D.collider != null)
            return true;

        return false;
    }
}
