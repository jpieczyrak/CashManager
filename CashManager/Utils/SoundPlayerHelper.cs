using System;
using System.Media;

using CashManager.Properties;

namespace CashManager.Utils
{
    public static class SoundPlayerHelper
    {
        public static void PlaySound(Sound sound)
        {
            if (!Settings.Default.SoundEnabled) return;

            var soundPlayer = new SoundPlayer();

            switch (sound)
            {
                case Sound.AddTransaction:
                    soundPlayer.SoundLocation = Environment.CurrentDirectory + @"\Resources\Sounds\coin-drop-into-pot.wav";
                    break;
                case Sound.About:
                    soundPlayer.SoundLocation = Environment.CurrentDirectory + @"\Resources\Sounds\coin-drop-on-concrete.wav";
                    break;
                case Sound.AppStart:
                    soundPlayer.SoundLocation = Environment.CurrentDirectory + @"\Resources\Sounds\cash-register-purchase.wav";
                    break;
                default:
                    return;
            }

            soundPlayer.Load();
            soundPlayer.Play();
        }

        public enum Sound
        {
            AddTransaction,
            About,
            AppStart
        }
    }
}