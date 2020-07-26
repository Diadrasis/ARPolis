using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

public class ExtraTools : EditorWindow {
	string myScene = "Venetian";
	string myXml = "data";
	int mySize = 2;
	float myAreaHeight=45f;
	float myRadius=5f;
	
	bool groupEnabled;
	bool isKamSets;
	Color colorStart = Color.red;
	Color colorPoint = Color.green;
	GameObject gss;
	Camera myKam;

	bool printNames;
    string myFolder = "", message="";
    bool bResizeTo_1024, bResizeTo_2048;
    int textureQuality = 75;

	GameObject fatherContainer;
	GameObject prefabGb;
	GameObject prefabGb2;
	GameObject prefabGb3;
	int itemsToSpawn=1;
	int distBetweenItems=10;
	int areaSide=20;

	[MenuItem("StaGe/AssetBundles/Build AssetBundle From Selection - List Names (texture - sounds)")]
	static void ExportResourceNamesNoTrack () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		uint myCrc= 1;

		List<string> names = new List<string> ();

		if (Selection.objects.Length > 0) {
			for (int i = 0; i < Selection.objects.Length; i++) {
				Debug.Log (Selection.objects [i].name);
				names.Add(Selection.objects [i].name);
			}
		}


		if (path.Length != 0) {
			// Build the resource file from the active selection.
			BuildPipeline.BuildAssetBundleExplicitAssetNames(Selection.objects, names.ToArray(), path, out myCrc,BuildAssetBundleOptions.CompleteAssets,BuildTarget.Android);
		}

