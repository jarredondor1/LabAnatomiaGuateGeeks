using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Education;
using GuateGeeks.AnatomyLab.XR;

namespace GuateGeeks.AnatomyLab.UI {
// Tarjetas espaciales inspiradas en la mesa y los paneles de AWS VR Architect.
public sealed class SpatialLabUI : MonoBehaviour {
 public LabRuntime Lab;
 public GameObject CatalogCard;
 public Button ExtractButton;
 public Button ReturnButton;
 public string Query {get;private set;}="";
 public string Category {get;private set;}="todos";
 readonly List<Button> suggestions=new List<Button>();
 readonly List<AnatomyStructure> matches=new List<AnatomyStructure>();
 readonly List<Button> systemCards=new List<Button>();
 readonly List<string> systemCardIds=new List<string>();
 readonly List<Button> categoryButtons=new List<Button>();
 Button previousButton,nextButton,backspaceButton,clearButton,spaceButton,isolateButton,transparencyButton,previousSystemButton,nextSystemButton;
 Text queryLabel,resultLabel,nameLabel,systemLabel,descriptionLabel,statusLabel,splitLabel,heightLabel,actionHintLabel,systemPageLabel;
 Slider split,heightSlider;DesktopControllerSimulator desktop;Transform root;
 RectTransform catalogCard,reviewCard,exploreCard,systemsPanel,quickMenu;
 readonly Vector3[] desktopPositions=new Vector3[4];
 readonly Quaternion[] desktopRotations=new Quaternion[4];
 bool menuHeld;
 int page,systemPage;bool ready;

