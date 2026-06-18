using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AllVerseSO))]
public class BibbleCustomEditor : EditorWindow
{
    AllVerseSO db;
    Vector2 scroll;

    [MenuItem("Tools/Bibble Custom Editor")]
    static void Open()
    {
       GetWindow<BibbleCustomEditor>();
    }

    void OnGUI()
    {
        db = (AllVerseSO)EditorGUILayout.ObjectField("Database", db, typeof(AllVerseSO), false);

        if (db == null) return;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Book", GUILayout.Width(120));
        GUILayout.Label("Passage", GUILayout.Width(100));
        GUILayout.Label("PageSet", GUILayout.Width(100));
        GUILayout.Label("Verse", GUILayout.Width(400));
        EditorGUILayout.EndHorizontal();

        float y = 40;
        float rowHeight = 20;

        foreach (var book in db.books)
        {
            int bookRows = book.passages.Sum(c => c.pages.Sum(d => d.verses.Count));
            float bookHeight = bookRows * rowHeight;

            Rect bookRect = new Rect(10, y, 100, bookHeight);
            book.book = EditorGUI.TextField(bookRect, book.book);

            foreach (var passage in book.passages)
            {
                int passageRows = passage.pages.Sum(d => d.verses.Count);
                float passageHeight = passageRows * rowHeight;

                Rect passageRect = new Rect(110, y, 100, passageHeight);
                passage.title = EditorGUI.TextField(passageRect, passage.title);

                foreach (var page in passage.pages)
                {
                    int pageSetRows = page.verses.Count;
                    float pageSetHeight = pageSetRows * rowHeight;

                    Rect pageSetRect = new Rect(210, y, 100, pageSetHeight);
                    page.pageID = EditorGUI.TextField(pageSetRect, page.pageID);

                    foreach(var verse in page.verses)
                    {
                        Rect verseRect = new Rect(310, y, 400, rowHeight);
                        verse.verse = EditorGUI.TextField(verseRect, verse.verse);
                        y += rowHeight;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
