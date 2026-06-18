using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Books
{
    public string bookID;
    public string book;
    public List<Passage> passages;
}
[Serializable]
public class Passage
{
    public string passageID;
    public string title;
    public List<PageSet> pages;
}
[Serializable]
public class PageSet
{
    public string pageID;
    public List<Verse> verses;
}
[Serializable]
public class Verse
{
    public string verse;
}
