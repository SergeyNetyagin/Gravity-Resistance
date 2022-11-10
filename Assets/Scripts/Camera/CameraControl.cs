using UnityEngine;

public class CameraControl : MonoBehaviour {

    [Header( "НАСТРОЙКА СЛЕЖЕНИЯ КАМЕРЫ ЗА КОРАБЛЁМ" )]
    [Tooltip( "Коэффициент регулировки фокуса камеры по отношению к высоте экрана: если необходимо фокусировать камеру в позицию чуть ниже (-) или выше (+) центра экрана; по умолчанию = -0.1" )]
    [SerializeField]
    [Range( -0.3f, 0.3f )]
    private float correction_rate_y = -0.1f;

    [SerializeField]
    [Tooltip( "Кривая затухания скорости камеры на отрезке [0, 1]: если игрок близко к центру экрана, камера центрируется медленно, и наоборот; максимум дампинга кривой = 10" )]
    private AnimationCurve damping;

    [Header( "АНИМАЦИИ ДРОЖАНИЯ КАМЕРЫ С ЭФФЕКТАМИ" )]
    [SerializeField]
    [Tooltip( "Максимальный уровень повреждений, после котрого включается лёгкая тряска камеры; по умолчанию = 5" )]
    [Range( 1f, 10f )]
    private float shake_max_damage_soft = 5f;
    public float Shake_max_damage_soft { get { return shake_max_damage_soft; } }

    [SerializeField]
    [Tooltip( "Максимальный уровень повреждений, после котрого включается сильная тряска камеры; по умолчанию = 15" )]
    [Range( 10f, 20f )]
    private float shake_max_damage_hard = 15f;
    public float Shake_max_damage_hard { get { return shake_max_damage_hard; } }

    [SerializeField]
    [Tooltip( "Максимальный уровень повреждений, после котрого включается ужасная тряска камеры; по умолчанию = 25" )]
    [Range( 20f, 30f )]
    private float shake_max_damage_awful = 25f;
    public float Shake_max_damage_awful { get { return shake_max_damage_awful; } }

    [SerializeField]
    [Tooltip( "Максимальный уровень повреждений, после котрого возникает вероятность образования утечки топлива; по умолчанию = 15" )]
    [Range( 1f, 30f )]
    private float shake_max_damage_leaks = 15f;
    public float Shake_max_damage_leaks { get { return shake_max_damage_leaks; } }

    [Space( 10 )]
    [Tooltip( "Эффект вибрации камеры при сильной перегрузке двигателя (при падении тяги или в определённых опасных зонах)" )]
    [SerializeField]
    private AnimationBehaviour
        shake_engine_on_x; [SerializeField] private AnimationBehaviour
        shake_engine_on_y;

    [SerializeField]
    private MonoBehaviour engine_effect;

    [Tooltip( "Эффект вибрации камеры при лёгком ударе" )]
    [SerializeField]
    [Space( 10 )]
    private AnimationBehaviour
        shake_soft_on_x; [SerializeField] private AnimationBehaviour
        shake_soft_on_y;
    
    [SerializeField]
    private MonoBehaviour soft_effect;

    [Tooltip( "Эффект вибрации камеры при сильном ударе" )]
    [SerializeField]
    [Space( 10 )]
    private AnimationBehaviour
        shake_hard_on_x; [SerializeField] private AnimationBehaviour
        shake_hard_on_y;

    [SerializeField]
    private MonoBehaviour hard_effect;

    [Tooltip( "Эффект вибрации камеры при мощнейшем ударе" )]
    [SerializeField]
    [Space( 10 )]
    private AnimationBehaviour
        shake_awful_on_x; [SerializeField] private AnimationBehaviour
        shake_awful_on_y;

    [SerializeField]
    private MonoBehaviour awful_effect;

    private AnimationGraphic animation_shaking;

    private Transform cached_transform;

    private Vector2
        player_offset = Vector2.zero;

    private Vector3
        screen_position = Vector3.zero,
        current_position = Vector3.zero,
        player_screen_position = Vector3.zero;

    private Vector3
        screen_min_position = Vector3.zero,
        screen_max_position = Vector3.zero;

    private float
        starting_offset_y = 0f,
        lerp_rate_x = 0f,
        lerp_rate_y = 0f;

    public bool Is_shaking { get { return animation_shaking.enabled; } }
    public void StartShaking( ShakeType shake_type ) { SetShaking( shake_type ); animation_shaking.enabled = true; }
    public void StopShaking() { ResetShaking(); animation_shaking.enabled = false; }
        
    // Use this for initialization #############################################################################################################################################
    void Start () {

        cached_transform = transform;

        animation_shaking = GetComponent<AnimationGraphic>();
        animation_shaking.enabled = false;

        // Определяем высоту камеры по отношению к центральной точке экрана, учитывая угол наклона камеры
        starting_offset_y = Mathf.Abs( cached_transform.position.z ) * Mathf.Tan( cached_transform.eulerAngles.x * Mathf.Deg2Rad );
        starting_offset_y += starting_offset_y * correction_rate_y;

        current_position.x = Game.Player_transform.position.x;
        current_position.y = Game.Player_transform.position.y + starting_offset_y;
        current_position.z = cached_transform.position.z;

        // Мгновенно фокусируем камеру на игроке
        cached_transform.position = current_position;
    }

