//using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 0618 // Disabled warning due to SetVertices being deprecated until new release with SetMesh() is available.



public class TextSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
{
    //public RectTransform TextPopup_Prefab_01;
    [SerializeField]
    public List<TextSelectorFieldItem> fieldItems = new List<TextSelectorFieldItem>();

    //private RectTransform m_TextPopup_RectTransform;
    private TextMeshProUGUI m_TextPopup_TMPComponent;
    private const string k_LinkText = "You have selected link <#ffff00>";
    private const string k_WordText = "Word Index: <#ffff00>";


    private TextMeshProUGUI m_TextMeshPro;
    private Canvas m_Canvas;
    private Camera m_Camera;

    // Flags
    private bool isHoveringObject;
    private int m_selectedWord = -1;
    private int m_selectedLink = -1;
    private int m_lastIndex = -1;

    private Matrix4x4 m_matrix;

    private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

    void Awake()
    {
        m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();

        m_Canvas = gameObject.GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            m_Camera = null;
        else
            m_Camera = m_Canvas.worldCamera;

        //// Create pop-up text object which is used to show the link information.
        //m_TextPopup_RectTransform = Instantiate(TextPopup_Prefab_01) as RectTransform;
        //m_TextPopup_RectTransform.SetParent(m_Canvas.transform, false);
        //m_TextPopup_TMPComponent = m_TextPopup_RectTransform.GetComponentInChildren<TextMeshProUGUI>();
        //m_TextPopup_RectTransform.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    void OnDisable()
    {
        // UnSubscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }

    void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == m_TextMeshPro)
        {
            // Update cached vertex data.
            m_cachedMeshInfoVertexData = m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
        }
    }

    void LateUpdate()
    {
        if (isHoveringObject)
        {

            #region Word Selection Handling
            /*
            //Check if Mouse intersects any words and if so assign a random color to that word.
            int wordIndex = TMP_TextUtilities.FindIntersectingWord(m_TextMeshPro, Input.mousePosition, m_Camera);

            // Clear previous word selection.
            if (m_TextPopup_RectTransform != null && m_selectedWord != -1 && (wordIndex == -1 || wordIndex != m_selectedWord))
            {
                TMP_WordInfo wInfo = m_TextMeshPro.textInfo.wordInfo[m_selectedWord];

                // Iterate through each of the characters of the word.
                for (int i = 0; i < wInfo.characterCount; i++)
                {
                    int characterIndex = wInfo.firstCharacterIndex + i;

                    // Get the index of the material / sub text object used by this character.
                    int meshIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;

                    // Get the index of the first vertex of this character.
                    int vertexIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].vertexIndex;

                    // Get a reference to the vertex color
                    Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;

                    Color32 c = vertexColors[vertexIndex + 0].Tint(1.33333f);

                    vertexColors[vertexIndex + 0] = c;
                    vertexColors[vertexIndex + 1] = c;
                    vertexColors[vertexIndex + 2] = c;
                    vertexColors[vertexIndex + 3] = c;
                }

                // Update Geometry
                m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

                m_selectedWord = -1;
            }


            // Word Selection Handling
            if (wordIndex != -1 && wordIndex != m_selectedWord && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                m_selectedWord = wordIndex;

                TMP_WordInfo wInfo = m_TextMeshPro.textInfo.wordInfo[wordIndex];

                // Iterate through each of the characters of the word.
                for (int i = 0; i < wInfo.characterCount; i++)
                {
                    int characterIndex = wInfo.firstCharacterIndex + i;

                    // Get the index of the material / sub text object used by this character.
                    int meshIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;

                    int vertexIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].vertexIndex;

                    // Get a reference to the vertex color
                    Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;

                    Color32 c = vertexColors[vertexIndex + 0].Tint(0.75f);

                    vertexColors[vertexIndex + 0] = c;
                    vertexColors[vertexIndex + 1] = c;
                    vertexColors[vertexIndex + 2] = c;
                    vertexColors[vertexIndex + 3] = c;
                }

                // Update Geometry
                m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            }
           */
            #endregion


            #region Example of Link Handling
            // Check if mouse intersects with any links.
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);

            // Clear previous link selection if one existed.
            if ((linkIndex == -1 && m_selectedLink != -1) || linkIndex != m_selectedLink)
            {
                //m_TextPopup_RectTransform.gameObject.SetActive(false);
                m_selectedLink = -1;
            }

            /*
            // Handle new Link selection.
            if (linkIndex != -1 && linkIndex != m_selectedLink)
            {
                m_selectedLink = linkIndex;

                TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

                // Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\""); // Example of how to retrieve the Link ID and Link Text.

                Vector3 worldPointInRectangle;

                RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TextMeshPro.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);

                switch (linkInfo.GetLinkID())
                {
                    case "id_01": // 100041637: // id_01
                        m_TextPopup_RectTransform.position = worldPointInRectangle;
                        m_TextPopup_RectTransform.gameObject.SetActive(true);
                        m_TextPopup_TMPComponent.text = k_LinkText + " ID 01";
                        break;
                    case "id_02": // 100041638: // id_02
                        m_TextPopup_RectTransform.position = worldPointInRectangle;
                        m_TextPopup_RectTransform.gameObject.SetActive(true);
                        m_TextPopup_TMPComponent.text = k_LinkText + " ID 02";
                        break;
                }
            }
            */
            #endregion

        }
        else
        {
            // Restore any character that may have been modified
            if (m_lastIndex != -1)
            {
                RestoreCachedVertexAttributes(m_lastIndex);
                m_lastIndex = -1;
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter()");
        isHoveringObject = true;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit()");
        isHoveringObject = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Application.isEditor) Debug.Log("Click at POS: " + eventData.position + "  World POS: " + eventData.worldPosition);

        #region Example of Link Handling
        // Check if mouse intersects with any links.
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);

