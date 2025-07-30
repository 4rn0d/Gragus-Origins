using Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        private PlayerController _player;
        [SerializeField] private Text valueText;

        private void Start()
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                _player = playerObj.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (_player && valueText)
            {
                valueText.text = _player.grapes.ToString();
            }
        }
    }
}