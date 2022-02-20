using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPS
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject menu;


        public void Active(bool state)
        {
            menu.SetActive(state);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        public void Restart()
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

        public void Back()
        {
            Manager.Instance.LoaderCanvas(true);
            SceneManager.LoadScene("PreviewUI");
        }
    }
}
