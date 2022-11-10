using UnityEngine;
using UnityEngine.Networking;

public class ZoomControl : MonoBehaviour {

    [Tooltip( "Минимально допустимое увеличение; по умолчанию = 0.5" )]
    [SerializeField]
    [Range( 0.1f, 1.0f )]
    private float min_zoom = 0.5f;

    [Tooltip( "Максимально допустимое увеличение; по умолчанию = 2.0" )]
    [SerializeField]
    [Range( 1.0f, 5.0f )]
    private float max_zoom = 2.0f;

    [Tooltip( "Скорость увеличения / уменьшения; по умолчанию = 0.5" )]
    [SerializeField]
    [Range( 0.05f, 1.0f )]
    private float zoom_speed = 0.5f;

    private Vector2 saved_touch_distance = new Vector2( 0.0f, 0.0f );

    private float
        change_zoom = 0f,
        current_zoom = 1f,
        current_zoom_inversed = 1f;

    private float 
        camera_field_of_view = 10f,
        camera_orthographic_size = 10f;

    public float Screen_scale { get { return current_zoom; } }
    public float Screen_scale_inversed { get { return current_zoom; } }
        
    // Starting initialization #################################################################################################################################################
    void Start() {

        if( Game.Camera.orthographic ) camera_orthographic_size = Game.Camera.orthographicSize;
        else camera_field_of_view = Game.Camera.fieldOfView;
    }

    // #########################################################################################################################################################################
    void Update() {

        // Если это не локальный игрок, обработка зума не производится
        if( Game.Is( GameState.Paused ) ) return;

        #if UNITY_IOS
        if( GetTouchDrag() ) ScreenZoomInOut();     
        #elif UNITY_ANDROID
        if( GetTouchDrag() ) ScreenZoomInOut();
        #elif UNITY_STANDALONE
        if( GetMouseScroll() ) ScreenZoomInOut();
        #endif
    }

    // Detect a touch dragging #################################################################################################################################################
    bool GetTouchDrag() {

        if( Input.touchCount != 2 ) return false;

        if( (Input.GetTouch( 0 ).phase == TouchPhase.Began) || (Input.GetTouch( 1 ).phase != TouchPhase.Began) ) {

            saved_touch_distance.x = Mathf.Abs( Input.GetTouch( 1 ).position.x - Input.GetTouch( 0 ).position.x );
            saved_touch_distance.y = Mathf.Abs( Input.GetTouch( 1 ).position.y - Input.GetTouch( 0 ).position.y );

            return false;
        }

        change_zoom = zoom_speed * Time.deltaTime;

        if( Mathf.Abs( Input.GetTouch( 1 ).position.x - Input.GetTouch( 0 ).position.x ) < saved_touch_distance.x ) change_zoom = -change_zoom;
        else if( Mathf.Abs( Input.GetTouch( 1 ).position.y - Input.GetTouch( 0 ).position.y ) < saved_touch_distance.y ) change_zoom = -change_zoom;

        saved_touch_distance.x = Mathf.Abs( Input.GetTouch( 1 ).position.x - Input.GetTouch( 0 ).position.x );
        saved_touch_distance.y = Mathf.Abs( Input.GetTouch( 1 ).position.y - Input.GetTouch( 0 ).position.y );

        return true;
    }

    // Detect a mouse scrolling ################################################################################################################################################
    bool GetMouseScroll() {

        change_zoom = -Input.GetAxis( "Mouse ScrollWheel" );

        if( change_zoom != 0f ) return true;

        return false;
    }
    
    // Zoom the screen #########################################################################################################################################################
    void ScreenZoomInOut() {

        current_zoom -= change_zoom;
        change_zoom = 0f;

        if( current_zoom > max_zoom ) current_zoom = max_zoom;
        else if( current_zoom < min_zoom ) current_zoom = min_zoom;

        current_zoom_inversed = 1f / current_zoom;

        if( Game.Camera.orthographic ) Game.Camera.orthographicSize = camera_orthographic_size * current_zoom_inversed;
        else Game.Camera.fieldOfView = camera_field_of_view * current_zoom_inversed;

        Game.Navigator.RefreshNavigatorScale();
        Game.Navigator.RepositionNavigatorPanel();
    }
}