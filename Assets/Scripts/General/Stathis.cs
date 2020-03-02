using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//tools for less code and time
//@2016 Stathis Georgiou
using System.IO;
using System;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using UnityEngine.Video;
using System.Threading;

namespace StaGeUnityTools
{
	public class Methods : MonoBehaviour
	{

        public static void Print(string val)
        {
            if(Application.platform==RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                //Debug.Log(val);
            }
        }

        public static void Print(string val, bool isWarning)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (isWarning) { Debug.LogWarning(val); } else { Debug.Log(val); }
            }
        }

        //
        /// <summary>
        /// return color from hex string or html
        /// eg. hexValue = #696969
        /// eg. hexValue = yellow
        /// if error then keeps the default color hexValue = def
        /// </summary>
        /// <param name="hexValue"></param>
        /// <param name="def"></param>
        /// red, cyan, blue, darkblue, lightblue, purple, yellow, 
        /// lime, fuchsia, white, silver, grey, black, orange, brown, 
        /// maroon, green, olive, navy, teal, aqua, magenta
        /// <returns></returns>
        public static Color HexColor(string hexValue, Color def)
        {
            Color newCol;
            if (ColorUtility.TryParseHtmlString(hexValue, out newCol))
            {
                return newCol;
            }

            return def;
        }

		///calculate physical inches with pythagoras theorem
		public static float DeviceDiagonalSizeInInches ()
		{
			float screenWidth = Screen.width / Screen.dpi;
			float screenHeight = Screen.height / Screen.dpi;
			float diagonalInches = Mathf.Sqrt (Mathf.Pow (screenWidth, 2) + Mathf.Pow (screenHeight, 2));
			
			#if UNITY_EDITOR
			Debug.Log ("Getting device inches: " + diagonalInches);
			#endif
			
			return diagonalInches;
		}
	}

	#region FILE MANAGER

	public class File_Manager : MonoBehaviour
	{
		#region XML

		public static float XmlGetVersion (string myXmlFile, string myXmlNode)
		{
			
			XmlDocument draft = new XmlDocument ();

            string serverExcludedComments = Regex.Replace (myXmlFile, "(<!--(.*?)-->)", string.Empty);

            // Debug.Log("get version >> " + serverExcludedComments);

            string serverExcludedComments2 = serverExcludedComments.Replace(Environment.NewLine, string.Empty); //serverExcludedComments.Trim();// Regex.Replace(serverExcludedComments, @"\s+", "");

            //Debug.Log("get version >> "+ serverExcludedComments2);

            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (serverExcludedComments2.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
            {
                serverExcludedComments2 = serverExcludedComments2.Remove(0, _byteOrderMarkUtf8.Length);
            }

            //System.IO.StringReader stringReader = new System.IO.StringReader(myXmlFile);
            //stringReader.Read();
            //draft.LoadXml(stringReader.ReadToEnd());

            draft.LoadXml(serverExcludedComments2);

           // Debug.Log("############");

            XmlNode serverNodeVersion = draft.SelectSingleNode ("/" + myXmlNode);

			if (serverNodeVersion != null) {
				if (serverNodeVersion.Attributes ["version"].Value != null) {
					
					string currVersion = serverNodeVersion.Attributes ["version"].Value;

					float myVersion = 0f;
					float.TryParse (currVersion, out myVersion);

					return myVersion;

				}
			}

			return -1f;
		}

		#endregion

		#region GET FILE EXTENSION

		public enum Ext
		{
			TXT,
			XML,
            JPG,
            PNG,
            JPEG,
            MP4
		}
		//, PNG, JPG, JPEG, MP4, OGG, MP3}
		public static Ext exten = Ext.TXT;

		static string myExtension (Ext xt)
		{

            if (xt == Ext.TXT)
            {
                return ".txt";
            }
            else if (xt == Ext.XML)
            {
                return ".xml";
            }
            else if (xt == Ext.JPG)
            {
                return ".jpg";
            }
            else if (xt == Ext.JPEG)
            {
                return ".jpeg";
            }
            else if (xt == Ext.PNG)
            {
                return ".png";
            }
            else if (xt == Ext.MP4)
            {
                return ".mp4";
            }

            return ".txt";

            #region not in use
            /*

			switch (xt) {
//			case(Ext.JPEG):
//				return ".jpeg";
//				break;
			case(Ext.XML):
				return ".xml";
				break;
//			case(Ext.PNG):
//				return ".png";
//				break;
//			case(Ext.JPG):
//				return ".jpg";
//				break;
//			case(Ext.MP3):
//				return ".mp3";
//				break;
//			case(Ext.MP4):
//				return ".mp4";
//				break;
//			case(Ext.OGG):
//				return ".ogg";
//				break;
			case(Ext.TXT):
				return ".txt";
				break;
			default:
				return ".txt";
			}

*/

            #endregion

        }

        #endregion

        #region SAVE FILE

        public static bool CheckIfFileExists(string fileUrl)
        {
            return File.Exists(fileUrl);
        }

        public static bool CheckIfDirectoryExists(string fileUrl)
        {
            return Directory.Exists(Path.GetDirectoryName(fileUrl));
        }


        //Save Data
        public static void saveData (string dataToSave, string dataFileName, Ext fileExtension)
		{
			string tempPath = getPath_MultiPlatform ();

			tempPath = Path.Combine (tempPath, dataFileName + myExtension (fileExtension));

			//Convert To Json then to bytes
			//		string jsonData = JsonUtility.ToJson(dataToSave, true);
			byte[] myBytes = Encoding.UTF8.GetBytes (dataToSave);

			//Create Directory if it does not exist
			if (!Directory.Exists (Path.GetDirectoryName (tempPath))) {
				Directory.CreateDirectory (Path.GetDirectoryName (tempPath));
            }
            //else
            //{
            //    //avoid violation
            //    //delete first?
            //    return;
            //}
            //Debug.Log(path);

            try
            {
                File.WriteAllBytes(tempPath, myBytes);
                #if UNITY_EDITOR
                Debug.Log("Saved " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                #endif
            }
            catch (Exception e)
            {
                #if UNITY_EDITOR
                Debug.LogWarning("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
                #endif

            }

        }

        public static void saveImage(byte[] myBytes, string dataFileName)
        {
            string tempPath = getPath_MultiPlatform();

            tempPath = Path.Combine(tempPath, dataFileName);

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }

            try
            {
                File.WriteAllBytes(tempPath, myBytes);

                Methods.Print("Saved " + dataFileName + " to: " + tempPath.Replace("/", "\\"));

            }
            catch (Exception e)
            {
                Methods.Print("Failed To save " + dataFileName + " to: " + tempPath.Replace("/", "\\"), true);
                Methods.Print("Error: " + e.Message, true);
            }
        }

        #endregion

        #region DELETE FILE

        public static bool deleteData (string dataFileName, Ext fileExtension)
		{
			bool success = false;

			//Load Data
			string tempPath = getPath_MultiPlatform ();
			tempPath = Path.Combine (tempPath, dataFileName + myExtension (fileExtension));

			//Exit if Directory or File does not exist
			if (!File.Exists (Path.GetDirectoryName (tempPath))) {
				Methods.Print(dataFileName + " Directory does not exist to " + tempPath);
				return false;
			}

			if (!File.Exists (tempPath)) {
                Methods.Print(dataFileName + " does not exist > " + tempPath);
				return false;
			}

			try {
				File.Delete (tempPath);

                Methods.Print(dataFileName + " deleted from: " + tempPath.Replace ("/", "\\"));

				success = true;

			} catch (Exception e) {
                Methods.Print("Failed To Delete " + dataFileName + ": " + e.Message, true);
			}

			return success;
		}


        public static bool deleteFileWithExtencionInName(string dataFileName)
        {
            bool success = false;

            //Load Data
            string tempPath = getPath_MultiPlatform();
            tempPath = Path.Combine(tempPath, dataFileName);

            //Exit if Directory or File does not exist
            if (!File.Exists(Path.GetDirectoryName(tempPath)))
            {
                Methods.Print(dataFileName + " Directory does not exist to " + tempPath);
                return false;
            }

            if (!File.Exists(tempPath))
            {
                Methods.Print(dataFileName + " does not exist > " + tempPath);
                return false;
            }

            try
            {
                File.Delete(tempPath);

                Methods.Print(dataFileName + " deleted from: " + tempPath.Replace("/", "\\"));

                success = true;

            }
            catch (Exception e)
            {
                Methods.Print("Failed To Delete " + dataFileName + ": " + e.Message, true);
            }

            return success;
        }

        #endregion

        #region LOAD FILE

        ///Load file from application persistentDataPath
        public static T loadData<T> (string dataFileName, Ext fileExtension)
		{
			string tempPath = getPath_MultiPlatform (); //Path.Combine(Application.persistentDataPath, "data");
			tempPath = Path.Combine (tempPath, dataFileName + myExtension (fileExtension));

			//Exit if Directory or File does not exist
			if (!Directory.Exists (Path.GetDirectoryName (tempPath))) {
                Methods.Print(dataFileName + " Directory does not exist at " + tempPath);
				return default(T);
			}

			if (!File.Exists (tempPath)) {
				Methods.Print(dataFileName + " does not exist in " + tempPath);
				return default(T);
			}

			//Load saved Json
			byte[] newBytes = null;
			try {
				newBytes = File.ReadAllBytes (tempPath);
				Methods.Print("Loaded " + dataFileName + " from: " + tempPath.Replace ("/", "\\"));
			} catch (Exception e) {
                Methods.Print("Failed To Load " + dataFileName + " from: " + tempPath.Replace ("/", "\\"), true);
                Methods.Print("Error: " + e.Message, true);
			}

			//if (fileExtension == Ext.TXT || fileExtension == Ext.XML) {
			//Convert to json string
			string newData = Encoding.UTF8.GetString (newBytes);
			//}

			//Convert to Object
			//object resultValue =  JsonUtility.FromJson<T>(newData);
			return (T)Convert.ChangeType (newData, typeof(T));
		}

		#endregion

		#region GET PATH

		/// <summary>
		/// Gets the path of data folder - multi platform.
		/// </summary>
		public static string getPath_MultiPlatform ()
		{
			string path = string.Empty;
			//get folder location of current platform
			#if UNITY_EDITOR
			path = Path.Combine (Application.persistentDataPath, "data");
			#elif UNITY_ANDROID
			path = Path.Combine(GetAndroidDocumentsPath(), "data");
			#elif UNITY_IOS || UNITY_IPHONE
			path = Path.Combine(GetiPhoneDocumentsPath(), "data");
			#else
			path = Path.Combine(Application.persistentDataPath, "data");
			#endif
			return path;
		}

		private static string GetiPhoneDocumentsPath ()
		{ 
			string path = Application.persistentDataPath;
			return path;

//			string path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
//			path = path.Substring(0, path.LastIndexOf('/'));  
//			return path + "/Documents";
		}

		private static string GetAndroidDocumentsPath ()
		{ 
			string path = Application.persistentDataPath;
			return path;
		}

		#endregion
	}

	#endregion

	#region RESOURCES LOAD

	public class Tools_Load : MonoBehaviour
	{

        public static Texture2D DiskLoadNewTexture2D(string FileName)
        {
            // Load a PNG or JPG image from disk to a Texture2D
            return LoadTexture(Path.Combine(File_Manager.getPath_MultiPlatform(), FileName));

        }

        /// <summary>
        /// loads a PNG or JPEG image from disk and returns it as a Sprite
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="PixelsPerUnit"></param>
        /// <returns></returns>
        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
        {

            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            Texture2D SpriteTexture = LoadTexture(FilePath);

            if(SpriteTexture == null)
            {
                Debug.LogWarning("error loading texture...");
                return null;
            }

            return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        }

        /// <summary>
        /// loads a PNG or JPEG image from disk and returns it as a Sprite
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="PixelsPerUnit"></param>
        /// <returns></returns>
        public static Sprite DiskLoadNewSprite(string FileName, float PixelsPerUnit = 100.0f)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            Texture2D SpriteTexture = LoadTexture(Path.Combine(File_Manager.getPath_MultiPlatform(), FileName));

            if (SpriteTexture == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("error loading texture..."+ FileName);
#endif
                return null;
            }

            return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        }

        /// <summary>
        /// Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference
        /// </summary>
        public static Sprite ConvertTextureToSprite(Texture2D texture, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
        }

        /// <summary>
        /// Load a PNG or JPG file from disk to a Texture2D
        /// Returns null if load fails
        /// </summary>
        static Texture2D LoadTexture(string FilePath)
        {

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
#if UNITY_EDITOR
            Debug.LogWarning("image from "+FilePath+" is null");
#endif

            return null;                     // Return null if load failed
        }

        /// <summary>
        /// Loads the sprite from resources folder
        /// folder = select the folder conatining sprite
        /// file = name of image sprite
        /// removeFileExtencion = if true file name must be with extencion (etc. .png or .jpg)
        /// </summary>
        public static Sprite LoadSpriteFromResources (string folder, string file, bool removeFileExtencion)
		{
			if (removeFileExtencion) {
				if (file.Contains (".")) {
					file = file.Substring (0, file.Length - 4); //truncate the file extension
				} else {
					Debug.LogWarning ("file name does not have an extencion");
				}
			}

			Sprite spr = Resources.Load <Sprite> (folder + "/" + file);

			if (!spr) {
				Debug.LogWarning ("Sprite Not Found in " + folder + "/" + file);
				return null;
			}

			return  spr;
		}

		public static Sprite LoadSpriteFromResources (string folder, string file)
		{
			Sprite spr = Resources.Load <Sprite> (folder + "/" + file);

			if (!spr) {
				Debug.LogWarning ("Sprite Not Found in " + folder + "/" + file);
				return null;
			}

			return  spr;
		}

		public static Sprite LoadSpriteFromResources (string file)
		{
			Sprite spr = Resources.Load <Sprite> (file);

			if (!spr) {
				Debug.LogWarning ("Sprite Not Found in Resources/" + file);
				return null;
			}

			return  spr;
		}

		public static Texture2D LoadTexture (string folder, string t)
		{
			return (Texture2D)Resources.Load (folder + "/" + t) as Texture2D;
		}

        public static Texture2D LoadTexture2dFromResources(string folder, string file, bool removeFileExtencion)
        {
            if (removeFileExtencion)
            {
                if (file.Contains("."))
                {
                    file = file.Substring(0, file.Length - 4); //truncate the file extension
                }
                else
                {
                    Debug.LogWarning("file name does not have an extencion");
                }
            }

            Texture2D text2D = Resources.Load(folder + "/" + file, typeof(Texture2D)) as Texture2D;// Resources.Load<Sprite>(folder + "/" + file);

            if (!text2D)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Sprite Not Found in " + folder + "/" + file);
#endif
                return null;
            }

            return text2D;
        }


    }

	#endregion

	#region UI TOOLS

	public class Tools_UI : MonoBehaviour
	{
        public static void ForceRebuildLayout(RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        #region check ray on ui

        public static PointerEventData eventDataCurrentPosition;

		/// <summary>
		/// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current. This is a replacement
		/// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
		/// </summary>
		public static bool IsPointerOverUIObject ()
		{
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData (EventSystem.current);
			eventDataCurrentPosition.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
//			Debug.Log("clicks at = "+eventDataCurrentPosition.position);
			
			List<RaycastResult> results = new List<RaycastResult> ();
			EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
			//Debug.Log("rays = "+results.Count);

//			#if UNITY_EDITOR
//			if(results.Count>0){
//				Debug.LogWarning("UI = "+results[0].gameObject.name);
//			}else{
//				Debug.LogWarning("UI no results!!!");
//			}
//			#endif

			return results.Count > 0;
		}

		public static bool IsPointerOverUIObject (Vector2 pos)
		{
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData (EventSystem.current);
			eventDataCurrentPosition.position = pos;
			//Debug.Log("clicks = "+eventDataCurrentPosition.clickCount);
			
			List<RaycastResult> results = new List<RaycastResult> ();
			EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
			//Debug.Log("rays = "+results.Count);
			return results.Count > 0;
		}

		public static bool IsPointerOverUIObject (string onoma)
		{
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData (EventSystem.current);
			eventDataCurrentPosition.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			//Debug.Log("clicks = "+eventDataCurrentPosition.clickCount);

			if (!eventDataCurrentPosition.selectedObject) {
				return false;
			}
			
			if (eventDataCurrentPosition.selectedObject.name == onoma) {
				#if UNITY_EDITOR
				Debug.Log (onoma);
				#endif
				return true;
			}

			return false;

		}

		public static bool IsPointerOverUIObjectTag (string tag)
		{
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData (EventSystem.current);
			eventDataCurrentPosition.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			//Debug.Log("clicks = "+eventDataCurrentPosition.clickCount);
			
			if (!eventDataCurrentPosition.selectedObject) {
				return false;
			}
			
			if (eventDataCurrentPosition.selectedObject.tag == tag) {
				#if UNITY_EDITOR
				Debug.Log ("tag = " + tag);
				#endif
				return true;
			}
			#if UNITY_EDITOR
			Debug.Log ("tag ERROR = " + tag);
			#endif
			return false;
			
		}

        #endregion

        #region RectTransform Functions

        public static Vector2 GetScaledRectSize(RectTransform rekt)
        {
            return new Vector2(rekt.rect.width, rekt.rect.height);
        }

        #endregion

        #region image - rawimage tools

        /// <summary>
        /// Rescales the image to fit in preffered dimensions
        /// </summary>
        public static void RescaleImage (Image img, Vector2 prefferedSize)
		{
			if (img.sprite == null) {
				Debug.LogWarning ("Image " + img.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Rescales the image to fit in parent dimensions
		/// </summary>
		public static void RescaleImage (Image img)
		{
			if (img.sprite == null) {
				Debug.LogWarning ("Image " + img.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = img.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Rescales the image to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the iamge to fit vertical
		/// </summary>
		public static void RescaleImage (Image img, bool fitVertical)
		{
			if (img.sprite == null) {
				Debug.LogWarning ("Image " + img.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = img.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					img.rectTransform.eulerAngles = new Vector3 (0f, 0f, 90f);
				}
			}

			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}


		/// <summary>
		/// Rescales the image to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the image to fit vertical
		/// rotLeft enables 90 degrees left rotation if true else right
		/// left rotation = left side of image is moving at the bottom of the screen
		/// right rotation = left side of image is moving at the top of the screen
		/// </summary>
		public static void RescaleImage (Image img, bool fitVertical, bool rotLeft)
		{
			if (img.sprite == null) {
				Debug.LogWarning ("Image " + img.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = img.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					if (rotLeft) {
						img.rectTransform.eulerAngles = new Vector3 (0f, 0f, 90f);
					} else {
						img.rectTransform.eulerAngles = new Vector3 (0f, 0f, -90f);
					}
				}
			}

			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in preffered dimensions
		/// </summary>
		public static void RescaleRawImage (RawImage rawImg, Vector2 prefferedSize)
		{
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image " + rawImg.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in preffered dimensions
		/// </summary>
		public static void RescaleRawImage (RawImage rawImg, Vector2 prefferedSize, bool keepWidth)
		{
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image " + rawImg.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (!keepWidth) {
				if (X > prefferedSize.x) {
					// metavliti
					float dx = prefferedSize.x / X;
					//rescale
					Y = Y * dx;
					X = X * dx;
				}
			}
			
			rekt.sizeDelta = new Vector2 (X, Y);
		}

		//RescaleRawImage_keepWidth

		/// <summary>
		/// Rescales the rawImage to fit in preffered dimensions 
		/// Y size only if keepWdth is true
		/// X and Y if keepWidth is false
		/// </summary>
		public static void RescaleRawImage_keepWidth (RawImage rawImg, Vector2 prefferedSize, bool keepWidth)
		{
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image " + rawImg.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;
			
			if (!keepWidth) {
				if (X > prefferedSize.x) {
					// metavliti
					float dx = prefferedSize.x / X;
					//rescale
					Y = Y * dx;
					X = X * dx;
				}
			}
			
			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in parent's dimensions
		/// </summary>
		public static void RescaleRawImage (RawImage rawImg)
		{
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image " + rawImg.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = rawImg.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the iamge to fit vertical
		/// </summary>
		public static void RescaleRawImage (RawImage rawImg, bool fitVertical)
		{
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image " + rawImg.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = rawImg.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					rawImg.rectTransform.eulerAngles = new Vector3 (0f, 0f, 90f);
				}
			}

			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}


		/// <summary>
		/// Rescales the rawImage to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the image to fit vertical
		/// rotLeft enables 90 degrees left rotation if true else right
		/// left rotation = left side of image is moving at the bottom of the screen
		/// right rotation = left side of image is moving at the top of the screen
		/// </summary>
		public static void RescaleRawImage (RawImage rawImg, bool fitVertical, bool rotLeft)
		{
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image " + rawImg.gameObject.name + " has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = rawImg.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					if (rotLeft) {
						rawImg.rectTransform.eulerAngles = new Vector3 (0f, 0f, 90f);
					} else {
						rawImg.rectTransform.eulerAngles = new Vector3 (0f, 0f, -90f);
					}
				}
			}

			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2 (X, Y);
		}

		/// <summary>
		/// Resizes the size of the texture into container rect transform.
		/// Valid for Image and RawImage only!!!
		/// </summary>
		/// <param name="container">Container.</param>
		/// <param name="fitVertical">If set to <c>true</c> fit vertical.</param>
		/// <param name="rotLeft">If set to <c>true</c> rot left.</param>
		public static void ResizeTextureToContainerSize (RectTransform container, bool fitVertical, bool rotLeft)
		{

			bool isSprite = false;

			RawImage rawImg = container.GetComponent<RawImage> ();
			Image img = null;

			//if not a raw image try for image
			if (!rawImg) {
				img = container.GetComponent<Image> ();
				isSprite = true;
			}

			if (rawImg == null && img == null) {
				Debug.LogWarning ("container " + container.gameObject.name + " is not valid Image or RawImage !");
				return;
			}
			
			Vector2 prefferedSize = container.sizeDelta;
			
			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					if (rotLeft) {
						container.eulerAngles = new Vector3 (0f, 0f, 90f);
					} else {
						container.eulerAngles = new Vector3 (0f, 0f, -90f);
					}
				}
			}
			
//			RectTransform rekt = rawImg.GetComponent<RectTransform> ();

			// diastaseis eikonas
			Vector2 spriteSize = Vector2.zero;

			if (!isSprite) {
				spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			} else {
				spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			}

			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;
			
			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}
			
			container.sizeDelta = new Vector2 (X, Y);
		}

		public static void ResizeTextureToContainerSize (RectTransform container, bool expandWidth)
		{

			bool isSprite = false;

			RawImage rawImg = container.GetComponent<RawImage> ();
			Image img = null;

			//if not a raw image try for image
			if (!rawImg) {
				img = container.GetComponent<Image> ();
				isSprite = true;
			}

			if (rawImg == null && img == null) {
				Debug.LogWarning ("container " + container.gameObject.name + " is not valid Image or RawImage !");
				return;
			}

			Vector2 prefferedSize = container.sizeDelta;

			// diastaseis eikonas
			Vector2 spriteSize = Vector2.zero;

			if (!isSprite) {
				spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			} else {
				spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			}

			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (!expandWidth) {
				if (X > prefferedSize.x) {
					// metavliti
					float dx = prefferedSize.x / X;
					//rescale
					Y = Y * dx;
					X = X * dx;
				}
			}

			container.sizeDelta = new Vector2 (X, Y);
		}

		public static void MoveDown (RawImage rm)
		{
			rm.rectTransform.pivot = new Vector2 (0.5f, 0f);
			rm.rectTransform.anchorMin = new Vector2 (0.5f, 0f); 
			rm.rectTransform.anchorMax = new Vector2 (0.5f, 0f); 
		}

		public enum Mode
		{
            none,
			center,
			downCenter,
            downLeft,
            downRight,
			upCenter,
            upLeft,
            upRight,
			leftCenter,
			rightCenter
        };

		public static void Move (RectTransform rt, Mode mode)
		{
            float x=0.5f, y=0.5f;

            switch (mode)
            {
                case Mode.none:
                    return;
                case Mode.center:
                    x = y = 0.5f;
                    break;
                case Mode.downCenter:
                    x = 0.5f;
                    y = 0f;
                    break;
                case Mode.downLeft:
                    x = 0.0f;
                    y = 0f;
                    break;
                case Mode.downRight:
                    x = 1.0f;
                    y = 0.0f;
                    break;
                case Mode.upCenter:
                    x = 0.5f;
                    y = 1f;
                    break;
                case Mode.upLeft:
                    x = 0.0f;
                    y = 1f;
                    break;
                case Mode.upRight:
                    x = 1f;
                    y = 1f;
                    break;
                case Mode.leftCenter:
                    x = 0f;
                    y = 0.5f;
                    break;
                case Mode.rightCenter:
                    x = 1f;
                    y = 0.5f;
                    break;
                default:
                    return;
            }

            if (mode == Mode.center) {
				x = y = 0.5f;
			} else if (mode == Mode.downCenter) {
				x = 0.5f;
				y = 0f;
			} else if (mode == Mode.upCenter) {
				x = 0.5f;
				y = 1f;
			} else if (mode == Mode.leftCenter) {
				x = 0f;
				y = 0.5f;
			} else if (mode == Mode.rightCenter) {
				x = 1f;
				y = 0.5f;
			}

			Vector2 val = new Vector2 (x, y);

			rt.pivot = val;
			rt.anchorMin = val; 
			rt.anchorMax = val;
        }

        public static bool hasChangedSizeDelta (Vector2 currentSize, Vector2 prevFrameSize)
		{
			if (prevFrameSize != currentSize) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resizes the rect_ relative to parent.
		/// krataei tin analogia sizedelta se sxesi me tin allagi 
		/// tou parent sizedelta oste to child na paramenei
		/// se megethos idio analogika os pros ton parent rectTrasform
		/// </summary>
		public static void ResizeMoveRect_RelativeToParent (Vector2 actualParentSize, RectTransform parentRect, Vector2 actualChildSize, RectTransform childRect, Vector2 actualChildPosition, bool needScaling)
		{

			if (needScaling) {
				Vector2 posostoSizing = parentRect.sizeDelta;

				float dX = posostoSizing.x / actualParentSize.x;
				float dY = posostoSizing.y / actualParentSize.y;

				childRect.sizeDelta = new Vector2 (actualChildSize.x * dX, actualChildSize.y * dY);
			}

			Vector2 childNewPosition = childRect.localPosition;

			childNewPosition.x = actualChildPosition.x / actualParentSize.x * parentRect.sizeDelta.x;
			childNewPosition.y = actualChildPosition.y / actualParentSize.y * parentRect.sizeDelta.y;

			childRect.localPosition = childNewPosition;

//			childRect.localPosition = calculateOnParentResize (actualChildSize, parentRect, childRect.localPosition);

		}

		public static Vector2 calculateOnParentResize (Vector2 actualParentSize, RectTransform parentRect, Vector2 childPosition, Vector2 actualChildPosition)
		{

			Vector2 childNewPosition = childPosition;
			
			childNewPosition.x = actualChildPosition.x / actualParentSize.x * parentRect.sizeDelta.x;
			childNewPosition.y = actualChildPosition.y / actualParentSize.y * parentRect.sizeDelta.y;
			
			return childNewPosition;

		}

		public static void ScaleSizeDelta (RectTransform targetRekt, float scale, Vector2 minMax)
		{
			if (targetRekt != null && scale != 1.0f) {
				Vector2 finalScale = targetRekt.sizeDelta;

				finalScale.x *= scale;
				finalScale.y *= scale;
				
				finalScale.x = Mathf.Clamp (finalScale.x, minMax.x, minMax.y);
				finalScale.y = Mathf.Clamp (finalScale.y, minMax.x, minMax.y);
				
				targetRekt.sizeDelta = finalScale;
			}
		}

		#endregion

		#region text tools

		/// <summary>
		/// Resets the position of scroll keimeno to start
		/// </summary>
		public static void ResetKeimeno (Text keimeno)
		{
			//init text at start
			RectTransform rektKeimeno = keimeno.GetComponent<RectTransform> ();
			RectTransform rektContainerKeimeno = keimeno.transform.parent.GetComponent<RectTransform> ();

			rektKeimeno.localPosition = Vector3.zero;

			#if UNITY_EDITOR
			if (rektKeimeno.sizeDelta.y <= rektContainerKeimeno.sizeDelta.y) {  
				Debug.Log ("keimeo is smaller than parent");
			} else {															
				Debug.Log ("keimeo is bigger than parent -> " + rektKeimeno.sizeDelta.y + " > " + rektContainerKeimeno.sizeDelta.y);
			}
			#endif
		}

        #endregion


        //		public static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas) {
        //
        //			Vector3[] corners = new Vector3[4];
        //			Vector3[] screenCorners = new Vector3[2];
        //
        //			rectTransform.GetWorldCorners(corners);
        //
        //			if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        //			{
        //				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
        //				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        //			}
        //			else
        //			{
        //				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
        //				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        //			}
        //
        //			screenCorners[0].y = Screen.height - screenCorners[0].y;
        //			screenCorners[1].y = Screen.height - screenCorners[1].y;
        //
        //			return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
        //		}
    }

    #endregion

}
