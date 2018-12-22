using WMPLib;

namespace EnglishTrainPro.DataFactory
{
    /// <summary>
    /// 為了解決WindowsMediaPlayer部分Bug的Class
    /// </summary>
    class MediaPlayerHelper
    {
        private WindowsMediaPlayer player;
        private readonly string URL;
        public MediaPlayerHelper(string url)
        {
            URL = url;
        }
        public void Pause()
        {
            if (player != null)
            {
                player.controls.pause();
            }
        }
        public void PlayFromStart()
        {
            if (player == null)
            {
                player = new WindowsMediaPlayer();
                player.URL = URL;
            }
            else
            {
                player.controls.currentPosition = 0;
                player.controls.play();
            }
        }
        public void Play()
        {
            if (player == null)
            {
                player = new WindowsMediaPlayer();
                player.URL = URL;
            }
            else
            {
                player.controls.play();
            }
        }
        public void Stop()
        {
            if(player != null)
            {
                player.controls.stop();
            }
        }
        ~MediaPlayerHelper()
        {
            player = null;
        }
    }
}
