using UnityEngine;
using UnityEngine.UI;

public class AnimationColor : MonoBehaviour {

    [SerializeField]
    private bool 
        alpha_only = true,
        restore_on_disable = true;

    [SerializeField]
    private AnimationBehaviour color_a;
    public AnimationBehaviour Color_a { get { return color_a; } }
        
    [SerializeField]
    private AnimationBehaviour color_r;
    public AnimationBehaviour Color_r { get { return color_r; } }

    [SerializeField]
    private AnimationBehaviour color_g;
    public AnimationBehaviour Color_g { get { return color_g; } }

    [SerializeField]
    private AnimationBehaviour color_b;
    public AnimationBehaviour Color_b { get { return color_b; } }

    private bool
        is_change_a = false,
        is_change_r = false,
        is_change_g = false,
        is_change_b = false;

    private Color32 
        start_color,
        current_color;

    private Text text;
    private Image image;

    // Starting initialization #################################################################################################################################################
    void Awake() {

        text = GetComponent<Text>();
        image = GetComponent<Image>();
    }
    
    // On enable object ########################################################################################################################################################
    void OnEnable() {

        if( (color_a != null) && color_a.Has_curve ) is_change_a = true;
        if( (color_r != null) && color_r.Has_curve ) is_change_r = true;
        if( (color_g != null) && color_g.Has_curve ) is_change_g = true;
        if( (color_b != null) && color_b.Has_curve ) is_change_b = true;

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
        if( is_change_r ) current_color.r = (byte) color_r.Evaluate( Time.deltaTime );
        if( is_change_g ) current_color.g = (byte) color_g.Evaluate( Time.deltaTime );
        if( is_change_b ) current_color.b = (byte) color_b.Evaluate( Time.deltaTime );

        if( image != null ) image.color = current_color;
        else if( text != null ) text.color = current_color;
	}
}