		Debug.Log ("CRC = "+myCrc);


	}

	[MenuItem("StaGe/AssetBundles/Build AssetBundle From Selection - No dependency tracking (texture - sounds)")]
	static void ExportResourceNoTrack () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.
			BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path,BuildAssetBundleOptions.UncompressedAssetBundle,BuildTarget.Android);
		}

		Debug.Log (BuildTarget.Android);
	}

	[MenuItem("StaGe/AssetBundles/Build AssetBundle From Selection - Track dependencies (prefab)")]
	static void ExportResource () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
            // Build the resource file from the active selection.
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
			BuildPipeline.BuildAssetBundle (Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies,BuildTarget.Android);// | BuildAssetBundleOptions.CompleteAssets);
			Selection.objects = selection;
		}
	}

	[MenuItem("StaGe/Clear Project Cache")]
	static void ClearCache () {
		Debug.LogWarning ("free space before = "+Caching.spaceFree);
		Caching.ClearCache ();
		Debug.LogWarning ("free space after = "+Caching.spaceFree);
	}

	// Add menu named "My Window" to the Window menu
	[MenuItem ("StaGe/Extra Tools")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		ExtraTools window = (ExtraTools)EditorWindow.GetWindow (typeof (ExtraTools));
		window.Show();
	}

	void OnInspectorUpdate() {
		Repaint();
	}

	List<Vector3> posForSpawn=new List<Vector3>();

	bool waitForCheck=false;

	void RandomSpawnItems()
	{

		if(areaSide<distBetweenItems*3)
		{
			Debug.LogWarning("the area is too small!!");
			if(EditorUtility.DisplayDialog("Area is small!", "Set a bigger area", "OK")){
				
			}
			return;
		}
		if(fatherContainer)
		{
			posForSpawn.Clear();

			Vector3 initPos = fatherContainer.transform.position;

			float minX = initPos.x - areaSide;
			float maxX = initPos.x + areaSide;
			float minZ = initPos.z - areaSide;
			float maxZ = initPos.z + areaSide;



			if(itemsToSpawn>0 && prefabGb)
			{
				for(int i=0; i<itemsToSpawn; i++)
				{
					Vector3 newPos = new Vector3(UnityEngine.Random.Range(minX,maxX),initPos.y, UnityEngine.Random.Range(minZ,maxZ));

					if(posForSpawn.Count>0)
					{
						for(int s=0; s<posForSpawn.Count; s++)
						{
							if(Vector3.Distance(newPos,posForSpawn[s])<distBetweenItems)
							{
								newPos = new Vector3(UnityEngine.Random.Range(minX,maxX),initPos.y, UnityEngine.Random.Range(minZ,maxZ));

								s=0;
							}
						}
					}

					RaycastHit hit;
					
					//hit down from last y position of player
					Ray downRay = new Ray(new Vector3(newPos.x,100f,newPos.y), Vector3.down);
					
					if (Physics.Raycast(downRay, out hit,Mathf.Infinity)){

						//get hit distance and add person height
						newPos.y = hit.point.y;// (100f - hit.distance) -20f;
						Debug.Log(newPos.y);
						Debug.Log(hit.point.y);
					}

					GameObject b = tyxaioPrefab();

					if(b)
					{

						GameObject gb = Instantiate(b,Vector3.zero,Quaternion.identity)as GameObject;
						
						gb.name="spitaki_"+i;

//						gb.transform.SetParent(fatherContainer.transform);

						Vector3 newRot = gb.transform.eulerAngles;
						
						newRot.y = UnityEngine.Random.Range(0f,180f);
						
						gb.transform.eulerAngles = newRot;
						
						gb.transform.localPosition = newPos;

						posForSpawn.Add(newPos);
						
					}
					
					
				}
			}
			else
			{
				Debug.LogWarning("Please Set a Prefab!");
			}

		}else{Debug.LogWarning("Please Set a FatherContainer!");}
	}

	GameObject tyxaioPrefab()
	{
		int p = UnityEngine.Random.Range(0,4);

		if(p==1){
			if(prefabGb){
				return prefabGb;
			}else
			{
				Debug.LogWarning("Please Set Prefab 1");
				return null;
			}
		}else
		if(p==2){
			if(prefabGb){
				return prefabGb2;
			}else
			{
				Debug.LogWarning("Please Set Prefab 2");
				return null;
			}
		}else
		if(p==3){
			if(prefabGb){
				return prefabGb3;
			}else
			{
				Debug.LogWarning("Please Set Prefab 3");
				return null;
			}
		}else{
			if(prefabGb){
				return prefabGb;
			}else
			{
				Debug.LogWarning("Please Set Prefab 1");
				return null;
			}
		}
	}

	public Font myFont;

	bool showRandomGenerator;
	bool showScreenShot;
	bool showSceneSettings;
		
	void OnGUI () {

		fatherContainer = (GameObject) EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Father Obj", fatherContainer, typeof(GameObject),true);

		GUILayout.Label ("", EditorStyles.boldLabel);

		prefabGb = (GameObject) EditorGUI.ObjectField(new Rect(3, 30, position.width - 6, 20), "prefab 1", prefabGb, typeof(GameObject));

		GUILayout.Label ("", EditorStyles.boldLabel);
		
		prefabGb2 = (GameObject) EditorGUI.ObjectField(new Rect(3, 60, position.width - 6, 20), "prefab 2", prefabGb2, typeof(GameObject));

		GUILayout.Label ("", EditorStyles.boldLabel);
		
		prefabGb3 = (GameObject) EditorGUI.ObjectField(new Rect(3, 90, position.width - 6, 20), "prefab 3", prefabGb3, typeof(GameObject));

		GUILayout.Label ("", EditorStyles.boldLabel);
		
		myFont = (Font) EditorGUI.ObjectField(new Rect(3, 120, position.width - 6, 20), "font", myFont, typeof(Font));

		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("", EditorStyles.boldLabel);


		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);
		
		if(GUILayout.Button("Change font for all texts in Father Obj")){
			if(fatherContainer)
			{
				if(myFont)
				{
					ChangeAllFonts();
				}
				else
				{
					if(EditorUtility.DisplayDialog("Missing font", "Set a font first", "OK")){
						
					}
				}
			}
			else
			{
				if(EditorUtility.DisplayDialog("Missing father", "Set a father first", "OK")){
					
				}
			}
		}
		
		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

		if(GUILayout.Button("Add Tag Occlusion in all Mesh Renderes in Father Obj")){
			//DestroyCollAndAnimators();
			AddTagOcclusion();
		}

		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);
		
		if(GUILayout.Button("Add collider and rigibody for all meshes in Father Obj")){
			if(fatherContainer)
			{
//				if(myFont)
//				{
					AddColRigi();
//				}
//				else
//				{
//					if(EditorUtility.DisplayDialog("Missing font", "Set a font first", "OK")){
//						
//					}
//				}
			}
			else
			{
				if(EditorUtility.DisplayDialog("Missing father", "Set a father first", "OK")){
					
				}
			}
		}
		
		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

