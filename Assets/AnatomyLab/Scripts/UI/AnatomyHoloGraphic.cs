using UnityEngine;
using UnityEngine.UI;

namespace GuateGeeks.AnatomyLab.UI {
// Una sola malla dibuja el vidrio recortado, los bordes y las marcas luminosas.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class AnatomyHoloGraphic:MaskableGraphic {
 public Color Accent=AnatomyHoloTheme.Cyan;
 public bool Detailed=true;
 float highlight,reveal=1;
 bool ghost;
 public float Highlight {get=>highlight;set{float next=Mathf.Clamp01(value);if(Mathf.Approximately(next,highlight))return;highlight=next;SetVerticesDirty();}}
 public float Reveal {get=>reveal;set{float next=Mathf.Clamp01(value);if(Mathf.Approximately(next,reveal))return;reveal=next;SetVerticesDirty();}}
 public bool Ghost {get=>ghost;set{if(ghost==value)return;ghost=value;SetVerticesDirty();}}

 protected override void OnPopulateMesh(VertexHelper mesh){
  mesh.Clear();var r=rectTransform.rect;if(r.width<=0||r.height<=0)return;
  float cut=Mathf.Min(Detailed?20:9,r.width*.18f,r.height*.3f);
  var points=new[]{new Vector2(r.xMin+cut,r.yMin),new Vector2(r.xMax,r.yMin),new Vector2(r.xMax,r.yMax-cut),new Vector2(r.xMax-cut,r.yMax),new Vector2(r.xMin,r.yMax),new Vector2(r.xMin,r.yMin+cut)};
  float shown=Mathf.SmoothStep(0,1,reveal);var fill=color;fill.a*=shown;
  mesh.AddVert(r.center,fill,Vector2.zero);
  for(int i=0;i<points.Length;i++){
   var shade=Color.Lerp(fill,Accent,.07f+.08f*highlight)*Mathf.Lerp(.72f,1f,Mathf.InverseLerp(r.yMin,r.yMax,points[i].y));shade.a=fill.a*(ghost?.55f:1f);mesh.AddVert(points[i],shade,Vector2.zero);
  }
  for(int i=0;i<points.Length;i++)mesh.AddTriangle(0,i+1,(i+1)%points.Length+1);
  var glow=Accent;glow.a=(Detailed?.10f:ghost?0f:.05f)+.22f*highlight;glow.a*=shown;
  if(glow.a>.004f)for(int i=0;i<points.Length;i++)Glow(mesh,points[i],points[(i+1)%points.Length],Detailed?16f:7f,glow);
  var edge=Accent;edge.a=(ghost?.16f:Detailed?.42f:.34f)+.5f*highlight;
  for(int i=0;i<points.Length;i++)if(!ghost||i==0||highlight>.01f)Segment(mesh,points[i],points[(i+1)%points.Length],1.2f+highlight*.6f,edge);
  float corner=Mathf.Min(44,r.width*.22f,r.height*.44f);
  var bracket=Accent;bracket.a=Detailed?.95f:ghost?.45f*highlight:.55f+.45f*highlight;
  if(bracket.a>.01f){float opening=highlight*5f;
   Segment(mesh,new Vector2(r.xMin-opening,r.yMax-corner),new Vector2(r.xMin-opening,r.yMax+opening),2.8f,bracket);
   Segment(mesh,new Vector2(r.xMin-opening,r.yMax+opening),new Vector2(r.xMin+corner,r.yMax+opening),2.8f,bracket);
   Segment(mesh,new Vector2(r.xMax-corner,r.yMin-opening),new Vector2(r.xMax+opening,r.yMin-opening),2.8f,bracket);
  }
  var inset=Accent;inset.a=.14f;
  Segment(mesh,new Vector2(r.xMin+6,r.yMin+12),new Vector2(r.xMin+6,r.yMax-12),1f,inset);
  Segment(mesh,new Vector2(r.xMin+12,r.yMax-6),new Vector2(r.xMax-12,r.yMax-6),1f,inset);
  var warm=AnatomyHoloTheme.Amber;warm.a=.82f;
  for(int i=0;i<3;i++)Segment(mesh,new Vector2(r.xMin+18+i*9,r.yMin+5),new Vector2(r.xMin+23+i*9,r.yMin+5),2f,warm);
  if(reveal<1){float y=Mathf.Lerp(r.yMax,r.yMin,shown);var sweep=Accent;sweep.a=.9f*(1-shown*shown);Segment(mesh,new Vector2(r.xMin-6,y),new Vector2(r.xMax+6,y),2.2f,sweep);}
  if(!Detailed)return;
  var scan=Accent;scan.a=.032f;
  for(float y=r.yMin+26;y<r.yMax-20;y+=26)Segment(mesh,new Vector2(r.xMin+10,y),new Vector2(r.xMax-10,y),.7f,scan);
  for(int i=0;i<7;i++)Segment(mesh,new Vector2(r.xMax-11,r.yMax-45-i*9),new Vector2(r.xMax-5,r.yMax-45-i*9),1.6f,edge);
 }

 static void Glow(VertexHelper mesh,Vector2 a,Vector2 b,float width,Color tint){
  var direction=b-a;if(direction.sqrMagnitude<.0001f)return;
  var outward=new Vector2(direction.y,-direction.x).normalized*width;var clear=tint;clear.a=0;int start=mesh.currentVertCount;
  mesh.AddVert(a,tint,Vector2.zero);mesh.AddVert(b,tint,Vector2.zero);mesh.AddVert(b+outward,clear,Vector2.zero);mesh.AddVert(a+outward,clear,Vector2.zero);
  mesh.AddTriangle(start,start+1,start+2);mesh.AddTriangle(start,start+2,start+3);
 }

 static void Segment(VertexHelper mesh,Vector2 a,Vector2 b,float width,Color tint){
  var direction=b-a;if(direction.sqrMagnitude<.0001f)return;
  var normal=new Vector2(-direction.y,direction.x).normalized*(width*.5f);int start=mesh.currentVertCount;
  mesh.AddVert(a-normal,tint,Vector2.zero);mesh.AddVert(a+normal,tint,Vector2.zero);
  mesh.AddVert(b+normal,tint,Vector2.zero);mesh.AddVert(b-normal,tint,Vector2.zero);
  mesh.AddTriangle(start,start+1,start+2);mesh.AddTriangle(start,start+2,start+3);
 }
}
}