 public void Initialize(LabRuntime lab,DesktopControllerSimulator controls){
  Lab=lab;desktop=controls;root=new GameObject("Tarjetas espaciales de anatomía").transform;
  var catalog=Card("Catálogo anatómico",new Vector3(-1.62f,1.63f,1.32f),new Vector2(730,1240),-12);
  catalogCard=catalog;CatalogCard=catalog.gameObject;Header(catalog,"01  /  CATÁLOGO ANATÓMICO",730);
  Label(catalog,"ELIGE UNA LETRA CON EL MANDO IZQUIERDO",25,-76,680,34,20,AnatomyHoloTheme.Muted);
  Button(catalog,"MASCULINO",25,-125,325,45,()=>Lab.SwitchReference("male")).interactable=Lab.ReferenceId!="male";
  Button(catalog,"FEMENINO",378,-125,325,45,()=>Lab.SwitchReference("female")).interactable=Lab.ReferenceId!="female";
  Label(catalog,"EXPLORA POR GRUPO O BUSCA POR LETRA",25,-184,680,33,20,AnatomyHoloTheme.Cyan);
  string[] groups={"TODOS","HUESOS","MÚSCULOS","ÓRGANOS","VASOS","NERVIOS","REPRODUCTOR","RESPIRATORIO","URINARIO"};
  for(int i=0;i<groups.Length;i++){var group=groups[i];var button=Button(catalog,group,25+(i%3)*230,-225-(i/3)*54,220,46,()=>SetCategory(group));button.GetComponentInChildren<Text>().alignment=TextAnchor.MiddleCenter;categoryButtons.Add(button);}
  queryLabel=Label(catalog,"PIEZAS DESTACADAS",25,-392,680,38,24,AnatomyHoloTheme.White);
  const string alphabet="ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
  for(int i=0;i<alphabet.Length;i++){char letter=alphabet[i];var b=Button(catalog,letter.ToString(),25+(i%9)*75,-445-(i/9)*56,69,49,()=>AppendLetter(letter));b.name="Letra "+letter;b.GetComponentInChildren<Text>().alignment=TextAnchor.MiddleCenter;}
  backspaceButton=Button(catalog,"BORRAR",25,-623,210,43,Backspace,true);
  clearButton=Button(catalog,"LIMPIAR",252,-623,210,43,ClearSearch,true);
  spaceButton=Button(catalog,"ESPACIO",480,-623,223,43,()=>AppendLetter(' '),true);
  resultLabel=Label(catalog,"SUGERENCIAS",25,-690,680,30,20,AnatomyHoloTheme.Cyan);
  for(int i=0;i<5;i++){int index=i;var b=Button(catalog,"",25,-730-i*67,680,59,()=>Choose(index));suggestions.Add(b);}
  previousButton=Button(catalog,"‹ ANTERIOR",25,-1075,325,45,()=>{page=Mathf.Max(0,page-1);RefreshSearch();},true);
  nextButton=Button(catalog,"SIGUIENTE ›",378,-1075,325,45,()=>{if((page+1)*5<matches.Count)page++;RefreshSearch();},true);
  Label(catalog,"GATILLO  seleccionar    ·    AGARRE  mover tarjeta",25,-1145,680,35,18,AnatomyHoloTheme.Muted);

  var review=Card("Evaluación de pieza",new Vector3(1.92f,1.69f,1.32f),new Vector2(730,960),12);reviewCard=review;
  Header(review,"02  /  EVALUACIÓN DE PIEZA",730);
  nameLabel=Label(review,"SELECCIONA UNA ESTRUCTURA",25,-84,680,80,31,AnatomyHoloTheme.White);
  systemLabel=Label(review,"SISTEMA ANATÓMICO",25,-175,680,38,20,AnatomyHoloTheme.Cyan);
  descriptionLabel=Label(review,"Apunta al cuerpo y pulsa el gatillo. También puedes elegir una letra en el catálogo para encontrar una pieza.",25,-228,680,315,24,AnatomyHoloTheme.White);
  actionHintLabel=Label(review,"SELECCIONA UNA PIEZA PARA COMENZAR",25,-556,680,48,16,AnatomyHoloTheme.Amber);
  ExtractButton=Button(review,"EXTRAER Y EXAMINAR",25,-616,680,56,TakeSelected);
  ReturnButton=Button(review,"DEVOLVER AL CUERPO",25,-684,680,52,ReturnSelected,true);
  isolateButton=Button(review,"AISLAR / VER TODO",25,-750,324,51,()=>{if(Lab.Isolated)Lab.ExitIsolation();else Lab.Isolate();},true);
  transparencyButton=Button(review,"TRANSPARENCIA",378,-750,325,51,Lab.ToggleTransparency,true);
  Button(review,"NUEVO RETO",25,-820,324,51,Lab.AskQuestion,true);
  Button(review,"ENSAMBLAJE",378,-820,325,51,Lab.BeginPuzzle,true);

  var explore=Card("Exploración del cuerpo",new Vector3(1.92f,.54f,1.32f),new Vector2(730,330),12);exploreCard=explore;
  Header(explore,"03  /  MESA DE EXPLORACIÓN",730);
  Label(explore,"SEPARAR PIEZAS",25,-68,250,36,22,AnatomyHoloTheme.White);
  splitLabel=Label(explore,"0 %",280,-68,100,36,22,AnatomyHoloTheme.Amber);
  split=Slider(explore,25,-122,360,Lab.SetExplosion);
  Button(explore,"RESTABLECER CUERPO",403,-85,300,65,()=>{desktop?.Release();Lab.ResetBody();});
  Label(explore,"TU ESTATURA",25,-174,235,34,22,AnatomyHoloTheme.White);
  heightLabel=Label(explore,"",259,-174,126,34,23,AnatomyHoloTheme.Amber);
  heightSlider=Slider(explore,25,-230,360,value=>Lab.SetHeightCm(Mathf.RoundToInt(value)),AnatomyHeight.MinimumCm,AnatomyHeight.MaximumCm);
  Button(explore,"− 1 CM",403,-180,140,57,()=>Lab.SetHeightCm(Lab.HeightCm-1));
  Button(explore,"+ 1 CM",563,-180,140,57,()=>Lab.SetHeightCm(Lab.HeightCm+1));
  statusLabel=Label(explore,"",25,-285,680,30,18,AnatomyHoloTheme.Muted);
  var title=Card("Marca del laboratorio",new Vector3(.25f,3.18f,4.35f),new Vector2(1050,150),0);
  Label(title,"GUATEGEEKS  /  LABORATORIO DE ANATOMÍA",30,-22,990,60,35,AnatomyHoloTheme.White);
  Label(title,"EXPLORA  ·  EXTRAE  ·  EVALÚA",30,-88,990,40,24,AnatomyHoloTheme.Cyan);
  BuildSystemCards();BuildQuickMenu();
  var cards=new[]{catalogCard,reviewCard,exploreCard,systemsPanel};for(int i=0;i<cards.Length;i++){desktopPositions[i]=cards[i].position;desktopRotations[i]=cards[i].rotation;}
  // El laboratorio abre con la misma distribución espacial en Unity y en Quest.
  // El usuario puede acercar las tarjetas desde el menú rápido cuando lo necesite.
  ApplyComfortLayout(false);
  Lab.Selection.Changed+=SelectionChanged;Lab.Changed+=Refresh;ready=true;RefreshSearch();SelectionChanged(Lab.Selection.Selected);Refresh();
 }

