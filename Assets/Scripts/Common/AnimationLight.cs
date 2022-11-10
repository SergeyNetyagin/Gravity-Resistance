using UnityEngine;

public class AnimationLight : MonoBehaviour {

    private const float max_intensity = 8.0f;

    [SerializeField]
    private bool restore_on_disable = true;

    [SerializeField]
    private AnimationBehaviour intensity;
    public AnimationBehaviour Intensity { get { return intensity; } }

    private bool is_change_light = false;

    private float
        start_intensity,
        current_intensity;

    private Light light_component;
    
    // Starting initialization #################################################################################################################################################
    void Awake() {

        light_component = GetComponent<Light>();
    }
    
    // On enable object ########################################################################################################################################################
    void OnEnable() {

        if( (intensity != null) && intensity.Has_curve ) is_change_light = true;

        start_intensity = current_intensity = light_component.intensity;
    }

    // On disable object #######################################################################################################################################################
    void OnDisable() {

        if( !restore_on_disable ) return;

        light_component.intensity = start_intensity;
    }

	// Update is called once per frame #########################################################################################################################################
	void Update () {

        if( is_change_light ) current_intensity = intensity.Evaluate( Time.deltaTime );

        light_component.intensity = current_intensity;
	}
}