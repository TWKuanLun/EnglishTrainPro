using WMPLib;

namespace EnglishTrainPro.DataFactory
{
    class MediaPlayerHelper
    {
        private WindowsMediaPlayer player;
        private readonly string URL;
        public MediaPlayerHelper(string url)
        {
            URL = url;
            player = new WindowsMediaPlayer();
        }
        public void Pause()
        {
            checkPlayerInit();
            player.controls.pause();
        }
        public void PlayFromStart()
        {
            checkPlayerInit();
            player.controls.currentPosition = 0;
            player.controls.play();
        }
        public void Play()
        {
            checkPlayerInit();
            player.controls.play();
        }
        public void Stop()
        {
            checkPlayerInit();
            player.controls.stop();
        }
        private void checkPlayerInit()
        {
            if (string.IsNullOrEmpty(player.URL))
            {
                player.URL = URL;
                player.controls.stop();
            }
        }
    }
}
