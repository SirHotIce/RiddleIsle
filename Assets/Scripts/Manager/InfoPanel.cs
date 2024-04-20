using TMPro;
using UnityEngine;

namespace Manager
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private TextMeshProUGUI lives;
        [SerializeField] private TextMeshProUGUI level;

        public void UpdateScore(int scoreValue)
        {
            score.text = $"Score: {scoreValue}";
        }
        public void UpdateLives(int lifeValue)
        {
            lives.text = $"{lifeValue}";
        }
        public void UpdateLevel(int levelValue)
        {
            level.text = $"Level: {levelValue}";
        }
    
    
    }
}
