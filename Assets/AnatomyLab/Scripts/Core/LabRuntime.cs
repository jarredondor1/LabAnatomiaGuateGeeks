using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using GuateGeeks.AnatomyLab.Anatomy;
using GuateGeeks.AnatomyLab.Interaction;
using GuateGeeks.AnatomyLab.ExplodedView;
using GuateGeeks.AnatomyLab.Search;
using GuateGeeks.AnatomyLab.Education;
using GuateGeeks.AnatomyLab.XR;
namespace GuateGeeks.AnatomyLab.Core {
public sealed class LabRuntime : MonoBehaviour {
 public AnatomyCatalog Catalog; public AnatomyStreamingManager Streaming; public AnatomySystemManager Systems;
 public AnatomySelectionManager Selection=new AnatomySelectionManager(); public ExplodedViewController Explosion=new ExplodedViewController();
 public AnatomySearchIndex Search; public QuizManager Quiz=new QuizManager();public EducationalContentDatabase Education;
 public Transform Body; public Camera ViewCamera; public Material Opaque,Transparent; public bool Ready; public string Status="Inicializando catálogo…";
 public string ReferenceId {get;private set;}="male";
 public int HeightCm {get;private set;}=AnatomyHeight.DefaultCm;
 public string SourceName=>ReferenceId=="female"?"HRA v1.5":"BodyParts3D 4.0";
 public string ReferenceName=>ReferenceId=="female"?"Femenino":"Masculino";
 public string Coverage=>ReferenceId=="female"?"Superficie corporal y órganos seleccionados. Esqueleto y musculatura parciales. Embarazo en una capa aparte.":"Referencia masculina BodyParts3D. La cobertura depende de las estructuras incluidas en el catálogo.";
 string SystemPreference(string id)=>"anatomy.system."+(ReferenceId=="female"?"female.":"")+id;
 public event Action Changed; public bool Isolated {get;private set;}
 public int TutorialStep;public bool TutorialDone;public float Volume=0.25f;public string Language="es";public bool LeftHanded;
 public readonly List<StructureInteractionAdapter> Extracted=new List<StructureInteractionAdapter>();
 MaterialPropertyBlock block; readonly Dictionary<string,Color> colors=new Dictionary<string,Color>();
 readonly HashSet<string> puzzle=new HashSet<string>(); bool puzzleActive; float puzzleStart;
 AudioSource audioSource; AudioClip tone; Vector3 initialBodyPosition; Quaternion initialBodyRotation; Vector2 referenceBounds; float pulseClock;
 IEnumerator Start(){
 block=new MaterialPropertyBlock();
 ReferenceId=PlayerPrefs.GetString("anatomy.reference","male")=="female"?"female":"male";
 Catalog=new LocalDatasetProvider().LoadCatalog(ReferenceId);GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(Catalog);Search=new AnatomySearchIndex(Catalog);Systems=new AnatomySystemManager(Catalog);Education=EducationalContentDatabase.Load(ReferenceId);Quiz.Load();
 Volume=PlayerPrefs.GetFloat("anatomy.volume",.25f);Language="es";LeftHanded=PlayerPrefs.GetInt("anatomy.leftHanded",0)==1;TutorialDone=PlayerPrefs.GetInt("anatomy.tutorial",0)==1;
 foreach(var sys in Catalog.systems){colors[sys.id]=sys.defaultColor;Systems.Visible[sys.id]=PlayerPrefs.GetInt(SystemPreference(sys.id),sys.defaultVisibility?1:0)==1;}
 referenceBounds=AnatomyHeight.Bounds(Catalog);
 HeightCm=Mathf.Clamp(PlayerPrefs.GetInt("anatomy.heightCm",AnatomyHeight.DefaultCm),AnatomyHeight.MinimumCm,AnatomyHeight.MaximumCm);
 initialBodyPosition=Body.position;initialBodyRotation=Body.rotation;ApplyBodyHeight();
 Streaming=gameObject.AddComponent<AnatomyStreamingManager>();Streaming.Initialize(Catalog,Body,Opaque,Transparent);Streaming.IsSystemDesired=id=>Systems.Visible[id];Streaming.Added+=s=>{s.Moving=Explosion.Factor>0;Refresh(s);Changed?.Invoke();};Streaming.Removed+=id=>{if(Streaming.Loaded.TryGetValue(id,out var removed)){var a=removed.Object.GetComponent<StructureInteractionAdapter>();if(a!=null){a.DisableInteraction();Extracted.Remove(a);}}if(Selection.Selected?.Data.id==id)Selection.Clear();};
 Systems.Changed+=(id,on)=>{if(on)StartCoroutine(Streaming.LoadSystemAsync(id));foreach(var s in Streaming.Loaded.Values)if(s.Data.systemId==id)Refresh(s);PlayerPrefs.SetInt(SystemPreference(id),on?1:0);Changed?.Invoke();};
 Selection.Changed+=Selected;Selection.HoverChanged+=s=>{if(s!=null)AdvanceTutorial(0);RefreshAll();Changed?.Invoke();};
 audioSource=gameObject.AddComponent<AudioSource>();audioSource.spatialBlend=0; tone=AudioClip.Create("Generated feedback",2205,1,22050,false);var samples=new float[2205];for(int i=0;i<samples.Length;i++)samples[i]=Mathf.Sin(i*2*Mathf.PI*660/22050)*.12f*(1-i/(float)samples.Length);tone.SetData(samples,0);
 Ready=true;
 foreach(var sys in Catalog.systems)if(Systems.Visible[sys.id])yield return Streaming.LoadSystemAsync(sys.id);
 Status=ReferenceName+" · "+Catalog.structures.Length+" estructuras · sin conexión";Changed?.Invoke();
 }
 public void SwitchReference(string reference){
 if(!Ready||reference==ReferenceId)return;
 // Validate before committing the choice; reload the scene to clear meshes, grabs, quizzes and UI atomically.
 new LocalDatasetProvider().LoadCatalog(reference);
 Ready=false;Status="Cambiando referencia anatómica…";
 foreach(var adapter in Extracted.ToArray())if(adapter!=null){adapter.Released-=Released;adapter.DisableInteraction();}
 Extracted.Clear();StopAllCoroutines();Streaming.StopAllCoroutines();
 PlayerPrefs.SetString("anatomy.reference",reference);PlayerPrefs.Save();
 SceneManager.LoadSceneAsync(gameObject.scene.name);
 }
 public string Name(AnatomyStructure s){
  string system=s.systemId;foreach(var entry in Catalog.systems)if(entry.id==s.systemId){system=entry.displayName;break;}
  return AnatomyPieceSummary.Title(s,s.displayName,system);
 }
 void Selected(StructureState s){RefreshAll();if(s!=null){AdvanceTutorial(1);Feedback();Status=Name(s.Data);if(Quiz.Active){bool correct=Quiz.Answer(s.Data.id);Status=correct?"¡Correcto! Inicia el siguiente reto.":"Inténtalo de nuevo.";Feedback(correct?1.3f:.65f);}}Changed?.Invoke();}
 public void Feedback(float pitch=1){if(audioSource!=null){audioSource.pitch=pitch;audioSource.PlayOneShot(tone,Volume);}HapticFeedbackService.Pulse();}
 public void RefreshAll(){if(Streaming==null)return;foreach(var s in Streaming.Loaded.Values)Refresh(s);}
 public void Refresh(StructureState s){
 bool visible=Systems.Visible[s.Data.systemId]&&s.Visibility!=StructureVisibility.Hidden&&(!Isolated||Selection.Selected==s||s.Extracted);
 s.Object.SetActive(visible);if(!visible)return;
 s.Renderer.sharedMaterial=s.Visibility==StructureVisibility.Transparent?Transparent:Opaque;
 var color=colors[s.Data.systemId];if(Selection.Selected==s)color=Color.Lerp(color,Color.white,.6f);else if(Selection.Hovered==s)color=Color.Lerp(color,Color.cyan,.45f);
 if(s.Extracted){var a=s.Object.GetComponent<StructureInteractionAdapter>();if(a!=null&&a.CanSnap)color=Color.green;}
 color.a=s.Visibility==StructureVisibility.Transparent?(s.Data.defaultTransparent?.09f:.24f):1;block.SetColor("_BaseColor",color);block.SetColor("_Color",color);s.Renderer.SetPropertyBlock(block);
 }
 public StructureState Pick(Ray ray){
 StructureState best=null;float nearest=12;
 foreach(var s in Streaming.Loaded.Values){if(!s.Object.activeInHierarchy||(s.Data.defaultTransparent&&s.Visibility==StructureVisibility.Transparent))continue;var tr=s.Object.transform;var local=new Ray(tr.InverseTransformPoint(ray.origin),tr.InverseTransformDirection(ray.direction));
 if(s.Mesh.bounds.IntersectRay(local,out float distance)){var hit=tr.TransformPoint(local.GetPoint(distance));float world=Vector3.Distance(ray.origin,hit);if(world<nearest){nearest=world;best=s;}}}return best;
 }
 public void SelectById(string id){StartCoroutine(SelectLoaded(id));}
 IEnumerator SelectLoaded(string id){
 AnatomyStructure data=null;foreach(var d in Catalog.structures)if(d.id==id){data=d;break;}if(data==null)yield break;
 ExitIsolation();Systems.ShowSystem(data.systemId);while(Streaming.Loading.Contains(data.systemId))yield return null;
 if(Streaming.Loaded.TryGetValue(id,out var s)){s.Visibility=StructureVisibility.Opaque;Selection.Select(s);}
 }
 public void SetExplosion(float value){Explosion.SetFactor(value);foreach(var s in Streaming.Loaded.Values)s.Moving=true;if(value>0)AdvanceTutorial(4);Changed?.Invoke();}
 public void Isolate(){if(Selection.Selected==null)return;Isolated=true;RefreshAll();Changed?.Invoke();}
 public void ExitIsolation(){Isolated=false;RefreshAll();Changed?.Invoke();}
 public void Hide(){if(Selection.Selected==null)return;Selection.Selected.Visibility=StructureVisibility.Hidden;Selection.Clear();RefreshAll();}
 public void ToggleTransparency(){var s=Selection.Selected;if(s==null)return;s.Visibility=s.Visibility==StructureVisibility.Transparent?StructureVisibility.Opaque:StructureVisibility.Transparent;Refresh(s);}
 public void SetSystemTransparency(string id){foreach(var s in Streaming.Loaded.Values)if(s.Data.systemId==id){s.Visibility=StructureVisibility.Transparent;Refresh(s);}}
 public void ExtractSelected(){if(Selection.Selected!=null)Extract(Selection.Selected);}
 public StructureInteractionAdapter Extract(StructureState s){
 if(s.Extracted)return s.Object.GetComponent<StructureInteractionAdapter>();
 var a=s.Object.AddComponent<StructureInteractionAdapter>();a.Initialize(s,Body);a.Released+=Released;Extracted.Add(a);
 s.Object.transform.position=ViewCamera.transform.position+ViewCamera.transform.forward*.65f-Vector3.up*.15f;
 AdvanceTutorial(2);Feedback();Changed?.Invoke();return a;
 }
 void Released(StructureInteractionAdapter a,bool valid){if(valid){ReturnToBody(a.State);if(puzzle.Remove(a.State.Data.id)){Quiz.Statistics.correctPlacements++;if(puzzle.Count==0){puzzleActive=false;Quiz.Statistics.completionTime=Time.time-puzzleStart;Status="Ensamblaje completado";}Quiz.Save();}}else if(puzzleActive){Quiz.Statistics.wrongPlacements++;Quiz.Save();}}
 public void ReturnSelected(){if(Selection.Selected!=null)ReturnToBody(Selection.Selected);}
 public void ReturnToBody(StructureState s){var a=s.Object.GetComponent<StructureInteractionAdapter>();if(a!=null){a.Released-=Released;a.DisableInteraction();Extracted.Remove(a);Destroy(a);}s.Extracted=false;s.Returning=true;s.Moving=true;s.Object.transform.SetParent(Body,true);s.Target=s.Data.originalPosition;AdvanceTutorial(3);Feedback(1.2f);Changed?.Invoke();}
 public void SetHeightCm(int centimeters){
  int selected=Mathf.Clamp(centimeters,AnatomyHeight.MinimumCm,AnatomyHeight.MaximumCm);
  if(selected==HeightCm)return;
  HeightCm=selected;PlayerPrefs.SetInt("anatomy.heightCm",HeightCm);
  if(Catalog!=null&&Body!=null)ApplyBodyHeight();
  Changed?.Invoke();
 }
 void ApplyBodyHeight(){
  float scale=HeightCm*.01f/Mathf.Max(.01f,referenceBounds.y-referenceBounds.x);
  initialBodyPosition.y=-referenceBounds.x*scale;
  Body.position=new Vector3(Body.position.x,initialBodyPosition.y,Body.position.z);
  Body.localScale=Vector3.one*scale;
 }
 public void ResetBody(){Explosion.SetFactor(0);Isolated=false;puzzle.Clear();puzzleActive=false;Body.SetPositionAndRotation(initialBodyPosition,initialBodyRotation);Body.localScale=Vector3.one*(HeightCm*.01f/Mathf.Max(.01f,referenceBounds.y-referenceBounds.x));
 foreach(var s in Streaming.Loaded.Values){if(s.Extracted)ReturnToBody(s);s.Returning=true;s.Moving=true;s.Visibility=s.Data.defaultTransparent?StructureVisibility.Transparent:StructureVisibility.Opaque;}Systems.ResetVisibility();Selection.Clear();RefreshAll();Changed?.Invoke();}
 public void FocusStructure(string id){if(Streaming.Loaded.TryGetValue(id,out var s))Selection.Select(s);}
 public void CenterBody(){Body.position=initialBodyPosition;Body.rotation=initialBodyRotation;}
 public void Rotate(float angle){Body.Rotate(Vector3.up,angle,Space.World);}
 public void Scale(float factor){if(ViewCamera==null||Body==null)return;var rig=ViewCamera.GetComponentInParent<DesktopRig>();if(rig!=null)rig.ZoomToward(Body.position,factor);}
 public void AskQuestion(){var choices=new List<StructureState>();foreach(var s in Streaming.Loaded.Values)if(s.Object.activeInHierarchy)choices.Add(s);if(choices.Count==0)return;var target=choices[UnityEngine.Random.Range(0,choices.Count)];Quiz.Ask(target.Data.id);Selection.Clear();Status="Encuentra: "+Name(target.Data);Changed?.Invoke();}
 public void BeginPuzzle(){StartCoroutine(Puzzle());}
 IEnumerator Puzzle(){ResetBody();puzzleActive=true;puzzleStart=Time.time;int count=0;foreach(var e in Education.entries){yield return SelectLoaded(e.id);if(Selection.Selected==null)continue;var s=Selection.Selected;var a=Extract(s);a.transform.position=new Vector3(-.55f+count*.25f,1.05f,1.0f);puzzle.Add(s.Data.id);if(++count>=5)break;}Status="Devuelve las "+puzzle.Count+" piezas con el agarre. Verde = posición y orientación válidas.";Changed?.Invoke();}
 public void SkipTutorial(){TutorialDone=true;PlayerPrefs.SetInt("anatomy.tutorial",1);Changed?.Invoke();}
 void AdvanceTutorial(int step){if(TutorialStep==step){TutorialStep++;if(TutorialStep>=5)SkipTutorial();}}
 public void SavePreferences(){PlayerPrefs.SetFloat("anatomy.volume",Volume);PlayerPrefs.SetString("anatomy.language",Language);PlayerPrefs.SetInt("anatomy.leftHanded",LeftHanded?1:0);PlayerPrefs.Save();Changed?.Invoke();}
 void Update(){if(!Ready)return;float t=1-Mathf.Exp(-Time.deltaTime*8);
 float localRadius=1.72f/Mathf.Max(.1f,Body.localScale.x);
 float localCeiling=Mathf.Max(referenceBounds.y+.15f,2.5f/Mathf.Max(.1f,Body.localScale.y));
 foreach(var s in Streaming.Loaded.Values){if(s.Extracted||!s.Moving)continue;var tr=s.Object.transform;var target=s.Returning?s.Data.originalPosition:ExplodedViewController.Position(s.Data.originalPosition,new Vector3(0,.875f,0),Explosion.Factor,s.Data.bounds,localRadius,localCeiling);tr.localPosition=Vector3.Lerp(tr.localPosition,target,t);tr.localRotation=Quaternion.Slerp(tr.localRotation,s.Data.originalRotation,t);tr.localScale=Vector3.Lerp(tr.localScale,s.Data.originalScale,t);if(Vector3.Distance(tr.localPosition,target)<.0001f&&Quaternion.Angle(tr.localRotation,s.Data.originalRotation)<.05f){tr.localPosition=target;tr.localRotation=s.Data.originalRotation;tr.localScale=s.Data.originalScale;s.Moving=s.Returning&&Explosion.Factor>0;s.Returning=false;}}
 if(Quiz.Active)Quiz.Statistics.elapsed+=Time.deltaTime;
 pulseClock+=Time.deltaTime;if(pulseClock>.1f){pulseClock=0;foreach(var a in Extracted)if(a!=null)Refresh(a.State);}
 }
 void OnDestroy(){if(tone!=null)Destroy(tone);PlayerPrefs.Save();}
}
}
