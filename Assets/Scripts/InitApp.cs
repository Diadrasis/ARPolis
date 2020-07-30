using ARPolis.Data;
using ARPolis.Info;
using StaGeUnityTools;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARPolis
{

    public class InitApp : MonoBehaviour
    {
        AsyncOperation loadAsync;

        //public delegate void InitAction();
        //public static InitAction OnDataLoaded;

        //bool isDataLoaded;

        private void Awake()
        {
            //B.Init();

            //StaticData.lang = PlayerPrefs.GetString("Lang");

            //get terms
            //AppData.Instance.Init();

            //OnDataLoaded += DataLoaded;
            //isDataLoaded = false;

            //InfoManager.Instance.Init();
        }

        //void DataLoaded() { isDataLoaded = true; }

        IEnumerator Start()
        {
           // while (!isDataLoaded) yield return null;
            yield return new WaitForSeconds(0.5f);
            loadAsync = SceneManager.LoadSceneAsync(1);//, LoadSceneMode.Additive);
            while (!loadAsync.isDone)
            {
                //if (B.isEditor) Debug.Log("loading ..." + (loadAsync.progress + 0.1f).ToString());
                yield return null;
            }
            yield break;
        }

    }

}


