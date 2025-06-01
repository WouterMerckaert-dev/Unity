using UnityEngine;
using UnityEngine.Video;

public class RickRollManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Verwijzing naar de VideoPlayer
    public GameObject rickRollVideoObject; // Het GameObject dat de VideoPlayer bevat
    public Board board;
    private float originalVolume;




    private void Start()
    {
        // Zorg dat de RickRoll video in het begin uit staat
        rickRollVideoObject.SetActive(false);
    }

    public void PlayRickRoll()
    {
        Debug.Log("Playing RickRoll");
        rickRollVideoObject.SetActive(true);

        // Mute background music if it's assigned
        if (board.backgroundMusic != null)
        {
            board.backgroundMusic.Pause(); // Pauzeer de muziek
        }

        videoPlayer.loopPointReached += OnVideoEnd; // Set the event for when the video ends
        videoPlayer.Play();
        Time.timeScale = 0;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Ending RickRoll");

        // Stop the video
        videoPlayer.Stop();

        // Stop audio if it’s playing
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
            videoPlayer.GetTargetAudioSource(0).Stop();
        }

        // Remove the event listener
        videoPlayer.loopPointReached -= OnVideoEnd;

        // Hide the video
        rickRollVideoObject.SetActive(false);

        // Restore background music volume
        if (board.backgroundMusic != null)
        {
            board.backgroundMusic.UnPause(); // start de muziek


            // Resume game time
            Time.timeScale = 1;
        }
    }


    private void OnDestroy()
    {
        // Verwijder de listener wanneer het object wordt vernietigd
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}
