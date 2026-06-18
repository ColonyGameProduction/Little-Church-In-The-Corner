using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GoogleSheetImporter
{
    static string itemSheetURL = "https://docs.google.com/spreadsheets/d/1W4u67wi3bEByBzH5DQ_lrYXkrnPFuEK61HCJQ3B6SXE/export?format=csv&gid=0";
    static string bookSheetURL = "https://docs.google.com/spreadsheets/d/1W4u67wi3bEByBzH5DQ_lrYXkrnPFuEK61HCJQ3B6SXE/export?format=csv&gid=1418541123";
    static string passageSheetURL = "https://docs.google.com/spreadsheets/d/1W4u67wi3bEByBzH5DQ_lrYXkrnPFuEK61HCJQ3B6SXE/export?format=csv&gid=440482099";
    static string verseDataURL = "https://docs.google.com/spreadsheets/d/1W4u67wi3bEByBzH5DQ_lrYXkrnPFuEK61HCJQ3B6SXE/export?format=csv&gid=680206945";

    static List<string[]> ParseCSV(string text)
    {
        List<string[]> rows = new List<string[]>();

        List<string> currentRow = new List<string>();
        string currentField = "";

        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                currentRow.Add(currentField);
                currentField = "";
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (currentField.Length > 0 || currentRow.Count > 0)
                {
                    currentRow.Add(currentField);
                    rows.Add(currentRow.ToArray());
                    currentRow = new List<string>();
                    currentField = "";
                }
            }
            else
            {
                currentField += c;
            }
        }

        // Add last row
        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField);
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }


    [MenuItem("Tools/Import Items From Google Sheets")]
    public static void ImportItemData()
    {
        WebClient client = new WebClient();
        string csv = client.DownloadString(itemSheetURL);

        Debug.Log(csv);

        string[] lines = csv.Split('\n');


        string pathAllItem = $"Assets/SO/AllItemSO.asset";
        string pathAllSetsItem = $"Assets/SO/AllSetsItemSO.asset";
        AllItemSO allItem = AssetDatabase.LoadAssetAtPath<AllItemSO>(pathAllItem);
        AllSetsItemSO allSetsItem = AssetDatabase.LoadAssetAtPath<AllSetsItemSO>(pathAllSetsItem);

        if(allItem == null)
        {
            allItem = ScriptableObject.CreateInstance<AllItemSO>();
            AssetDatabase.CreateAsset(allItem, pathAllItem);
        }

        if(allSetsItem == null)
        {
            allSetsItem = ScriptableObject.CreateInstance<AllSetsItemSO>();
            AssetDatabase.CreateAsset (allSetsItem, pathAllSetsItem);
        }

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            string itemID = values[1];
            string setID = values[2];
            string setName = values[3];
            string itemName = values[4];
            string itemType = values[5];
            string itemRarity = values[6];
            string itemPrice = values[7];

            string pathItem = $"Assets/SO/itemSO/{itemID}.asset";
            string pathSetItem = $"Assets/SO/SetItemSO/{setID}.asset";

            setItemSO setItem = AssetDatabase.LoadAssetAtPath<setItemSO>(pathSetItem);

            if (setItem == null)
            {
                setItem = ScriptableObject.CreateInstance<setItemSO>();
                AssetDatabase.CreateAsset(setItem, pathSetItem);
            }

            setItem.setID = setID;
            setItem.setName = setName;

            ItemSO item = AssetDatabase.LoadAssetAtPath<ItemSO>(pathItem);

            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemSO>();
                AssetDatabase.CreateAsset(item, pathItem);
            }

            item.itemID = itemID;
            if (System.Enum.TryParse(itemRarity, true, out E_ItemRarity rarity))
            {
                item.itemRarity = rarity;
            }
            else
            {
                Debug.LogError("Invalid rarity: " + itemRarity);
            }

            item.itemType = itemType;
            item.itemName = itemName;
            if (int.TryParse(itemPrice, out int number))
            {
                item.itemPrice = number;
            }
            else
            {
                Debug.LogError("Invalid number: " + itemPrice);
            }


            if (!setItem.items.Contains(item))
            {
                setItem.items.Add(item);
            }

            if(!allItem.items.Contains(item))
            {
                allItem.items.Add(item);
            }

            if(!allSetsItem.items.Contains(setItem) && setItem != null)
            {
                allSetsItem.items.Add(setItem);
            }
            EditorUtility.SetDirty(item);
            EditorUtility.SetDirty(setItem);
        }

        EditorUtility.SetDirty(allItem);
        EditorUtility.SetDirty(allSetsItem);
        AssetDatabase.SaveAssets();
        Debug.Log("Import Complete!");
    }

    [MenuItem("Tools/Import Passage From Google Sheets")]
    public static void ImportPassageData()
    {
        WebClient client = new WebClient();
        string bookCsv = client.DownloadString(bookSheetURL);
        string passageCsv = client.DownloadString(passageSheetURL);
        string verseCsv = client.DownloadString(verseDataURL);

        //Debug.Log(bookCsv);
        //Debug.Log(passageCsv);
        //Debug.Log(verseCsv);

        string[] bookLines = bookCsv.Split('\n');
        string[] passageLines = passageCsv.Split('\n');
        List<string[]> verseRows = ParseCSV(verseCsv);

        string pathBibleData = $"Assets/SO/AllVerseSO.asset";
        AllVerseSO allVerseSO = AssetDatabase.LoadAssetAtPath<AllVerseSO>(pathBibleData);

        Dictionary<string, Books> bookMap = new Dictionary<string, Books>();
        Dictionary<string, Passage> passageMap = new Dictionary<string, Passage>();
        Dictionary<string, PageSet> pageMap = new Dictionary<string, PageSet>();

        if (allVerseSO == null)
        {
            allVerseSO = ScriptableObject.CreateInstance<AllVerseSO>();
            AssetDatabase.CreateAsset(allVerseSO, pathBibleData);
        }


        // =========================
        // 1. BOOKS
        // =========================



        foreach (var line in bookLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = line.Split(',');

            string bookID = values[0];
            string bookName = values[1];

            if (!bookMap.ContainsKey(bookID))
            {
                Books book = new Books
                {
                    bookID = bookID,
                    book = bookName,
                    passages = new List<Passage>()
                };

                bookMap.Add(bookID, book);
            }
        }

        Debug.Log("Books Done");

        // =========================
        // 2. PASSAGES
        // =========================
        foreach (var line in passageLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = line.Split(',');

            string passageID = values[0];
            string bookID = values[1];
            string title = values[2];

            if (!passageMap.ContainsKey(passageID))
            {
                Passage passage = new Passage
                {
                    passageID = passageID,
                    title = title,
                    pages = new List<PageSet>()
                };

                passageMap.Add(passageID, passage);

                // Link to Book
                if (bookMap.ContainsKey(bookID))
                {
                    bookMap[bookID].passages.Add(passage);
                }
            }
        }

        Debug.Log("Passages Done");

        // =========================
        // 3. PAGES
        // =========================
        foreach (var values in verseRows)
        {

            if (values.Length < 2) continue;


            string passageID = values[1];
            string pageID = values[2] + passageID;

            if (!pageMap.ContainsKey(pageID))
            {
                PageSet page = new PageSet
                {
                    pageID = pageID,
                    verses = new List<Verse>()
                };

                pageMap.Add(pageID, page);

                // Link to Passage
                if (passageMap.ContainsKey(passageID))
                {
                    passageMap[passageID].pages.Add(page);
                }
            }
        }

        Debug.Log("Pages Done");

        // =========================
        // 4. VERSES
        // =========================
        string verseTextNow = string.Empty;
        string pastPageID = string.Empty;
        foreach (var values in verseRows)
        {
            if (values.Length < 2) continue;

            string passageID = values[1];
            string pageID = values[2] + passageID;
            string verseText = values[4];

            if (pageMap.ContainsKey(pageID))
            {
                pageMap[pageID].verses.Add(new Verse
                {
                    verse = verseText
                });
            }
        }

        // =========================
        // FINAL ASSIGNMENT
        // =========================
        allVerseSO.books = new List<Books>(bookMap.Values);

        Debug.Log("Import Complete!");




        //for (int i = 1; i < bookLines.Length; i++)
        //{
        //    if (string.IsNullOrWhiteSpace(bookLines[i])) continue;

        //    string[] bookValues = bookLines[i].Split(',');

        //    string books = bookValues[1];
        //    string booksID = bookValues[2];

        //    Books currBooks;

        //    // Check if index already exists
        //    if (i < allVerseSO.books.Count)
        //    {
        //        currBooks = allVerseSO.books[i];

        //        // If somehow null, replace it
        //        if (currBooks == null)
        //        {
        //            currBooks = new Books();
        //            allVerseSO.books[i] = currBooks;
        //        }
        //    }
        //    else
        //    {
        //        // Create new if list not long enough
        //        currBooks = new Books();
        //        allVerseSO.books.Add(currBooks);
        //    }

        //    currBooks.book = books;
        //    currBooks.bookID = booksID;

        //    for (int j = 1; j < passageLines.Length; j++)
        //    {
        //        if (string.IsNullOrWhiteSpace(passageLines[j])) continue;


        //        string[] passageValues = passageLines[j].Split(',');
        //        if (booksID != passageValues[1])
        //        {
        //            continue;
        //        }

        //        string passage = passageValues[2];
        //        string passageID = passageValues[0];

        //        Passage currPassage;

        //        // Check if index already exists
        //        if (i < allVerseSO.books[i].passages.Count)
        //        {
        //            currPassage = allVerseSO.books[i].passages[j];

        //            // If somehow null, replace it
        //            if (currPassage == null)
        //            {
        //                currPassage = new Passage();
        //                allVerseSO.books[i].passages[j] = currPassage;
        //            }
        //        }
        //        else
        //        {
        //            // Create new if list not long enough
        //            currPassage = new Passage();
        //            allVerseSO.books[i].passages.Add(currPassage);
        //        }

        //        currPassage.title = books;
        //        currPassage.passageID = passageID;
        //    }
        //}
    }

    
}