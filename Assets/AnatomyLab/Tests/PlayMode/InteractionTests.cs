using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
namespace GuateGeeks.AnatomyLab.Tests {
public class InteractionTests {
 LabRuntime lab;string referencePreference;readonly Dictionary<string,int> preferences=new Dictionary<string,int>();
 [UnitySetUp] public IEnumerator Setup(){referencePreference=PlayerPrefs.GetString("anatomy.reference","");PlayerPrefs.SetString("anatomy.reference","male");foreach(var sys in new LocalDatasetProvider().LoadCatalog("female").systems){var key="anatomy.system.female."+sys.id;preferences[key]=PlayerPrefs.GetInt(key,-1);PlayerPrefs.SetInt(key,sys.defaultVisibility?1:0);}var c=new LocalDatasetProvider().LoadCatalog();foreach(var sys in c.systems){var key="anatomy.system."+sys.id;preferences[key]=PlayerPrefs.GetInt(key,-1);PlayerPrefs.SetInt(key,sys.id=="urinary"?1:0);}SceneManager.LoadScene("02_AnatomyLab");yield return null;lab=Object.FindAnyObjectByType<LabRuntime>();float end=Time.realtimeSinceStartup+90;while((!lab.Ready||lab.Streaming.Loading.Count>0||lab.Streaming.Loaded.Count==0)&&Time.realtimeSinceStartup<end)yield return null;Assert.IsTrue(lab.Ready);Assert.Greater(lab.Streaming.Loaded.Count,0);}
 [UnityTearDown] public IEnumerator Cleanup(){if(referencePreference.Length==0)PlayerPrefs.DeleteKey("anatomy.reference");else PlayerPrefs.SetString("anatomy.reference",referencePreference);foreach(var kv in preferences){if(kv.Value<0)PlayerPrefs.DeleteKey(kv.Key);else PlayerPrefs.SetInt(kv.Key,kv.Value);}preferences.Clear();var scene=SceneManager.CreateScene("TestCleanup"+Time.frameCount);SceneManager.SetActiveScene(scene);yield return SceneManager.UnloadSceneAsync("02_AnatomyLab");}
 [UnityTest] public IEnumerator FemaleReferenceSwitchClearsOldGrabsAndSupportsExploration(){
 yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.ExplorationWorkbench>();
 lab.Selection.Select(First());ui.Controls.TakeSelected();lab.Quiz.Ask(First().Data.id);lab.SetExplosion(.6f);yield return null;Assert.IsTrue(ui.Controls.Holding);
 var oldLab=lab;ui.ChangeReference("female");
 float end=Time.realtimeSinceStartup+90;
 do{yield return null;lab=Object.FindAnyObjectByType<LabRuntime>();}while((lab==null||lab==oldLab||!lab.Ready||lab.Streaming.Loading.Count>0)&&Time.realtimeSinceStartup<end);
 Assert.NotNull(lab);Assert.AreEqual("female",lab.ReferenceId);Assert.AreEqual(888,lab.Catalog.structures.Length);Assert.AreEqual(0,lab.Extracted.Count);Assert.AreEqual(0,lab.Explosion.Factor);Assert.IsFalse(lab.Quiz.Active);Assert.IsNull(lab.Selection.Selected);
 foreach(var state in lab.Streaming.Loaded.Values)StringAssert.StartsWith("HRAF",state.Data.id);
 Assert.IsFalse(lab.Systems.Visible["pregnancy"]);var skin=lab.Streaming.Loaded["HRAF0002"];Assert.AreEqual(StructureVisibility.Transparent,skin.Visibility);
 ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.ExplorationWorkbench>();Assert.NotNull(ui);Assert.IsFalse(ui.Controls.Holding);
 Assert.Greater(lab.Search.Search("ovario").Count,1);Assert.Greater(lab.Search.Search("trompa uterina").Count,1);Assert.AreEqual(7,lab.Education.entries.Length);
 var femaleAnchor=lab.Body.position;lab.SelectById("HRAF0473");end=Time.realtimeSinceStartup+30;while(lab.Selection.Selected==null&&Time.realtimeSinceStartup<end)yield return null;
 Assert.Less(Vector3.Distance(femaleAnchor,lab.Body.position),.001f,"La referencia femenina debe permanecer sobre la plataforma al seleccionar.");
 Assert.AreEqual("Cuerpo del útero",ui.InfoTitle.text);ui.GrabButton.onClick.Invoke();yield return new WaitForFixedUpdate();yield return null;Assert.IsTrue(ui.Controls.Holding);
 ui.Return();yield return new WaitForSeconds(1.2f);Assert.IsFalse(lab.Selection.Selected.Extracted);Assert.Less(Vector3.Distance(lab.Selection.Selected.Object.transform.localPosition,lab.Selection.Selected.Data.originalPosition),.005f);Assert.Less(Vector3.Distance(femaleAnchor,lab.Body.position),.001f);
 lab.ResetBody();yield return null;Assert.AreEqual(StructureVisibility.Transparent,skin.Visibility);
 // Switching back during a system load must discard the female loader and its scene objects.
 lab.Systems.ShowSystem("nervous");oldLab=lab;ui.ChangeReference("male");end=Time.realtimeSinceStartup+90;
 do{yield return null;lab=Object.FindAnyObjectByType<LabRuntime>();}while((lab==null||lab==oldLab||!lab.Ready||lab.Streaming.Loading.Count>0)&&Time.realtimeSinceStartup<end);
 Assert.AreEqual("male",lab.ReferenceId);Assert.AreEqual(2234,lab.Catalog.structures.Length);Assert.AreEqual(0,lab.Extracted.Count);foreach(var state in lab.Streaming.Loaded.Values)StringAssert.StartsWith("FJ",state.Data.id);
 }
 StructureState First(){foreach(var s in lab.Streaming.Loaded.Values)return s;return null;}
 [UnityTest] public IEnumerator HolographicSkinKeepsDesktopAndQuestControlsInteractive(){
 yield return null;
 var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.ExplorationWorkbench>();Assert.NotNull(ui);
 var root=ui.transform.Find("Marco adaptable");Assert.NotNull(root);
 var names=new[]{"Cabecera","Navegación anatómica","Ficha contextual","Mesa de exploración"};
 foreach(var name in names){var panel=root.Find(name);Assert.NotNull(panel,name);var glass=panel.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.AnatomyHoloGraphic>();Assert.NotNull(glass,name);Assert.IsFalse(glass.raycastTarget,name);Assert.IsTrue(panel.GetComponent<UnityEngine.UI.Image>().raycastTarget,name);}
 Assert.AreEqual(new Vector2(350,720),((RectTransform)root.Find("Navegación anatómica")).sizeDelta);
 Assert.AreEqual(new Vector2(370,720),((RectTransform)root.Find("Ficha contextual")).sizeDelta);
 Assert.AreEqual(new Vector2(1030,128),((RectTransform)root.Find("Mesa de exploración")).sizeDelta);
 Assert.IsTrue(ui.GrabButton.GetComponent<UnityEngine.UI.Image>().raycastTarget);
 Assert.NotNull(ui.GrabButton.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.AnatomyHoloGraphic>());
 var quest=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.LabPanel>();Assert.NotNull(quest);
 Assert.NotNull(quest.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.AnatomyHoloGraphic>(true));
 var vrButtons=quest.GetComponentsInChildren<UnityEngine.UI.Button>(true);Assert.Greater(vrButtons.Length,0);
 Assert.IsTrue(vrButtons[0].GetComponent<UnityEngine.UI.Image>().raycastTarget);
 }
 [UnityTest] public IEnumerator SpatialLaboratoryProvidesMovableCardsAlphabetAndBrandedTable(){
 yield return null;
 var stage=GameObject.Find("Mesa holográfica GuateGeeks");Assert.NotNull(stage);
 Assert.NotNull(stage.transform.Find("Logotipo GuateGeeks"));
 var cards=GameObject.Find("Tarjetas espaciales de anatomía");Assert.NotNull(cards);
 Assert.NotNull(cards.transform.Find("Catálogo anatómico"));
 Assert.NotNull(cards.transform.Find("Evaluación de pieza"));
 Assert.NotNull(cards.transform.Find("Exploración del cuerpo"));
 var systems=cards.transform.Find("Panel de sistemas anatómicos");Assert.NotNull(systems);
 Assert.Less(systems.position.z,lab.Body.position.z,"El bloque debe quedar al alcance, delante del cuerpo.");
 Assert.GreaterOrEqual(systems.GetComponentsInChildren<UnityEngine.UI.Button>(true).Length,14,"Todos los sistemas con piezas deben permanecer en el bloque paginado.");
 Assert.NotNull(systems.Find("07  REPRODUCTIVO"),"El sistema reproductor debe estar disponible junto a las otras capas.");
 Assert.NotNull(systems.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.SpatialPanelHandle>(true),"El bloque de sistemas debe poder moverse.");
 int letters=0,handles=0;foreach(var item in cards.GetComponentsInChildren<Transform>(true)){if(item.name.StartsWith("Letra "))letters++;if(item.name=="Mover tarjeta")handles++;}
 Assert.AreEqual(27,letters);Assert.GreaterOrEqual(handles,4);
 var logo=stage.transform.Find("Logotipo GuateGeeks").GetComponent<Renderer>();Assert.NotNull(logo.sharedMaterial.GetTexture("_MainTex"));
 Assert.AreEqual("GuateGeeksSDF",logo.sharedMaterial.GetTexture("_MainTex").name);
 Assert.AreEqual(-.003f,logo.transform.position.y,.01f,"El emblema debe estar al ras del piso.");
 Assert.Greater(Vector3.Dot(logo.transform.right,Vector3.right),.99f,"El logotipo debe leerse desde el puesto del visitante.");
 Assert.AreEqual(new Vector3(.25f,-.003f,2.1f),logo.transform.position,"El emblema debe cubrir la zona central bajo el cuerpo.");
 Assert.GreaterOrEqual(logo.transform.localScale.x,2.4f,"El emblema debe ocupar el centro de la plataforma.");
 Assert.NotNull(GameObject.Find("Sala envolvente GuateGeeks"));
 Assert.NotNull(GameObject.Find("Horizonte de Guatemala · cielo panorámico"));
 Assert.NotNull(GameObject.Find("Techo hexagonal holográfico"));
 Assert.GreaterOrEqual(cards.GetComponentsInChildren<GuateGeeks.AnatomyLab.UI.AnimatedCornerEyes>(true).Length,5);
 }
 [UnityTest] public IEnumerator MainButtonsFitWithoutCardsCoveringEachOther(){
  yield return null;
  var catalog=(RectTransform)GameObject.Find("Catálogo anatómico").transform;
  var review=(RectTransform)GameObject.Find("Evaluación de pieza").transform;
  var explore=(RectTransform)GameObject.Find("Exploración del cuerpo").transform;
  var systems=(RectTransform)GameObject.Find("Panel de sistemas anatómicos").transform;
  var catalogBox=Envelope(catalog);var reviewBox=Envelope(review);var exploreBox=Envelope(explore);var systemsBox=Envelope(systems);
  Assert.Less(systemsBox.yMax,catalogBox.yMin,"El bloque de sistemas tapa botones del catálogo.");
  Assert.Less(systemsBox.xMax,lab.Body.position.x-.2f,"El bloque de sistemas debe dejar libre el cuerpo.");
  Assert.Less(systemsBox.xMax,reviewBox.xMin,"El bloque de sistemas tapa la evaluación.");
  Assert.Less(exploreBox.yMax,reviewBox.yMin,"Los ajustes deben quedar debajo de la evaluación.");
  foreach(var button in systems.GetComponentsInChildren<UnityEngine.UI.Button>(true)){
   var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);
   foreach(var corner in corners){var local=systems.InverseTransformPoint(corner);Assert.GreaterOrEqual(local.x,systems.rect.xMin-2,button.name);Assert.LessOrEqual(local.x,systems.rect.xMax+2,button.name);Assert.GreaterOrEqual(local.y,systems.rect.yMin-2,button.name);Assert.LessOrEqual(local.y,systems.rect.yMax+2,button.name);}
  }
  var reproductive=systems.Find("07  REPRODUCTIVO").gameObject;Assert.IsFalse(reproductive.activeSelf,"La primera página debe conservar seis botones grandes.");
  systems.Find("SIGUIENTES SISTEMAS ›").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
  Assert.IsTrue(reproductive.activeSelf,"La segunda página debe mostrar el sistema reproductor.");
  Assert.AreEqual(6,System.Array.FindAll(systems.GetComponentsInChildren<UnityEngine.UI.Button>(false),b=>b.name.Length>2&&char.IsDigit(b.name[0])).Length);
  bool wasVisible=lab.Systems.Visible["reproductive"];
  reproductive.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();yield return null;
  Assert.AreNotEqual(wasVisible,lab.Systems.Visible["reproductive"],"El botón paginado debe activar su sistema.");
 }
 static Rect Envelope(RectTransform rect){var corners=new Vector3[4];rect.GetWorldCorners(corners);float minX=float.MaxValue,minY=float.MaxValue,maxX=float.MinValue,maxY=float.MinValue;foreach(var corner in corners){minX=Mathf.Min(minX,corner.x);minY=Mathf.Min(minY,corner.y);maxX=Mathf.Max(maxX,corner.x);maxY=Mathf.Max(maxY,corner.y);}return Rect.MinMaxRect(minX,minY,maxX,maxY);}
 [UnityTest] public IEnumerator CatalogSelectionAndZoomKeepBodyOnPlatform(){
 var anchor=lab.Body.position;var match=lab.Search.Search("fémur",null,1);Assert.Greater(match.Count,0);
 lab.SelectById(match[0].id);float end=Time.realtimeSinceStartup+30;
 while((lab.Selection.Selected==null||lab.Selection.Selected.Data.id!=match[0].id)&&Time.realtimeSinceStartup<end)yield return null;
 Assert.Less(Vector3.Distance(anchor,lab.Body.position),.001f,"Seleccionar una estructura no debe desplazar el cuerpo.");
 lab.FocusStructure(match[0].id);Assert.Less(Vector3.Distance(anchor,lab.Body.position),.001f,"Enfocar no debe sacar el cuerpo de la plataforma.");
 lab.Scale(1.25f);Assert.AreEqual(Vector3.one*AnatomyHeight.ScaleFor(lab.Catalog,lab.HeightCm),lab.Body.localScale,"Acercar la vista no debe agrandar el modelo.");
 }
 [UnityTest] public IEnumerator HeightControlScalesWholeBodyAndResetKeepsStudentHeight(){
  yield return null;
  int previous=lab.HeightCm;var card=GameObject.Find("Exploración del cuerpo");Assert.NotNull(card);
  Assert.NotNull(card.transform.Find("Estatura en centímetros"),"La estatura debe poder ajustarse desde la tarjeta de exploración.");
  lab.SetHeightCm(183);
  var range=AnatomyHeight.Bounds(lab.Catalog);
  Assert.AreEqual(1.83f,(range.y-range.x)*lab.Body.localScale.y,.005f);
  Assert.AreEqual(0,lab.Body.position.y+range.x*lab.Body.localScale.y,.005f,"Los pies deben quedar al nivel del suelo.");
  lab.ResetBody();Assert.AreEqual(183,lab.HeightCm);Assert.AreEqual(1.83f,(range.y-range.x)*lab.Body.localScale.y,.005f);
  lab.SetHeightCm(previous);
 }
 [UnityTest] public IEnumerator SpatialPanelsStayReadableInQuestWithoutCoveringTheBody(){
  yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.SpatialLabUI>();Assert.NotNull(ui);
  var apply=typeof(GuateGeeks.AnatomyLab.UI.SpatialLabUI).GetMethod("ApplyComfortLayout");Assert.NotNull(apply,"Debe poder acercar las tarjetas a una distancia cómoda en Quest.");
  var bodyAnchor=lab.Body.position;var desktopPosition=ui.CatalogCard.transform.position;
  apply.Invoke(ui,new object[]{true});
  var head=lab.ViewCamera.transform.position;
  Assert.Greater(Vector3.Distance(head,ui.CatalogCard.transform.position),1.25f,"El catálogo no debe quedar pegado a los ojos.");
  Assert.Greater(Vector3.Distance(head,GameObject.Find("Evaluación de pieza").transform.position),1.25f,"La evaluación no debe ocupar toda la vista.");
  Assert.Greater(Vector3.Distance(head,GameObject.Find("Panel de sistemas anatómicos").transform.position),1.25f,"Los sistemas deben verse como tarjetas espaciales.");
  Assert.Greater(Vector3.Distance(ui.CatalogCard.transform.position,GameObject.Find("Evaluación de pieza").transform.position),1.5f,"Las tarjetas laterales no deben amontonarse como una pantalla plana.");
  Assert.Less(Vector3.Distance(bodyAnchor,lab.Body.position),.001f);
  apply.Invoke(ui,new object[]{false});Assert.Less(Vector3.Distance(desktopPosition,ui.CatalogCard.transform.position),.001f);
 }
 [UnityTest] public IEnumerator QuickMenuOpensAnatomyPanelsAndBringsThemHere(){
  yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.SpatialLabUI>();Assert.NotNull(ui);
  var root=GameObject.Find("Tarjetas espaciales de anatomía");Assert.NotNull(root);var menuTransform=root.transform.Find("Menú rápido de anatomía");Assert.NotNull(menuTransform,"Debe existir acceso compacto a los paneles.");var menu=menuTransform.gameObject;
  var toggle=typeof(GuateGeeks.AnatomyLab.UI.SpatialLabUI).GetMethod("ToggleQuickMenu");Assert.NotNull(toggle);
  toggle.Invoke(ui,null);Assert.IsTrue(menu.activeSelf);
  UnityEngine.UI.Button catalog=null,systems=null,bring=null;
  foreach(var button in menu.GetComponentsInChildren<UnityEngine.UI.Button>(true)){if(button.name=="CATÁLOGO")catalog=button;if(button.name=="SISTEMAS")systems=button;if(button.name=="TRAER AQUÍ")bring=button;}
  Assert.NotNull(catalog);Assert.NotNull(systems);Assert.NotNull(bring);catalog.onClick.Invoke();Assert.IsFalse(ui.CatalogCard.activeSelf);
  catalog.onClick.Invoke();Assert.IsTrue(ui.CatalogCard.activeSelf);
  var systemsPanel=root.transform.Find("Panel de sistemas anatómicos").gameObject;systems.onClick.Invoke();Assert.IsFalse(systemsPanel.activeSelf);
  bring.onClick.Invoke();Assert.IsTrue(systemsPanel.activeSelf);Assert.Less(Vector3.Distance(lab.Body.position,new Vector3(.25f,AnatomyHeight.BaseY(lab.Catalog,lab.HeightCm),2.1f)),.001f);
 }
 [UnityTest] public IEnumerator ExtractionActionsShowTheirCurrentState(){
 yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.SpatialLabUI>();Assert.NotNull(ui);
 UnityEngine.UI.Button returnButton=null;var review=ui.ExtractButton.transform.parent;var buttonNames=new List<string>();foreach(var button in review.GetComponentsInChildren<UnityEngine.UI.Button>(true)){buttonNames.Add(button.name);if(button.name=="DEVOLVER AL CUERPO"){returnButton=button;break;}}
 Assert.NotNull(returnButton,"Botones de la tarjeta: "+string.Join(", ",buttonNames));lab.Selection.Select(First());yield return null;
 Assert.IsTrue(ui.ExtractButton.interactable);Assert.IsFalse(returnButton.interactable,"No debe ofrecer devolución antes de extraer.");
 lab.ExtractSelected();yield return null;
 Assert.IsFalse(ui.ExtractButton.interactable,"No debe ofrecer una segunda extracción de la misma pieza.");Assert.IsTrue(returnButton.interactable);
 lab.ReturnSelected();yield return null;Assert.IsFalse(returnButton.interactable);
 }
 [UnityTest] public IEnumerator EvaluationExplainsSelectedPieceWithoutCodeOrStudySheet(){
  yield return null;
  var card=GameObject.Find("Evaluación de pieza");Assert.NotNull(card);
  var texts=card.GetComponentsInChildren<UnityEngine.UI.Text>();
  Assert.IsFalse(System.Array.Exists(texts,t=>t.text=="FICHA DE ESTUDIO"),"La evaluación no debe duplicarse en una ficha aparte.");
  lab.SelectById("FJ2564");
  float end=Time.realtimeSinceStartup+30;
  while((lab.Selection.Selected==null||lab.Selection.Selected.Data.id!="FJ2564")&&Time.realtimeSinceStartup<end)yield return null;
  Assert.NotNull(lab.Selection.Selected);
  texts=card.GetComponentsInChildren<UnityEngine.UI.Text>();
  Assert.IsFalse(System.Array.Exists(texts,t=>t.text.Contains("FJ2564")),"El código no debe aparecer en la tarjeta de evaluación.");
  Assert.IsTrue(System.Array.Exists(texts,t=>t.text.Contains("alimentos")&&t.text.Contains("digestión")),"El estómago necesita una explicación breve de su función.");
  Canvas.ForceUpdateCanvases();
  var explanation=System.Array.Find(texts,t=>t.text.Contains("Qué es:")&&t.text.Contains("Función principal:"));
  Assert.NotNull(explanation);
  Assert.LessOrEqual(explanation.preferredHeight,explanation.rectTransform.rect.height,"La descripción completa debe caber en la tarjeta.");
 }
 [UnityTest] public IEnumerator SpatialCatalogFiltersAndEvaluationExtractsSelectedPiece(){
 yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.SpatialLabUI>();Assert.NotNull(ui);
 var letter=ui.CatalogCard.GetComponentsInChildren<UnityEngine.UI.Button>();UnityEngine.UI.Button f=null;foreach(var item in letter)if(item.name=="Letra F"){f=item;break;}Assert.NotNull(f);f.onClick.Invoke();Assert.AreEqual("F",ui.Query);
 var match=lab.Search.Search("fémur",null,1);Assert.Greater(match.Count,0);lab.SelectById(match[0].id);float end=Time.realtimeSinceStartup+30;while((lab.Selection.Selected==null||lab.Selection.Selected.Data.id!=match[0].id)&&Time.realtimeSinceStartup<end)yield return null;
 Assert.IsTrue(ui.ExtractButton.interactable);ui.ExtractButton.onClick.Invoke();yield return null;Assert.IsTrue(lab.Selection.Selected.Extracted);
 lab.ReturnSelected();yield return null;Assert.IsFalse(lab.Selection.Selected.Extracted);
 }
 [UnityTest] public IEnumerator CentralTableUsesTheArchitectLabEmblemAndScanner(){
  yield return null;
  var table=GameObject.Find("Mesa holográfica GuateGeeks");Assert.NotNull(table);
  var bezel=table.transform.Find("Bisel metálico");Assert.NotNull(bezel);
  Assert.That(bezel.GetComponent<Renderer>().bounds.size.x,Is.EqualTo(3.74f).Within(.02f));
  var logo=table.transform.Find("Logotipo GuateGeeks");Assert.NotNull(logo);
  Assert.AreEqual("GuateGeeks/Logo",logo.GetComponent<Renderer>().sharedMaterial.shader.name);
  Assert.NotNull(table.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.AnatomyTableEyes>());
  Assert.NotNull(table.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.AnatomyTableScanner>());
  Assert.That(Vector3.Distance(lab.Body.position,new Vector3(.25f,AnatomyHeight.BaseY(lab.Catalog,lab.HeightCm),2.1f)),Is.LessThan(.001f));
 }
 [UnityTest] public IEnumerator CentralTableHasSpanishRimInscriptionAndArchitectMaterials(){
  yield return null;
  var table=GameObject.Find("Mesa holográfica GuateGeeks");Assert.NotNull(table);
  var inscription=table.transform.Find("Inscripción circular de anatomía");Assert.NotNull(inscription);
  var glyphs=inscription.GetComponentsInChildren<UnityEngine.UI.Text>();Assert.Greater(glyphs.Length,100);
  var words=new System.Text.StringBuilder();var center=new Vector3(.25f,0,2.1f);
  foreach(var glyph in glyphs){words.Append(glyph.text);var position=glyph.transform.position-center;Assert.That(new Vector2(position.x,position.z).magnitude,Is.EqualTo(1.755f).Within(.08f));}
  StringAssert.Contains("LABORATORIODEANATOMÍA",words.ToString().Replace(" ",""));
  Assert.AreEqual("GuateGeeks/LabMetal",table.transform.Find("Consola de proyección").GetComponent<Renderer>().sharedMaterial.shader.name);
  Assert.NotNull(table.transform.Find("Aro inferior de energía"));
 }
 [UnityTest] public IEnumerator GuidedCatalogFiltersWithoutExactNamesAndDisablesUnavailableActions(){
  yield return null;
  var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.SpatialLabUI>();Assert.NotNull(ui);
  var buttons=ui.CatalogCard.GetComponentsInChildren<UnityEngine.UI.Button>(true);
  var bones=System.Array.Find(buttons,b=>b.name=="HUESOS");Assert.NotNull(bones);bones.onClick.Invoke();
  Assert.AreEqual("huesos",ui.Category);
  Assert.IsFalse(ui.ExtractButton.interactable);
  var letter=System.Array.Find(buttons,b=>b.name=="Letra F");letter.onClick.Invoke();Assert.AreEqual("F",ui.Query);
  var femur=System.Array.Find(buttons,b=>b.GetComponentInChildren<UnityEngine.UI.Text>().text.Contains("Fémur"));Assert.NotNull(femur);
  var previous=System.Array.Find(buttons,b=>b.name=="‹ ANTERIOR");Assert.IsFalse(previous.interactable);
 }
 [UnityTest] public IEnumerator SpanishWorkbenchHasMinimalFooterAndNoControllerVisuals(){
 yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.ExplorationWorkbench>();Assert.NotNull(ui);
 Assert.IsFalse(ui.GetComponent<Canvas>().enabled);
 Assert.IsFalse(ui.SearchField.enabled,"El campo de búsqueda del panel oculto no debe generar un cursor sin Canvas.");
 var footer=ui.transform.Find("Marco adaptable/Mesa de exploración");Assert.NotNull(footer);Assert.AreEqual(1,footer.GetComponentsInChildren<UnityEngine.UI.Button>().Length);Assert.AreEqual(1,footer.GetComponentsInChildren<UnityEngine.UI.Slider>().Length);
 foreach(var label in ui.GetComponentsInChildren<UnityEngine.UI.Text>()){StringAssert.DoesNotContain("MANDO",label.text);StringAssert.DoesNotContain("EDITOR",label.text);StringAssert.DoesNotContain("RESET BODY",label.text);StringAssert.DoesNotContain("EXPLODED",label.text);}
 Assert.AreEqual(0,ui.Controls.GetComponentsInChildren<Renderer>().Length,"Desktop manipulation must not create visible controller geometry.");
 var stomach=lab.Search.Search("stomach",null,1);Assert.AreEqual(1,stomach.Count);Assert.AreEqual("Estómago",lab.Name(stomach[0]));Assert.AreEqual(stomach[0].id,lab.Search.Search("estómago",null,1)[0].id);Assert.AreEqual("es",lab.Language);
 }
 [UnityTest] public IEnumerator DesktopPointerRaycastAndCardDrag(){
 yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.SpatialLabUI>();lab.Selection.Select(First());yield return new WaitForSeconds(.2f);Canvas.ForceUpdateCanvases();Assert.IsTrue(ui.ExtractButton.interactable);
 var events=UnityEngine.EventSystems.EventSystem.current;var module=events.currentInputModule as UnityEngine.InputSystem.UI.InputSystemUIInputModule;Assert.NotNull(module);Assert.IsTrue(module.point.action.enabled);Assert.IsTrue(module.leftClick.action.enabled);
 var rect=ui.ExtractButton.GetComponent<RectTransform>();var point=RectTransformUtility.WorldToScreenPoint(lab.ViewCamera,rect.TransformPoint(rect.rect.center));var pointer=new UnityEngine.EventSystems.PointerEventData(events){position=point};var hits=new List<UnityEngine.EventSystems.RaycastResult>();events.RaycastAll(pointer,hits);Assert.IsTrue(hits.Count>0);Assert.AreEqual(ui.ExtractButton,hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>());
 UnityEngine.EventSystems.ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);yield return null;var controls=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.XR.DesktopControllerSimulator>();Assert.IsTrue(controls.Holding);controls.Release();
 var drag=ui.CatalogCard.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.SpatialPanelHandle>();var before=drag.Panel.position;rect=drag.GetComponent<RectTransform>();pointer.position=RectTransformUtility.WorldToScreenPoint(lab.ViewCamera,rect.TransformPoint(rect.rect.center));drag.OnPointerDown(pointer);pointer.position+=new Vector2(30,-20);drag.OnDrag(pointer);Assert.AreNotEqual(before,drag.Panel.position);drag.OnEndDrag(pointer);
 var systemsPanel=GameObject.Find("Panel de sistemas anatómicos");var systemsHandle=systemsPanel.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.SpatialPanelHandle>();before=systemsHandle.Panel.position;rect=systemsHandle.GetComponent<RectTransform>();pointer.position=RectTransformUtility.WorldToScreenPoint(lab.ViewCamera,rect.TransformPoint(rect.rect.center));systemsHandle.OnPointerDown(pointer);pointer.position+=new Vector2(-30,20);systemsHandle.OnDrag(pointer);Assert.AreNotEqual(before,systemsHandle.Panel.position,"El bloque de sistemas debe poder arrastrarse con el ratón.");systemsHandle.OnEndDrag(pointer);
 }
 [UnityTest] public IEnumerator SpatialCardsStayAboveFloorAndOutsidePlatformWhenMoved(){
 yield return null;
 var panel=GameObject.Find("Panel de sistemas anatómicos").GetComponent<RectTransform>();
 var handle=panel.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.SpatialPanelHandle>();
 var start=panel.position;var rotation=panel.rotation;
 var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(lab.ViewCamera,start)};
 handle.OnPointerDown(pointer);
 pointer.position=RectTransformUtility.WorldToScreenPoint(lab.ViewCamera,start+Vector3.down*5f);
 handle.OnDrag(pointer);AssertPanelAboveFloor(panel);handle.OnEndDrag(pointer);
 panel.SetPositionAndRotation(start,rotation);
 var hand=new object();var forward=panel.forward;
 Assert.IsTrue(handle.BeginGrab(hand,new Ray(start-forward,forward),Quaternion.identity));
 var underFloor=new Vector3(start.x,-3f,start.z);
 handle.MoveGrab(hand,new Ray(underFloor-forward,forward),Quaternion.identity);AssertPanelAboveFloor(panel);
 var inPedestal=new Vector3(.25f,.3f,2.1f);
 handle.MoveGrab(hand,new Ray(inPedestal-forward,forward),Quaternion.identity);AssertPanelOutsidePedestal(panel);
 var oppositeSide=new Vector3(3f,.3f,2.1f);
 handle.MoveGrab(hand,new Ray(oppositeSide-forward,forward),Quaternion.identity);Assert.Less(panel.position.x,.25f,"La tarjeta no debe atravesar la plataforma al mover el mando de un lado a otro.");
 handle.EndGrab(hand);panel.SetPositionAndRotation(start,rotation);
 Assert.IsTrue(handle.BeginGrab(hand,new Ray(start-forward,forward),Quaternion.identity));
 handle.MoveGrab(hand,new Ray(start-forward,forward),Quaternion.Euler(0,0,90));AssertPanelAboveFloor(panel);
 handle.EndGrab(hand);
 }
 static void AssertPanelAboveFloor(RectTransform panel){var corners=new Vector3[4];panel.GetWorldCorners(corners);foreach(var corner in corners)Assert.GreaterOrEqual(corner.y,.029f,"Ningún borde de la tarjeta debe atravesar el suelo.");}
 static void AssertPanelOutsidePedestal(RectTransform panel){var corners=new Vector3[4];panel.GetWorldCorners(corners);foreach(var corner in corners)if(corner.y<.73f)Assert.GreaterOrEqual(Vector2.Distance(new Vector2(corner.x,corner.z),new Vector2(.25f,2.1f)),1.8f,"La tarjeta no debe entrar en la base del cuerpo.");}
 [UnityTest] public IEnumerator FullLabVisualSmoke(){lab.Systems.ResetVisibility();float end=Time.realtimeSinceStartup+90;while(lab.Streaming.Loading.Count>0&&Time.realtimeSinceStartup<end)yield return null;Assert.Greater(lab.Streaming.Loaded.Count,200);yield return null;
 var camera=lab.ViewCamera;var target=new RenderTexture(1800,1200,24);camera.targetTexture=target;camera.Render();RenderTexture.active=target;var image=new Texture2D(1800,1200,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1800,1200),0,0);image.Apply();System.IO.Directory.CreateDirectory("Documentation/Previews");System.IO.File.WriteAllBytes("Documentation/Previews/laboratory.png",image.EncodeToPNG());camera.targetTexture=null;RenderTexture.active=null;Object.Destroy(image);Object.Destroy(target);yield return null;
 }
 [UnityTest] public IEnumerator PuzzleCountsOnlySnappedPlacements(){int correct=lab.Quiz.Statistics.correctPlacements;lab.BeginPuzzle();float end=Time.realtimeSinceStartup+90;while(lab.Extracted.Count<5&&Time.realtimeSinceStartup<end)yield return null;Assert.AreEqual(5,lab.Extracted.Count);var adapters=lab.Extracted.ToArray();foreach(var a in adapters){a.transform.SetPositionAndRotation(lab.Body.TransformPoint(a.State.Data.originalPosition),lab.Body.rotation*a.State.Data.originalRotation);a.ReleaseDesktop();}yield return null;Assert.AreEqual(correct+5,lab.Quiz.Statistics.correctPlacements);Assert.AreEqual(0,lab.Extracted.Count);}
 [UnityTest] public IEnumerator ExtractedPieceCanBeGrabbedThroughXRI(){var state=First();var adapter=lab.Extract(state);var direct=Object.FindAnyObjectByType<XRDirectInteractor>();direct.selectInput.inputSourceMode=XRInputButtonReader.InputSourceMode.ManualValue;direct.selectInput.manualPerformed=true;direct.selectInput.manualValue=1;direct.transform.position=state.Object.transform.position;yield return new WaitForFixedUpdate();var manager=Object.FindAnyObjectByType<XRInteractionManager>();if(!adapter.Grab.isSelected)manager.SelectEnter((IXRSelectInteractor)direct,(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)adapter.Grab);Assert.IsTrue(adapter.Grab.isSelected);direct.selectInput.manualPerformed=false;direct.selectInput.manualValue=0;manager.SelectExit((IXRSelectInteractor)direct,(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)adapter.Grab);Assert.IsFalse(adapter.Grab.isSelected);lab.ReturnToBody(state);yield return null;}
 [UnityTest] public IEnumerator ToggleDuringUnloadKeepsVisibleSystemLoaded(){const string id="skeleton";lab.Systems.ShowSystem(id);lab.Systems.HideSystem(id);var unloading=lab.StartCoroutine(lab.Streaming.UnloadSystemAsync(id));lab.Systems.ShowSystem(id);yield return unloading;float end=Time.realtimeSinceStartup+60;while(lab.Streaming.Loading.Contains(id)&&Time.realtimeSinceStartup<end)yield return null;int count=0;foreach(var s in lab.Streaming.Loaded.Values)if(s.Data.systemId==id){count++;Assert.IsTrue(s.Object.activeSelf);}Assert.AreEqual(204,count);}
 [UnityTest] public IEnumerator WorkbenchSelectionExtractionAndReturnButtons(){yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.ExplorationWorkbench>();Assert.NotNull(ui);var state=First();lab.Selection.Select(state);Assert.AreEqual(lab.Name(state.Data),ui.InfoTitle.text);ui.ShowDetails();Assert.NotNull(ui.GetComponentInChildren<GuateGeeks.AnatomyLab.UI.DraggablePanel>());ui.CloseDetails();ui.GrabButton.onClick.Invoke();yield return new WaitForFixedUpdate();yield return null;Assert.IsTrue(ui.Controls.Holding);Assert.IsTrue(state.Extracted);ui.Controls.RotateHeld(30);ui.ToggleGrip();Assert.IsFalse(ui.Controls.Holding);ui.Return();yield return new WaitForSeconds(1);Assert.IsFalse(state.Extracted);ui.ExplosionSlider.value=.5f;Assert.AreEqual(.5f,lab.Explosion.Factor);}
 [UnityTest] public IEnumerator WorkbenchSearchAndHandSwitch(){yield return null;var ui=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.UI.ExplorationWorkbench>();ui.SearchField.text="femur";yield return null;bool initial=ui.Controls.LeftHand;ui.Controls.ToggleHand();Assert.AreNotEqual(initial,ui.Controls.LeftHand);Assert.IsTrue(ui.SearchField.text=="femur");}
 [UnityTest] public IEnumerator SelectionAndIsolation(){var s=First();lab.Selection.Select(s);Assert.AreEqual(s,lab.Selection.Selected);lab.Isolate();foreach(var other in lab.Streaming.Loaded.Values)Assert.AreEqual(other==s,other.Object.activeSelf);lab.ExitIsolation();yield return null;Assert.IsTrue(s.Object.activeSelf);}
 [UnityTest] public IEnumerator ExtractionAndReturnRemovePhysics(){var s=First();lab.Selection.Select(s);lab.ExtractSelected();Assert.IsTrue(s.Extracted);Assert.NotNull(s.Object.GetComponent<Rigidbody>());lab.ReturnToBody(s);yield return new WaitForSeconds(1.3f);Assert.IsFalse(s.Extracted);Assert.IsNull(s.Object.GetComponent<Collider>());Assert.Less(Vector3.Distance(s.Object.transform.localPosition,s.Data.originalPosition),.005f);}
 [UnityTest] public IEnumerator SnapRequiresPositionAndRotation(){var s=First();var a=lab.Extract(s);s.Object.transform.SetPositionAndRotation(lab.Body.TransformPoint(s.Data.originalPosition),lab.Body.rotation*s.Data.originalRotation);Assert.IsTrue(a.CanSnap);s.Object.transform.rotation*=Quaternion.Euler(0,100,0);Assert.IsFalse(a.CanSnap);s.Object.transform.position+=Vector3.one;Assert.IsFalse(a.CanSnap);lab.ReturnToBody(s);yield return null;}
 [UnityTest] public IEnumerator RightRayGripTakesAnatomyWithoutTeleportingTheBody(){
  var state=First();lab.Selection.Select(state);lab.Isolate();var pose=state.Object.transform.position;var body=lab.Body.position;
  var center=state.Object.transform.TransformPoint(state.Mesh.bounds.center);var ray=new Ray(center-Vector3.forward,Vector3.forward);
  var input=Object.FindAnyObjectByType<GuateGeeks.AnatomyLab.XR.LabInput>();
  Assert.IsTrue(input.TryBeginPieceGrab(ray,Quaternion.identity));
  Assert.IsTrue(state.Extracted);Assert.Less(Vector3.Distance(pose,state.Object.transform.position),.001f,"El agarre no debe hacer saltar la pieza hacia la cámara.");
  Assert.IsFalse(state.Object.GetComponent<StructureInteractionAdapter>().Grab.enabled,"El agarre de contacto no debe disputar la pieza al rayo durante la manipulación.");
  Assert.AreEqual(body,lab.Body.position,"El cuerpo debe permanecer en la plataforma.");
  state.Object.transform.position+=Vector3.right*.3f;
  input.EndPieceGrab();Assert.IsTrue(state.Extracted);Assert.IsTrue(state.Object.GetComponent<StructureInteractionAdapter>().Grab.enabled);yield return null;
 }
 [UnityTest] public IEnumerator RightControllerCanMoveEnlargeRotateAndReturnAPiece(){
  var state=First();var adapter=lab.Extract(state);var body=lab.Body.position;var originalScale=state.Object.transform.localScale;
  var center=state.Object.transform.TransformPoint(state.Mesh.bounds.center);var ray=new Ray(center-Vector3.forward,Vector3.forward);
  var grip=new StructureRayManipulator();Assert.IsTrue(grip.Begin(adapter,ray,Quaternion.identity));
  grip.Step(new Ray(ray.origin+Vector3.right*.3f,Vector3.forward),Quaternion.identity,new Vector2(.8f,.8f),true,false,false,Vector3.zero,Vector3.zero,1f);
  Assert.Greater(state.Object.transform.localScale.x,originalScale.x,"El botón A debe ampliar únicamente la pieza agarrada.");
  Assert.Greater(Vector3.Distance(state.Object.transform.TransformPoint(state.Mesh.bounds.center),center),.1f,"La pieza debe seguir el rayo del mando.");
  Assert.Greater(Quaternion.Angle(state.Object.transform.rotation,Quaternion.identity),1f,"La palanca horizontal debe girar la pieza.");
  var beforeTwoHands=state.Object.transform.localScale.x;
  grip.Step(ray,Quaternion.identity,Vector2.zero,false,false,true,Vector3.left*.2f,Vector3.right*.2f,.02f);
  grip.Step(ray,Quaternion.identity,Vector2.zero,false,false,true,Vector3.left*.5f,Vector3.right*.5f,.02f);
  Assert.Greater(state.Object.transform.localScale.x,beforeTwoHands,"Separar los dos mandos debe ampliar la pieza.");
  Assert.AreEqual(body,lab.Body.position);Assert.AreEqual(Vector3.one*AnatomyHeight.ScaleFor(lab.Catalog,lab.HeightCm),lab.Body.localScale);
  grip.End();lab.ReturnToBody(state);yield return new WaitForSeconds(1.2f);
  Assert.That(Vector3.Distance(state.Object.transform.localScale,state.Data.originalScale),Is.LessThan(.005f),"Al devolverla, la pieza recupera su tamaño anatómico.");
 }
 [UnityTest] public IEnumerator SystemToggleAndReset(){var s=First();lab.Systems.HideSystem(s.Data.systemId);Assert.IsFalse(s.Object.activeSelf);lab.Systems.ShowSystem(s.Data.systemId);Assert.IsTrue(s.Object.activeSelf);lab.Selection.Select(s);lab.ToggleTransparency();lab.SetExplosion(1);lab.ResetBody();yield return new WaitForSeconds(1.2f);Assert.AreEqual(0,lab.Explosion.Factor);Assert.AreEqual(StructureVisibility.Opaque,s.Visibility);Assert.Less(Vector3.Distance(s.Object.transform.localPosition,s.Data.originalPosition),.005f);}
 [UnityTest] public IEnumerator QuizFeedbackTracksAttempts(){var s=First();int prior=lab.Quiz.Statistics.correct;lab.Quiz.Ask(s.Data.id);Assert.IsFalse(lab.Quiz.Answer("wrong"));lab.Selection.Select(s);Assert.AreEqual(prior+1,lab.Quiz.Statistics.correct);Assert.IsFalse(lab.Quiz.Active);yield return null;}
 [UnityTest] public IEnumerator SearchLoadsHiddenSystemAndFocusDoesNotMoveCamera(){var matches=lab.Search.Search("femur");Assert.Greater(matches.Count,0);Vector3 head=lab.ViewCamera.transform.position;lab.SelectById(matches[0].id);float end=Time.realtimeSinceStartup+90;while((lab.Selection.Selected==null||lab.Selection.Selected.Data.id!=matches[0].id)&&Time.realtimeSinceStartup<end)yield return null;Assert.AreEqual(matches[0].id,lab.Selection.Selected.Data.id);Assert.AreEqual(head,lab.ViewCamera.transform.position);}
 [UnityTest] public IEnumerator ExplodedStructuresRemainSelectable(){var s=First();lab.SetExplosion(.75f);yield return new WaitForSeconds(1);var ray=new Ray(s.Object.transform.position-Vector3.forward*.01f,Vector3.forward);Assert.NotNull(lab.Pick(ray));}
}
}
