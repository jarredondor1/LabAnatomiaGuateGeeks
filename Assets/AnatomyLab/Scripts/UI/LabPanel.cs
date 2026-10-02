using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Utilities;
namespace GuateGeeks.AnatomyLab.UI {
public sealed class LabPanel : MonoBehaviour {
 public LabRuntime Lab;public Transform Content;public Text Status,Detail,Tutorial;public GameObject QuickPanel;
 public string Page="ANATOMÍA"; public string Query="";int resultPage; Font font;
 public bool SearchActive=>Page=="BUSCAR";int row;bool initialized;
 static readonly string[] tutorial={"1/5 · Apunta a una estructura","2/5 · Selecciona con el gatillo o clic","3/5 · Pulsa EXTRAER","4/5 · Pulsa DEVOLVER","5/5 · Prueba EXPLODED VIEW"};
 IEnumerator Start(){font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");while(!Lab.Ready)yield return null;initialized=true;Lab.Selection.Changed+=_=>{if(Page=="ESTRUCTURA"||Page=="APRENDER")Render();};if(Keyboard.current!=null)Keyboard.current.onTextInput+=Typed;Render();}
 void OnDestroy(){if(Keyboard.current!=null)Keyboard.current.onTextInput-=Typed;}
 void Typed(char c){if(SearchActive&&!char.IsControl(c)&&Query.Length<80){Query+=c;resultPage=0;Render();}}
 void Update(){if(!initialized)return;Status.text=Lab.Status;Detail.text=Lab.Selection.Hovered==null?"Apunta para identificar":Lab.Name(Lab.Selection.Hovered.Data);Tutorial.text=Lab.TutorialDone?"":tutorial[Mathf.Clamp(Lab.TutorialStep,0,4)];if(SearchActive&&Keyboard.current!=null&&Keyboard.current.backspaceKey.wasPressedThisFrame){if(Query.Length>0)Query=Query.Substring(0,Query.Length-1);Render();}}
 public void Open(string page){Page=page;Render();}
 public void ToggleQuick(){QuickPanel.SetActive(!QuickPanel.activeSelf);}
 public void Render(){if(!initialized)return;foreach(Transform child in Content)Destroy(child.gameObject);row=0;
 switch(Page){
 case "ANATOMÍA":
 Label("ATLAS HUMANO\nBodyParts3D · referencia masculina",60,26);
 Label("Explora 2,234 piezas independientes.\nElige un sistema o busca una estructura.",58);
 Button("EXPLORAR SISTEMAS",()=>Open("SISTEMAS"));Button("BUSCAR ESTRUCTURA",()=>Open("BUSCAR"));Button("SELECCIÓN Y ACCIONES",()=>Open("ESTRUCTURA"));
 Button("CENTRAR CUERPO",Lab.CenterBody);Button("RESTABLECER CUERPO",Lab.ResetBody);Button("SALTAR TUTORIAL",Lab.SkipTutorial);
 Label("BodyParts3D, © The Database Center for Life Science\nCC Attribution 4.0 International\nContenido educativo de referencia.",74,16);break;
 case "SISTEMAS":
 Label("CAPAS ANATÓMICAS",38,24);
 for(int i=0;i<Lab.Catalog.systems.Length;i++){var s=Lab.Catalog.systems[i];Button((Lab.Systems.Visible[s.id]?"●  ":"○  ")+s.displayName+"  ·  "+s.structures.Length,()=>{Lab.Systems.ToggleSystem(s.id);Lab.Feedback();Render();},30);}
 Button("MOSTRAR TODO",()=>{Lab.Systems.ShowAll();Render();},34);Button("OCULTAR TODO",()=>{Lab.Systems.HideAll();Render();},34);break;
 case "BUSCAR":
 Label("BUSCAR  /  "+(Query.Length==0?"nombre, sinónimo o ID":Query),44,22);
 var results=Lab.Search.Search(Query,null,10000);Label(results.Count+" resultados · página "+(resultPage+1),30,18);
 for(int i=resultPage*6;i<Mathf.Min(results.Count,resultPage*6+6);i++){var d=results[i];Button(Lab.Name(d)+"  ["+d.id+"]",()=>{Lab.SelectById(d.id);Open("ESTRUCTURA");},40);}
 Button("‹ ANTERIOR    /    SIGUIENTE ›",()=>{resultPage=(resultPage+1)%Mathf.Max(1,Mathf.CeilToInt(results.Count/6f));Render();},34);
 var keyboard=Rect("Keyboard",Content,0,-row,650,142);row+=146;
 const string keys="QWERTYUIOPASDFGHJKLZXCVBNM";
 for(int i=0;i<keys.Length;i++){char c=keys[i];var b=CreateButton(c.ToString(),keyboard,i%10*64,-(i/10)*46,59,41,()=>{Query+=c;resultPage=0;Render();});}
 Button("ESPACIO",()=>{Query+=" ";Render();},32);Button("BORRAR",()=>{Query=Query.Length>0?Query.Substring(0,Query.Length-1):"";resultPage=0;Render();},32);Button("LIMPIAR BÚSQUEDA",()=>{Query="";resultPage=0;Render();},32);break;
 case "ESTRUCTURA":case "APRENDER":
 var selected=Lab.Selection.Selected;if(selected==null){Label("Selecciona una estructura del cuerpo\no utiliza el buscador.",90,24);Button("BUSCAR",()=>Open("BUSCAR"));break;}
 var data=selected.Data;Label(Lab.Name(data),70,27);Label(data.systemId+" · "+data.id+" · "+data.parentStructure,38,17);Label(data.description,76,19);
 if(Page=="APRENDER"){var e=Lab.Education.Find(data.id);Label(e==null?"Identificación anatómica.\nExplicación funcional pendiente de revisión docente.":e.function+"\n"+e.location,120,20);Label(e==null?"Fuente: BodyParts3D 4.0":e.source,42,16);}
 Button("AISLAR / SALIR DEL AISLAMIENTO",()=>{if(Lab.Isolated)Lab.ExitIsolation();else Lab.Isolate();});Button("OCULTAR",Lab.Hide);Button("TRANSPARENCIA",Lab.ToggleTransparency);Button("TRANSPARENCIA DEL SISTEMA",()=>Lab.SetSystemTransparency(data.systemId));Button("EXTRAER",Lab.ExtractSelected);Button("DEVOLVER A POSICIÓN",Lab.ReturnSelected);Button("ENFOCAR",()=>Lab.FocusStructure(data.id));Button("CERRAR SELECCIÓN",()=>{Lab.Selection.Clear();Open("ANATOMÍA");});break;
 case "EXPLORAR":
 Label("EXPLODED VIEW",50,27);Label("Separación radial de las piezas\nLa cámara permanece en tu control.",72);
 for(int i=0;i<=4;i++){float value=i*.25f;Button((i*25)+" %",()=>Lab.SetExplosion(value),38);}
 var sliderGo=Rect("Explosion slider",Content,15,-row-10,610,30);row+=62;var slider=sliderGo.gameObject.AddComponent<Slider>();slider.minValue=0;slider.maxValue=1;
 var bg=sliderGo.gameObject.AddComponent<Image>();bg.color=new Color(.1f,.23f,.28f);var handle=Rect("Handle",sliderGo,0,0,28,38);var hi=handle.gameObject.AddComponent<Image>();hi.color=Color.cyan;slider.handleRect=handle;slider.targetGraphic=hi;slider.direction=Slider.Direction.LeftToRight;slider.value=Lab.Explosion.Factor;slider.onValueChanged.AddListener(Lab.SetExplosion);
 Button("GIRAR IZQUIERDA",()=>Lab.Rotate(-20));Button("GIRAR DERECHA",()=>Lab.Rotate(20));Button("AUMENTAR ESCALA",()=>Lab.Scale(1.15f));Button("REDUCIR ESCALA",()=>Lab.Scale(.87f));Button("RESET BODY",Lab.ResetBody);break;
 case "APRENDIZAJE":
 Label("APRENDER HACIENDO",52,26);Button("FICHA EDUCATIVA",()=>Open("APRENDER"));Button("NUEVO RETO: ENCUENTRA…",Lab.AskQuestion);Button("ANATOMY PUZZLE",Lab.BeginPuzzle);
 Label("Reto: selecciona la estructura pedida.\nPuzzle: devuelve las piezas con el agarre.\nEl verde indica posición y orientación válidas.",115,20);
 Button("VER ESTADÍSTICAS",()=>Open("APRENDIZAJE"));var stats=Lab.Quiz.Statistics;Label($"Intentos: {stats.questions}\nCorrectos: {stats.correct} · Incorrectos: {stats.incorrect}\nAciertos: {stats.Percentage:0}%\nEnsamblaje: {stats.correctPlacements} correctas / {stats.wrongPlacements} incorrectas\nTiempo de ensamblaje: {stats.completionTime:0.0} s",150,22);break;
 case "AJUSTES":
 Label("AJUSTES LOCALES",48,26);Button("IDIOMA: "+Lab.Language,()=>{Lab.Language=Lab.Language=="es"?"en":"es";Lab.SavePreferences();Render();});Button("MANO PRINCIPAL: "+(Lab.LeftHanded?"IZQUIERDA":"DERECHA"),()=>{Lab.LeftHanded=!Lab.LeftHanded;Lab.SavePreferences();Render();});Button("AUDIO: "+Mathf.RoundToInt(Lab.Volume*100)+" %",()=>{Lab.Volume=Lab.Volume>=1?0:Lab.Volume+.25f;Lab.SavePreferences();Render();});
 Button("PERFORMANCE",()=>QualitySettings.SetQualityLevel(0));Button("BALANCED",()=>QualitySettings.SetQualityLevel(1));Button("QUALITY",()=>QualitySettings.SetQualityLevel(2));
 Button("DESCARGAR SISTEMAS OCULTOS",()=>{foreach(var s in Lab.Catalog.systems)if(!Lab.Systems.Visible[s.id])StartCoroutine(Lab.Streaming.UnloadSystemAsync(s.id));});Label("Escritorio: clic seleccionar · arrastre derecho girar\nR restablecer · E extraer · G mover pieza\nT devolver · espacio panel rápido\nQuest: gatillo seleccionar · grip agarrar\nStick derecho: giro voluntario de 30°",130,18);break;
 }
 foreach(var text in GetComponentsInChildren<Text>(true))text.text=LocalizationService.Translate(text.text,Lab.Language);
 }
 void Label(string text,int height,int size=22){var r=Rect("Label",Content,0,-row,650,height);var t=r.gameObject.AddComponent<Text>();t.font=font;t.fontSize=size;t.color=new Color(.79f,.9f,.94f);t.text=text;t.supportRichText=false;row+=height+4;}
 void Button(string text,Action click,int height=42){CreateButton(text,Content,0,-row,650,height,click);row+=height+7;}
 public static RectTransform Rect(string name,Transform parent,float x,float y,float width,float height){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(width,height);return r;}
 public static Button CreateButton(string label,Transform parent,float x,float y,float width,float height,Action action){var r=Rect(label,parent,x,y,width,height);var im=r.gameObject.AddComponent<Image>();im.color=new Color(.055f,.18f,.22f,.98f);var b=r.gameObject.AddComponent<Button>();b.targetGraphic=im;var cs=b.colors;cs.highlightedColor=new Color(.4f,.9f,1);cs.pressedColor=new Color(.2f,.7f,.8f);b.colors=cs;b.onClick.AddListener(()=>action());var tr=Rect("Text",r,12,-2,width-24,height-4);var t=tr.gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=height<35?17:20;t.color=Color.white;t.alignment=TextAnchor.MiddleLeft;t.text=label;t.raycastTarget=false;return b;}
}
}
