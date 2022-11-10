using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum NavigatorMode {

    Off,
    Station,
    Mission
}

public class CanvasNavigator : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Использовать ли изображения основного кольца вокруг корабля при включении навигатора (иначе будут отображаться только стрелки и расстояния до объектов)" )]
    private bool use_main_ring = true;
    public bool Use_main_ring { get { return use_main_ring; } }

    [SerializeField]
    [Tooltip( "Использовать ли изображения дополнительного кольца вокруг корабля при включении навигатора (дополнительное кольцо включается при обнаружении кросс-объектов)" )]
    private bool use_cross_ring = true;
    public bool Use_cross_ring { get { return use_cross_ring; } }

    [SerializeField]
    [Tooltip( "Минимальное расстояние между кораблём и фронтальным опасным объектом, на котором возникает предупреждение о фронтальной опасности (чтобы радар не реагировал на небольшие физические смещения по оси z); по умолчанию = 1" )]
    [Range( 0.5f, 2.0f )]
    private float min_cross_distance = 1f;

    [SerializeField]
    private string
        off_mode_key,
        station_mode_key,
        mission_mode_key;

    [SerializeField]
    [Tooltip( "Список всех станций уровня: прикреплён к групповому объекту, объединяющему все станции" )]
    private ListStations list_stations;

    [SerializeField]
    private GameObject panel_navigator;
    public GameObject Panel_navigator { get { return panel_navigator; } }

    [SerializeField]
    private GameObject main_ring;
    public GameObject Main_ring { get { return main_ring; } }

    [SerializeField]
    private GameObject cross_ring;
    public GameObject Cross_ring { get { return cross_ring; } }

    [SerializeField]
    private GameObject target_pointer;
    public GameObject Target_pointer { get { return target_pointer; } }

    [SerializeField]
    private GameObject radar_pointer;
    public GameObject Radar_pointer { get { return radar_pointer; } }

    [SerializeField]
    private Transform
        player_target_transform,
        player_radar_transform;

    [SerializeField]
    private RectTransform
        main_ring_rect_transform,
        cross_ring_rect_transform,
        target_arrow_rect_transform,
        radar_arrow_rect_transform;

    [SerializeField]
    private Sprite 
        arrow_sprite,
        arc_sprite;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Скорость обновления элементов навигатора на экране: сколько раз в секунду рассчитывается поворот стрелок; по умолчанию = 30; если ноль, то работает в режиме обычного Update()" )]
    [Range( 0, 60 )]
    private int update_speed = 30;

    [SerializeField]
    [Tooltip( "Скорость сканирования: сколько раз в секунду сканируется пространство вокруг корабля (с такой же частотой изменяется расстояние на экране); по умолчанию = 10" )]
    [Range( 5, 30 )]
    private int scanning_speed = 10;

    [Space( 10 )]
    [SerializeField]
    private EffectiveText text_navigator_mode;

    [SerializeField]
    private EffectiveText text_target_distance;

    [SerializeField]
    private EffectiveText text_radar_distance;

    private bool has_target = false;
    public bool Has_target { get { return has_target; } }

    private IDetecting
        detecting_target,
        detecting_object;

    private RectTransform
        target_pointer_rect_transform,
        radar_pointer_rect_transform;

    private Transform
        text_target_transform,
        text_radar_transform;

    private const float 
        target_distance = 0f,
        radar_distance = 0f;
        
    private RectTransform rect_transform;
    public RectTransform Rect_transform { get { return rect_transform; } }

    private Ship ship;

    private Vector2
        distance,
        screen_rate,
        target_screen_resolution;

    private Vector3 operation = new Vector3( 0f, 0f, 0f );
    private Vector3 orientation = new Vector3( 0f, 0f, 0f );

    private WaitForSeconds refresh_targets_wait_for_seconds;
    private WaitForSeconds update_arrows_wait_for_seconds;

    private float update_time = 0f;

    private bool is_enabled = false;
    public bool Is_enabled { get { return is_enabled; } }

    public void Enable() {

        has_target = false;
        is_enabled = (current_mode != NavigatorMode.Off);

        target_pointer.SetActive( false );
        if( !Game.Radar.Is_enabled ) radar_pointer.SetActive( false );

        main_ring.SetActive( use_main_ring ? (is_enabled || Game.Radar.Is_enabled) : false );
        cross_ring.SetActive( false );

        RefreshNavigatorScale();

        panel_navigator.SetActive( is_enabled || Game.Radar.Is_enabled );
    }

    public void Disable() {

        has_target = false;
        is_enabled = false;

        target_pointer.SetActive( false );

        main_ring.SetActive( use_main_ring ? (is_enabled || Game.Radar.Is_enabled) : false );
        cross_ring.SetActive( false );

        panel_navigator.SetActive( is_enabled || Game.Radar.Is_enabled );
    }

    private NavigatorMode current_mode = NavigatorMode.Off;
    public NavigatorMode Current_mode { get { return current_mode; } }

    public string Mode_name { get {

            return (current_mode == NavigatorMode.Off) ? 
                Game.Localization.GetTextValue( off_mode_key ) : ((current_mode == NavigatorMode.Station) ? 
                Game.Localization.GetTextValue( station_mode_key ) : 
                Game.Localization.GetTextValue( mission_mode_key ) );
        }
    }

    // Starting initialization #################################################################################################################################################
    void Start() {

        ship = Game.Player.Ship;

        cross_ring.SetActive( false );

        text_target_transform = text_target_distance.Text_component.transform;
        text_radar_transform = text_radar_distance.Text_component.transform;

        target_screen_resolution.x = GetComponent<CanvasScaler>().referenceResolution.x;
        target_screen_resolution.y = GetComponent<CanvasScaler>().referenceResolution.y;

        screen_rate.x = target_screen_resolution.x / Screen.width;
        screen_rate.y = target_screen_resolution.y / Screen.height;

        rect_transform = panel_navigator.GetComponent<RectTransform>();

        target_pointer_rect_transform = target_pointer.GetComponent<RectTransform>();
        radar_pointer_rect_transform = radar_pointer.GetComponent<RectTransform>();

        refresh_targets_wait_for_seconds = new WaitForSeconds( 1f / ((float) scanning_speed) );
        StartCoroutine( RefreshTargets() );

        update_time = 1f / ((float) update_speed);
        update_arrows_wait_for_seconds = new WaitForSeconds( update_time );
        StartCoroutine( UpdateCoroutine() );
    }

    // Update the navigator position ###########################################################################################################################################
    IEnumerator UpdateCoroutine() {

        while( !Game.Is( GameState.Complete ) ) {

            // Корректируем расход топлива для включённого радара (если игра на паузе, временно расход не считается)
            if( Game.Radar.Is_enabled && !Game.Is( GameState.Paused ) )
                Game.Player.CalculateFuelReserve( update_time, ship.Fuel_radar_usage * ship.Radar_power.Available * ship.Radar_power.Upgrade_max_game_inversed );

            // Если вдруг топливо закончилось, радар временно перестаёт находить цели
            if( ship.Fuel_capacity.Available <= 0f ) Game.Radar.Pause();

            // НАВИГАТОР /////////////////////////////////////////////////////////////////
            // Правильно сориентировать указатель навигатора, если он включён и имеет цель
            // ///////////////////////////////////////////////////////////////////////////
            if( is_enabled && has_target ) {

                player_target_transform.LookAt( detecting_target.Detected_point );

                if( Mathf.Abs( player_target_transform.eulerAngles.y ) <= 180f ) operation.z = (- player_target_transform.eulerAngles.x) - 90f;
                else operation.z = player_target_transform.eulerAngles.x + 90f;

                target_pointer_rect_transform.eulerAngles = operation;

                if( ((operation.z <= -90f) && (operation.z >= -180f )) || ((operation.z >= 90f) && (operation.z <= 180f )) ) orientation.z = 180f;
                else orientation.z = 0f;
                text_target_transform.localEulerAngles = orientation;

                // Включаем указатель навигатора
                if( !target_pointer.activeInHierarchy ) target_pointer.SetActive( true );
            }

            // Если навигатор выключен или не имеет цели, отключить указатель навигатора
            else if( target_pointer.activeInHierarchy ) target_pointer.SetActive( false );

            // РАДАР /////////////////////////////////////////////////////////////////
            // Правильно сориентировать указатель радара, если он включён и имеет цель
            // ///////////////////////////////////////////////////////////////////////
            if( Game.Radar.Is_enabled && Game.Radar.Has_target ) {

                player_radar_transform.LookAt( detecting_object.Detected_point );

                if( Mathf.Abs( player_radar_transform.eulerAngles.y ) <= 180f ) operation.z = (- player_radar_transform.eulerAngles.x) - 90f;
                else operation.z = player_radar_transform.eulerAngles.x + 90f;

                radar_pointer_rect_transform.eulerAngles = operation;

                if( ((operation.z <= -90f) && (operation.z >= -180f )) || ((operation.z >= 90f) && (operation.z <= 180f )) ) orientation.z = 180f;
                else orientation.z = 0f;
                text_radar_transform.localEulerAngles = orientation;

                // Если обнаружена зона, делаем указатель в виде сектора дуги; если объект - в виде стрелки
                if( detecting_object.Is_zone ) Game.Radar.Image_radar_arrow.sprite = arc_sprite;
                else Game.Radar.Image_radar_arrow.sprite = arrow_sprite;

                // Включаем указатель радара
                if( !radar_pointer.activeInHierarchy ) radar_pointer.SetActive( true );
            }

            // Если радар выключен или не имеет цели, отключить показ указателя радара и кросс-кольца
            else if( radar_pointer.activeInHierarchy ) radar_pointer.SetActive( false );

            yield return update_arrows_wait_for_seconds;
        }

        yield break;
    }

    // Update the navigator position ###########################################################################################################################################
    IEnumerator RefreshTargets() {

        while( !Game.Is( GameState.Complete ) ) {

            // НАВИГАТОР //////////////////////////////////////////////////////
            // Обнаружить ближайшую цель для навигатора, если навигатор включён
            // ////////////////////////////////////////////////////////////////
            if( is_enabled ) {

                detecting_target = DetectNearestTarget();

                if( (detecting_target != null) && !detecting_target.Is_visible ) {

                    if( !text_target_distance.Text_component.enabled ) text_target_distance.Text_component.enabled = true;
                    text_target_distance.RewriteDottedFloat( detecting_target.Magnitude * 0.01f, 2 ).Append( Game.Separator_space ).Append( Game.Unit_distance_km );
                }

                else {

                    if( text_target_distance.Text_component.enabled ) text_target_distance.Text_component.enabled = false;
                }
            }
            
            // РАДАР //////////////////////////////////////////////////
            // Обнаружить ближайшую цель для радара, если радар включён
            // ////////////////////////////////////////////////////////
            if( Game.Radar.Is_enabled ) {

                detecting_object = Game.Radar.DetectNearestObject();

                if( (detecting_object != null) && !detecting_object.Is_visible ) {

                    if( !text_radar_distance.Text_component.enabled ) text_radar_distance.Text_component.enabled = true;
                    text_radar_distance.RewriteDottedFloat( detecting_object.Magnitude * 0.01f, 2 ).Append( Game.Separator_space ).Append( Game.Unit_distance_km );
                }

                else {

                    if( text_radar_distance.Text_component.enabled ) text_radar_distance.Text_component.enabled = false;
                }
            }

            yield return refresh_targets_wait_for_seconds;
        }

        yield break;
    }

    // Меняет визуальный радиус колец навигатора ###############################################################################################################################
    public void SetRingRadius( float main_radius, float main_childs_radius, float cross_radius, float cross_childs_radius, float arrows_radius ) {

        // Устанавливаем новые размеры главного кольца и его сегментов
        main_ring_rect_transform.sizeDelta = new Vector2( main_radius, main_radius );
        for( int i = 0; i < main_ring_rect_transform.childCount; i++ ) main_ring_rect_transform.GetChild( i ).GetComponent<RectTransform>().sizeDelta = new Vector2( main_childs_radius, main_childs_radius );

        // Устанавливаем новые размеры кросс-кольца и его сегментов
        cross_ring_rect_transform.sizeDelta = new Vector2( cross_radius, cross_radius );
        for( int i = 0; i < cross_ring_rect_transform.childCount; i++ ) cross_ring_rect_transform.GetChild( i ).GetComponent<RectTransform>().sizeDelta = new Vector2( cross_childs_radius, cross_childs_radius );

        // Устанавливаем расстояния для стрелок, указывающих на объекты
        target_arrow_rect_transform.anchoredPosition3D = new Vector3( target_arrow_rect_transform.anchoredPosition3D.x, arrows_radius, target_arrow_rect_transform.anchoredPosition3D.z );
        radar_arrow_rect_transform.anchoredPosition3D = new Vector3( target_arrow_rect_transform.anchoredPosition3D.x, arrows_radius, target_arrow_rect_transform.anchoredPosition3D.z );
    }

    // Choose the next navgation's mode ########################################################################################################################################
    public void SwitchToNextMode() {

        current_mode++;

        if( current_mode > NavigatorMode.Mission ) current_mode = NavigatorMode.Off;
    }

    // Set scale of the navigator ##############################################################################################################################################
    public void RefreshNavigatorScale() {

        operation.x = Game.Zoom_control.Screen_scale;
        operation.y = Game.Zoom_control.Screen_scale;
        operation.z = 1f;

        rect_transform.localScale = operation;
    }

    // Calculate position of the navigator panel ###############################################################################################################################
    public void RepositionNavigatorPanel() {

        if( !is_enabled && !Game.Radar.Is_enabled ) return;

        rect_transform.position = Game.Camera.WorldToScreenPoint( Game.Player.Navigator_point_transform.position );
    }

    // Detect the nearest target ###############################################################################################################################################
    private IDetecting DetectNearestTarget() {

        IDetecting target = null;

        if( current_mode == NavigatorMode.Station ) target = list_stations.DetectNearestStation();
        else if( current_mode == NavigatorMode.Mission ) target = Game.Scenario_control.DetectMissionPoint();

        // Если ближайший объект обнаружен, устанавливаем флаг наличия цели
        if( target == null ) has_target = false;
        else has_target = true;

        return target;
    }
}