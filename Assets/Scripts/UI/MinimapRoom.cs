namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public enum RoomType
    {
        Normal,
        Special,
        Final
    }

    public class MinimapRoom : MonoBehaviour
    {
        [Header("UI Elements")]
        public Image background;
        public Image icon;

        [Header("Room Icons")]
        public Sprite specialIcon;
        public Sprite finalIcon;

        public void SetColor(Color color)
        {
            if (background != null)
                background.color = color;
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetRoomType(RoomType type)
        {
            switch (type)
            {
                case RoomType.Normal:
                    icon.enabled = false;
                    break;
                case RoomType.Special:
                    icon.sprite = specialIcon;
                    icon.enabled = true;
                    break;
                case RoomType.Final:
                    icon.sprite = finalIcon;
                    icon.enabled = true;
                    break;
            }
        }
    }

}