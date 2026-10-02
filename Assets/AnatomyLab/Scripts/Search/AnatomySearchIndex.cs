using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Search {
public sealed class AnatomySearchIndex {
 readonly AnatomyStructure[] entries; readonly string[] terms;
 public AnatomySearchIndex(AnatomyCatalog catalog) {
 entries=catalog.structures; terms=new string[entries.Length];
 for(int i=0;i<entries.Length;i++) {var d=entries[i]; terms[i]=Normalize(d.id+" "+d.canonicalName+" "+d.displayName+" "+d.latinName+" "+string.Join(" ",d.synonyms)+" "+string.Join(" ",d.searchableTerms));}
 }
 public static string Normalize(string value) {
 var b=new StringBuilder(); foreach(char c in (value??"").Normalize(NormalizationForm.FormD)) if(CharUnicodeInfo.GetUnicodeCategory(c)!=UnicodeCategory.NonSpacingMark) b.Append(char.ToLowerInvariant(c)); return b.ToString();
 }
 public List<AnatomyStructure> Search(string query, string system=null, int limit=30) {
 var found=new List<AnatomyStructure>(); var words=Normalize(query).Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);
 for(int i=0;i<entries.Length && found.Count<limit;i++) {
 if(system!=null && entries[i].systemId!=system) continue; bool match=true;
 foreach(var word in words) if(!terms[i].Contains(word)){match=false;break;}
 if(match)found.Add(entries[i]);
 } return found;
 }
}
}
