using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Anatomy;
using GuateGeeks.AnatomyLab.Search;
using GuateGeeks.AnatomyLab.ExplodedView;
namespace GuateGeeks.AnatomyLab.Tests {
public class CatalogTests {
 AnatomyCatalog catalog;
 [SetUp] public void Setup(){catalog=new LocalDatasetProvider().LoadCatalog();}
 [Test] public void CompleteCatalogHas2234UniqueMeshes(){catalog.Validate();Assert.AreEqual(2234,catalog.structures.Length);Assert.AreEqual(2234,catalog.structures.Select(s=>s.id).Distinct().Count());}
 [Test] public void DuplicateIdsAreRejected(){catalog.structures[1].id=catalog.structures[0].id;Assert.Throws<ArgumentException>(()=>catalog.Validate());}
 [Test] public void MissingSystemIsRejected(){catalog.structures[0].systemId="invalid";Assert.Throws<ArgumentException>(()=>catalog.Validate());}
 [Test] public void SearchFindsEnglishSpanishAndAccents(){var index=new AnatomySearchIndex(catalog);Assert.Greater(index.Search("heart").Count,0);Assert.Greater(index.Search("corazon").Count,0);Assert.AreEqual(index.Search("fémur").Count,index.Search("femur").Count);}
 [Test] public void SearchFiltersSystems(){var result=new AnatomySearchIndex(catalog).Search("","urinary");Assert.Greater(result.Count,0);Assert.IsTrue(result.All(s=>s.systemId=="urinary"));}
 [Test] public void ExplosionPreservesOriginalAndHandlesCenter(){var original=new Vector3(.1f,1,0);var result=ExplodedViewController.Position(original,Vector3.zero,1);Assert.Greater(Vector3.Distance(result,Vector3.zero),original.magnitude);Assert.AreEqual(original,ExplodedViewController.Position(original,Vector3.zero,0));Assert.IsFalse(float.IsNaN(ExplodedViewController.Position(Vector3.zero,Vector3.zero,1).y));}
 [Test] public void VisibilityIsolationAndResetRestoreDefaults(){var manager=new AnatomySystemManager(catalog);manager.HideAll();Assert.IsTrue(manager.Visible.Values.All(v=>!v));manager.IsolateSystem("urinary");Assert.IsTrue(manager.Visible["urinary"]);Assert.AreEqual(1,manager.Visible.Values.Count(v=>v));manager.ShowAll();Assert.IsTrue(manager.Visible.Values.All(v=>v));manager.ResetVisibility();foreach(var s in catalog.systems)Assert.AreEqual(s.defaultVisibility,manager.Visible[s.id]);}
 [Test] public void NewStateHasImmutableSourceTransform(){var data=catalog.structures[0];var state=new StructureState(data);state.Target+=Vector3.one;state.Extracted=true;state.Visibility=StructureVisibility.Hidden;Assert.AreNotEqual(state.Target,data.originalPosition);}
 [Test] public void AllMeshesHaveValidIndicesAndFiniteBounds(){foreach(var data in catalog.structures){var asset=Resources.Load<TextAsset>(data.meshReference);Assert.NotNull(asset,data.id);var mesh=AnatomyStreamingManager.ReadMesh(asset.bytes);Assert.AreEqual(data.vertexCount,mesh.vertexCount,data.id);Assert.AreEqual(data.triangleCount*3,mesh.triangles.Length,data.id);foreach(var i in mesh.triangles)if(i<0||i>=mesh.vertexCount)Assert.Fail(data.id+" invalid index");Assert.IsFalse(float.IsNaN(mesh.bounds.size.x));UnityEngine.Object.DestroyImmediate(mesh);Resources.UnloadAsset(asset);}}
}
}
