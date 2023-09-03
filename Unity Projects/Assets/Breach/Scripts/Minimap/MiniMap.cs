using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MiniMapMode
{
    Mini, Fullscreen
}
public class MiniMap : MonoBehaviour
{
    public static MiniMap Instance;

    [SerializeField]
    Vector2 miniSizeDimensions = new Vector2(400, 400);

    [SerializeField]
    Vector2 fullScreenDimensions = new Vector2(1000, 1000);

    [SerializeField]
    float scrollSpeed = 0.1f;

    [SerializeField]
    float maxZoom = 10f;

    [SerializeField]
    float minZoom = 1f;

    [SerializeField]
    Terrain terrain;

    [SerializeField]
    RectTransform scrollViewRectTransform;

    [SerializeField]
    RectTransform contentRectTransform;

    [SerializeField]
   Transform miniMapCanvas;
    [SerializeField]
    MiniMapIcon miniMapIconPrefab;


    Matrix4x4 transformationMatrix;

    Vector2 centerTranslation, scaleRatio;
    float scroll = 0.0001f;
    float zoomingScale = 1f;

    Vector2 halfVector2 = new Vector2(0.5f, 0.5f);
    private MiniMapMode currentMiniMapMode = MiniMapMode.Mini;
    private MiniMapIcon playerMiniMapIcon;
    Dictionary<MiniMapWorldObject, MiniMapIcon> miniMapWorldObjectsLookup = new Dictionary<MiniMapWorldObject, MiniMapIcon>();

