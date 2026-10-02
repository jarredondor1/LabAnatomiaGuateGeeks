using System;
using System.Collections.Generic;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Anatomy {
public sealed class AnatomySystemManager {
 public readonly Dictionary<string,bool> Visible=new Dictionary<string,bool>();
 readonly AnatomyCatalog catalog; public event Action<string,bool> Changed;
 public AnatomySystemManager(AnatomyCatalog c) {catalog=c; foreach(var s in c.systems)Visible[s.id]=s.defaultVisibility;}
 void Set(string id,bool value){if(!Visible.ContainsKey(id))throw new ArgumentException(id); Visible[id]=value; Changed?.Invoke(id,value);}
 public void ShowSystem(string id)=>Set(id,true);
 public void HideSystem(string id)=>Set(id,false);
 public void ToggleSystem(string id)=>Set(id,!Visible[id]);
 public void IsolateSystem(string id){foreach(var s in catalog.systems)Set(s.id,s.id==id);}
 public void ShowAll(){foreach(var s in catalog.systems)Set(s.id,true);}
 public void HideAll(){foreach(var s in catalog.systems)Set(s.id,false);}
 public void ResetVisibility(){foreach(var s in catalog.systems)Set(s.id,s.defaultVisibility);}
}
}
