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
        public Button btnNarrationNow;
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
            if (string.IsNullOrWhiteSpace(filename)) return;
            if (currentFileName == filename.Trim())
            {
                if (audioSource.isPlaying) { StopAudio(); }
                else { ReplayAudio(); }
                return;
            }
            if (coroutineDownloadAudio != null) StopCoroutine(coroutineDownloadAudio);
            currentFileName = filename.Trim();
            coroutineDownloadAudio = StartCoroutine(GetAudioStream(currentFileName, AudioType.WAV, PlayAudio));
        }

        void PlayAudio(AudioClip clip)
        {
            coroutineDownloadAudio = null;
            if (clip == null) return;
            audioSource.clip = clip;
            audioSource.Play();
        }

        void ReplayAudio()
        {
            StopAudio();
            audioSource.Play();
        }

        public void StopAudio() { audioSource.Stop(); }

        private IEnumerator GetAudioStream(string uri, AudioType audioType, System.Action<AudioClip> callback)
        {
            if (btnNarrationNow) btnNarrationNow.interactable = false;

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
                    yield return new WaitUntil(() => dlAudioHandler.audioClip.loadState == AudioDataLoadState.Loaded);
                    callback(dlAudioHandler.audioClip);
                }
            }
            if (btnNarrationNow) btnNarrationNow.interactable = true;

            yield break;
        }

    }

}


