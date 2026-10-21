using UnityEngine;

namespace GuateGeeks.AnatomyLab.UI {
// Barrido periférico lento de la mesa, equivalente al escáner del laboratorio AWS.
public sealed class AnatomyTableScanner:MonoBehaviour {
 void Update(){transform.Rotate(Vector3.up,Time.unscaledDeltaTime*6f,Space.Self);}
}
}
