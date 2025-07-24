using UnityEngine;

namespace Player
{
    public static class SaveManager
    {
        private static string GetPotionSlotKey(int index) => $"PotionSlot_{index}";

        public static bool IsPotionSlotUnlocked(int index)
        {
            return PlayerPrefs.GetInt(GetPotionSlotKey(index), index == 0 ? 1 : 0) == 1;
        }

        public static void UnlockPotionSlot(int index)
        {
            PlayerPrefs.SetInt(GetPotionSlotKey(index), 1);
            PlayerPrefs.Save();
        }

    }

}