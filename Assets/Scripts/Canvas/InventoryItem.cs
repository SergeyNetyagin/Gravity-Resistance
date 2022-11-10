using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour {

    public int Index { get; set; }
    public Value Value { get; set; }

    [System.NonSerialized]
    private Image frame_image;
    public Image Frame_image { get { return frame_image; } }

    [System.NonSerialized]
    private Shadow frame_shadow;
    public Shadow Frame_shadow { get { return frame_shadow; } }

    [System.NonSerialized]
    private Image picture_image;
    public Image Picture_image { get { return picture_image; } }

    [System.NonSerialized]
    private Shadow picture_shadow;
    public Shadow Picture_shadow { get { return picture_shadow; } }

    [System.NonSerialized]
    private Image container_image;
    public Image Container_image { get { return container_image; } }

    [System.NonSerialized]
    private InventoryTooltip inventory_tooltip;
    public InventoryTooltip Inventory_tooltip { get { return inventory_tooltip; } }

    public bool Is_empty { get; set; }
    public bool Is_absent { get; set; }

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private Inventory inventory;
    private Transform inventory_transform;

    private Transform picture_transform;
    public Transform Picture_transform { get { return picture_transform; } }

    private Transform tooltip_transform;
    public Transform Tooltip_transform { get { return tooltip_transform; } }

    [System.NonSerialized]
    private Vector3 touch_position = Vector3.zero;
            
    // Use this for initialization #############################################################################################################################################
	void OnEnable() {

        if( cached_transform == null ) {

            cached_transform = GetComponent<Transform>();
                
            inventory = GetComponentInParent<Inventory>();
            inventory_transform = inventory.GetComponent<Transform>();

            inventory_tooltip = GetComponentInChildren<InventoryTooltip>();
            tooltip_transform = inventory_tooltip.GetComponent<Transform>();

            container_image = GetComponentInChildren<InventoryContainer>().GetComponent<Image>();
            picture_image = GetComponentInChildren<InventoryPicture>().GetComponent<Image>();
            picture_shadow = picture_image.GetComponent<Shadow>();
            frame_image = GetComponent<Image>();
            frame_shadow = GetComponent<Shadow>();

            picture_transform = picture_image.GetComponent<Transform>();
        }
    }
}