        // Clear previous link selection if one existed.
        if ((linkIndex == -1 && m_selectedLink != -1) || linkIndex != m_selectedLink)
        {
           // m_TextPopup_RectTransform.gameObject.SetActive(false);
            m_selectedLink = -1;
        }

        // Handle new Link selection.
        if (linkIndex != -1 && linkIndex != m_selectedLink)
        {
            m_selectedLink = linkIndex;

            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

            if (Application.isEditor) Debug.Log(linkInfo.GetLinkID());

            // Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\""); // Example of how to retrieve the Link ID and Link Text.

            Vector3 worldPointInRectangle;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TextMeshPro.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);

            //switch (linkInfo.GetLinkID())
            //{
            //    case "id_01": // 100041637: // id_01
            //        Application.OpenURL("https://www.tmibuilder.gr/en/home-page/");
            //        break;
            //    case "id_02": // 100041638: // id_02
            //        Application.OpenURL("mailto:tmibuilder@iit.demokritos.gr");
            //        break;
            //}

            string linkID = linkInfo.GetLinkID();

            foreach(TextSelectorFieldItem item in fieldItems)
            {
                if (item.ID == linkID)
                {
                    if (item.URL.Contains("@"))
                    {
                        SendEmail(item.URL, item.MailSubject, item.MailBody);
                    }
                    else
                    {
                        Application.OpenURL(item.URL);
                    }
                }
            }
        }
        #endregion

    }

    void SendEmail(string mail, string subject, string body)
    {
        string _email = mail;
        string _subject = MyEscapeURL(subject);
        string _body = MyEscapeURL(body);
        Application.OpenURL("mailto:" + _email + "?subject=" + _subject + "&body=" + _body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp()");

        // Restore any character that may have been modified
        if (m_lastIndex != -1)
        {
            RestoreCachedVertexAttributes(m_lastIndex);
            m_lastIndex = -1;
        }
    }


    void RestoreCachedVertexAttributes(int index)
    {
        if (index == -1 || index > m_TextMeshPro.textInfo.characterCount - 1) return;

        // Get the index of the material / sub text object used by this character.
        int materialIndex = m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;

        // Get the index of the first vertex of the selected character.
        int vertexIndex = m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;

        // Restore Vertices
        // Get a reference to the cached / original vertices.
        Vector3[] src_vertices = m_cachedMeshInfoVertexData[materialIndex].vertices;

        // Get a reference to the vertices that we need to replace.
        Vector3[] dst_vertices = m_TextMeshPro.textInfo.meshInfo[materialIndex].vertices;

        // Restore / Copy vertices from source to destination
        dst_vertices[vertexIndex + 0] = src_vertices[vertexIndex + 0];
        dst_vertices[vertexIndex + 1] = src_vertices[vertexIndex + 1];
        dst_vertices[vertexIndex + 2] = src_vertices[vertexIndex + 2];
        dst_vertices[vertexIndex + 3] = src_vertices[vertexIndex + 3];

        // Restore Vertex Colors
        // Get a reference to the vertex colors we need to replace.
        Color32[] dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;

        // Get a reference to the cached / original vertex colors.
        Color32[] src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;

        // Copy the vertex colors from source to destination.
        dst_colors[vertexIndex + 0] = src_colors[vertexIndex + 0];
        dst_colors[vertexIndex + 1] = src_colors[vertexIndex + 1];
        dst_colors[vertexIndex + 2] = src_colors[vertexIndex + 2];
        dst_colors[vertexIndex + 3] = src_colors[vertexIndex + 3];

        // Restore UV0S
        // UVS0
        Vector2[] src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
        Vector2[] dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
        dst_uv0s[vertexIndex + 0] = src_uv0s[vertexIndex + 0];
        dst_uv0s[vertexIndex + 1] = src_uv0s[vertexIndex + 1];
        dst_uv0s[vertexIndex + 2] = src_uv0s[vertexIndex + 2];
        dst_uv0s[vertexIndex + 3] = src_uv0s[vertexIndex + 3];

        // UVS2
        Vector2[] src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
        Vector2[] dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
        dst_uv2s[vertexIndex + 0] = src_uv2s[vertexIndex + 0];
        dst_uv2s[vertexIndex + 1] = src_uv2s[vertexIndex + 1];
        dst_uv2s[vertexIndex + 2] = src_uv2s[vertexIndex + 2];
        dst_uv2s[vertexIndex + 3] = src_uv2s[vertexIndex + 3];


        // Restore last vertex attribute as we swapped it as well
        int lastIndex = (src_vertices.Length / 4 - 1) * 4;

        // Vertices
        dst_vertices[lastIndex + 0] = src_vertices[lastIndex + 0];
        dst_vertices[lastIndex + 1] = src_vertices[lastIndex + 1];
        dst_vertices[lastIndex + 2] = src_vertices[lastIndex + 2];
        dst_vertices[lastIndex + 3] = src_vertices[lastIndex + 3];

        // Vertex Colors
        src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;
        dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;
        dst_colors[lastIndex + 0] = src_colors[lastIndex + 0];
        dst_colors[lastIndex + 1] = src_colors[lastIndex + 1];
        dst_colors[lastIndex + 2] = src_colors[lastIndex + 2];
        dst_colors[lastIndex + 3] = src_colors[lastIndex + 3];

        // UVS0
        src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
        dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
        dst_uv0s[lastIndex + 0] = src_uv0s[lastIndex + 0];
        dst_uv0s[lastIndex + 1] = src_uv0s[lastIndex + 1];
        dst_uv0s[lastIndex + 2] = src_uv0s[lastIndex + 2];
        dst_uv0s[lastIndex + 3] = src_uv0s[lastIndex + 3];

        // UVS2
        src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
        dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
        dst_uv2s[lastIndex + 0] = src_uv2s[lastIndex + 0];
        dst_uv2s[lastIndex + 1] = src_uv2s[lastIndex + 1];
        dst_uv2s[lastIndex + 2] = src_uv2s[lastIndex + 2];
        dst_uv2s[lastIndex + 3] = src_uv2s[lastIndex + 3];

        // Need to update the appropriate 
        m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

}
