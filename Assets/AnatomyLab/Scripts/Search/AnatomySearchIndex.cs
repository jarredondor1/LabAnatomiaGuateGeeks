using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Search {
public sealed class AnatomySearchIndex {
 readonly AnatomyStructure[] entries; readonly string[] terms;
 static readonly string[] Featured={"Corazón","Cerebro","Estómago","Fémur","Pulmón","Hígado","Riñón","Útero","Ovario","Páncreas","Tráquea","Aorta"};
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
 public List<AnatomyStructure> Discover(string query,string category="todos",int limit=50){
  var normalized=Normalize(query).Trim();var words=normalized.Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);
  var ranked=new List<KeyValuePair<int,AnatomyStructure>>();
  for(int i=0;i<entries.Length;i++){
   var part=entries[i];if(!InCategory(part.systemId,category))continue;
   var name=Normalize(part.displayName);bool named=!name.StartsWith("estructura ·")&&!name.StartsWith("estructura ");
   if(words.Length==0&&!named)continue;
   bool match=true;foreach(var word in words)if(!terms[i].Contains(word)){match=false;break;}if(!match)continue;
   int rank=named?200:0;
   if(words.Length>0){if(name==normalized)rank+=600;else if(name.StartsWith(normalized))rank+=350;else if(name.Contains(normalized))rank+=170;}
   for(int j=0;j<Featured.Length;j++)if(name.StartsWith(Normalize(Featured[j]))){rank+=500-j*12;break;}
   ranked.Add(new KeyValuePair<int,AnatomyStructure>(rank,part));
  }
  ranked.Sort((a,b)=>{int byRank=b.Key.CompareTo(a.Key);return byRank!=0?byRank:string.Compare(a.Value.displayName,b.Value.displayName,StringComparison.CurrentCultureIgnoreCase);});
  var found=new List<AnatomyStructure>();for(int i=0;i<ranked.Count&&found.Count<limit;i++)found.Add(ranked[i].Value);return found;
 }
 static bool InCategory(string system,string category){
  switch(Normalize(category)){
   case "huesos":return system=="skeleton";
   case "musculos":return system=="muscular";
   case "vasos":return system=="arterial"||system=="venous"||system=="cardiovascular";
   case "nervios":return system=="nervous";
   case "reproductor":return system=="reproductive";
   case "respiratorio":return system=="respiratory";
   case "urinario":return system=="urinary";
   case "organos":return system!="skeleton"&&system!="muscular"&&system!="arterial"&&system!="venous"&&system!="nervous";
   default:return true;
  }
 }
}
}
