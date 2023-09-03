using UnityEngine;

public class MiniMapWorldObject : MonoBehaviour
{
    // make sure this is true for the player object, this is what is used to center the map
    [SerializeField]
    private bool isPlayer = false;

    public Sprite Icon;
    public Color IconColor = Color.white;
    public string Text;
    public int TextSize = 10;
    private bool isMiniMap = false;

    public static MiniMapWorldObject instance;


    private void Start()
    {
        MiniMap.Instance.RegisterMiniMapWorldObject(this, isPlayer);
        isMiniMap = true;
    }

    private void Update()
    {
        if (!isMiniMap)
        {
            if (this != null)
            {
                MiniMap.Instance.RegisterMiniMapWorldObject(this, isPlayer);
            }
            isMiniMap = true;
        }
    }
    
    private void OnDestroy()
    {
        if (this != null)
            MiniMap.Instance.DestroyCorrespondingMiniMapIcon(this);
    }

    
}