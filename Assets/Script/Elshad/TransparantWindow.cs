using System;
using System.Runtime.InteropServices;
using UnityEngine;
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
        SetClickThrough(Physics2D.OverlapPoint(RaycastHit2D))
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

    private bool IsThereACollider()
    {

    }
}
