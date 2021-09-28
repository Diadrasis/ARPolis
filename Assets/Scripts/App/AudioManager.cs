using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ARPolis
{
    public class AudioManager : Singleton<AudioManager>
    {
        protected AudioManager() { }

        private AudioSource audioSource;
        public Button btnPlayNow, btnPauseNow, btnStopNow;
        private Coroutine coroutineDownloadAudio;
        private string currentFileName;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource) audioSource = FindObjectOfType<AudioSource>();
            gameObject.SetActive(audioSource != null);
        }

        public void PlayNarration(string filename)
        {
            if(Application.isEditor) Debug.Log("PlayNarration " + filename);
            if (string.IsNullOrWhiteSpace(filename))
            {
                if (btnPlayNow) btnPlayNow.gameObject.SetActive(true);
                return;
            }
            if(currentFileName == filename.Trim()) {
                if (btnPauseNow) { btnPauseNow.gameObject.SetActive(true); }
                if (btnPlayNow) { btnPlayNow.gameObject.SetActive(false); }
                if (btnStopNow) { btnStopNow.gameObject.SetActive(true); }
                ResumePlay(); 
                return; 
            }
            if (coroutineDownloadAudio != null) StopCoroutine(coroutineDownloadAudio);
            currentFileName = filename.Trim();
            coroutineDownloadAudio = StartCoroutine(GetAudioStream(currentFileName, AudioType.OGGVORBIS, PlayAudio));
        }

        void PlayAudio(AudioClip clip)
        {
            coroutineDownloadAudio = null;
            if (clip == null)
            {
                btnPlayNow.interactable = true;
                Debug.Log("unable to play audio file... aborting");
                currentFileName = string.Empty;
                return;
            }
            audioSource.clip = clip;
            audioSource.Play();

            if (btnPauseNow) { btnPauseNow.gameObject.SetActive(true); }
            if (btnPlayNow) { btnPlayNow.gameObject.SetActive(false); }
            if (btnStopNow) { btnStopNow.gameObject.SetActive(true); }

            Invoke(nameof(StopAudio), clip.length);
        }

        public void ResumePlay()
        {
            audioSource.Play();
        }

        public void PauseAudio() { 
            audioSource.Pause(); 
            if (btnPauseNow) { btnPauseNow.gameObject.SetActive(false); }
            if (btnPlayNow) { btnPlayNow.gameObject.SetActive(true); }
            if (btnStopNow) { btnStopNow.gameObject.SetActive(true); }

        }

        void ReplayAudio()
        {
            StopAudio();
            audioSource.Play();
        }

        public void StopAudio() {
            //currentFileName = string.Empty; //>> bug: downloads file again
            audioSource.Stop(); 
            CancelInvoke();
            if (btnPauseNow) { btnPauseNow.gameObject.SetActive(false); }
            if (btnPlayNow) { btnPlayNow.gameObject.SetActive(true); }
            if (btnStopNow) { btnStopNow.gameObject.SetActive(false); }
        }

        private IEnumerator GetAudioStream(string uri, AudioType audioType, System.Action<AudioClip> callback)
        {
            btnPlayNow.interactable = false;
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
            {
                DownloadHandlerAudioClip dlAudioHandler = new DownloadHandlerAudioClip(www.uri, audioType);
                dlAudioHandler.streamAudio = true;
                www.downloadHandler = dlAudioHandler;
                if (Application.isEditor) Debug.Log("Requesting audio stream at: " + uri);
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.Log(www.error);
                    callback(null);
                }
                else
                {
                    float timeStart = Time.realtimeSinceStartup;
                    //yield return new WaitUntil(() => dlAudioHandler.audioClip.loadState == AudioDataLoadState.Loaded);
                    while(dlAudioHandler.audioClip.loadState == AudioDataLoadState.Loading || dlAudioHandler.audioClip.loadState == AudioDataLoadState.Unloaded)
                    {
                        float waitTime = Time.realtimeSinceStartup - timeStart;
                        if (waitTime > 3f)
                        {
                            callback(null);
                            yield break;
                        }
                        yield return null;
                    }
                    if (dlAudioHandler.audioClip.loadState == AudioDataLoadState.Failed)
                    {
                        Debug.Log("Failed to load audio file: " + uri);
                        callback(null);
                    }
                    else
                    {
                        callback(dlAudioHandler.audioClip);
                    }
                }
            }

            btnPlayNow.interactable = true;

            yield break;
        }

    }

}


