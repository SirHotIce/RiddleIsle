using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
   public class MainMenu : MonoBehaviour
   {
      public void Play()
      {
         SceneManager.LoadScene("MainScene");
      }

      public void Exit()
      {
         Application.Quit();
      }
   }
}
