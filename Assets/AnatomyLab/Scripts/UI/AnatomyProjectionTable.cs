using System.Collections.Generic;
using GuateGeeks.AnatomyLab.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GuateGeeks.AnatomyLab.UI {
// Acabado e inscripción de la mesa, equivalentes a la mesa del laboratorio de arquitectura.
public sealed class AnatomyProjectionTable : MonoBehaviour {
 const float InscriptionRadius=1.755f;
 const string Inscription="GUATEGEEKS · LABORATORIO DE ANATOMÍA · EXPLORA · EXTRAE · EVALÚA · GUATEMALA · ";
 static readonly Vector3 Center=new Vector3(.25f,0,2.1f);
 readonly List<Material> ownedMaterials=new List<Material>();
 bool initialized;

 public void Initialize(LabRuntime lab){
  if(initialized)return;
  initialized=true;
  var metal=Resources.Load<Shader>("LabMetal");
  if(metal==null){Debug.LogError("Falta el material metálico de la mesa holográfica.");return;}
  Finish("Pedestal blindado",metal,new Color(.045f,.085f,.11f));
  Finish("Faldón metálico",metal,new Color(.10f,.17f,.22f));
  Finish("Consola de proyección",metal,new Color(.12f,.20f,.26f));
  Finish("Bisel metálico",metal,new Color(.17f,.24f,.29f));
  Inscribe(lab.ViewCamera);
  for(int i=0;i<3;i++)EnergyRing(i);
 }

 void Finish(string name,Shader shader,Color tint){
  var part=transform.Find(name);if(part==null)return;
  var material=new Material(shader){name=name+" · titanio"};material.SetColor("_Color",tint);
  ownedMaterials.Add(material);part.GetComponent<Renderer>().sharedMaterial=material;
 }

 void Inscribe(Camera viewCamera){
  var root=new GameObject("Inscripción circular de anatomía",typeof(RectTransform),typeof(Canvas));
  var rect=root.GetComponent<RectTransform>();rect.SetParent(transform,false);
  rect.localPosition=Center+Vector3.up*.748f;
  rect.localRotation=Quaternion.Euler(90,0,0);
  rect.localScale=Vector3.one*.001f;
  rect.sizeDelta=new Vector2(3800,3800);
  var canvas=root.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=viewCamera;
  var font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var color=Color.Lerp(new Color(.20f,.73f,.57f),new Color(.04f,.75f,.90f),.45f);color.a=.85f;
  string text=Inscription+Inscription;float step=360f/text.Length;
  for(int i=0;i<text.Length;i++){
   if(text[i]==' ')continue;
   float theta=180-i*step,a=theta*Mathf.Deg2Rad;
   var glyph=new GameObject("Grabado " + i,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));
   var glyphRect=glyph.GetComponent<RectTransform>();glyphRect.SetParent(rect,false);
   glyphRect.sizeDelta=new Vector2(70,70);
   glyphRect.anchoredPosition=new Vector2(Mathf.Sin(a),Mathf.Cos(a))*(InscriptionRadius*1000);
   glyphRect.localRotation=Quaternion.Euler(0,0,180-theta);
   var label=glyph.GetComponent<Text>();label.font=font;label.fontSize=50;label.alignment=TextAnchor.MiddleCenter;
   label.color=color;label.text=text[i].ToString();label.raycastTarget=false;
  }
 }

 void EnergyRing(int index){
  var root=new GameObject(index==0?"Aro inferior de energía":"Aro de energía "+(index+1));
  root.transform.SetParent(transform,false);
  var line=root.AddComponent<LineRenderer>();line.useWorldSpace=false;line.loop=true;
  line.positionCount=128;line.startWidth=line.endWidth=index==1?.012f:.007f;
  var shader=Resources.Load<Shader>("LabUnlit");
  if(shader==null)shader=Shader.Find("Universal Render Pipeline/Unlit");
  var material=new Material(shader){name="Luz quetzal del pedestal"};
  var color=index==1?new Color(.11f,.62f,.53f):new Color(.05f,.29f,.34f);
  material.SetColor(shader.name=="GuateGeeks/LabUnlit"?"_Color":"_BaseColor",color);
  ownedMaterials.Add(material);line.sharedMaterial=material;
  float height=.16f+index*.15f;
  for(int i=0;i<128;i++){float angle=i*Mathf.PI*2/128;line.SetPosition(i,Center+new Vector3(Mathf.Sin(angle)*1.38f,height,Mathf.Cos(angle)*1.38f));}
 }

 void OnDestroy(){foreach(var material in ownedMaterials)if(material!=null)Destroy(material);}
}
}
