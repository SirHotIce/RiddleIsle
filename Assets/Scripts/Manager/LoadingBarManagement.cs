using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
  public class LoadingBarManagement : MonoBehaviour
  {
    private float value;

    public float Value
    {
      get => value;
      set => this.value = value;
    }

    [SerializeField] private Slider loadingBar;

    private void Update()
    {
      loadingBar.value = Value;
    }
  }
}