    private bool MatrixExisted = false;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // CalculateTransformationMatrix();
        CalculateMapTransformation();
        MatrixExisted = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // toggle mode
            SetMiniMapMode(currentMiniMapMode == MiniMapMode.Mini ? MiniMapMode.Fullscreen : MiniMapMode.Mini);
        }



        //  float scroll = Input.GetAxis("Mouse ScrollWheel");
        // ScrollMap(scroll);
        Scrollcale();
        UpdateMiniMapIcons();
        CenterMiniMapOnPlayer();
    }

    public void RegisterMiniMapWorldObject(MiniMapWorldObject miniMapWorldObject, bool isPlayer = false)
    {
        var miniMapIcon = Instantiate(miniMapIconPrefab);
        miniMapIcon.transform.SetParent(contentRectTransform);
        //miniMapIcon.transform.localPosition.z = 0; ;
        miniMapIcon.SetIcon(miniMapWorldObject.Icon);
        miniMapIcon.SetColor(miniMapWorldObject.IconColor);
        miniMapIcon.SetText(miniMapWorldObject.Text);
        miniMapIcon.SetTextSize(miniMapWorldObject.TextSize);
        miniMapWorldObjectsLookup[miniMapWorldObject] = miniMapIcon;

        if (isPlayer)
            playerMiniMapIcon = miniMapIcon;
        var pPos = playerMiniMapIcon.transform.localPosition;
        playerMiniMapIcon.transform.localPosition = new Vector3(playerMiniMapIcon.transform.localPosition.x, playerMiniMapIcon.transform.localPosition.y);
       // Debug.Log("playerMiniMapIcon local position " + playerMiniMapIcon.transform.localPosition);
      // Debug.Log("playerMiniMapIcon position " + playerMiniMapIcon.transform.position);
    }

    public void DestroyCorrespondingMiniMapIcon(MiniMapWorldObject miniMapWorldObject)
    {
        if (miniMapWorldObjectsLookup.TryGetValue(miniMapWorldObject, out MiniMapIcon icon))
        {
            miniMapWorldObjectsLookup.Remove(miniMapWorldObject);
            if(icon!=null)
            Destroy(icon.gameObject);
        }
    }

    public void SetMiniMapMode(MiniMapMode mode)
    {
        if (mode == currentMiniMapMode)
            return;

        switch (mode)
        {
            case MiniMapMode.Mini:
                scrollViewRectTransform.sizeDelta = miniSizeDimensions;
                scrollViewRectTransform.anchorMin = Vector2.one;
                scrollViewRectTransform.anchorMax = Vector2.one;
                scrollViewRectTransform.pivot = Vector2.one;
                currentMiniMapMode = MiniMapMode.Mini;
                break;
            case MiniMapMode.Fullscreen:
                scrollViewRectTransform.sizeDelta = fullScreenDimensions;
                scrollViewRectTransform.anchorMin = halfVector2;
                scrollViewRectTransform.anchorMax = halfVector2;
                scrollViewRectTransform.pivot = halfVector2;
                currentMiniMapMode = MiniMapMode.Fullscreen;
                break;
        }
    }

    void ScrollMap(float scroll)
    {
        if (scroll == 0)
            return;

        float currentMapScale = contentRectTransform.localScale.x;
        // we need to scale the scroll speed by the current map scale to keep the zooming linear
        // float scrollAmount = (scroll > 0 ? scrollSpeed : -scrollSpeed) * currentMapScale;
        float scrollAmount = scroll * scrollSpeed;
        float newScale = currentMapScale + scrollAmount;
        float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
        contentRectTransform.localScale = Vector3.one * clampedScale;
    }

    void CenterMiniMapOnPlayer()
    {
        if (playerMiniMapIcon != null)
        {
            float mapScale = contentRectTransform.transform.localScale.x;
           
            // we simply move the map in the opposite direction the player moved, scaled by the mapscale 
            //contentRectTransform.transform.localPosition = (-playerMiniMapIcon.transform.localPosition * mapScale); //move in 3D 

            var xx = -playerMiniMapIcon.transform.localPosition.x * mapScale; // x component of the Icon
            var yy = -playerMiniMapIcon.transform.localPosition.y * mapScale; // z component of the Icon
            var zz = -contentRectTransform.transform.position.z;
            // contentRectTransform.transform.localPosition.x = xx;
            contentRectTransform.transform.localPosition = new Vector3(xx, yy,0);
           // Debug.Log(playerMiniMapIcon.transform.localPosition);
        }
    }

    void UpdateMiniMapIcons()
    {
        // scale icons by the inverse of the mapscale to keep them a consitent size
        float iconScale = 1 / contentRectTransform.transform.localScale.x;

        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;
           // var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);
            var mapPosition = WorldToMapPosition(miniMapWorldObject.transform.position);
            miniMapIcon.RectTransform.anchoredPosition = mapPosition;
            var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
            miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
            miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
        }
    }

    Vector2 WorldPositionToMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    Vector2 WorldToMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        pos = pos + centerTranslation;

        return Vector2.Scale(pos, scaleRatio);
    }

    void CalculateTransformationMatrix()
    {
        var miniMapDimensions = contentRectTransform.rect.size;
        var center = contentRectTransform.rect.center;
        var pos = contentRectTransform.rect.position;
       // Debug.Log("center" + center);
       // Debug.Log("position" + pos);
       // Debug.Log("map dim" + miniMapDimensions);
        var terrainDimensions = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        //Debug.Log("terrainDimensions"+ terrainDimensions);
       // Debug.Log("map dim" + miniMapDimensions);
        var scaleRatio = miniMapDimensions / terrainDimensions;
        //Debug.Log("scale" + terrainDimensions);
        var translation = -miniMapDimensions / 2;
       translation.y = -1 * translation.y;
       transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);

        
        //  {scaleRatio.x,   0,           0,   translation.x},
        //  {  0,        scaleRatio.y,    0,   translation.y},
        //  {  0,            0,           0,            0},
        //  {  0,            0,           0,            0}
    }

    void CalculateMapTransformation()
    {
        var miniMapDimensions = contentRectTransform.rect.size;
        var terrainDimensions = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        centerTranslation = -terrainDimensions/2;
        centerTranslation.y = -1 * centerTranslation.y;
        scaleRatio = miniMapDimensions / terrainDimensions;
    }

    public void ZoomIn()
    {
        scroll += 0.1f;
        zoomingScale= GetScale(scroll);
    }
    public void ZoomIout()
    {
        scroll -= 0.1f;
       zoomingScale = GetScale(scroll);
    }

    float GetScale(float scroll)
    {
     //   if (scroll == 0)
           // return 1.0f;

        float currentMapScale = contentRectTransform.localScale.x;
        // we need to scale the scroll speed by the current map scale to keep the zooming linear
        // float scrollAmount = (scroll > 0 ? scrollSpeed : -scrollSpeed) * currentMapScale;
        float scrollAmount = scroll * scrollSpeed;
        float newScale = currentMapScale + scrollAmount;
        float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
        Debug.Log(clampedScale);
        return clampedScale;
    }

    void Scrollcale()
    {
       // if (zoomingScale == 1.0f) return; 
        contentRectTransform.localScale = Vector3.one * zoomingScale;
        //Debug.Log();
    }
}
