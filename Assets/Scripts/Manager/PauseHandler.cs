using TMPro;
using UnityEngine;

namespace Manager
{
    public class PauseHandler : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI score;

        private void OnEnable()
        {
            score.text = $"Score: {FindObjectOfType<GameManager>().Score}";
        }
    }
}
