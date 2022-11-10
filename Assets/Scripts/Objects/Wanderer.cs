using UnityEngine;

[RequireComponent( typeof( Rigidbody) )]
[RequireComponent( typeof( AnimationMovement ) )]
public class Wanderer : MonoBehaviour {

    //[SerializeField]
    //[Tooltip( "Ключ названия типа данного объекта (например, <Странствующий объект>); в базе локализаций пишется с большой буквы" )]
    //private string kind_name_key;
    //public string Kind_name_key { get { return kind_name_key; } }
    //public string Kind_name { get { return Game.Localization.GetTextValue( kind_name_key ); } }

    [Space( 10 )]
    [SerializeField]
    private bool control_kinematic = false;

    [HideInInspector]
    public float Free_time = 0f;

    [HideInInspector]
    public float Activation_time = 0f;

    private Rigidbody physics;
    public Rigidbody Physics { get { return physics; } }

    private Renderer cached_renderer;
    public Renderer Cached_renderer { get { return cached_renderer; } }
        
    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private AnimationMovement animation_movement;
    public AnimationMovement Animation_movement { get { return animation_movement; } }

    private bool is_busy = false;
    public bool Is_busy { get { return is_busy; } set { is_busy = value; } }

    // Starting initialization #################################################################################################################################################
    void Awake() {

        cached_transform = transform;
        physics = GetComponent<Rigidbody>();
        cached_renderer = GetComponent<Renderer>();
        animation_movement = GetComponent<AnimationMovement>();
    }

    // Disable <Rigidbody.isKinematic> if it need ##############################################################################################################################
    void OnEnable() {

        if( control_kinematic ) physics.isKinematic = false;
    }

    // Enable <Rigidbody.isKinematic> if it need ###############################################################################################################################
    void OnDisable() {

        if( control_kinematic ) physics.isKinematic = true;
    }
    
    // Sleep the wanderer ######################################################################################################################################################
    public void Sleep( bool busy_state ) {

        is_busy = busy_state;

        Free_time = 0f;

        physics.Sleep();
        physics.isKinematic = true;
        gameObject.SetActive( false );
    }

    // Awake the wanderer ######################################################################################################################################################
    public void WakeUp( Transform activate_transform, bool busy_state ) {

        is_busy = busy_state;

        if( activate_transform != null ) Cached_transform.position = activate_transform.position;

        gameObject.SetActive( true );
        physics.isKinematic = false;
        physics.WakeUp();
    }
}