    // Update is called once per frame #########################################################################################################################################
    void LateUpdate() {

        // Остановть дрожь камеры, если анимация полность проиграна или остановлена извне
        if( animation_shaking.enabled && animation_shaking.Is_stopped ) StopShaking();

        // Подготовить данные для фокусирования камеры на игроке
        player_screen_position = Game.Camera.WorldToScreenPoint( Game.Player_transform.position );

        // Плавно фокусировать камеру на игроке по координате X
        player_offset.x = cached_transform.position.x - Game.Player_transform.position.x;
        lerp_rate_x = Game.Zoom_control.Screen_scale * Time.deltaTime * damping.Evaluate( Mathf.Abs( current_position.x - Game.Player_transform.position.x ) );
        current_position.x = Mathf.Lerp( current_position.x, Game.Player_transform.position.x, lerp_rate_x );

        // Плавно фокусировать камеру на игроке по координате Y
        player_offset.y = cached_transform.position.y - Game.Player_transform.position.y;
        lerp_rate_y = Game.Zoom_control.Screen_scale * Time.deltaTime * damping.Evaluate( Mathf.Abs( current_position.y - Game.Player_transform.position.y ) );
        current_position.y = Mathf.Lerp( current_position.y, Game.Player_transform.position.y + starting_offset_y, lerp_rate_y );

        // Откорректировать позицию камеры
        cached_transform.position = current_position;

        // Откорректировать позицию навигатора, если он включен
        if( Game.Radar.Is_enabled || Game.Navigator.Is_enabled ) Game.Navigator.Rect_transform.position = Game.Camera.WorldToScreenPoint( Game.Player.Navigator_point_transform.position );
    }

    // Shake the camera on explosions or hard collisions #######################################################################################################################
    private void SetShaking( ShakeType shake_type ) {

        switch( shake_type ) {

            case ShakeType.Engine:

                shake_engine_on_x.SetMode( AnimationMode.Loop );
                shake_engine_on_y.SetMode( AnimationMode.Loop );
                animation_shaking.SetMotionBehaviourX( shake_engine_on_x );
                animation_shaking.SetMotionBehaviourY( shake_engine_on_y );
                if( engine_effect != null ) engine_effect.enabled = true;
                break;

            case ShakeType.Soft:

                shake_soft_on_x.SetMode( AnimationMode.Once );
                shake_soft_on_y.SetMode( AnimationMode.Once );
                animation_shaking.SetMotionBehaviourX( shake_soft_on_x );
                animation_shaking.SetMotionBehaviourY( shake_soft_on_y );
                if( soft_effect != null ) soft_effect.enabled = true;
                break;

            case ShakeType.Hard:

                shake_hard_on_x.SetMode( AnimationMode.Once );
                shake_hard_on_y.SetMode( AnimationMode.Once );
                animation_shaking.SetMotionBehaviourX( shake_hard_on_x );
                animation_shaking.SetMotionBehaviourY( shake_hard_on_y );
                if( hard_effect != null ) hard_effect.enabled = true;
                break;

            case ShakeType.Awful:

                shake_awful_on_x.SetMode( AnimationMode.Once );
                shake_awful_on_y.SetMode( AnimationMode.Once );
                animation_shaking.SetMotionBehaviourX( shake_awful_on_x );
                animation_shaking.SetMotionBehaviourY( shake_awful_on_y );
                if( awful_effect != null ) awful_effect.enabled = true;
                break;
        }
    }

    // Shake the camera on explosions or hard collisions #######################################################################################################################
    private void ResetShaking() {

        animation_shaking.Motion_on_x.Reset();
        animation_shaking.Motion_on_y.Reset();

        if( engine_effect != null ) engine_effect.enabled = false;
        if( soft_effect != null ) soft_effect.enabled = false;
        if( hard_effect != null ) hard_effect.enabled = false;
        if( awful_effect != null ) awful_effect.enabled = false;
    }
    
    // Make positions in frustum of the camera #################################################################################################################################
    public void MakeFrustumPosition( ref Vector3 min_position, ref Vector3 max_position, float point_z ) {

        min_position.x = 0f;
        min_position.y = 0f;
        min_position.z = point_z;
        min_position = Game.Camera.ScreenToWorldPoint( min_position );

        max_position.x = Screen.width;
        max_position.y = Screen.height;
        max_position.z = point_z;
        max_position = Game.Camera.ScreenToWorldPoint( max_position );
    }
    
    // Calculate a visibility in camera's frustum zone #########################################################################################################################
    public bool IsInFrustum( Vector3 point ) {

        screen_position = Game.Camera.WorldToScreenPoint( point );
            
        if( (screen_position.x < 0f) || (screen_position.x > Screen.width) ) return false;
        if( (screen_position.y < 0f) || (screen_position.y > Screen.height) ) return false;

        return true;
    }
        
    // Calculate a visibility in camera's frustum zone #########################################################################################################################
    public bool IsInFrustum( Renderer object_renderer ) {

        screen_min_position = Game.Camera.WorldToScreenPoint( object_renderer.bounds.min );
        screen_max_position = Game.Camera.WorldToScreenPoint( object_renderer.bounds.max );
            
        if( (screen_min_position.x < 0f) || (screen_max_position.x > Screen.width) ) return false;
        if( (screen_min_position.y < 0f) || (screen_max_position.y > Screen.height) ) return false;

        return true;
    }

    // Calculate a visibility in camera's clipping distance ####################################################################################################################
    public bool IsInClippingZone( Transform cached_transform ) {

        if( cached_transform.position.z < Game.Camera.nearClipPlane ) return false;
        if( cached_transform.position.z > Game.Camera.farClipPlane ) return false;

        return true;
    }
}