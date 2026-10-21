using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Anatomy;
using GuateGeeks.AnatomyLab.Search;
using GuateGeeks.AnatomyLab.ExplodedView;
using GuateGeeks.AnatomyLab.Education;
namespace GuateGeeks.AnatomyLab.Tests {
public class CatalogTests {
 AnatomyCatalog catalog;
 [SetUp] public void Setup(){catalog=new LocalDatasetProvider().LoadCatalog();}
 [Test] public void FemaleCatalogHasIndependentGeometryAndSpanishReproductiveNames(){
 var female=new LocalDatasetProvider().LoadCatalog("female");female.Validate();GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(female);
 Assert.AreEqual(888,female.structures.Length);Assert.AreEqual(888,female.structures.Select(d=>d.id).Distinct().Count());Assert.IsFalse(female.structures.Any(d=>catalog.structures.Any(m=>m.id==d.id)));
 Assert.AreEqual(38,female.structures.Count(d=>d.systemId=="reproductive"));Assert.AreEqual(8,female.structures.Count(d=>d.systemId=="pregnancy"));Assert.IsFalse(female.systems.Single(d=>d.id=="pregnancy").defaultVisibility);
 var search=new AnatomySearchIndex(female);Assert.IsTrue(search.Search("útero").Any(d=>d.displayName=="Cuerpo del útero"));Assert.IsTrue(search.Search("ovario").Any(d=>d.displayName=="Ovario izquierdo"));Assert.IsTrue(search.Search("mama").Count>0);Assert.IsTrue(search.Search("testis").Count==0);
 Assert.AreEqual(1,female.structures.Count(d=>d.defaultTransparent));
 }
 [Test] public void FemaleMeshesHaveValidIndicesAndSourceBounds(){
 var female=new LocalDatasetProvider().LoadCatalog("female");long triangles=0;
 foreach(var data in female.structures){var asset=Resources.Load<TextAsset>(data.meshReference);Assert.NotNull(asset,data.id);var mesh=AnatomyStreamingManager.ReadMesh(asset.bytes);Assert.AreEqual(data.vertexCount,mesh.vertexCount,data.id);var indices=mesh.triangles;Assert.AreEqual(data.triangleCount*3,indices.Length,data.id);foreach(var i in indices)if(i<0||i>=mesh.vertexCount)Assert.Fail(data.id+" índice inválido");Assert.Less(Vector3.Distance(mesh.bounds.size,data.bounds.size),.0001f,data.id);triangles+=data.triangleCount;UnityEngine.Object.DestroyImmediate(mesh);Resources.UnloadAsset(asset);}
 Assert.AreEqual(5557599,triangles);
 }
 [Test] public void CompleteCatalogHas2234UniqueMeshes(){catalog.Validate();Assert.AreEqual(2234,catalog.structures.Length);Assert.AreEqual(2234,catalog.structures.Select(s=>s.id).Distinct().Count());}
 [Test] public void PersonalHeightPlacesFeetOnFloorForBothReferences(){
  foreach(var reference in new[]{"male","female"}){
   var body=new LocalDatasetProvider().LoadCatalog(reference);
   var range=AnatomyHeight.Bounds(body);
   Assert.AreEqual(0,range.x,.001f,reference);
   foreach(var height in new[]{130,175,210}){
    var scale=AnatomyHeight.ScaleFor(body,height);
    Assert.AreEqual(height/100f,(range.y-range.x)*scale,.005f,reference);
    Assert.AreEqual(0,AnatomyHeight.BaseY(body,height)+range.x*scale,.005f,reference);
   }
  }
 }
 [Test] public void EveryCatalogPieceHasAUsefulSpecificExplanation(){
  foreach(var reference in new[]{"male","female"}){
   var body=new LocalDatasetProvider().LoadCatalog(reference);GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(body);
   foreach(var part in body.structures){
    var explanation=AnatomyPieceSummary.Describe(part,body.systems.Single(s=>s.id==part.systemId).displayName);
    StringAssert.Contains("Qué es:",explanation,part.id);
    StringAssert.Contains("Función principal:",explanation,part.id);
    Assert.Greater(explanation.Length,125,part.id);
    StringAssert.DoesNotContain("estructura que forma parte del sistema",explanation,part.id);
    StringAssert.DoesNotContain("como parte de este conjunto",explanation,part.id);
   }
  }
 }
 [Test] public void PieceSummariesExplainKnownOrgansAndDoNotExposeUnknownCodes(){
  GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(catalog);
  var stomach=catalog.structures.Single(s=>s.id=="FJ2564");
  StringAssert.Contains("alimentos",AnatomyPieceSummary.Describe(stomach,"Digestivo"));
  StringAssert.DoesNotContain(stomach.id,AnatomyPieceSummary.Title(stomach,stomach.displayName,"Digestivo"));
  var female=new LocalDatasetProvider().LoadCatalog("female");GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(female);
  var unknown=female.structures.Single(s=>s.id=="HRAF0025");
  StringAssert.DoesNotContain(unknown.id,AnatomyPieceSummary.Title(unknown,unknown.displayName,"Nervioso"));
  StringAssert.DoesNotContain(unknown.id,AnatomyPieceSummary.Describe(unknown,"Nervioso"));
  StringAssert.DoesNotContain("Explora su posición",AnatomyPieceSummary.Describe(unknown,"Nervioso"));
 }
 [Test] public void EvaluationSeparatesIdentityAndMainFunctionWithoutInventingSpecificRoles(){
  var stomach=catalog.structures.Single(s=>s.id=="FJ2564");
  var stomachText=AnatomyPieceSummary.Describe(stomach,"Digestivo");
  StringAssert.Contains("Qué es:",stomachText);
  StringAssert.Contains("Función principal:",stomachText);
  StringAssert.Contains("jugos gástricos",stomachText);
  StringAssert.Contains("proteínas",stomachText);
  var femur=catalog.structures.Single(s=>s.id=="FJ3259");
  StringAssert.Contains("muslo",AnatomyPieceSummary.Describe(femur,"Óseo"));
  StringAssert.Contains("soporta el peso",AnatomyPieceSummary.Describe(femur,"Óseo"));
  var female=new LocalDatasetProvider().LoadCatalog("female");
  var ovary=female.structures.Single(s=>s.id=="HRAF0469");
  StringAssert.Contains("óvulos",AnatomyPieceSummary.Describe(ovary,"Reproductor"));
  StringAssert.Contains("hormonas",AnatomyPieceSummary.Describe(ovary,"Reproductor"));
  var kidneyCapsule=female.structures.Single(s=>s.id=="HRAF0565");
  var capsuleText=AnatomyPieceSummary.Describe(kidneyCapsule,"Urinario");
  StringAssert.Contains("Qué es:",capsuleText);
  StringAssert.Contains("Función principal:",capsuleText);
  StringAssert.Contains("protege",capsuleText);
  StringAssert.DoesNotContain("filtra la sangre",capsuleText);
  var ureter=catalog.structures.Single(s=>s.id=="FJ3144");
  StringAssert.Contains("riñón",AnatomyPieceSummary.Describe(ureter,"Urinario"));
  StringAssert.Contains("vejiga",AnatomyPieceSummary.Describe(ureter,"Urinario"));
  var bronchialTree=catalog.structures.Single(s=>s.id=="FJ2441");
  StringAssert.Contains("aire",AnatomyPieceSummary.Describe(bronchialTree,"Respiratorio"));
 }
 [Test] public void EvaluationAddsUsefulContextWithoutReviewMessages(){
  var stomach=catalog.structures.Single(s=>s.id=="FJ2564");
  Assert.Greater(AnatomyPieceSummary.Describe(stomach,"Digestivo").Length,220);
  var female=new LocalDatasetProvider().LoadCatalog("female");
  var retina=female.structures.Single(s=>s.id=="HRAF0025");
  var retinaText=AnatomyPieceSummary.Describe(retina,"Nervioso");
  StringAssert.Contains("luz",retinaText);
  StringAssert.Contains("nervio óptico",retinaText);
  var vagina=female.structures.Single(s=>s.id=="HRAF0431");
  var vaginaText=AnatomyPieceSummary.Describe(vagina,"Reproductivo");
  StringAssert.Contains("cuello del útero",vaginaText);
  StringAssert.Contains("menstruación",vaginaText);
  StringAssert.Contains("parto",vaginaText);
  foreach(var part in catalog.structures.Concat(female.structures)){
   var explanation=AnatomyPieceSummary.Describe(part,"Sistema anatómico");
   StringAssert.DoesNotContain("requiere revisión",explanation);
   StringAssert.DoesNotContain("no documenta",explanation);
  }
 }
 [Test] public void DuplicateIdsAreRejected(){catalog.structures[1].id=catalog.structures[0].id;Assert.Throws<ArgumentException>(()=>catalog.Validate());}
 [Test] public void MissingSystemIsRejected(){catalog.structures[0].systemId="invalid";Assert.Throws<ArgumentException>(()=>catalog.Validate());}
 [Test] public void SearchFindsEnglishSpanishAndAccents(){var index=new AnatomySearchIndex(catalog);Assert.Greater(index.Search("heart").Count,0);Assert.Greater(index.Search("corazon").Count,0);Assert.AreEqual(index.Search("fémur").Count,index.Search("femur").Count);}
 [Test] public void SearchFiltersSystems(){var result=new AnatomySearchIndex(catalog).Search("","urinary");Assert.Greater(result.Count,0);Assert.IsTrue(result.All(s=>s.systemId=="urinary"));}
 [Test] public void NewSystemsAreDiscoverableForBothReferences(){
  foreach(var reference in new[]{"male","female"}){
   var body=new LocalDatasetProvider().LoadCatalog(reference);GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(body);var search=new AnatomySearchIndex(body);
   foreach(var pair in new[]{new[]{"reproductor","reproductive"},new[]{"respiratorio","respiratory"},new[]{"urinario","urinary"}}){
    var choices=search.Discover("",pair[0],50);
    Assert.IsNotEmpty(choices,reference+" / "+pair[0]);
    Assert.IsTrue(choices.All(s=>s.systemId==pair[1]),reference+" / "+pair[0]);
   }
  }
 }
 [Test] public void ExplosionPreservesOriginalAndHandlesCenter(){var original=new Vector3(.1f,1,0);var result=ExplodedViewController.Position(original,Vector3.zero,1);Assert.Greater(Vector3.Distance(result,Vector3.zero),original.magnitude);Assert.AreEqual(original,ExplodedViewController.Position(original,Vector3.zero,0));Assert.IsFalse(float.IsNaN(ExplodedViewController.Position(Vector3.zero,Vector3.zero,1).y));}
 [Test] public void ExpandedAnatomyStaysInsideTheProjectionWorkspace(){
  foreach(var reference in new[]{"male","female"}){
   var anatomy=new LocalDatasetProvider().LoadCatalog(reference);
   foreach(var part in anatomy.structures){
    var at=ExplodedViewController.Position(part.originalPosition,new Vector3(0,.875f,0),1,part.bounds,1.72f,2.5f);
    var extent=part.bounds.extents;
    Assert.LessOrEqual(new Vector2(at.x,at.z).magnitude+new Vector2(extent.x,extent.z).magnitude,1.72f+.0001f,part.id);
    Assert.GreaterOrEqual(at.y-extent.y,-.015f,part.id);
    Assert.LessOrEqual(at.y+extent.y,2.5f+.0001f,part.id);
    Assert.LessOrEqual(Vector3.Distance(at,part.originalPosition),.45f,part.id);
   }
  }
 }
 [Test] public void DiscoveryOffersFamiliarPartsWithoutKnowingTheirNames(){
  GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(catalog);
  var search=new AnatomySearchIndex(catalog);
  var featured=search.Discover("", "todos", 20);
  Assert.IsTrue(featured.Any(s=>s.displayName.StartsWith("Fémur")));
  Assert.IsTrue(featured.Any(s=>s.displayName=="Estómago"));
  Assert.IsTrue(search.Discover("", "huesos", 20).All(s=>s.systemId=="skeleton"));
  Assert.IsTrue(search.Discover("", "órganos", 30).Any(s=>s.displayName=="Estómago"));
  Assert.IsTrue(search.Discover("fem", "huesos", 20).Any(s=>s.displayName.Contains("Fémur")));
 }
 [Test] public void MuscleAndNerveCategoriesOfferSpanishChoicesForBothBodies(){
  foreach(var reference in new[]{"male","female"}){
   var body=new LocalDatasetProvider().LoadCatalog(reference);
   GuateGeeks.AnatomyLab.Utilities.SpanishAnatomy.Apply(body);
   var search=new AnatomySearchIndex(body);
   foreach(var category in new[]{"músculos","nervios"}){
    var choices=search.Discover("",category,30);
    Assert.GreaterOrEqual(choices.Count,6,reference+" / "+category);
    Assert.IsTrue(choices.All(s=>!s.displayName.StartsWith("Estructura ·")),reference+" / "+category);
   }
   Assert.IsTrue(search.Discover("", "músculos",30).Any(s=>s.displayName.Contains("recto")),reference);
   Assert.IsTrue(search.Discover("", "nervios",30).Any(s=>s.displayName.Contains("óptico")),reference);
  }
 }
 [Test] public void VisibilityIsolationAndResetRestoreDefaults(){var manager=new AnatomySystemManager(catalog);manager.HideAll();Assert.IsTrue(manager.Visible.Values.All(v=>!v));manager.IsolateSystem("urinary");Assert.IsTrue(manager.Visible["urinary"]);Assert.AreEqual(1,manager.Visible.Values.Count(v=>v));manager.ShowAll();Assert.IsTrue(manager.Visible.Values.All(v=>v));manager.ResetVisibility();foreach(var s in catalog.systems)Assert.AreEqual(s.defaultVisibility,manager.Visible[s.id]);}
 [Test] public void NewStateHasImmutableSourceTransform(){var data=catalog.structures[0];var state=new StructureState(data);state.Target+=Vector3.one;state.Extracted=true;state.Visibility=StructureVisibility.Hidden;Assert.AreNotEqual(state.Target,data.originalPosition);}
 [Test] public void AllMeshesHaveValidIndicesAndFiniteBounds(){foreach(var data in catalog.structures){var asset=Resources.Load<TextAsset>(data.meshReference);Assert.NotNull(asset,data.id);var mesh=AnatomyStreamingManager.ReadMesh(asset.bytes);Assert.AreEqual(data.vertexCount,mesh.vertexCount,data.id);Assert.AreEqual(data.triangleCount*3,mesh.triangles.Length,data.id);foreach(var i in mesh.triangles)if(i<0||i>=mesh.vertexCount)Assert.Fail(data.id+" invalid index");Assert.IsFalse(float.IsNaN(mesh.bounds.size.x));UnityEngine.Object.DestroyImmediate(mesh);Resources.UnloadAsset(asset);}}
}
}
