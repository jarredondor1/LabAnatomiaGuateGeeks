using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using GuateGeeks.AnatomyLab.UI;
using System.Reflection;

namespace GuateGeeks.AnatomyLab.Tests {
public class HoloVisualTests {
 [Test] public void SharedPaletteMatchesArchitectReference(){
  Assert.AreEqual("5CE1F2",ColorUtility.ToHtmlStringRGB(AnatomyHoloTheme.Cyan));
  Assert.AreEqual("FFB24F",ColorUtility.ToHtmlStringRGB(AnatomyHoloTheme.Amber));
  var green=typeof(AnatomyHoloTheme).GetField("Green");Assert.NotNull(green,"Falta el color de estado activo de la referencia.");
  Assert.AreEqual("7CF0BD",ColorUtility.ToHtmlStringRGB((Color)green.GetValue(null)));
 }

 [Test] public void GlassSupportsArchitectHoverAndQuietSecondaryActions(){
  var type=typeof(AnatomyHoloGraphic);
  Assert.NotNull(type.GetProperty("Highlight"),"El vidrio debe reaccionar al puntero o rayo.");
  Assert.NotNull(type.GetProperty("Ghost"),"Las acciones secundarias deben ser discretas hasta recibir el puntero.");
  Assert.NotNull(type.GetProperty("Reveal"),"Los paneles deben poder materializarse con un barrido.");
 }

 [Test] public void ArchitectEnvironmentAndBrandAssetsAreAvailable(){
  Assert.NotNull(Resources.Load<Shader>("LabPanorama"));
  Assert.NotNull(Resources.Load<Shader>("LabGrid"));
  Assert.NotNull(Resources.Load<Shader>("LabCanopy"));
  Assert.NotNull(Resources.Load<Shader>("LabShaft"));
  Assert.NotNull(Resources.Load<Shader>("GeekEyesAnimated"));
  Assert.NotNull(Resources.Load<Texture2D>("Branding/GeekEyes"));
  Assert.NotNull(Resources.Load<Texture2D>("Branding/GuateGeeksFull"));
 }

 [Test] public void EnvironmentFallbackUsesShaderPackagedForQuest(){
  var go=new GameObject("Prueba de entorno");
  Material material=null;
  try{
   var environment=go.AddComponent<AnatomyStarkEnvironment>();
   var method=typeof(AnatomyStarkEnvironment).GetMethod("Mat",BindingFlags.Instance|BindingFlags.NonPublic);
   material=(Material)method.Invoke(environment,new object[]{null,Color.cyan});
   Assert.AreEqual("GuateGeeks/LabUnlit",material.shader.name);
   Assert.AreEqual(Color.cyan,material.GetColor("_Color"));
  }finally{Object.DestroyImmediate(go);}
 }

 [Test] public void StageIndicatorsUseShaderPackagedForQuest(){
  var method=typeof(AnatomyStageVisuals).GetMethod("Material",BindingFlags.Static|BindingFlags.NonPublic);
  var material=(Material)method.Invoke(null,new object[]{Color.cyan});
  try{
   Assert.AreEqual("GuateGeeks/LabUnlit",material.shader.name);
   Assert.AreEqual(Color.cyan,material.GetColor("_Color"));
  }finally{Object.DestroyImmediate(material);}
 }

 [Test] public void GlassClipsCornersAndNeverCapturesButtonClicks(){
  var go=new GameObject("Panel",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));
  try{
   var rect=go.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(200,100);
   var image=go.GetComponent<Image>();image.raycastTarget=true;
   var glass=AnatomyHoloTheme.Decorate(rect,true);
   Assert.IsFalse(glass.raycastTarget);
   Assert.IsTrue(image.raycastTarget);
   Assert.AreEqual(rect,glass.transform.parent);
   var mesh=new VertexHelper();
   typeof(AnatomyHoloGraphic).GetMethod("OnPopulateMesh",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(VertexHelper)},null).Invoke(glass,new object[]{mesh});
   Assert.Greater(mesh.currentVertCount,35);
   var corner=new UIVertex();mesh.PopulateUIVertex(ref corner,1);
   Assert.Greater(corner.position.x,rect.rect.xMin,"El vidrio debe recortar la esquina inferior izquierda.");
   Assert.AreEqual(rect.rect.yMin,corner.position.y,0.01f);
   mesh.Dispose();
  }finally{Object.DestroyImmediate(go);}
 }
}
}
