using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Scripts
{
    public class Menu : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject menuBackground;
        public GameObject playBackground;
        public GameObject settingsBackground;
        public GameObject controlsBackground;
        public GameObject thankYouPanel;

        [Header("Default Buttons")]
        public GameObject menuFirstButton;         // e.g., Play
        public GameObject playFirstButton;         // e.g., Normal
        public GameObject settingsFirstButton;     // e.g., ToggleFullscreen
        public GameObject controlsFirstButton;     // e.g., Back in Controls
        public GameObject thankYouFirstButton;     // e.g., Exit button on ThankYouPanel

        private void Start()
        {
            bool showThankYou = PlayerPrefs.GetInt("BossDefeated") == 1;

            if (thankYouPanel != null)
                thankYouPanel.SetActive(showThankYou);

            if (showThankYou)
            {
                PlayerPrefs.SetInt("BossDefeated", 0);
                menuBackground.SetActive(false);
                playBackground.SetActive(false);
                settingsBackground.SetActive(false);
                controlsBackground.SetActive(false);
                StartCoroutine(SetFirstSelected(thankYouFirstButton));
            }
            else
            {
                menuBackground.SetActive(true);
                playBackground.SetActive(false);
                settingsBackground.SetActive(false);
                controlsBackground.SetActive(false);
                StartCoroutine(SetFirstSelected(menuFirstButton));
            }
        }

        // ------------------------
        // Panel Switching Methods
        // ------------------------

        public void OpenPlayMenu()
        {
            SwitchMenu(menuBackground, playBackground, playFirstButton);
        }

        public void OpenSettingsMenu()
        {
            SwitchMenu(menuBackground, settingsBackground, settingsFirstButton);
        }

        public void OpenControlsMenu()
        {
            SwitchMenu(menuBackground, controlsBackground, controlsFirstButton);
        }

        public void BackToMainMenu()
        {
            thankYouPanel.SetActive(false);
            playBackground.SetActive(false);
            settingsBackground.SetActive(false);
            controlsBackground.SetActive(false);
            menuBackground.SetActive(true);

            StartCoroutine(SetFirstSelected(menuFirstButton));
        }

        private void SwitchMenu(GameObject current, GameObject next, GameObject firstButton)
        {
            current.SetActive(false);
            next.SetActive(true);
            StartCoroutine(SetFirstSelected(firstButton));
        }

        private IEnumerator SetFirstSelected(GameObject button)
        {
            yield return null; // Wait for UI to rebuild
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button);
        }

        // ------------------------
        // Scene & Game Control Methods
        // ------------------------

        public void PlayNormal()
        {
            SceneManager.LoadScene("ProceduralyGeneratedMap");
        }

        public void PlayTuto()
        {
            SceneManager.LoadScene("Tuto");
        }

        public void PlayBoss()
        {
            SceneManager.LoadScene("FinalBoss");
        }

        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }

        public void ResetProgress()
        {
            SaveManager.ResetPotionSlots();
        }
    }
}