//		GUILayout.Label ("", EditorStyles.boldLabel);
//		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

		if(GUILayout.Button("Destroy Mesh Colliders in Father")){
			DestroyCollAndAnimators(1);
		}
		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);
		if(GUILayout.Button("Destroy Animators in Father")){
			DestroyCollAndAnimators(2);
		}

		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);
//		GUILayout.Label ("", EditorStyles.boldLabel);
//		
//
//		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);
		//GUILayout.Label ("Τοποθετηση prefabs σε μια περιοχη", EditorStyles.boldLabel);
		showRandomGenerator = GUILayout.Toggle(showRandomGenerator,"Τοποθετηση prefabs σε μια περιοχη", EditorStyles.boldLabel);

		if(showRandomGenerator)
		{
			GUILayout.Label ("Ποσα prefab να δημιουργηθουν", EditorStyles.boldLabel);
			itemsToSpawn = EditorGUILayout.IntSlider("Ποσα prefabs",itemsToSpawn,1,50);

			GUILayout.Label ("", EditorStyles.boldLabel);
			GUILayout.Label ("Αποσταση μεταξυ των prefab", EditorStyles.boldLabel);
			distBetweenItems = EditorGUILayout.IntSlider("Αποσταση",distBetweenItems,1,50);

			GUILayout.Label ("", EditorStyles.boldLabel);
			GUILayout.Label ("Εμβαδον περιοχης", EditorStyles.boldLabel);
			areaSide = EditorGUILayout.IntSlider("Μηκος πλευρας",areaSide,20,500);

			GUILayout.Label ("", EditorStyles.boldLabel);

			if(GUILayout.Button("Εκτελεση τυχαιας δημιουργιας")){
				if(fatherContainer)
				{
					if(itemsToSpawn>0 && prefabGb)
					{
						RandomSpawnItems();
					}
					else
					{
						if(EditorUtility.DisplayDialog("Missing prefab", "Set a prefab first", "OK")){
							
						}
					}
				}
				else
				{
					if(EditorUtility.DisplayDialog("Missing father", "Set a father first", "OK")){
						
					}
				}
			}
		}

		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);



//		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

//		GUILayout.Label ("Extra Settings", EditorStyles.boldLabel);
//		
//		//		GUILayout.Label ("", EditorStyles.boldLabel);
//		GUILayout.Label (" Mesh Colliders in Scene", EditorStyles.boldLabel);
//		
//		if(GUILayout.Button("Enable Mesh Colliders")){
//			EnableColliders();
//		}
//		if(GUILayout.Button("Disable Mesh Colliders")){
//			DisableColliders();
//		}
//
//		GUILayout.Label (" Mesh Renderers in Scene", EditorStyles.boldLabel);
//		
//		if(GUILayout.Button("Enable Mesh Renderers")){
//			EnableRenderes();
//		}
//		if(GUILayout.Button("Disable Mesh Renderers")){
//			DisableRenderes();
//		}
//		
//		GUILayout.Label (" Shadows in Scene", EditorStyles.boldLabel);
//		
//		if(GUILayout.Button("Enable Shadows")){
//			OnOff_CastRecieve_Shadows(true);
//		}
//		if(GUILayout.Button("Disable Shadows")){
//			OnOff_CastRecieve_Shadows(false);
//		}

		showScreenShot =  GUILayout.Toggle (showScreenShot ,"Screenshot Settings", EditorStyles.boldLabel);

		if(showScreenShot)
		{
			GUILayout.Label ("", EditorStyles.boldLabel);
			if(!gss){
				gss=GameObject.Find("Camera_ScreenShot");
				
				if(gss && !myKam){
					myKam=gss.GetComponent<Camera>();
				}
			}

			if(!myKam){
				if(GUILayout.Button("Set New Camera")){
					SetCamera();
					if(EditorUtility.DisplayDialog("Help", "Move [Camera_ScreenShot] in the position you want to take a photo", "OK")){

					}
				}
			}else{
				mySize = EditorGUILayout.IntSlider ("Size of png", mySize, 2, 8);
				GUILayout.Label ("", EditorStyles.boldLabel);
				if(GUILayout.Button("Orthographic")){
					SetOrtho();
				}
			//	GUILayout.Label ("", EditorStyles.boldLabel);
				if(GUILayout.Button("Perspective")){
					SetPersp();
				}
				GUILayout.Label ("", EditorStyles.boldLabel);
				if(GUILayout.Button("Capture")){
					if(mySize>4){
						if(EditorUtility.DisplayDialog("Warning", "The size of png is big . That means that it needs more time to capture", "OK" , "Cancel")){
							Screenshot();
						}
					}else{
						Screenshot();
					}
				}
			}
		}

		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

		showSceneSettings =  GUILayout.Toggle (showSceneSettings ,"Check Into the Scene", EditorStyles.boldLabel);
