using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ARPolis.Server
{

    public class ServerController : Singleton<ServerController>
    {

        protected ServerController() { }

        public void Init()
        {
            Debug.Log("Server Controller initialized");
            //get urls from static class
            //test communication - internet
        }


        public void LoginUser(string username, string pass)
        {
            if (B.isEditor) Debug.LogWarning("login for user: " + username);
        }

        public void SignUpUser(string username, string pass)
        {
            if (B.isEditor) Debug.LogWarning("sign up for user: " + username);
        }

        public void LoginAnonymous()
        {
            if (B.isEditor) Debug.LogWarning("login anonymous");
        }

    }

}
