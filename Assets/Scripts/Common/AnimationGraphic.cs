using UnityEngine;
using System.Collections;

public enum MotionMode {

    Movement,
    Rotation
}

public enum EvaluationMode {

    Relative,
    Absolute
}

public class AnimationGraphic : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Режим метода преобразования: перемещение или вращение; по умолчанию = Movement" )]
    private MotionMode motion_mode = MotionMode.Movement;

    [SerializeField]
    [Tooltip( "Режим характера преобразования: абсолютный (тело движется по графику функции) или относительный (к позиции тела добавляется значение графика функции); по умолчанию = Relative" )]
    private EvaluationMode evaluation_mode = EvaluationMode.Relative;

    [SerializeField]
    [Tooltip( "Восстанавливать ли на момент отключения скрипта первоначальное состояние объекта, какое было до начала его перемещения" )]
    public bool restore_on_disable = false;

    [SerializeField]
    [Range( 1f, 100f )]
    [Tooltip( "Через какой промежуток времени повторять обновления в случае физического перемещения, один раз за X секунд; по умолчанию = 10.0" )]
    private float physical_refresh_time = 10f;

    [SerializeField]
    [Range( 0, 300 )]
    [Tooltip( "С какой скоростью выполнять обновления в случае математического перемещения, раз в секунду; по умолчанию = 60 (если 0, то по Time.deltaTime)" )]
    private int transformed_refresh_speed = 60;
    private float transformed_refresh_time = 0f;
    public void SetTransformedRefreshTime( float time ) { transformed_refresh_time = time; }

    [SerializeField]
    [Space( 10 )]
    private AnimationBehaviour motion_on_x;
    public AnimationBehaviour Motion_on_x { get { return motion_on_x; } }
    public void SetMotionBehaviourX( AnimationBehaviour behaviour ) {  motion_on_x = behaviour; }

    [SerializeField]
    private AnimationBehaviour motion_on_y;
    public AnimationBehaviour Motion_on_y { get { return motion_on_y; } }
    public void SetMotionBehaviourY( AnimationBehaviour behaviour ) {  motion_on_y = behaviour; }

    [SerializeField]
    private AnimationBehaviour motion_on_z;
    public AnimationBehaviour Motion_on_z { get { return motion_on_z; } }
    public void SetMotionBehaviourZ( AnimationBehaviour behaviour ) {  motion_on_z = behaviour; }

    private bool
        is_motion_on_x = false,
        is_motion_on_y = false,
        is_motion_on_z = false;

    private const float min_refresh_time = 0.015f;

    private Vector3 start_motion;

    private bool is_enabled = false;

    private Rigidbody physics;
    private Transform cached_transform;

    private Vector3 
        force = Vector3.zero,
        motion = Vector3.zero;

    private WaitForSeconds physical_wait_for_seconds;
    private WaitForSeconds transformed_wait_for_seconds;
    
    public bool Is_stopped { get { return (motion_on_x.Is_stopped || motion_on_y.Is_stopped || motion_on_z.Is_stopped); } }
    
    // Starting initialization #################################################################################################################################################
    void Awake() {

        physics = GetComponent<Rigidbody>();
        cached_transform = GetComponent<Transform>();

        start_motion = (motion_mode == MotionMode.Movement) ? cached_transform.position : cached_transform.localEulerAngles;

        if( transformed_refresh_speed != 0 ) transformed_refresh_time = 1f / transformed_refresh_speed;
        else transformed_refresh_time = 0f;

        physical_wait_for_seconds = new WaitForSeconds( physical_refresh_time );
        transformed_wait_for_seconds = new WaitForSeconds( transformed_refresh_time );
    }

    // On enable object ########################################################################################################################################################
    void OnEnable() {

        if( (motion_on_x != null) && motion_on_x.Has_curve ) is_motion_on_x = true;
        if( (motion_on_y != null) && motion_on_y.Has_curve ) is_motion_on_y = true;
        if( (motion_on_z != null) && motion_on_z.Has_curve ) is_motion_on_z = true;

        is_enabled = true;

        if( (physics == null) || physics.isKinematic ) StartCoroutine( TransformedRefresh() );
        else StartCoroutine( PhysicalRefresh() );
    }
    
    // On disable object #######################################################################################################################################################
    void OnDisable() {

        is_enabled = false;

        if( restore_on_disable && (motion_mode == MotionMode.Movement) ) cached_transform.position = start_motion;
        else if( restore_on_disable && (motion_mode == MotionMode.Rotation) ) cached_transform.localEulerAngles = start_motion;
    }

	// If isn't used physics ###################################################################################################################################################
	IEnumerator TransformedRefresh() {

        transformed_refresh_time = Time.time - Time.deltaTime;

        while( is_enabled ) {

            transformed_refresh_time = Time.time - transformed_refresh_time;

            motion = Vector3.zero;
            
            if( is_motion_on_x ) motion.x = motion_on_x.Evaluate( transformed_refresh_time );
            if( is_motion_on_y ) motion.y = motion_on_y.Evaluate( transformed_refresh_time );
            if( is_motion_on_z ) motion.z = motion_on_z.Evaluate( transformed_refresh_time );

            if( motion_mode == MotionMode.Movement ) {

                if( evaluation_mode == EvaluationMode.Relative ) cached_transform.position += motion;
                else cached_transform.position = motion;
            }

            else {

                if( evaluation_mode == EvaluationMode.Relative ) cached_transform.localEulerAngles += motion;
                else cached_transform.localEulerAngles = motion;
            }

            transformed_refresh_time = Time.time;

            yield return transformed_wait_for_seconds;
        }

        yield break;
    }

	// If is used physics ######################################################################################################################################################
	IEnumerator PhysicalRefresh() {

        Vector3 average_speed;

        average_speed.x = motion_on_x.Average_speed;
        average_speed.y = motion_on_y.Average_speed;
        average_speed.z = motion_on_z.Average_speed;

        if( motion_mode == MotionMode.Movement ) physics.velocity = average_speed;
        else physics.angularVelocity = average_speed;

        while( is_enabled ) {

            force = Vector3.zero;

            // Определяем, необходимо ли откорректировать перемещение вдоль оси X
            if( is_motion_on_x ) {

                if( (average_speed.x > 0f) && (physics.velocity.x < average_speed.x) ) force.x = average_speed.x;
                else if( (average_speed.x < 0f) && (physics.velocity.x > average_speed.x) ) force.x = average_speed.x;
            }

            // Определяем, необходимо ли откорректировать перемещение вдоль оси Y
            if( is_motion_on_y ) {

                if( (average_speed.y > 0f) && (physics.velocity.y < average_speed.y) ) force.y = average_speed.y;
                else if( (average_speed.y < 0f) && (physics.velocity.y > average_speed.y) ) force.y = average_speed.y;
            }
            
            // Определяем, необходимо ли откорректировать перемещение вдоль оси Z
            if( is_motion_on_z ) {

                if( (average_speed.z > 0f) && (physics.velocity.z < average_speed.z) ) force.z = average_speed.z;
                else if( (average_speed.z < 0f) && (physics.velocity.z > average_speed.z) ) force.z = average_speed.z;
            }

            // Если есть необходимость в корректировке перемещения, применяем силу
            if( force != Vector3.zero ) {

                if( motion_mode == MotionMode.Movement ) physics.AddForce( force, ForceMode.Impulse );
                else physics.AddTorque( force, ForceMode.Impulse );
            }

            yield return physical_wait_for_seconds;
        }

        physics.angularVelocity = Vector3.zero;

        yield break;
    }
}