using UnityEngine;
using UnityEngine.EventSystems;
namespace GuateGeeks.AnatomyLab.UI {
public sealed class DraggablePanel:MonoBehaviour,IDragHandler {
 public RectTransform Panel;
 public void OnDrag(PointerEventData e){var canvas=GetComponentInParent<Canvas>();Panel.anchoredPosition+=e.delta/canvas.scaleFactor;var parent=(RectTransform)Panel.parent;var p=Panel.anchoredPosition;p.x=Mathf.Clamp(p.x,0,Mathf.Max(0,parent.rect.width-Panel.rect.width));p.y=Mathf.Clamp(p.y,-Mathf.Max(0,parent.rect.height-Panel.rect.height),0);Panel.anchoredPosition=p;}
}
}
