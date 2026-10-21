using System;
using System.Collections.Generic;
using UnityEngine;
namespace GuateGeeks.AnatomyLab.Core {
[Serializable] public class AnatomyStructure {
 public string id, canonicalName, displayName, latinName, systemId, category, description, meshReference, materialReference, parentStructure;
 public string[] synonyms = Array.Empty<string>(), childStructures = Array.Empty<string>(), searchableTerms = Array.Empty<string>();
 public Vector3 originalPosition, originalScale = Vector3.one;
 public Quaternion originalRotation = Quaternion.identity;
 public Bounds bounds;
 public int vertexCount, triangleCount;
 public bool localized, defaultTransparent;
}
[Serializable] public class AnatomySystem {
 public string id, name, displayName; public Color defaultColor = Color.white; public string[] structures; public bool defaultVisibility;
}
[Serializable] public class AnatomyCatalog {
 public string dataset, attribution; public AnatomyStructure[] structures; public AnatomySystem[] systems;
 public void Validate() {
  var ids=new HashSet<string>(); var sys=new HashSet<string>();
  foreach(var s in systems) if(!sys.Add(s.id)) throw new ArgumentException("Duplicate system "+s.id);
  foreach(var s in structures) {
   if(string.IsNullOrEmpty(s.id)||!ids.Add(s.id)) throw new ArgumentException("Duplicate/empty ID "+s.id);
   if(!sys.Contains(s.systemId)) throw new ArgumentException("Missing system "+s.systemId);
   if(string.IsNullOrEmpty(s.meshReference)) throw new ArgumentException("Missing mesh "+s.id);
  }
 }
}
public interface IAnatomyDatasetProvider { AnatomyCatalog LoadCatalog(); }
public sealed class LocalDatasetProvider : IAnatomyDatasetProvider {
 public AnatomyCatalog LoadCatalog() => LoadCatalog("male");
 public AnatomyCatalog LoadCatalog(string reference) {
  if(reference!="male"&&reference!="female")throw new ArgumentException("Referencia anatómica desconocida: "+reference);
  var asset=Resources.Load<TextAsset>(reference=="female"?"Anatomy/Female/anatomy_catalog":"Anatomy/anatomy_catalog");
  if(asset==null) throw new InvalidOperationException("[Dataset] Run Tools/AnatomyPipeline/import.py first.");
  var catalog=JsonUtility.FromJson<AnatomyCatalog>(asset.text); catalog.Validate(); return catalog;
 }
}
public enum StructureVisibility { Opaque, Transparent, Hidden }
public sealed class StructureState {
 public readonly AnatomyStructure Data; public GameObject Object; public Mesh Mesh; public MeshRenderer Renderer;
 public StructureVisibility Visibility; public bool Extracted, Returning, Moving; public Vector3 Target;
 public StructureState(AnatomyStructure data) {Data=data; Target=data.originalPosition; Visibility=data.defaultTransparent?StructureVisibility.Transparent:StructureVisibility.Opaque;}
}
}