//		GUILayout.Label ("", EditorStyles.boldLabel);
		if(showSceneSettings)
		{
			GUILayout.Label ("", EditorStyles.boldLabel);
			
			if(GUILayout.Button("Check All Textures into Scene")){
				GetAllTextures();
			}
			
			printNames = GUILayout.Toggle(printNames,"Εκτύπωσε και τα ονόματα");

			GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

			if(GUILayout.Button("Check All Triangles and Vertexes into Scene")){
				GetTriangles();
			}

			GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

			if(GUILayout.Button("Find All Colliders into Scene")){
				FindAllColliders();
			}

            GUILayout.Label("--------------------------------------------------", EditorStyles.boldLabel);

            if (GUILayout.Button("Check All Textures into Project"))
            {
                FindAllTexturesInProject();
            }

            GUILayout.Label("--------------------------------------------------", EditorStyles.boldLabel);

            GUILayout.Label("Type folder path - eg images/folder1 ");
            myFolder = GUILayout.TextField(myFolder);
            GUILayout.Space(10f);
            if (!bResizeTo_2048) bResizeTo_1024 = GUILayout.Toggle(bResizeTo_1024, "Resize all to max 1024");
            GUILayout.Space(10f);
            if (!bResizeTo_1024) bResizeTo_2048 = GUILayout.Toggle(bResizeTo_2048, "Resize all to max 2048");
            GUILayout.Space(10f);
            if(bResizeTo_1024 || bResizeTo_2048) textureQuality = EditorGUILayout.IntSlider("Ποιότητα εικόνας", textureQuality, 75, 100);
            GUILayout.Space(10f);
            
            if (GUILayout.Button("Check All Textures into StreamingAssets"))
            {
                if (bResizeTo_1024 || bResizeTo_2048)
                {
                    if (EditorUtility.DisplayDialog("About to Resize Images Warning", "All textures in StreamingAssets/"+myFolder+" ", "OK", "Cancel"))
                    {
                        FindAllTexturesInStreamingAssets();
                    }
                }
                else
                {
                    FindAllTexturesInStreamingAssets();
                }
            }

            GUILayout.Space(10f);
            Color c = GUI.color;
            GUI.color = Color.cyan;
            GUILayout.Label(message);//, new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 14 });
            GUI.color = c;
        }

		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

		//	EditorGUILayout.TextField ("SceneViewCamera position", ""+SceneView.lastActiveSceneView.pivot);
	}

	int totalVertexes = 0;
	int totalTriangles = 0;

	void GetTriangles(){

		totalVertexes = 0;
		totalTriangles = 0;
		int maxTriangles=-1;
		string onoma = "Null";
		int maxVertex=-1;
		string onoma2 = "Null";

		foreach(MeshFilter mf in FindObjectsOfType(typeof(MeshFilter)))
		{
			if(mf){
				if(mf.sharedMesh!=null){
					totalVertexes += mf.sharedMesh.vertexCount;
					totalTriangles += mf.sharedMesh.triangles.Length/3;

					if(mf.sharedMesh.vertexCount>maxVertex){
						maxVertex=mf.sharedMesh.vertexCount;
						onoma2 = mf.name;
						if(onoma2.Contains("Mesh")){
							onoma2 = mf.transform.parent.name;
							if(onoma2.Contains("Group") || onoma2.Contains("Mesh")){
								onoma2 = mf.transform.parent.parent.name;
								if(onoma2.Contains("Group") || onoma2.Contains("Mesh")){
									onoma2 = mf.transform.parent.parent.name;
									if(onoma2.Contains("Group") || onoma2.Contains("Mesh")){
										onoma2 = mf.transform.parent.parent.name;
										if(onoma2.Contains("Group") || onoma2.Contains("Mesh")){
											onoma2 = mf.transform.parent.parent.name;
										}
									}
								}
							}
						}
					}

					if(mf.sharedMesh.triangles.Length/3>maxTriangles){
						maxTriangles=mf.sharedMesh.triangles.Length/3;
						onoma = mf.name;
						if(onoma.Contains("Mesh")){
							onoma = mf.transform.parent.name;
							if(onoma.Contains("Group") || onoma.Contains("Mesh")){
								onoma = mf.transform.parent.parent.name;
								if(onoma.Contains("Group") || onoma.Contains("Mesh")){
									onoma = mf.transform.parent.parent.name;
									if(onoma.Contains("Group") || onoma.Contains("Mesh")){
										onoma = mf.transform.parent.parent.name;
										if(onoma.Contains("Group") || onoma.Contains("Mesh")){
											onoma = mf.transform.parent.parent.name;
										}
									}
								}
							}
						}
					}

				}else{
					if(EditorUtility.DisplayDialog("ERROR Empty Mesh!", "Mesh "+mf.name, "OK")){
						
					}
				}
			}
		}

		if(EditorUtility.DisplayDialog("Calculation Completed", "Total vertex = "+totalVertexes.ToString()+"\nTotal triangles ="+totalTriangles.ToString()+"\n"+onoma2+" has Max vertexes ="+maxVertex.ToString()+"\n"+onoma+" has Max triangles ="+maxTriangles.ToString(), "OK")){
			
		}
	}

	void AddColRigi(){
		MeshRenderer[] meshes = fatherContainer.GetComponentsInChildren<MeshRenderer>(true);
		
		//		Text[] txts = FindObjectsOfType<Text>();
		
		int a =0;
		foreach(MeshRenderer m in meshes){
			if(m){
				m.gameObject.AddComponent<MeshCollider>();
				Rigidbody rig = m.gameObject.AddComponent<Rigidbody>();
				rig.useGravity = true;
				a++;
				m.gameObject.name="house_"+a;
			}
		}
		Debug.Log(a+" houses found..");
	}

	void ChangeAllFonts(){
		Text[] txts = fatherContainer.GetComponentsInChildren<Text>(true);

//		Text[] txts = FindObjectsOfType<Text>();

		int a =0;
		foreach(Text txt in txts){
			if(txt){
				txt.font = myFont;
				a++;
			}
		}
		Debug.Log(a+" texts change");
	}

	void SetPersp(){
		if(myKam){
			myKam.orthographic=false;
			myKam.transform.eulerAngles=new Vector3(90f,0,0f);
		}
	}

	void SetOrtho(){
		if(myKam){
			myKam.orthographic=true;
			myKam.orthographicSize=-1046f;
			myKam.farClipPlane=1000f;
			myKam.transform.eulerAngles=new Vector3(90f,-180f,0f);
		}
	}

	void Screenshot(){
		if(myKam){
			ScreenCapture.CaptureScreenshot("Map_"+System.DateTime.Now.ToString("yyyy_MM_dd HH.mm.ss")+".png",mySize);
			Debug.Log("png is inside project folder");
		}else{
			Debug.Log("No Camera Found!");
			Debug.Log("Set new Camera");
		}
	}

	void SetCamera(){
		GameObject g = new GameObject();
		g.name="Camera_ScreenShot";
		g.tag="MainCamera";
		g.transform.position=new Vector3(0f,777f,0f);
		g.transform.eulerAngles=new Vector3(90f,0f,0f);
		g.AddComponent<Camera>();
		myKam=g.GetComponent<Camera>();
		myKam.useOcclusionCulling=false;
		isKamSets=true;
		Debug.Log("Move camera to take screenshot");
	}
	
	void EnableColliders(){
		MeshCollider[] meshes = GameObject.FindObjectsOfType<MeshCollider>();
		
		foreach(MeshCollider col in meshes){
			if(!col.name.StartsWith("Terrain")){
				if(!col.enabled){
					col.enabled=true;
				}
			}
		}
		
		Debug.Log("Enabled All Mesh Colliders !");
	}

	void AddTagOcclusion()
	{
		if(fatherContainer)
		{

			MeshRenderer[] meshes = fatherContainer.GetComponentsInChildren<MeshRenderer>();

			if (meshes.Length <= 0) {
				return;
			}

			int ss =0;
			int tt =0;

			foreach(MeshRenderer mes in meshes)
			{
				if (mes) {
					Material[] mat = mes.sharedMaterials;//.GetComponent<Material> ();
					GameObject gb = mes.gameObject;//.GetComponent<GameObject> ();

					if (mat[0]) {
						if (mat[0].shader != null) {
							if((mat[0].shader.name.StartsWith("Transparent")) || (mat[0].shader.name.StartsWith("Mobile/Particles")) || (mat[0].shader.name.StartsWith("Unlit"))){
								gb.tag = "Occludee Static";
								tt++;
							}else{
								gb.tag = "Occluder Static";
								ss++;
							}
						}
					}
				}
			}

			Debug.Log(tt+" transparent meshes");
			Debug.Log(ss+" diffuse meshes");

		}
	}

	void DestroyCollAndAnimators(int x)
	{
		if(fatherContainer)
		{
			if (x == 1) {
				MeshCollider[] meshes = fatherContainer.GetComponentsInChildren<MeshCollider> ();

				int ss = 0;

				foreach (MeshCollider col in meshes) {
					if (col)
						DestroyImmediate (col);
					ss++;
				}

				Debug.Log (ss + " colliders destroyed");

			} else if (x == 2) {

				Animator[] anims = fatherContainer.GetComponentsInChildren<Animator> ();

				int pp = 0;

				foreach (Animator anim in anims) {
					if (anim)
						DestroyImmediate (anim);
					pp++;
				}

				Debug.Log (pp + " animators destroyed");
			}
		}
	}

	void FindAllColliders(){

		Material[] mats = Resources.FindObjectsOfTypeAll<Material> ();
		Texture[] texs = Resources.FindObjectsOfTypeAll<Texture> ();

		foreach (Texture tex in texs) {
			if (tex.height > 2048 || tex.width > 2048) {
				Debug.LogWarning (tex.name+" is bigger tha 2048!!");
			}
		}

		Debug.LogWarning("Textures " + texs.Length);
		Debug.LogWarning("Materials " + mats.Length);

//		foreach (Material mat in mats) {
//			if (mat.mainTexture == null) {
//				Debug.Log ("Material " + mat.name + " is empty");
//			}
//		}

		Debug.LogWarning("Components " + Resources.FindObjectsOfTypeAll<Component>().Length);

		MeshCollider[] meshes = GameObject.FindObjectsOfType<MeshCollider>();
		BoxCollider[] boxes = GameObject.FindObjectsOfType<BoxCollider>();
		SphereCollider[] sferes = GameObject.FindObjectsOfType<SphereCollider>();

		Debug.Log ("Mesh Colliders are " + meshes.Length);
		Debug.Log ("Box Colliders are " + boxes.Length);
		Debug.Log ("Sferes Colliders are " + sferes.Length);
	}
	
	void DisableColliders(){
		MeshCollider[] meshes = GameObject.FindObjectsOfType<MeshCollider>();
		
		foreach(MeshCollider col in meshes){
//			if(col.name=="BigTree"){
				if(col.enabled){
					col.enabled=false;
				}
//			}
		}

//		GameObject father = GameObject.Find("windmills");
//
//		if(father){
//			MeshCollider[] paidia = father.GetComponentsInChildren<MeshCollider>();
//			foreach(MeshCollider paidi in paidia){
//				if(paidi){
//					paidi.enabled=false;
//				}
//			}
//		}else{
//			Debug.Log("father not found");
//		}
		
		Debug.Log("Disabled All Mesh Colliders !");
	}

	void EnableRenderes(){
		MeshRenderer[] meshes = GameObject.FindObjectsOfType<MeshRenderer>();
		
		foreach(MeshRenderer rend in meshes){
			if(!rend.enabled){
				rend.enabled=true;
				
			}
		}
		
		Debug.Log("Enabled All MeshRenderer !");
	}

	void DisableRenderes(){
		MeshRenderer[] meshes = GameObject.FindObjectsOfType<MeshRenderer>();
		
		foreach(MeshRenderer rend in meshes){
			if(rend.enabled){
				rend.enabled=false;
				
			}
		}
		
		Debug.Log("Enabled All MeshRenderer !");
	}
	
	void OnOff_CastRecieve_Shadows(bool kane){
		MeshRenderer[] meshes = GameObject.FindObjectsOfType<MeshRenderer>();
		
		foreach(MeshRenderer m in meshes){
			if(m){
				m.castShadows=kane;
				m.receiveShadows=kane;
			}
		}
		
		if(kane==true){
			Debug.Log("Enabled All Mesh Shadows !");
		}else{
			Debug.Log("Disabled All Mesh Shadows !");
		}
	}

	int w,h,texLenght;

	void GetAllTextures(){
        UnityEngine.Object[] allTX = Resources.FindObjectsOfTypeAll(typeof(Texture2D));


		texLenght=0;

		if(allTX.Length>0){
			foreach(Texture2D tx in allTX){
				if (tx != null)
				{
					string assetPath = AssetDatabase.GetAssetOrScenePath(tx);
					TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
					
					if (importer != null) {
						object[] args = new object[2] { 0, 0 };
						MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
						mi.Invoke(importer, args);
						
						w = (int)args[0];
						h = (int)args[1];
						
						if(w>2048 || h>2048){
							Debug.LogWarning(tx.name+ " is bigger than 2048");
						}

						//if(importer.textureFormat==TextureImporterFormat.

//						if(importer.compressionQuality==50){
//							Debug.Log(tx.name+" has 50% compression");
//						}else{
//							Debug.Log(tx.name+" has "+importer.compressionQuality+"% compression");
//						}

//						return true;

						if(printNames){
							Debug.LogWarning(tx.name);
						}

						texLenght++;
					}
				}
			}

			Debug.LogWarning("all textures are "+texLenght);
			
		}
	}

    void FindAllTexturesInProject()
    {
        texLenght = 0;

        foreach (string s in AssetDatabase.GetAllAssetPaths()
             .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                 || s.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) 
                 || s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                 || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)))
        {
            Texture2D tx = (Texture2D)AssetDatabase.LoadAssetAtPath(s, typeof(Texture2D));

            if (tx != null)
            {
                string assetPath = AssetDatabase.GetAssetOrScenePath(tx);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer != null)
                {
                    object[] args = new object[2] { 0, 0 };
                    MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(importer, args);

                    w = (int)args[0];
                    h = (int)args[1];

                    if (w > 2048 || h > 2048)
                    {
                        Debug.LogWarning(tx.name + " is bigger than 2048 and is in "+assetPath);
                    }

                    if (printNames)
                    {
                        Debug.LogWarning(tx.name);
                    }

                    texLenght++;
                }
            }
        }

        Debug.LogWarning("all textures in project are " + texLenght);

    }

    void FindAllTexturesInStreamingAssets()
    {

        texLenght = 0;
        long fileSize = 0;
        string bigFileName = string.Empty;

        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);

        if (!string.IsNullOrEmpty(myFolder))
        {
            string pFolder = Path.Combine(Application.streamingAssetsPath, myFolder);
            if (Directory.Exists(pFolder))
            {
                dir = new DirectoryInfo(pFolder);
            }
            else
            {
                Debug.LogWarning(myFolder + " folder does not exist!!! ...aborting");
                return;
            }
        }

        FileInfo[] infos = dir.GetFiles("*.*", SearchOption.AllDirectories);

        foreach (FileInfo fileInfo in infos)
        {
            string ext = fileInfo.Extension.ToUpper();  //Debug.Log(ext);

            if (ext != ".JPG" && ext != ".JPEG" && ext != ".PNG" && ext != ".TIFF" && ext != ".TIF") continue;

            //Debug.Log(fileInfo.Name);
            message = fileInfo.Name;

            texLenght++;
            if (fileInfo.Length > fileSize)
            {
                fileSize = fileInfo.Length;
                bigFileName = fileInfo.Name;
            }

            if (bResizeTo_2048 || bResizeTo_1024)
            {

                int newSize = bResizeTo_1024 ? 1024 : 2048;

                Texture2D tex = null;
                byte[] fileData;

                if (File.Exists(fileInfo.FullName))
                {
                    fileData = File.ReadAllBytes(fileInfo.FullName);
                    tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                }

                if (tex != null)
                {
                    int w = tex.width;
                    int h = tex.height;

                    if (w > newSize || h > newSize)
                    {

                        //Debug.Log("Resising " + fileInfo.Name + " from " + w + " x " + h);

                        bool isWidthBigger = w > h;

                        float log = isWidthBigger ? w / (float)newSize : h / (float)newSize;

                        if (w == h) log = 1f;

                       // Debug.Log("log = " + log);

                        if (isWidthBigger) { w = newSize; h = Mathf.RoundToInt(h / log); } else { h = newSize; w = Mathf.RoundToInt(w / log); }

                        if (w == h) { w = h = newSize; }

                       // Debug.Log("to new size " + w + " x " + h);

                        tex = Resize(tex, w, h);

                        bool isPng = ext == ".PNG";

                       // Debug.Log("is png = " + isPng);

                        SaveTextureToFile(tex, fileInfo.FullName, isPng, textureQuality);

                    }

                }

                tex = null;

            }

        }

        message = "Total textures in StreamingAssets/" +myFolder+" = " + texLenght;
        
        //convert to a string in megabytes.
        string bytesToMb = ConvertBytesToMegabytes(fileSize).ToString("0.00");
        Debug.LogWarning("and the biggest is " + bigFileName + " with " + bytesToMb + " Mb");
    }

    void SaveTextureToFile(Texture2D texture, string filename, bool hasAlpha, int quality)
    {
        if (!hasAlpha) { System.IO.File.WriteAllBytes(filename, texture.EncodeToJPG(quality)); }
        else { System.IO.File.WriteAllBytes(filename, texture.EncodeToPNG()); }
    }

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }

    double ConvertBytesToMegabytes(long bytes)
    {
        return (bytes / 1024f) / 1024f;
    }


    bool GetImageSize(Texture2D asset){//, out int width, out int height) {
		if (asset != null) {
			string assetPath = AssetDatabase.GetAssetPath(asset);
			TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			
			if (importer != null) {
				object[] args = new object[2] { 0, 0 };
				MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
				mi.Invoke(importer, args);
				
				w = (int)args[0];
				h = (int)args[1];

				if(w>2048 || h>2048){
					Debug.LogWarning(asset.name+ "is bigger than 2048");
				}
				
				return true;
			}
		}
		
		w = h = 0;
		return false;
	}




//	@MenuItem("Diadrasis/Save Texture to file")
//	void Apply () {
//		
//		Texture2D texture= Selection.activeObject as Texture2D;
//		if (texture == null) {
//			EditorUtility.DisplayDialog(
//				"Select Texture",
//				"You Must Select a Texture first!",
//				"Ok");
//			return;
//		}
//		
//		var path = EditorUtility.SaveFilePanel(
//			"Save texture as PNG",
//			"",
//			texture.name + ".png",
//			"png");
//		
//		if(path.Length != 0) {
//			// Convert the texture to a format compatible with EncodeToPNG
//			if(texture.format != TextureFormat.ARGB32 && texture.format != TextureFormat.RGB24){
//				Texture2D newTexture =new Texture2D(texture.width, texture.height);
//				newTexture.SetPixels(texture.GetPixels(0),0);
//				texture = newTexture;
//			}
//			var pngData = texture.EncodeToPNG();
//			if (pngData != null)
//				File.WriteAllBytes(path, pngData);
//		}
//	}


	
}
