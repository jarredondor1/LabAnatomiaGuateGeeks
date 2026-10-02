using System;
using UnityEngine;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Interaction {
public sealed class AnatomySelectionManager {
 public StructureState Selected {get;private set;} public StructureState Hovered {get;private set;}
 public event Action<StructureState> Changed,HoverChanged;
 public void Select(StructureState state){Selected=state;Changed?.Invoke(state);}
 public void Hover(StructureState state){if(Hovered==state)return;Hovered=state;HoverChanged?.Invoke(state);}
 public void Clear(){Select(null);Hover(null);}
}
}
