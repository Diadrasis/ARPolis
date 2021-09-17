using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    #region Variables
    public int server_user_id;
    public int local_user_id { get; private set; }
    public string username;
    public string password;
    //public Survey survey; // Why not just the local id???

    public static readonly string PREFS_KEY = "users";
    public static readonly string USERS = "users";

    public static readonly string USER = "user";
    public static readonly string SERVER_USER_ID = "server_user_id";
    public static readonly string LOCAL_USER_ID = "local_user_id";
    public static readonly string USERNAME = "username";
    public static readonly string PASSWORD = "password";
    //public static readonly string SURVEY = "survey";
    #endregion

    #region Methods
    // Constructor for creating locally
    public User(string _username, string _password) //, Survey _survey
    {
        server_user_id = -1;
        local_user_id = GetAvailableUserID();
        username = _username;
        password = _password;
        //survey = _survey;
    }

    // Constructor for Loading from Player Prefs
    private User(int _server_user_id, int _local_user_id, string _username, string _password) // , Survey _survey
    {
        server_user_id = _server_user_id;
        local_user_id = _local_user_id;
        username = _username;
        password = _password;
        //survey = _survey;
    }

    // Constructor for Downloading from server
    /*public User(int _server_user_id, string _username, string _password)
    {
        server_user_id = _server_user_id;
        local_user_id = -1;
        username = _username;
        password = _password;
        //survey = null;
    }*/

    public static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(USERS);
        }

        return xml;
    }

    private static int GetAvailableUserID()
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        if (xml.isNull)
            return 0;

        // Get all user ids
        HashSet<int> userIDs = new HashSet<int>();

        OnlineMapsXMLList userIDNodes = xml.FindAll("/" + USERS + "/" + USER + "/" + LOCAL_USER_ID); //"/users/user/id"

        foreach (OnlineMapsXML node in userIDNodes)
        {
            int nodeId = node.Get<int>(node.element);
            //Debug.Log("nodeId = " + nodeId);
            userIDs.Add(nodeId);
        }

        int index = 0;

        do
        {
            if (!userIDs.Contains(index))
                break;
            index++;
        }
        while (true);
        //Debug.Log("index = " + index);
        return index;
    }

    public static void Delete(int _userId)
    {
        OnlineMapsXML xml = GetXML();

        OnlineMapsXML userToDelete = xml.Find("/" + USERS + "/" + USER + "[" + LOCAL_USER_ID + "=" + _userId + "]");
        if (!userToDelete.isNull)
            userToDelete.Remove();
        //Debug.Log("XML after deleting area: " + xml.outerXml);
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }

    /*public static void RemoveIdToDelete(int _idToRemove)
    {
        // Load previously saved ids array
        int[] loadedIdsToDelete = PlayerPrefsX.GetIntArray(AREAS_TO_DELETE_PREFS_KEY);

        if (loadedIdsToDelete.Length > 0)
        {
            // Create a new int array based on the loaded ids array
            int[] idsToDelete = new int[loadedIdsToDelete.Length - 1];
            //Debug.Log("RemoveIdToDelete, idsToDelete length = " + idsToDelete.Length);
            // Insert all loaded ids to the new ids array except the _idToRemove
            int i = 0;
            foreach (int id in loadedIdsToDelete)
            {
                if (id == _idToRemove)
                {
                    //Debug.Log("id == _idToRemove: " + (id == _idToRemove));
                    continue;
                }

                idsToDelete[i] = id;
                //Debug.Log("id to delete = " + idsToDelete[i]);
                i++;
            }

            // Save new ids array
            PlayerPrefsX.SetIntArray(AREAS_TO_DELETE_PREFS_KEY, idsToDelete);
            PlayerPrefs.Save();
        }
    }*/

    /*public static void AddIdToDelete(int _idToDelete)
    {
        // Load previously saved ids array
        int[] loadedIdsToDelete = PlayerPrefsX.GetIntArray(AREAS_TO_DELETE_PREFS_KEY);

        // Create a new int array based on the loaded ids array
        int[] idsToDelete = new int[loadedIdsToDelete.Length + 1];
        //Debug.Log("AddIdToDelete, idsToDelete length = " + idsToDelete.Length);
        // Insert all loaded ids to the new ids array
        for (int i = 0; i < loadedIdsToDelete.Length; i++)
        {
            idsToDelete[i] = loadedIdsToDelete[i];
            //Debug.Log("id to delete = " + idsToDelete[i]);
        }

        // Insert the new id
        idsToDelete[idsToDelete.Length - 1] = _idToDelete;
        //Debug.Log("id to delete = " + idsToDelete[idsToDelete.Length - 1]);
        // Save new ids array
        PlayerPrefsX.SetIntArray(AREAS_TO_DELETE_PREFS_KEY, idsToDelete);
        PlayerPrefs.Save();
    }*/

    /*public static void RemoveAreaIdToEdit(int _idToRemove)
    {
        // Load previously saved ids array
        int[] loadedIdsToEdit = PlayerPrefsX.GetIntArray(EDITED_AREAS_TO_UPLOAD_PREFS_KEY);

        if (loadedIdsToEdit.Length > 0)
        {
            bool containsId = false;

            // Check if id is included
            foreach (int id in loadedIdsToEdit)
            {
                if (id == _idToRemove)
                {
                    containsId = true;
                    break;
                }
            }

            if (containsId)
            {
                // Create a new int array based on the loaded ids array
                int[] idsToEdit = new int[loadedIdsToEdit.Length - 1];
                //Debug.Log("RemoveAreaIdToEdit, idsToEdit length = " + idsToEdit.Length);
                // Insert all loaded ids to the new ids array except the _idToRemove
                int i = 0;
                foreach (int id in loadedIdsToEdit)
                {
                    if (id == _idToRemove)
                    {
                        //Debug.Log("id == _idToRemove: " + (id == _idToRemove));
                        continue;
                    }

                    idsToEdit[i] = id;
                    //Debug.Log("id to edit = " + idsToEdit[i]);
                    i++;
                }

                // Save new ids array
                PlayerPrefsX.SetIntArray(EDITED_AREAS_TO_UPLOAD_PREFS_KEY, idsToEdit);
                PlayerPrefs.Save();
            }
        }
    }*/

    /*public static void AddAreaIdToEdit(int _idToAdd)
    {
        // Load previously saved ids array
        int[] loadedIdsToEdit = PlayerPrefsX.GetIntArray(EDITED_AREAS_TO_UPLOAD_PREFS_KEY);

        // Check if id has already been added (edited)
        foreach (int id in loadedIdsToEdit)
        {
            if (id == _idToAdd)
            {
                Debug.Log("Id has been added already");
                return;
            }
        }

        // Create a new int array based on the loaded ids array
        int[] idsToEdit = new int[loadedIdsToEdit.Length + 1];
        //Debug.Log("AddAreaIdToEdit, idsToEdit length = " + idsToEdit.Length);
        // Insert all loaded ids to the new ids array
        for (int i = 0; i < loadedIdsToEdit.Length; i++)
        {
            idsToEdit[i] = loadedIdsToEdit[i];
            //Debug.Log("id to edit = " + idsToEdit[i]);
        }

        // Insert the new id
        idsToEdit[idsToEdit.Length - 1] = _idToAdd;
        //Debug.Log("id to edit = " + idsToEdit[idsToEdit.Length - 1]);
        // Save new ids array
        PlayerPrefsX.SetIntArray(EDITED_AREAS_TO_UPLOAD_PREFS_KEY, idsToEdit);
        PlayerPrefs.Save();
    }*/

    //public static int[] GetServerIdsToDelete() => PlayerPrefsX.GetIntArray(AREAS_TO_DELETE_PREFS_KEY);

    /*private static List<cArea> GetEditedAreasToUpload()
    {
        // List of areas
        List<cArea> areasToUpload = new List<cArea>();

        // Load previously saved ids array
        int[] idsToUpload = PlayerPrefsX.GetIntArray(EDITED_AREAS_TO_UPLOAD_PREFS_KEY);

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        foreach (int id in idsToUpload)
        {
            // Load Area
            OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + SERVER_AREA_ID + "=" + id + "]");
            if (areaNode.isNull)
            {
                Debug.Log("Area with id: " + id + " has been deleted!");
                continue;
            }

            cArea loadedArea = Load(areaNode);

            if (loadedArea != null)
                areasToUpload.Add(loadedArea);
        }

        return areasToUpload;
    }*/

    // Get areas with server id = -1
    /*public static List<cArea> GetAreasToUpload()
    {
        // List of areas
        List<cArea> areasToUpload = new List<cArea>(); // TODO: combine with GetEditedAreasToUpload() in a HashSet

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get paths with server_area_id = -1
        OnlineMapsXMLList areaNodes = xml.FindAll("/" + AREAS + "/" + AREA + "[" + SERVER_AREA_ID + "= -1" + "]");

        foreach (OnlineMapsXML areaNode in areaNodes)
        {
            if (areaNode.isNull)
            {
                Debug.Log("Area has been deleted!");
                continue;
            }

            cArea loadedArea = Load(areaNode);

            if (loadedArea != null)
                areasToUpload.Add(loadedArea);
        }

        // Get edited areas
        List<cArea> editedAreasToUpload = GetEditedAreasToUpload();

        if (editedAreasToUpload.Count > 0)
        {
            // Add the edited areas to the areasToUpload list if they are not already added
            foreach (cArea areaToAdd in editedAreasToUpload)
            {
                if (!areasToUpload.Exists(area => area.local_area_id == areaToAdd.local_area_id))
                    areasToUpload.Add(areaToAdd);
            }
        }

        return areasToUpload;
    }*/

    public static void Save(User _userToSave)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Check if area is already saved
        OnlineMapsXML userSaved = xml.Find("/" + USERS + "/" + USER + "[" + LOCAL_USER_ID + "=" + _userToSave.local_user_id + "]");
        if (!userSaved.isNull)
        {
            Debug.Log("User is already saved!");
            return;
        }

        // Create a new user node
        OnlineMapsXML userNode = xml.Create(USER);
        userNode.Create(SERVER_USER_ID, _userToSave.server_user_id);
        userNode.Create(LOCAL_USER_ID, _userToSave.local_user_id);
        userNode.Create(USERNAME, _userToSave.username);
        userNode.Create(PASSWORD, _userToSave.password);
        //areaNode.Create(SURVEY);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
        
        // Debug
        //Debug.Log(xml.outerXml);
    }

    /*public static void SaveFromServer(cArea _areaToSave)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Check if area is already saved
        OnlineMapsXML areaSaved = xml.Find("/" + AREAS + "/" + AREA + "[" + SERVER_AREA_ID + "=" + _areaToSave.server_area_id + "]");
        // if area is already downloaded edit the area instead.
        if (!areaSaved.isNull)
        {
            Debug.Log("Area is already downloaded!");
            // Get local area id
            int local_area_id = areaSaved.Get<int>(LOCAL_AREA_ID);

            // Set local area id
            _areaToSave.local_area_id = local_area_id;

            // Edit area
            Edit(_areaToSave);
            return;
        }

        // Create a new area node
        OnlineMapsXML areaNode = xml.Create(AREA);
        areaNode.Create(SERVER_AREA_ID, _areaToSave.server_area_id);
        areaNode.Create(LOCAL_AREA_ID, GetAvailableAreaID());
        areaNode.Create(TITLE, _areaToSave.title);
        areaNode.Create(TITLE_ENGLISH, _areaToSave.titleEnglish);
        areaNode.Create(POSITION, _areaToSave.position);
        areaNode.Create(ZOOM, _areaToSave.zoom);
        areaNode.Create(AREA_CONSTRAINTS_MIN, new Vector2(_areaToSave.areaConstraintsMin.x, _areaToSave.areaConstraintsMin.y));
        areaNode.Create(AREA_CONSTRAINTS_MAX, new Vector2(_areaToSave.areaConstraintsMax.x, _areaToSave.areaConstraintsMax.y));
        areaNode.Create(VIEW_CONSTRAINTS_MIN, new Vector2(_areaToSave.viewConstraintsMin.x, _areaToSave.viewConstraintsMin.y));
        areaNode.Create(VIEW_CONSTRAINTS_MAX, new Vector2(_areaToSave.viewConstraintsMax.x, _areaToSave.viewConstraintsMax.y));
        areaNode.Create(PATHS);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }*/

    /*public static void SaveAreas(List<cArea> _areasToSave)
    {
        foreach (cArea areaToSave in _areasToSave)
        {
            Save(areaToSave);
        }
    }*/

    /*public static void Edit(cArea _areaToEdit)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get area
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _areaToEdit.local_area_id + "]");
        if (areaNode.isNull)
        {
            Debug.Log("Cannot edit because area is not saved!");
            return;
        }

        // Remove old and Create new values
        areaNode.Remove(TITLE);
        areaNode.Create(TITLE, _areaToEdit.title);
        areaNode.Remove(TITLE_ENGLISH);
        areaNode.Create(TITLE_ENGLISH, _areaToEdit.titleEnglish);
        areaNode.Remove(POSITION);
        areaNode.Create(POSITION, _areaToEdit.position);
        areaNode.Remove(ZOOM);
        areaNode.Create(ZOOM, _areaToEdit.zoom);
        areaNode.Remove(AREA_CONSTRAINTS_MIN);
        areaNode.Create(AREA_CONSTRAINTS_MIN, _areaToEdit.areaConstraintsMin);
        areaNode.Remove(AREA_CONSTRAINTS_MAX);
        areaNode.Create(AREA_CONSTRAINTS_MAX, _areaToEdit.areaConstraintsMax);
        areaNode.Remove(VIEW_CONSTRAINTS_MIN);
        areaNode.Create(VIEW_CONSTRAINTS_MIN, _areaToEdit.viewConstraintsMin);
        areaNode.Remove(VIEW_CONSTRAINTS_MAX);
        areaNode.Create(VIEW_CONSTRAINTS_MAX, _areaToEdit.viewConstraintsMax);
        //Debug.Log("Edited xml = " + xml.outerXml);
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }*/

    /*public static cArea GetAreaByTitle(string _areaTitle)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find path
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + TITLE + "=" + _areaTitle + "]");
        if (!areaNode.isNull)
        {
            Debug.Log(string.Format("Area with title {0} could not be loaded!", _areaTitle));

            return null;
        }

        // Load path
        cArea loadedArea = Load(areaNode);

        return loadedArea;
    }*/

    /*public static void SetServerAreaId(int _local_area_id, int _server_area_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find path
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _local_area_id + "]");

        // Load path
        cArea loadedArea = Load(areaNode);
        loadedArea.server_area_id = _server_area_id;

        // Edit path
        EditServerAreaId(loadedArea);
    }

    private static void EditServerAreaId(cArea _areaToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new path
        OnlineMapsXML areaNode = xml.Find("/" + AREAS + "/" + AREA + "[" + LOCAL_AREA_ID + "=" + _areaToEdit.local_area_id + "]");
        areaNode.Remove(SERVER_AREA_ID);
        areaNode.Create(SERVER_AREA_ID, _areaToEdit.server_area_id);
        Debug.Log("Edited xml = " + xml.outerXml);
        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }*/

    /*private static User Load(OnlineMapsXML _areaNode)
    {
        int server_area_id = _areaNode.Get<int>(SERVER_AREA_ID);
        int local_area_id = _areaNode.Get<int>(LOCAL_AREA_ID);
        string title = _areaNode.Get<string>(TITLE);
        string titleEnglish = _areaNode.Get<string>(TITLE_ENGLISH);
        Vector2 position = _areaNode.Get<Vector2>(POSITION);
        int zoom = _areaNode.Get<int>(ZOOM);
        Vector2 areaConstraints_min = _areaNode.Get<Vector2>(AREA_CONSTRAINTS_MIN);
        Vector2 areaConstraints_max = _areaNode.Get<Vector2>(AREA_CONSTRAINTS_MAX);
        Vector2 viewConstraints_min = _areaNode.Get<Vector2>(VIEW_CONSTRAINTS_MIN);
        Vector2 viewConstraints_max = _areaNode.Get<Vector2>(VIEW_CONSTRAINTS_MAX);

        // Create cArea and add it to loadedAreas list
        cArea loadedArea = new cArea(server_area_id, local_area_id, title, titleEnglish, position, zoom, areaConstraints_min, areaConstraints_max, viewConstraints_min, viewConstraints_max);
        return loadedArea;
    }*/

    public static User LoadUserByUsername(string _username)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find user node
        OnlineMapsXML userNode = xml.Find("/" + USERS + "/" + USER + "[" + USERNAME + "='" + _username + "']");
        if (userNode.isNull)
        {
            Debug.Log(string.Format("User with username {0} could not be loaded!", _username));
            return null;
        }

        // Load user
        User loadedUser = Load(userNode);

        return loadedUser;
    }

    public static User LoadUserByLocalId(int _local_user_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find user node
        OnlineMapsXML userNode = xml.Find("/" + USERS + "/" + USER + "[" + LOCAL_USER_ID + "=" + _local_user_id + "]");
        Debug.Log("Find : " + userNode.outerXml);
        Debug.Log("_local_user_id : " + _local_user_id);
        if (userNode.isNull)
        {
            Debug.Log(string.Format("User with local user id {0} could not be loaded!", _local_user_id));
            return null;
        }

        // Load user
        User loadedUser = Load(userNode);

        return loadedUser;
    }

    private static User Load(OnlineMapsXML _userNode)
    {
        int server_user_id = _userNode.Get<int>(SERVER_USER_ID);
        int local_user_id = _userNode.Get<int>(LOCAL_USER_ID);
        string username = _userNode.Get<string>(USERNAME);
        string password = _userNode.Get<string>(PASSWORD);

        // Create user and return
        User loadedUser = new User(server_user_id, local_user_id, username, password); //, survey
        return loadedUser;
    }

    public static User Reload(User _userToReload)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Get path from xml and Edit values
        OnlineMapsXML userNode = xml.Find("/" + USERS + "/" + USER + "[" + LOCAL_USER_ID + "=" + _userToReload.local_user_id + "]");
        int server_user_id = userNode.Get<int>(SERVER_USER_ID);
        int local_user_id = userNode.Get<int>(LOCAL_USER_ID);
        string username = userNode.Get<string>(USERNAME);
        string password = userNode.Get<string>(PASSWORD);
        //Survey survey = null;//Survey.LoadSurveyOfUser(userNode[SURVEY]);

        // Create cArea and add it to loadedAreas list
        User reloadedUser = new User(server_user_id, local_user_id, username, password); //, survey
        return reloadedUser;
    }

    /*public static List<cArea> LoadAreas()
    {
        // If the key does not exist, returns.
        if (!PlayerPrefs.HasKey(PREFS_KEY))
            return null;

        // Init list of cArea to add paths to
        List<cArea> loadedAreas = new List<cArea>();

        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);

        // Load areas
        foreach (OnlineMapsXML node in xml)
        {
            loadedAreas.Add(Load(node));
        }
        //Debug.Log(xml.outerXml);
        return loadedAreas;
    }*/

    internal void OrderBy(Func<object, object> p)
    {
        throw new NotImplementedException();
    }
    #endregion
}

/*[Serializable]
public class UserData
{
    public int server_area_id;
    //public int local_area_id;
    public string title;
    public string titleEnglish;
    public string position; // longitude, latitude (x, y)
    public int zoom;
    public string areaConstraintsMin; // minLongitude, minLatitude (x, y)
    public string areaConstraintsMax; // maxLongitude, maxLatitude (x, y)
    public string viewConstraintsMin; // minLongitude, minLatitude (x, y)
    public string viewConstraintsMax; // maxLongitude, maxLatitude (x, y)
}*/