 RectTransform Card(string name,Vector3 position,Vector2 size,float yaw){
  var go=new GameObject(name,typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster),typeof(TrackedDeviceGraphicRaycaster));go.transform.SetParent(root,false);
  var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=Lab.ViewCamera;
  var rect=(RectTransform)go.transform;rect.sizeDelta=size;rect.pivot=new Vector2(.5f,.5f);rect.position=position;rect.rotation=Quaternion.Euler(0,yaw,0);rect.localScale=Vector3.one*.00165f;
  var surface=go.AddComponent<Image>();surface.color=Color.clear;surface.raycastTarget=true;AnatomyHoloTheme.Decorate(rect,true);return rect;
 }
 void Header(RectTransform panel,string text,float width){
  Label(panel,text,25,-22,width-50,42,28,AnatomyHoloTheme.Cyan);
  var handle=LabPanel.Rect("Mover tarjeta",panel,width-230,-22,201,37);var image=handle.gameObject.AddComponent<Image>();image.color=Color.clear;image.raycastTarget=true;var glass=AnatomyHoloTheme.Decorate(handle,false,true);glass.Ghost=true;
  var mover=handle.gameObject.AddComponent<SpatialPanelHandle>();mover.Panel=panel;Label(handle,"MOVER",14,-3,98,32,18,AnatomyHoloTheme.Muted);
  AnimatedCornerEyes.Create(handle,118,-1,74);
  AddHover(handle.gameObject,glass);
 }
 Text Label(Transform parent,string value,float x,float y,float width,float height,int size,Color color){
  var rt=LabPanel.Rect("Texto",parent,x,y,width,height);var label=rt.gameObject.AddComponent<Text>();label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");label.fontSize=size;label.fontStyle=size>=25?FontStyle.Bold:FontStyle.Normal;label.color=color;label.text=value;label.raycastTarget=false;label.supportRichText=false;label.horizontalOverflow=HorizontalWrapMode.Wrap;label.verticalOverflow=VerticalWrapMode.Truncate;if(size>=25){label.resizeTextForBestFit=true;label.resizeTextMinSize=21;label.resizeTextMaxSize=size;}return label;
 }
 Button Button(Transform parent,string value,float x,float y,float width,float height,Action action,bool ghost=false,Color? accent=null){
  var b=LabPanel.CreateButton(value,parent,x,y,width,height,action);b.GetComponentInChildren<Text>().fontSize=height<50?19:22;
  var glass=b.GetComponentInChildren<AnatomyHoloGraphic>();if(glass!=null){glass.Ghost=ghost;if(accent.HasValue){glass.Accent=accent.Value;glass.SetVerticesDirty();}AddHover(b.gameObject,glass);}
  return b;
 }
 static void AddHover(GameObject target,AnatomyHoloGraphic glass){
  var events=target.AddComponent<EventTrigger>();
  var enter=new EventTrigger.Entry{eventID=EventTriggerType.PointerEnter};enter.callback.AddListener(_=>glass.Highlight=1);events.triggers.Add(enter);
  var exit=new EventTrigger.Entry{eventID=EventTriggerType.PointerExit};exit.callback.AddListener(_=>glass.Highlight=0);events.triggers.Add(exit);
 }
 Slider Slider(Transform parent,float x,float y,float width,UnityEngine.Events.UnityAction<float> changed,float minimum=0,float maximum=1){
  var track=LabPanel.Rect(minimum==0?"Separación anatómica":"Estatura en centímetros",parent,x,y,width,34);track.gameObject.AddComponent<Image>().color=new Color(.03f,.13f,.19f,.85f);AnatomyHoloTheme.Decorate(track);
  var slider=track.gameObject.AddComponent<Slider>();slider.minValue=minimum;slider.maxValue=maximum;slider.wholeNumbers=maximum>1;var handle=LabPanel.Rect("Control",track,0,0,26,42);var image=handle.gameObject.AddComponent<Image>();image.color=AnatomyHoloTheme.Amber;slider.handleRect=handle;slider.targetGraphic=image;slider.onValueChanged.AddListener(changed);return slider;
 }
 void BuildSystemCards(){
  string[] ordered={"skeleton","muscular","digestive","nervous","arterial","venous","reproductive","respiratory","urinary","cardiovascular","lymphatic","integumentary","endocrine","pregnancy","connective"};
  foreach(var id in ordered)foreach(var system in Lab.Catalog.systems)if(system.id==id&&system.structures!=null&&system.structures.Length>0)systemCardIds.Add(id);
  systemsPanel=Card("Panel de sistemas anatómicos",new Vector3(-1.62f,.32f,1.32f),new Vector2(830,390),-12);
  systemsPanel.localScale=Vector3.one*.0013f;
  Header(systemsPanel,"04  /  SISTEMAS ANATÓMICOS",830);
  Label(systemsPanel,"ACTIVA CAPAS Y ELIGE PIEZAS EN EL CATÁLOGO",28,-72,770,32,20,AnatomyHoloTheme.Muted);
  var accents=new[]{new Color(.79f,.96f,1),AnatomyHoloTheme.Amber,AnatomyHoloTheme.Green,new Color(.73f,.62f,1),new Color(1,.48f,.55f),new Color(.49f,.70f,1)};
  for(int i=0;i<systemCardIds.Count;i++){
   var id=systemCardIds[i];int slot=i%6,column=slot%3,row=slot/3;string name=id;
   foreach(var system in Lab.Catalog.systems)if(system.id==id){name=system.displayName;break;}
   var button=Button(systemsPanel,(i+1).ToString("00")+"  "+name.ToUpperInvariant(),28+column*265,-118-row*86,240,70,()=>{Lab.Systems.ToggleSystem(id);Lab.Feedback();Refresh();},false,accents[i%accents.Length]);
   button.GetComponentInChildren<Text>().alignment=TextAnchor.MiddleCenter;systemCards.Add(button);
  }
  previousSystemButton=Button(systemsPanel,"‹ ANTERIORES",28,-294,230,48,()=>ChangeSystemPage(-1),true);
  systemPageLabel=Label(systemsPanel,"",280,-299,260,38,20,AnatomyHoloTheme.Cyan);systemPageLabel.alignment=TextAnchor.MiddleCenter;
  nextSystemButton=Button(systemsPanel,"SIGUIENTES SISTEMAS ›",562,-294,240,48,()=>ChangeSystemPage(1),true);
  Label(systemsPanel,"GATILLO  seleccionar    ·    AGARRE  mover bloque",28,-353,770,26,16,AnatomyHoloTheme.Muted);
  RefreshSystemPage();
 }
 void ChangeSystemPage(int step){systemPage=Mathf.Clamp(systemPage+step,0,(systemCardIds.Count-1)/6);RefreshSystemPage();}
 void RefreshSystemPage(){
  for(int i=0;i<systemCards.Count;i++)systemCards[i].gameObject.SetActive(i/6==systemPage);
  previousSystemButton.interactable=systemPage>0;nextSystemButton.interactable=(systemPage+1)*6<systemCardIds.Count;
  systemPageLabel.text="PÁGINA "+(systemPage+1)+" / "+((systemCardIds.Count+5)/6);
 }
 void BuildQuickMenu(){
  quickMenu=Card("Menú rápido de anatomía",Vector3.zero,new Vector2(520,410),0);quickMenu.localScale=Vector3.one*.0011f;
  AnimatedCornerEyes.Create(quickMenu,442,-17,57);
  Label(quickMenu,"ACCESO RÁPIDO  /  ANATOMÍA",20,-20,480,40,25,AnatomyHoloTheme.Cyan);
  Button(quickMenu,"CATÁLOGO",20,-78,230,82,()=>CatalogCard.SetActive(!CatalogCard.activeSelf));
  Button(quickMenu,"EVALUACIÓN",270,-78,230,82,()=>reviewCard.gameObject.SetActive(!reviewCard.gameObject.activeSelf));
  Button(quickMenu,"EXPLORAR",20,-175,230,82,()=>exploreCard.gameObject.SetActive(!exploreCard.gameObject.activeSelf));
  Button(quickMenu,"SISTEMAS",270,-175,230,82,()=>systemsPanel.gameObject.SetActive(!systemsPanel.gameObject.activeSelf));
  Button(quickMenu,"TRAER AQUÍ",20,-272,480,82,()=>{ApplyComfortLayout(true);CatalogCard.SetActive(true);reviewCard.gameObject.SetActive(true);exploreCard.gameObject.SetActive(true);systemsPanel.gameObject.SetActive(true);},false,AnatomyHoloTheme.Green);
  quickMenu.gameObject.SetActive(false);
 }
 public void ToggleQuickMenu(){if(quickMenu==null)return;bool show=!quickMenu.gameObject.activeSelf;quickMenu.gameObject.SetActive(show);if(show)PlaceQuickMenu();}
 public void ApplyComfortLayout(bool useNearField){
  if(catalogCard==null||reviewCard==null||exploreCard==null||systemsPanel==null||Lab==null||Lab.ViewCamera==null)return;
  var cards=new[]{catalogCard,reviewCard,exploreCard,systemsPanel};
  if(!useNearField){for(int i=0;i<cards.Length;i++){cards[i].SetPositionAndRotation(desktopPositions[i],desktopRotations[i]);cards[i].localScale=Vector3.one*(i==3?.0013f:.00165f);}return;}
  var head=Lab.ViewCamera.transform;var yaw=Quaternion.Euler(0,head.eulerAngles.y,0);
  // Cada tarjeta queda a distancia de lectura y deja libre el centro del cuerpo.
  var offsets=new[]{new Vector3(-1.25f,.10f,1.7f),new Vector3(1.25f,-.05f,1.7f),new Vector3(1.25f,0,1.7f),new Vector3(-1.25f,0,1.7f)};
  float lowerCenter=Mathf.Max(.36f,head.position.y-1.18f);
  for(int i=0;i<cards.Length;i++){var position=head.position+yaw*offsets[i];if(i>=2)position.y=lowerCenter;cards[i].SetPositionAndRotation(position,Quaternion.LookRotation(position-head.position,Vector3.up));cards[i].localScale=Vector3.one*.00125f;}
 }
 void PlaceQuickMenu(){if(quickMenu==null||Lab==null||Lab.ViewCamera==null)return;
  var head=Lab.ViewCamera.transform;var yaw=Quaternion.Euler(0,head.eulerAngles.y,0);var position=head.position+yaw*new Vector3(-.45f,-.20f,1.15f);
  if(UnityEngine.XR.XRSettings.isDeviceActive){var left=InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);if(left.isValid&&left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition,out Vector3 devicePosition)){var tracking=head.parent;var hand=tracking!=null?tracking.TransformPoint(devicePosition):devicePosition;position=hand+yaw*Vector3.forward*.75f+Vector3.up*.13f;}}
  quickMenu.SetPositionAndRotation(position,Quaternion.LookRotation(position-head.position,Vector3.up));
 }
 void Update(){if(!ready)return;if(Keyboard.current!=null&&Keyboard.current.mKey.wasPressedThisFrame)ToggleQuickMenu();
  if(UnityEngine.XR.XRSettings.isDeviceActive){var left=InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);bool pressed=left.isValid&&left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton,out bool down)&&down;if(pressed&&!menuHeld)ToggleQuickMenu();menuHeld=pressed;}
  if(quickMenu!=null&&quickMenu.gameObject.activeSelf)PlaceQuickMenu();
 }
 public void AppendLetter(char letter){if(Query.Length>=40)return;Query+=letter;page=0;RefreshSearch();}
 public void SetCategory(string category){Category=category.ToLowerInvariant();page=0;RefreshSearch();}
 public void Backspace(){if(Query.Length>0)Query=Query.Substring(0,Query.Length-1);page=0;RefreshSearch();}
 public void ClearSearch(){Query="";page=0;RefreshSearch();}
 void RefreshSearch(){if(Lab==null||!Lab.Ready)return;matches.Clear();matches.AddRange(Lab.Search.Discover(Query,Category,200));
  queryLabel.text=Query.Trim().Length==0?"PIEZAS / "+Category.ToUpperInvariant():"BUSCAR / "+Query.ToUpperInvariant();
  resultLabel.text=matches.Count==0?"SIN RESULTADOS · ELIGE OTRO GRUPO O LETRA":matches.Count+" PIEZAS  ·  PÁGINA "+(page+1);
  for(int i=0;i<suggestions.Count;i++){int index=page*5+i;var button=suggestions[i];button.gameObject.SetActive(index<matches.Count);if(index<matches.Count){var part=matches[index];string system=part.systemId;foreach(var entry in Lab.Catalog.systems)if(entry.id==system){system=entry.displayName;break;}button.GetComponentInChildren<Text>().text=AnatomyPieceSummary.Title(part,Lab.Name(part),system);}}
  previousButton.interactable=page>0;nextButton.interactable=(page+1)*5<matches.Count;
  backspaceButton.interactable=Query.Length>0;clearButton.interactable=Query.Length>0;spaceButton.interactable=Query.Length>0&&!Query.EndsWith(" ");
  foreach(var button in categoryButtons){bool selected=button.GetComponentInChildren<Text>().text.ToLowerInvariant()==Category;var glass=button.GetComponentInChildren<AnatomyHoloGraphic>();if(glass!=null)glass.Highlight=selected?.85f:0;}
 }
 void Choose(int index){index+=page*5;if(index>=matches.Count)return;Lab.SelectById(matches[index].id);}
 void SelectionChanged(StructureState selected){if(nameLabel==null)return;
  if(selected==null){nameLabel.text="SELECCIONA UNA ESTRUCTURA";systemLabel.text="SISTEMA ANATÓMICO";descriptionLabel.text="Apunta al cuerpo y pulsa el gatillo. También puedes elegir una letra en el catálogo para encontrar una pieza.";UpdateActionState();return;}
  var data=selected.Data;string system=data.systemId;foreach(var entry in Lab.Catalog.systems)if(entry.id==system){system=entry.displayName;break;}
  nameLabel.text=AnatomyPieceSummary.Title(data,Lab.Name(data),system);
  systemLabel.text=system.ToUpperInvariant();descriptionLabel.text=AnatomyPieceSummary.Describe(data,system);
  UpdateActionState();
 }
 void UpdateActionState(){if(ExtractButton==null||ReturnButton==null)return;var selected=Lab.Selection.Selected;bool hasSelection=selected!=null;bool extracted=hasSelection&&selected.Extracted;ExtractButton.interactable=hasSelection&&!extracted;ReturnButton.interactable=extracted;isolateButton.interactable=hasSelection;transparencyButton.interactable=hasSelection;ExtractButton.GetComponentInChildren<Text>().text=extracted?"PIEZA EXTRAÍDA":"EXTRAER Y EXAMINAR";actionHintLabel.text=!hasSelection?"SELECCIONA UNA PIEZA PARA COMENZAR":extracted?"AGARRE DERECHO: MOVER · PALANCA: ALCANCE/GIRO\nA/B: TAMAÑO · DOS AGARRES: AMPLIAR":"PIEZA RESALTADA · AGARRE DERECHO PARA EXAMINAR";}
 void TakeSelected(){if(Lab.Selection.Selected==null)return;if(desktop!=null)desktop.TakeSelected();else Lab.ExtractSelected();SelectionChanged(Lab.Selection.Selected);}
 void ReturnSelected(){desktop?.Release();Lab.ReturnSelected();SelectionChanged(Lab.Selection.Selected);}
 void Refresh(){if(!ready||Lab==null||!Lab.Ready)return;statusLabel.text=Lab.Status;splitLabel.text=Mathf.RoundToInt(Lab.Explosion.Factor*100)+" %";split.SetValueWithoutNotify(Lab.Explosion.Factor);heightLabel.text=Lab.HeightCm+" CM";heightSlider.SetValueWithoutNotify(Lab.HeightCm);
  UpdateActionState();
  for(int i=0;i<systemCards.Count;i++){var button=systemCards[i];button.GetComponentInChildren<Text>(true).color=Lab.Systems.Visible[systemCardIds[i]]?AnatomyHoloTheme.Cyan:AnatomyHoloTheme.Muted;}
 }
 void OnDestroy(){if(Lab!=null){Lab.Selection.Changed-=SelectionChanged;Lab.Changed-=Refresh;}}
}
}
