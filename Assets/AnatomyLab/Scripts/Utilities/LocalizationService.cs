using System;
using System.Collections.Generic;
using UnityEngine;
namespace GuateGeeks.AnatomyLab.Utilities {
public static class LocalizationService {
 [Serializable] public class Entry {public string es,en;}
 [Serializable] public class Table {public Entry[] entries;}
 static Dictionary<string,Entry> lookup;
 public static string Translate(string value,string language){if(lookup==null){lookup=new Dictionary<string,Entry>();var data=Resources.Load<TextAsset>("Anatomy/localization");if(data!=null)foreach(var e in JsonUtility.FromJson<Table>(data.text).entries){lookup[e.es]=e;lookup[e.en]=e;}}return lookup.TryGetValue(value,out var entry)?(language=="en"?entry.en:entry.es):value;}
}
}
