using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.XR;
namespace GuateGeeks.AnatomyLab.UI {
// Camera-space UI for desktop; never attached to the headset in VR.
public sealed class ExplorationWorkbench:MonoBehaviour {
 public LabRuntime Lab;public DesktopControllerSimulator Controls; public InputField SearchField;
 public bool IsTyping=>SearchField!=null&&SearchField.isFocused;
 public bool IsDetailsOpen=>expanded!=null;
 public Text InfoTitle,InfoBody;public Slider ExplosionSlider;public Button GrabButton;
 Text status,hover,grip,separation;GameObject expanded;int searchPage;List<AnatomyStructure> queryResults;RectTransform root,results,detail;Button[] hits=new Button[7];AnatomyStructure[] found=new AnatomyStructure[7];
 Color cyan=AnatomyHoloTheme.Cyan,muted=AnatomyHoloTheme.Muted;float tick;
 public void Initialize(LabRuntime lab,DesktopControllerSimulator controls){Lab=lab;Controls=controls;
 var canvas=gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=lab.ViewCamera;canvas.planeDistance=.22f;canvas.sortingOrder=30;
 var scaler=gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,1000);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;gameObject.AddComponent<GraphicRaycaster>();root=new GameObject("Marco adaptable",typeof(RectTransform)).GetComponent<RectTransform>();root.SetParent(transform,false);root.anchorMin=root.anchorMax=root.pivot=new Vector2(.5f,.5f);root.sizeDelta=new Vector2(1600,1000);
 var top=Box(root,"Cabecera",20,18,1560,78);Label(top,"GUATEGEEKS  /  LABORATORIO DE ANATOMÍA",22,12,1450,28,25,cyan);Label(top,Lab.ReferenceName.ToUpperInvariant()+"  /  "+Lab.SourceName+"  /  SIN CONEXIÓN",22,46,960,20,14,muted);

