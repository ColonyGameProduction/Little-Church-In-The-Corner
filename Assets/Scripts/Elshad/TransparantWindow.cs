using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class TransparantWindow : MonoBehaviour
{
    #region Import From Windows API
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendedFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    #endregion


    #region ALL Constant
    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    #endregion

    private IntPtr hWnd;

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }


    private void Start()
    {
#if !UNITY_EDITOR_
        hWnd = GetActiveWindow();

        MARGINS margins = new MARGINS() { cxLeftWidth = -1};
        DwmExtendedFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif

        Application.runInBackground = true;

    }

    private void Update()
    {
        //SetClickThrough(IsSomethingBehindCursor());
    }

    private void SetClickThrough(bool clickThrough)
    {
        if (clickThrough)
        {
            SetWindowLong(hWnd,GWL_EXSTYLE, WS_EX_LAYERED|WS_EX_TRANSPARENT);
        }
        else
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }

    }

    public static bool IsSomethingBehindCursor()
    {
        // 🔹 Step 1: Check UI (Screen Space UI)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        // 🔹 Step 2: Check 3D collider
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit3D))
            return true;

        // 🔹 Step 3: Check 2D collider
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit2D.collider != null)
            return true;

        // 🔹 Step 4: Nothing found
        return false;
    }
}
