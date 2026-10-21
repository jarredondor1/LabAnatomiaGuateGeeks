using UnityEngine;
using UnityEngine.UI;

namespace GuateGeeks.AnatomyLab.UI {
// Apariencia compartida con el laboratorio AWS VR Architect de GuateGeeks.
public static class AnatomyHoloTheme {
 public static readonly Color Cyan=ColorFrom("#5CE1F2");
 public static readonly Color Amber=ColorFrom("#FFB24F");
 public static readonly Color Green=ColorFrom("#7CF0BD");
 public static readonly Color Muted=ColorFrom("#8FA9BC");
 public static readonly Color White=ColorFrom("#E8F7FF");
 public static readonly Color Panel=new Color(.018f,.06f,.088f,.90f);
 public static readonly Color Button=ColorFrom("#0D2B3BCC");
 public static readonly Color Grid=ColorFrom("#1F4B5E");

 static Color ColorFrom(string hex){ColorUtility.TryParseHtmlString(hex,out var value);return value;}

 // La imagen original recibe los clics; la malla solo dibuja.
 public static AnatomyHoloGraphic Decorate(RectTransform host,bool detailed=false,bool warm=false){
  var existing=host.GetComponentInChildren<AnatomyHoloGraphic>(true);
  if(existing!=null&&existing.transform.parent==host){existing.Detailed=detailed;existing.Accent=warm?Amber:Cyan;existing.color=detailed?Panel:Button;existing.SetVerticesDirty();return existing;}
  var go=new GameObject("Vidrio holográfico",typeof(RectTransform),typeof(CanvasRenderer));
  var rect=go.GetComponent<RectTransform>();rect.SetParent(host,false);rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=Vector2.zero;rect.offsetMax=Vector2.zero;rect.SetAsFirstSibling();
  var graphic=go.AddComponent<AnatomyHoloGraphic>();graphic.color=detailed?Panel:Button;graphic.Accent=warm?Amber:Cyan;graphic.Detailed=detailed;graphic.raycastTarget=false;
  return graphic;
 }
}
}