 Button(top,"FUENTES",1010,30,140,34,ShowSources);
 var male=Button(top,"MASCULINO",1170,30,170,34,()=>ChangeReference("male"));male.interactable=Lab.ReferenceId!="male";
 var female=Button(top,"FEMENINO",1360,30,170,34,()=>ChangeReference("female"));female.interactable=Lab.ReferenceId!="female";
 var left=Box(root,"Navegación anatómica",20,112,350,720);Label(left,"01   EXPLORAR CAPAS",20,16,320,28,22,cyan);
 var field=Rect(left,"Búsqueda",18,61,314,42);field.gameObject.AddComponent<Image>().color=Color.clear;AnatomyHoloTheme.Decorate(field);SearchField=field.gameObject.AddComponent<InputField>();var txt=Label(field,"",12,5,288,32,18,AnatomyHoloTheme.White);var placeholder=Label(field,"Buscar nombre o ID…",12,5,288,32,18,muted);SearchField.textComponent=txt;SearchField.placeholder=placeholder;SearchField.onValueChanged.AddListener(q=>{searchPage=0;Search(q);});
 results=Rect(left,"Resultados",18,116,314,276);for(int i=0;i<hits.Length;i++){int at=i;hits[i]=Button(results,"",0,i*39,314,34,()=>{if(found[at]!=null){SearchField.DeactivateInputField();Lab.SelectById(found[at].id);}});}
 Button(left,"‹ ANTERIOR",18,393,152,26,()=>{searchPage=Mathf.Max(0,searchPage-1);Search(SearchField.text);});Button(left,"SIGUIENTE ›",180,393,152,26,()=>{if((searchPage+1)*7<queryResults.Count)searchPage++;Search(SearchField.text);});
 Label(left,"SISTEMAS ANATÓMICOS",20,428,310,22,15,muted);
 for(int i=0;i<Lab.Catalog.systems.Length;i++){var s=Lab.Catalog.systems[i];var b=Button(left,s.displayName,18+i%2*162,461+i/2*30,152,26,()=>Lab.Systems.ToggleSystem(s.id));b.GetComponentInChildren<Text>().fontSize=14;}
 detail=Box(root,"Ficha contextual",1210,112,370,720);Label(detail,"02   INFORMACIÓN",20,16,330,28,22,cyan);
 InfoTitle=Label(detail,"Selecciona una estructura",20,62,330,80,26,AnatomyHoloTheme.White);InfoBody=Label(detail,"Apunta al cuerpo y haz clic.\n\nLa ficha se actualiza aquí, sin cambiar de pestaña.\n\nPuedes buscar una pieza interna y aislarla para verla mejor.",20,150,330,202,18,muted);
 Button(detail,"AMPLIAR INFORMACIÓN",20,363,330,34,ShowDetails);
 GrabButton=Button(detail,"EXTRAER PIEZA",20,408,330,44,Controls.TakeSelected);
 Button(detail,"SOLTAR / TOMAR PIEZA",20,461,330,40,ToggleGrip);Button(detail,"DEVOLVER AL CUERPO",20,510,330,40,Return);
 Button(detail,"AISLAR",20,559,158,36,()=>{if(Lab.Isolated)Lab.ExitIsolation();else Lab.Isolate();});Button(detail,"TRANSPARENCIA",190,559,160,36,Lab.ToggleTransparency);
 Button(detail,"ENFOCAR",20,604,158,36,()=>{if(Lab.Selection.Selected!=null)Lab.FocusStructure(Lab.Selection.Selected.Data.id);});Button(detail,"OCULTAR",190,604,160,36,()=>{Controls.Release();Lab.Hide();});
 Button(detail,"RÉTAME",20,657,158,34,Lab.AskQuestion);Button(detail,"ENSAMBLAJE",190,657,160,34,()=>{Controls.Release();Lab.BeginPuzzle();});
 hover=Label(root,"SELECCIONA  /  DESCUBRE  /  EXPLORA",405,117,775,34,22,cyan);
 grip=Label(root,"Selecciona una estructura para comenzar",405,759,775,32,18,cyan);
 var dock=Box(root,"Mesa de exploración",285,852,1030,128);Label(dock,"03  /  MESA DE EXPLORACIÓN",24,13,550,24,18,cyan);
 Label(dock,"SEPARAR PIEZAS",24,47,280,24,18,AnatomyHoloTheme.White);separation=Label(dock,"0 %",612,47,100,25,18,cyan);
 ExplosionSlider=Slider(dock,24,82,675,Lab.SetExplosion);
 Button(dock,"RESTABLECER CUERPO",736,42,270,54,()=>{Controls.Release();Lab.ResetBody();ExplosionSlider.SetValueWithoutNotify(0);});
 status=Label(root,"Cargando…",405,801,775,36,15,muted);
 Lab.Selection.Changed+=Selected;Selected(Lab.Selection.Selected);Search("");
 }
 void Search(string query){queryResults=Lab.Search.Search(query,null,10000);if(string.IsNullOrWhiteSpace(query)){queryResults.Clear();foreach(var e in Lab.Education.entries){var entry=Lab.Search.Search(e.id,null,1);if(entry.Count>0)queryResults.Add(entry[0]);}queryResults.AddRange(Lab.Search.Search("femur",null,2));}var list=queryResults;for(int i=0;i<hits.Length;i++){int index=searchPage*7+i;found[i]=index<list.Count?list[index]:null;hits[i].gameObject.SetActive(found[i]!=null);if(found[i]!=null)hits[i].GetComponentInChildren<Text>().text=Lab.Name(found[i]);}}
 void Selected(StructureState state){CloseDetails();if(state==null){InfoTitle.text="Selecciona una estructura";InfoBody.text=Lab.Coverage+"\n\nSelecciona con un clic o busca una estructura. Pulsa Extraer pieza y haz clic en el espacio central para soltarla.";}else{InfoTitle.text=Lab.Name(state.Data);string system=state.Data.systemId;foreach(var s in Lab.Catalog.systems)if(s.id==system){system=s.displayName;break;}var e=Lab.Education.Find(state.Data.id);InfoBody.text=system.ToUpperInvariant()+"\n"+state.Data.id+"  ·  "+state.Data.parentStructure+"\n\n"+state.Data.description+"\n\n"+(e==null?"Referencia: "+Lab.SourceName+".\nExplora su ubicación y relación con las piezas cercanas.":e.function);}}
 public void ShowDetails(){if(expanded!=null){Destroy(expanded);expanded=null;return;}var card=Box(root,"Ficha ampliada",425,180,750,620);expanded=card.gameObject;var drag=Rect(card,"Arrastra para mover",0,0,680,60);drag.gameObject.AddComponent<Image>().color=AnatomyHoloTheme.Button;drag.gameObject.AddComponent<DraggablePanel>().Panel=card;Label(drag,"FICHA ANATÓMICA  /  ARRASTRA PARA MOVER",20,16,650,26,20,cyan);Button(card,"X",690,12,44,36,ShowDetails);Label(card,InfoTitle.text,28,88,690,110,32,AnatomyHoloTheme.White);Label(card,InfoBody.text,28,215,690,300,24,muted);Button(card,"EXTRAER PIEZA",28,554,328,44,()=>{ShowDetails();Controls.TakeSelected();});Button(card,"VOLVER A EXPLORAR",378,554,344,44,ShowDetails);}
 public void ChangeReference(string reference){Controls.Release();CloseDetails();Lab.SwitchReference(reference);}
 public void ShowSources(){CloseDetails();var card=Box(root,"Fuentes y alcance",425,180,750,620);expanded=card.gameObject;Label(card,"FUENTES Y ALCANCE",28,28,690,48,28,cyan);Label(card,Lab.ReferenceName+" · "+Lab.Catalog.structures.Length+" estructuras\n\n"+Lab.Catalog.attribution+"\n\n"+Lab.Coverage+"\n\nAdaptación: ejes y pivotes para Unity, colores por sistema y nombres de interfaz en español. Algunas traducciones específicas siguen pendientes de revisión.",28,104,690,405,22,muted);Button(card,"VOLVER A EXPLORAR",28,554,694,44,CloseDetails);}
 public void CloseDetails(){if(expanded!=null){Destroy(expanded);expanded=null;}}
 public void ToggleGrip(){if(Controls.Holding)Controls.Release();else Controls.TakeSelected();}
 public void Return(){Controls.Release();Lab.ReturnSelected();}
 void Update(){if(Lab==null||!Lab.Ready)return;tick+=Time.unscaledDeltaTime;if(tick<.1f)return;tick=0;hover.text=Lab.Selection.Hovered==null?"SELECCIONA  /  DESCUBRE  /  EXPLORA":Lab.Name(Lab.Selection.Hovered.Data);grip.text=Controls.Holding?"PIEZA EXTRAÍDA  /  Mueve el ratón y haz clic para soltar":"Clic para seleccionar  /  Amplía la ficha para consultar";separation.text=Mathf.RoundToInt(Lab.Explosion.Factor*100)+" %";status.text=Lab.Status;GrabButton.interactable=Lab.Selection.Selected!=null;ExplosionSlider.SetValueWithoutNotify(Lab.Explosion.Factor);}
 void OnDestroy(){if(Lab!=null)Lab.Selection.Changed-=Selected;}
 RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)=>LabPanel.Rect(name,parent,x,-y,w,h);
 RectTransform Box(Transform p,string name,float x,float y,float w,float h){var r=Rect(p,name,x,y,w,h);r.gameObject.AddComponent<Image>().color=Color.clear;AnatomyHoloTheme.Decorate(r,true);return r;}
 Text Label(Transform p,string value,float x,float y,float w,float h,int size,Color color){var r=Rect(p,"Texto",x,y,w,h);var t=r.gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=size;t.fontStyle=size>=20?FontStyle.Bold:FontStyle.Normal;t.color=color;t.text=value;t.raycastTarget=false;t.supportRichText=false;return t;}
 Button Button(Transform p,string name,float x,float y,float w,float h,Action action){var b=LabPanel.CreateButton(name,p,x,-y,w,h,action);b.GetComponentInChildren<Text>().fontSize=h<37?15:17;return b;}
 Slider Slider(Transform p,float x,float y,float width,UnityEngine.Events.UnityAction<float> change){var r=Rect(p,"Separación anatómica",x,y,width,28);r.gameObject.AddComponent<Image>().color=Color.clear;AnatomyHoloTheme.Decorate(r);var slider=r.gameObject.AddComponent<Slider>();var handle=Rect(r,"Control",0,0,18,28);var image=handle.gameObject.AddComponent<Image>();image.color=AnatomyHoloTheme.Amber;slider.handleRect=handle;slider.targetGraphic=image;slider.minValue=0;slider.maxValue=1;slider.onValueChanged.AddListener(change);return slider;}
}
}
