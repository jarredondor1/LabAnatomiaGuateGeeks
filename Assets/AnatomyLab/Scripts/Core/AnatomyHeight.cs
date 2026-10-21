using UnityEngine;

namespace GuateGeeks.AnatomyLab.Core {
// Escala el catálogo completo, no solo las piezas visibles, para conservar una altura estable al cambiar capas.
public static class AnatomyHeight {
 public const int MinimumCm=130;
 public const int MaximumCm=210;
 public const int DefaultCm=175;

 public static Vector2 Bounds(AnatomyCatalog catalog){
  float bottom=float.PositiveInfinity,top=float.NegativeInfinity;
  foreach(var part in catalog.structures){
   float center=part.originalPosition.y+part.bounds.center.y*part.originalScale.y;
   float extent=part.bounds.extents.y*Mathf.Abs(part.originalScale.y);
   bottom=Mathf.Min(bottom,center-extent);top=Mathf.Max(top,center+extent);
  }
  return new Vector2(bottom,top);
 }
 public static float ScaleFor(AnatomyCatalog catalog,int centimeters){
  var bounds=Bounds(catalog);
  return Mathf.Clamp(centimeters,MinimumCm,MaximumCm)*.01f/Mathf.Max(.01f,bounds.y-bounds.x);
 }
 public static float BaseY(AnatomyCatalog catalog,int centimeters){
  return -Bounds(catalog).x*ScaleFor(catalog,centimeters);
 }
}
}
