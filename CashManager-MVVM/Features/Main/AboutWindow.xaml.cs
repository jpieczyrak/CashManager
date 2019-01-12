using System;
using System.Media;

using CashManager_MVVM.Features.Common;

namespace CashManager_MVVM.Features.Main
{
    public partial class AboutWindow : CustomWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            var soundPlayer = new SoundPlayer
            {
                SoundLocation = Environment.CurrentDirectory + @"\Resources\Sounds\coin-drop-on-concrete.wav"
            };
            soundPlayer.Load();
            soundPlayer.Play();
        }
    }
}