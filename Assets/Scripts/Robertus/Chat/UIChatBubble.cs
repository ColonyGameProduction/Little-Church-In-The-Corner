using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Container untuk UI chat bubble. Isinya referensi-referensi komponen Unity yang diperlukan untuk UI chat bubble.
/// Require component Canvas Group karena ini letaknya di prefab Chat Bubble. Canvas Group juga letaknya di prefab Chat Bubble supaya kalau alpha dari Canvas Group diatur, dia bakal ngefek ke semua child game object yang ada di dalamnya.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIChatBubble : MonoBehaviour
{
    [Header("Unity References")]
    public Image IMG_chatBubble;
    public Image IMG_face;
    public TextMeshProUGUI TMPUGUI_chatBubble;
    public RectTransform RT_containerRectTransform;

    [Header("Data")]
    /// <summary>
    /// Tinggi minimal untuk chat bubblenya. Kalau teksnya sedikit, maka chat bubblenya ga bakal mengecil mengikuti teks itu, tapi dia bakal mempertahankan ukuran minimal ini.
    /// Tapi, kalau teksnya banyak, chat bubblenya bakal membesar melebihi minimal ini.
    /// </summary>
    public float F_minHeight = 225f;

    private CanvasGroup CG_chatBubble;
    private LayoutElement LE_chatBubble;
    private Coroutine C_textAnimation;
    /// <summary>
    /// Ada ini biar ga stuttering pas ngeresize ukuran chat bubble. Soalnya mereka pakai variabel yang sama, yaitu LE_layoutElement, jadi tabrakan pas update valuenya.
    /// </summary>
    private bool B_moveUpAnimationDone;

    /// <summary>
    /// Ini default kecepatan untuk animasi teks. Dia bakal nunggu 0.1 detik untuk nampilin karakter/huruf selanjutnya.
    /// </summary>
    private const float F_BASE_TEXT_SPEED_WAIT_FOR_SECONDS = 0.1f;

    private void Awake()
    {
        CG_chatBubble = GetComponent<CanvasGroup>();
        LE_chatBubble = GetComponent<LayoutElement>();
    }

    /// <summary>
    /// Mengatur UI chat bubble berdasarkan data dari DialogComponent.
    /// </summary>
    /// <param name="SCR_dialogComponent">Data Dialog</param>
    public void Setup(DialogComponent SCR_dialogComponent)
    {
        //Pertama-tama dihide dulu soalnya ga semua dialog bakal dimunculin di awal. Dialog-dialog bakal dimunculin perlahan-lahan. Jadi, muncul atau engga bakal diatur di UIChatManager nantinya.
        Fade(0f);

        IMG_chatBubble.color = SCR_dialogComponent.COL_bubbleColour;
        TMPUGUI_chatBubble.text = SCR_dialogComponent.S_stringText;
        IMG_face.sprite = ChatManager.Instance.SCR_listOfFace.SCR_GetFace(SCR_dialogComponent.ENM_charFace).SPR_characterSprite;
    }

    /// <summary>
    /// Mengubah ukuran chat bubble supaya mengikuti panjang teks, tetapi dia ga bakal kurang dari ukuran minimal (makanya ada Mathf.Max).
    /// </summary>
    private void UpdateChatBubbleSize()
    {
        // Ada ini biar ga stuttering pas ngeresize ukuran chat bubble. Soalnya mereka pakai variabel yang sama, yaitu LE_layoutElement, jadi tabrakan pas update valuenya.
        if (!B_moveUpAnimationDone) return;
        LE_chatBubble.minHeight = Mathf.Max
            (
                TMPUGUI_chatBubble.renderedHeight + TMPUGUI_chatBubble.margin.y + TMPUGUI_chatBubble.margin.w,
                F_minHeight
            );
    }

    /// <summary>
    /// Buat ngefade seluruh chat bubble, seperti teks, chat bubble, dan muka orang.
    /// </summary>
    /// <param name="F_alpha">Nilai dari 0-1. 0 itu transparan/hilang, 1 itu kelihatan 100%</param>
    public void Fade(float F_alpha)
    {
        //Debug.Log(CG_chatBubble);
        LeanTween
            .alphaCanvas(CG_chatBubble, F_alpha, UIChatManager.Instance.F_animationDuration)
            .setEase(LeanTweenType.easeOutCubic);
    }

    /// <summary>
    /// Ini animasi pas chat bubblenya baru nongol. Ribet dan agak hacky karena adanya vertical layout group.
    /// </summary>
    public void MoveUpAnimation()
    {
        //Animasinya baru jalan, jadi belum selesai
        B_moveUpAnimationDone = false;

        //Tinggi awal dari chat bubble.
        float F_originalHeight = LE_chatBubble.minHeight;
        //Dibikin 0 supaya chat bubble yang lain bergerak ngikutin chat bubble ini. Yeah agak sulit buat jelasin cara kerja ini.
        LE_chatBubble.minHeight = 0f;

        //Kalau height di atas dibuat jadi 0, maka semua komponen di dalam chat bubblenya bakal ngaco.
        //Gara-gara itu, semua codingan di bawah ini untuk "offset" heightnya jadi 0, biar teksnya dan lainnya masih bisa dibaca dan ga kena distort.
        
        //Pertama-tama, ini bakal ngatur semua "offsetMin" (kalau di inspector, dia itu "Bottom" di rect transform) supaya dia dikurangin dengan height awal.
        IMG_chatBubble.rectTransform.offsetMin = new Vector2(IMG_chatBubble.rectTransform.offsetMin.x, IMG_chatBubble.rectTransform.offsetMin.y - F_originalHeight);
        //Abis itu, hasilnya disimpan di variabel ini. Tujuannya supaya pas animasiin nanti, dia bakal pakai nilai ini.
        float F_chatBubbleAlteredHeight = IMG_chatBubble.rectTransform.offsetMin.y;

        TMPUGUI_chatBubble.rectTransform.offsetMin = new Vector2(TMPUGUI_chatBubble.rectTransform.offsetMin.x, TMPUGUI_chatBubble.rectTransform.offsetMin.y - F_originalHeight);
        float F_textAlteredHeight = TMPUGUI_chatBubble.rectTransform.offsetMin.y;

        IMG_face.rectTransform.offsetMin = new Vector2(IMG_face.rectTransform.offsetMin.x, IMG_face.rectTransform.offsetMin.y - F_originalHeight);
        float F_faceAlteredHeight = IMG_face.rectTransform.offsetMin.y;

        //Animasi sesungguhnya, dari LE_chatBubble.minHeight (yaitu 0) ke F_originalHeight.
        LeanTween
            .value(LE_chatBubble.minHeight, F_originalHeight, UIChatManager.Instance.F_animationDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnUpdate((float F_heightValue) =>
            {
                //value itu nilai dari height saat animasiin. Ini bakal selalu berubah sampai animasinya selesai.
                LE_chatBubble.minHeight = F_heightValue;
                //Selagi heightnya dianimasiin, komponen lain yang tadi kena offset juga dibalikin ke nilai semula. Yang awalnya tadi dikurang dengan original height, sekarang ditambah dengan height yang sedang dianimasiin.
                IMG_chatBubble.rectTransform.offsetMin = new Vector2(IMG_chatBubble.rectTransform.offsetMin.x, F_chatBubbleAlteredHeight + F_heightValue);
                TMPUGUI_chatBubble.rectTransform.offsetMin = new Vector2(TMPUGUI_chatBubble.rectTransform.offsetMin.x, F_textAlteredHeight + F_heightValue);
                IMG_face.rectTransform.offsetMin = new Vector2(IMG_face.rectTransform.offsetMin.x, F_faceAlteredHeight + F_heightValue);
            })
            .setOnComplete(() =>
            {
                //Jadi true setelah animasinya selesai.
                B_moveUpAnimationDone = true;
            });
    }

    /// <summary>
    /// Ini untuk animasi fade out ke atas, yang bakal terjadi pas renungan baru muncul dan renungan lama dihilangin. Ini bakal ngefek ke dialog-dialog di renungan lama.
    /// </summary>
    public void FadeOutAndDestroyAnimation()
    {
        //Fade out
        Fade(0f);

        //Ini gerakin ke atas
        LeanTween
            .move
            (
                RT_containerRectTransform,
                //Ini bakal gerakin ke atas sejauh UIChatManager.Instance.F_fadeOutDistance
                new Vector3
                (
                    RT_containerRectTransform.anchoredPosition3D.x,
                    RT_containerRectTransform.anchoredPosition.y + UIChatManager.Instance.F_fadeOutDistance,
                    RT_containerRectTransform.anchoredPosition3D.z
                ), 
                UIChatManager.Instance.F_animationDuration
            )
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() =>
            {
                //Setelah selesai animasinya, dia bakal didestroy.
                Destroy(gameObject);
            });
    }

    /// <summary>
    /// Ini untuk memulai animasi teks seperti typewriter/ketikan.
    /// </summary>
    /// <param name="F_textSpeed">Kecepatan dari animasi. 1 adalah default, semakin kecil semakin lambat, semakin besar semakin cepat.</param>
    public void StartTextAnimation(float F_textSpeed)
    {
        //Kalau ternyata sudah ada animasi yang berjalan, berhentiin dulu animasi itu biar ga tabrakan.
        if (C_textAnimation != null)
        {
            StopCoroutine(C_textAnimation);
        }

        C_textAnimation = StartCoroutine(TextAnimation(F_BASE_TEXT_SPEED_WAIT_FOR_SECONDS / F_textSpeed));
    }

    /// <summary>
    /// Animasi teks seperti typewriter/ketikan.
    /// </summary>
    /// <param name="F_textSpeed">Kecepatan dari animasi. 1 adalah default, semakin kecil semakin lambat, semakin besar semakin cepat.</param>
    /// <returns></returns>
    private IEnumerator TextAnimation(float F_textSpeed)
    {
        //Max Visible Characters 0, artinya ga bakal ada teks yang muncul.
        TMPUGUI_chatBubble.maxVisibleCharacters = 0;
        //Harus ada ini supaya data Text Mesh Pro keupdate langsung.
        TMPUGUI_chatBubble.ForceMeshUpdate();
        //Nunggu satu frame supaya bener-bener keupdate.
        yield return null;

        //Berapa banyak karakter yang ada di dalam teks
        int I_totalVisibleCharacters = TMPUGUI_chatBubble.textInfo.characterCount;
        //Jumlah karakter yang sudah kelihatan saat ini
        int I_visibleCount = 0;

        //Seberapa lama WFS_delay antar karakter.
        WaitForSeconds WFS_delay = new WaitForSeconds(F_textSpeed);

        for (int I_visibleChar = 0; I_visibleChar < I_totalVisibleCharacters; I_visibleChar++)
        {
            //Dia bakal nambah sesuai dengan text speed. Semakin besar text speed, jumlah karakter yang bakal kelihatan (per frame) semakin banyak
            I_visibleCount += I_CalculateCharactersPerFrame(F_textSpeed);
            //Ini untuk ngeset supaya kelihatan karakter-karakternya.
            TMPUGUI_chatBubble.maxVisibleCharacters = I_visibleCount;
            //Kalau misalnya teksnya terlalu panjang, ini bakal update ukuran chat bubble.
            UpdateChatBubbleSize();

            yield return WFS_delay;
            //Kalau ternyata semuanya sudah muncul, langsung keluar dari loop.
            if (I_visibleCount > I_totalVisibleCharacters) break;
        }
        //Jaga-jaga, bikin supaya semua karakternya kelihatan.
        TMPUGUI_chatBubble.maxVisibleCharacters = I_totalVisibleCharacters;
        //Kalau udah selesai animasi teksnya, 
        ChatManager.Instance.I_amountOfTextAnimationDone++;
    }

    /// <summary>
    /// Ini untuk menghitung dalam satu frame, berapa banyak karakter yang bakal muncul dalam sebuah animasi teks.
    /// </summary>
    /// <param name="F_waitForSeconds">Semakin kecil nilainya, semakin banyak karakter yang bakal muncul</param>
    /// <returns></returns>
    private int I_CalculateCharactersPerFrame(float F_waitForSeconds)
    {
        //Agak ribet buat jelasinnya, jadi ga bakal aku jelasin. Anggap aja magic.
        int I_fps = (int)(1f / Time.deltaTime);
        float F_charactersPerFrame = 1 / (F_waitForSeconds * I_fps);
        //Kalau ternyata dalam satu frame ga nyampe satu karakter yang bakal muncul, anggap aja 1 karakter (misal kalau ternyata kecepatannya animasinya lambat)
        if (F_charactersPerFrame <= 1) return 1;

        int I_result = Mathf.FloorToInt(F_charactersPerFrame);
        float F_chanceForAdditionalCharacter = F_charactersPerFrame - I_result;

        //Cara kerja ini adalah kalau misalnya kecepatan teksnya tinggi, maka ada kemungkinan untuk nambah karakter yang bakal dimunculin sekaligus.
        //Yah, kurang lebih gitu.
        F_chanceForAdditionalCharacter = Mathf.Clamp01(F_chanceForAdditionalCharacter);
        float F_randomValue = UnityEngine.Random.Range(0f, 1f);
        if (F_chanceForAdditionalCharacter > F_randomValue)
            I_result++;

        return I_result;
    }
}
