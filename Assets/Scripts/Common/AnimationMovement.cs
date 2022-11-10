using UnityEngine;
using System.Collections;

public class AnimationMovement : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Восстанавливать ли на момент отключения скрипта первоначальное состояние объекта, какое было до начала перемещения" )]
    public bool restore_on_disable = false;

    [SerializeField]
    [Range( 0f, 100f )]
    [Tooltip( "Через какой промежуток времени повторять обновления в случае физического перемещения, один раз за N секунд; по умолчанию = 10 (если ноль, то на тело воздействует только один импульс - при старте)" )]
    private float physical_refresh_time = 10f;

    [SerializeField]
    [Range( 0, 300 )]
    [Tooltip( "С какой скоростью выполнять обновления в случае математического перемещения, раз в секунду; по умолчанию = 60 (если 0, то по Time.deltaTime)" )]
    private int transformed_refresh_speed = 60;
    private float transformed_refresh_time = 0f;
    public void SetTransformedRefreshTime( float time ) { transformed_refresh_time = time; }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Cкорость перемещения по оси X, единиц в секунду: по умолчанию = 0" )]
    [Range( -100f, 100f )]
    private float speed_on_x = 0f;
    public float Speed_on_x { get { return speed_on_x; } }
    public void SetSpeedOnX( float speed ) { speed_on_x = speed; is_move_on_x = (speed != 0f) ? true : false; }

    [SerializeField]
    [Tooltip( "Cкорость перемещения по оси Y, единиц в секунду: по умолчанию = 0" )]
    [Range( -100f, 100f )]
    private float speed_on_y = 0f;
    public float Speed_on_y { get { return speed_on_y; } }
    public void SetSpeedOnY( float speed ) { speed_on_y = speed; is_move_on_y = (speed != 0f) ? true : false; }

    [SerializeField]
    [Tooltip( "Cкорость перемещения по оси Z, единиц в секунду: по умолчанию = 0" )]
    [Range( -100f, 100f )]
    private float speed_on_z = 0f;
    public float Speed_on_z { get { return speed_on_z; } }
    public void SetSpeedOnZ( float speed ) { speed_on_z = speed; is_move_on_z = (speed != 0f) ? true : false; }

    private bool
        is_move_on_x = false,
        is_move_on_y = false,
        is_move_on_z = false;

    private const float min_refresh_time = 0.015f;

    private Vector3 start_position;

    private bool is_enabled = false;

    private Rigidbody physics;
    private Transform cached_transform;

    private Vector3 
        force = Vector3.zero,
        position = Vector3.zero;

    private WaitForSeconds physical_wait_for_seconds;
    private WaitForSeconds transformed_wait_for_seconds;

    // Starting initialization #################################################################################################################################################
    void Awake() {

        physics = GetComponent<Rigidbody>();
        cached_transform = GetComponent<Transform>() as Transform;

        start_position = cached_transform.position;

        if( transformed_refresh_speed != 0 ) transformed_refresh_time = 1f / transformed_refresh_speed;
        else transformed_refresh_time = 0f;

        physical_wait_for_seconds = new WaitForSeconds( physical_refresh_time );
        transformed_wait_for_seconds = new WaitForSeconds( transformed_refresh_time );
    }

    // On enable object ########################################################################################################################################################
    void OnEnable() {

        if( speed_on_x != 0 ) is_move_on_x = true;
        if( speed_on_y != 0 ) is_move_on_y = true;
        if( speed_on_z != 0 ) is_move_on_z = true;

        is_enabled = true;

        if( (physics == null) || physics.isKinematic ) StartCoroutine( TransformedRefresh() );
        else StartCoroutine( PhysicalRefresh() );
    }
    
    // On disable object #######################################################################################################################################################
    void OnDisable() {

        is_enabled = false;

        if( restore_on_disable ) cached_transform.position = start_position;
    }

	// If isn't used physics ###################################################################################################################################################
	IEnumerator TransformedRefresh() {
        
        transformed_refresh_time = Time.time - Time.deltaTime;

        while( is_enabled ) {

            transformed_refresh_time = Time.time - transformed_refresh_time;

            position = Vector3.zero;

            if( is_move_on_x ) position.x = speed_on_x * transformed_refresh_time;
            if( is_move_on_y ) position.y = speed_on_y * transformed_refresh_time;
            if( is_move_on_z ) position.z = speed_on_z * transformed_refresh_time;

            cached_transform.position += position;

            transformed_refresh_time = Time.time;

            yield return transformed_wait_for_seconds;
        }

        yield break;
    }

	// If is used physics ######################################################################################################################################################
	IEnumerator PhysicalRefresh() {

        physics.velocity = new Vector3( speed_on_x, speed_on_y, speed_on_z );

        if( physical_refresh_time == 0f ) yield break;

        while( is_enabled ) {

            force = Vector3.zero;

            // Определяем, необходимо ли откорректировать перемещение вдоль оси X
            if( is_move_on_x ) {

                if( (speed_on_x > 0f) && (physics.velocity.x < speed_on_x) ) force.x = speed_on_x;
                else if( (speed_on_x < 0f) && (physics.velocity.x > speed_on_x) ) force.x = speed_on_x;
            }

            // Определяем, необходимо ли откорректировать перемещение вдоль оси Y
            if( is_move_on_y ) {

                if( (speed_on_y > 0f) && (physics.velocity.y < speed_on_y) ) force.y = speed_on_y;
                else if( (speed_on_y < 0f) && (physics.velocity.y > speed_on_y) ) force.y = speed_on_y;
            }
            
            // Определяем, необходимо ли откорректировать перемещение вдоль оси Z
            if( is_move_on_z ) {

                if( (speed_on_z > 0f) && (physics.velocity.z < speed_on_z) ) force.z = speed_on_z;
                else if( (speed_on_z < 0f) && (physics.velocity.z > speed_on_z) ) force.z = speed_on_z;
            }

            // Если есть необходимость в корректировке перемещения, применяем силу
            if( force != Vector3.zero ) physics.AddForce( force, ForceMode.Impulse );

            yield return physical_wait_for_seconds;
        }

        physics.velocity = Vector3.zero;

        yield break;
    }
}