using UnityEngine;
using UnityEngine.UI;


public class OccupancyGridView : MonoBehaviour
{
    private Texture2D texture;
    private RawImage mOccupancyGridDipslay;
    [SerializeField]
    public GameObject mGameObject;
    public IOccupancyGrid mInterfaceOccupancyGrid;

    /// <summary>
    /// Update texture field with actual occupancy grid.
    /// </summary>
    public void updateTexture()
    {
        BuddyTools.Utils.MatToTexture2D(mInterfaceOccupancyGrid.OccupancyGrid.Mat, texture);
    }

    // Use this for initialization
    void Start()
    {
        mInterfaceOccupancyGrid = mGameObject.GetComponent<IOccupancyGrid>();
        mOccupancyGridDipslay = gameObject.GetComponent<RawImage>();
        texture = new Texture2D((int)mInterfaceOccupancyGrid.OccupancyGrid.Width, (int)mInterfaceOccupancyGrid.OccupancyGrid.Height, TextureFormat.ARGB32, false);
        mOccupancyGridDipslay.texture = texture;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        updateTexture();
    }

}
