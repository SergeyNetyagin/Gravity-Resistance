using UnityEngine;
using System.Collections;

public class AnimationRotation : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Восстанавливать ли на момент отключения скрипта первоначальное состояние объекта, какое было до начала вращения" )]
    public bool restore_on_disable = false;

    [SerializeField]
    [Range( 0f, 100f )]
    [Tooltip( "Через какой промежуток времени повторять обновления в случае физического вращения, один раз за N секунд; по умолчанию = 10 (если ноль, то на тело воздействует только один импульс - при старте)" )]
    private float physical_refresh_time = 10f;

    [SerializeField]
    [Range( 0, 300 )]
    [Tooltip( "С какой скоростью выполнять обновления в случае математического вращения, раз в секунду; по умолчанию = 60 (если 0, то по Time.deltaTime)" )]
    private int transformed_refresh_speed = 60;
    private float transformed_refresh_time = 0f;
    public void SetTransformedRefreshTime( float time ) { transformed_refresh_time = time; }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Cкорость вращения по оси X, градусов в секунду: по умолчанию = 0" )]
    [Range( -100f, 100f )]
    private float speed_on_x = 0f;
    public void SetSpeedOnX( float speed ) { speed_on_x = speed; is_rotate_on_x = (speed != 0f) ? true : false; }

    [SerializeField]
    [Tooltip( "Cкорость вращения по оси Y, градусов в секунду: по умолчанию = 0" )]
    [Range( -100f, 100f )]
    private float speed_on_y = 0f;
    public void SetSpeedOnY( float speed ) { speed_on_y = speed; is_rotate_on_y = (speed != 0f) ? true : false; }

    [SerializeField]
    [Tooltip( "Cкорость вращения по оси Z, градусов в секунду: по умолчанию = 0" )]
    [Range( -100f, 100f )]
    private float speed_on_z = 0f;
    public void SetSpeedOnZ( float speed ) { speed_on_z = speed; is_rotate_on_z = (speed != 0f) ? true : false; }

    private bool
        is_rotate_on_x = false,
        is_rotate_on_y = false,
        is_rotate_on_z = false;

    private const float min_refresh_time = 0.015f;

    private Vector3 start_rotation;

    private bool is_enabled = false;

    private Rigidbody physics;
    private Transform cached_transform;

    private Vector3 
        torque = Vector3.zero,
        rotation = Vector3.zero;

    private WaitForSeconds physical_wait_for_seconds;
    private WaitForSeconds transformed_wait_for_seconds;
    
    // Starting initialization #################################################################################################################################################
    void Awake() {

        physics = GetComponent<Rigidbody>();
        cached_transform = GetComponent<Transform>();

        start_rotation = cached_transform.localEulerAngles;

        if( transformed_refresh_speed != 0 ) transformed_refresh_time = 1f / transformed_refresh_speed;
        else transformed_refresh_time = 0f;

        physical_wait_for_seconds = new WaitForSeconds( physical_refresh_time );
        transformed_wait_for_seconds = new WaitForSeconds( transformed_refresh_time );
    }

    // On enable object ########################################################################################################################################################
    void OnEnable() {

        if( speed_on_x != 0f ) is_rotate_on_x = true;
        if( speed_on_y != 0f ) is_rotate_on_y = true;
        if( speed_on_z != 0f ) is_rotate_on_z = true;

        is_enabled = true;

        if( (physics == null) || physics.isKinematic ) StartCoroutine( TransformedRefresh() );
        else StartCoroutine( PhysicalRefresh() );
    }
    
    // On disable object #######################################################################################################################################################
    void OnDisable() {

        is_enabled = false;

        if( restore_on_disable ) cached_transform.localEulerAngles = start_rotation;
    }

	// If isn't used physics ###################################################################################################################################################
	IEnumerator TransformedRefresh() {

        transformed_refresh_time = Time.time - Time.deltaTime;

        while( is_enabled ) {

            transformed_refresh_time = Time.time - transformed_refresh_time;

            rotation = Vector3.zero;
            
            if( is_rotate_on_x ) rotation.x = speed_on_x * transformed_refresh_time;
            if( is_rotate_on_y ) rotation.y = speed_on_y * transformed_refresh_time;
            if( is_rotate_on_z ) rotation.z = speed_on_z * transformed_refresh_time;

            cached_transform.localEulerAngles += rotation;

            transformed_refresh_time = Time.time;

            yield return transformed_wait_for_seconds;
        }

        yield break;
    }

	// If is used physics ######################################################################################################################################################
	IEnumerator PhysicalRefresh() {

        physics.angularVelocity = new Vector3( speed_on_x, speed_on_y, speed_on_z );

        if( physical_refresh_time == 0f ) yield break;

        while( is_enabled ) {

            torque = Vector3.zero;

            // Определяем, необходимо ли откорректировать вращение по оси X
            if( is_rotate_on_x ) {

                if( (speed_on_x < 0f) && (physics.angularVelocity.x > speed_on_x) ) torque.x = speed_on_x;
                else if( (speed_on_x > 0f) && (physics.angularVelocity.x < speed_on_x) ) torque.x = speed_on_x;
            }

            // Определяем, необходимо ли откорректировать вращение по оси Y
            if( is_rotate_on_y ) {

                if( (speed_on_y < 0f) && (physics.angularVelocity.y > speed_on_y) ) torque.y = speed_on_y;
                else if( (speed_on_y > 0f) && (physics.angularVelocity.y < speed_on_y) ) torque.y = speed_on_y;
            }

            // Определяем, необходимо ли откорректировать вращение по оси Z
            if( is_rotate_on_z ) {

                if( (speed_on_z < 0f) && (physics.angularVelocity.z > speed_on_z) ) torque.z = speed_on_z;
                else if( (speed_on_z > 0f) && (physics.angularVelocity.z < speed_on_z) ) torque.z = speed_on_z;
            }
            
            // Если есть необходимость в корректировке вращения, применяем момент силы
            if( torque != Vector3.zero ) physics.AddTorque( torque, ForceMode.Impulse );

            yield return physical_wait_for_seconds;
        }

        physics.angularVelocity = Vector3.zero;

        yield break;
    }
}