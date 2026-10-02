using UnityEngine;
using UnityEngine.SceneManagement;
namespace GuateGeeks.AnatomyLab.Core {
public sealed class Bootstrap:MonoBehaviour {void Start(){Application.targetFrameRate=72;new LocalDatasetProvider().LoadCatalog();SceneManager.LoadScene("02_AnatomyLab");}}
}
