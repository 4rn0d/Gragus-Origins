using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject settingsMenuUI;
        [SerializeField] private GameObject volumeMenuUI;

        [Header("First Selectables")]
        [SerializeField] private GameObject pauseFirstButton;
        [SerializeField] private GameObject settingsFirstButton;
        [SerializeField] private GameObject volumeFirstButton;

        [Header("Audio")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("UI Components")]
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle fullscreenToggle;

        private Resolution[] _resolutions;
        public bool isPaused = false;

        private void Start()
        {
            // Setup pause state
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
            volumeMenuUI.SetActive(false);

            // Load resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = $"{_resolutions[i].width} x {_resolutions[i].height}";
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            // Load saved volume and fullscreen state
            if (audioMixer.GetFloat("volume", out float currentVolume))
            {
                volumeSlider.value = currentVolume;
            }

            fullscreenToggle.isOn = Screen.fullScreen;

            // Hook up UI events
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            volumeSlider.onValueChanged.AddListener(SetVolume);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        private void Update()
        {
            // You could use InputSystem here if needed
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
            settingsMenuUI.SetActive(false);
            volumeMenuUI.SetActive(false);
            StartCoroutine(SetFirstSelected(pauseFirstButton));
        }

        private void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
            volumeMenuUI.SetActive(false);
        }

        public void OpenSettingsMenu()
        {
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(true);
            volumeMenuUI.SetActive(false);
            StartCoroutine(SetFirstSelected(settingsFirstButton));
        }

        public void OpenVolumeMenu()
        {
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
            volumeMenuUI.SetActive(true);
            StartCoroutine(SetFirstSelected(volumeFirstButton));
        }

        public void BackToPauseMenu()
        {
            pauseMenuUI.SetActive(true);
            settingsMenuUI.SetActive(false);
            volumeMenuUI.SetActive(false);
            StartCoroutine(SetFirstSelected(pauseFirstButton));
        }

        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex >= 0 && resolutionIndex < _resolutions.Length)
            {
                Resolution res = _resolutions[resolutionIndex];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            }
        }

        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("volume", volume);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void ToMenu()
        {
            GameObject obj = GameObject.Find("Gragus(Clone)");
            if (obj != null)
                Destroy(obj);

            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private IEnumerator<UnityEngine.WaitForEndOfFrame> SetFirstSelected(GameObject button)
        {
            yield return new WaitForEndOfFrame(); // wait for UI to update
            EventSystem.current.SetSelectedGameObject(null); // Clear first
            EventSystem.current.SetSelectedGameObject(button); // Set first
        }
    }
}
