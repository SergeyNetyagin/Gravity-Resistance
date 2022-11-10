using UnityEngine;
using UnityEngine.UI;

public class AnimationColorAlpha : MonoBehaviour {

    [SerializeField]
    private bool restore_on_disable = true;

    [SerializeField]
    private AnimationBehaviour color_a;
    public AnimationBehaviour Color_a { get { return color_a; } }

    private bool is_change_a = false;

    private Color32 
        start_color,
        current_color;

    private Text text;
    private Image image;

    public void SetSpeed( float speed ) { color_a.SetSpeed( speed ); }

    // Starting initialization #################################################################################################################################################
    void Awake() {

        text = GetComponent<Text>();
        image = GetComponent<Image>();
    }

    // On enable object ########################################################################################################################################################
    void OnEnable() {

        if( (color_a != null) && color_a.Has_curve ) is_change_a = true;

        if( image != null ) start_color = current_color = image.color;
        else if( text != null ) start_color = current_color = text.color;
    }

    // On disable object #######################################################################################################################################################
    void OnDisable() {

        if( !restore_on_disable ) return;

        if( image != null ) image.color = start_color;
        else if( text != null ) text.color = start_color;
    }

	// Update is called once per frame #########################################################################################################################################
	void Update () {

        if( is_change_a ) current_color.a = (byte) color_a.Evaluate( Time.deltaTime );

        if( image != null ) image.color = current_color;
        else if( text != null ) text.color = current_color;
	}
}