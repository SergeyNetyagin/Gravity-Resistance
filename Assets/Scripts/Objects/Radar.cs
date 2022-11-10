using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum RadarMode {

    Off,
    Obstacle,
    Mineral
}

public interface IDetecting {

    bool Is_visible { get; }

    bool Is_zone { get; }
    bool Is_mineral { get; }
    bool Is_freight { get; }
    bool Is_obstacle { get; }
    bool Is_station { get; }
    bool Is_mission { get; }

    Transform Cached_transform { get; }

    int Layer_original { get; }

    float Radius { get; }
    float Diameter { get; }

    ZoneType Zone_type { get; }
    MineralType Mineral_type { get; }
    ObstacleType Obstacle_type { get; }

    float Magnitude { get; set; }
    float Sqr_magnitude { get; set; }
    Vector3 Detected_point { get; set; }

    void CalculateDimensions();
}

public class Radar : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Обнаруживать ли опасные зоны наряду с опасными объектами (если нет, то радар не видит опасные зоны, видит только объекты и минералы)" )]
    private bool detect_zones = true;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Ссылка на стрелку радара; необходима для того, чтобы менять цвет в зависимости от режима радара: объект / минерал" )]
    private Image image_radar_arrow;
    public Image Image_radar_arrow { get { return image_radar_arrow; } }

    [SerializeField]
    [Tooltip( "Ссылка на текстовое поле, в котором обновляется расстояние до объекта / минерала" )]
    private Text text_radar_distance;

    [SerializeField]
    [Tooltip( "Цвет стрелки и текста в зависимости от режима работы радара" )]
    private Color
        object_mode_color = Color.red,
        mineral_mode_color = Color.cyan;

    public Color Object_mode_color { get { return object_mode_color; } }
    public Color Mineral_mode_color { get { return mineral_mode_color; } }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Коэффициент приведения дистанции обнаружения радара в км в правильные единицы Юнити: по умолчанию = 100" )]
    [Range( 10f, 1000f )]
    private float radius_rate = 100f;

    [SerializeField]
    private string
        off_mode_key,
        obstacle_mode_key,
        mineral_mode_key;

    private int layer_detecting = 0;

    private ZoneControl zone;
    private ObstacleControl obstacle;

    private ZoneType max_found_zone = ZoneType.Gamma_radiation;
    private MineralType max_found_mineral = MineralType.Methane;
    private ObstacleType max_found_obstacle = ObstacleType.Structure;

    private List<IDetecting> detecting_list = new List<IDetecting>();

    private SphereCollider 
        radar_collider;

    private float 
        available_radius,
        available_power;

    private bool has_target = false;
    public bool Has_target { get { return has_target; } }
    public void Pause() { has_target = false; }

    private bool is_enabled = false;
    public bool Is_enabled { get { return is_enabled; } }

    public void Enable() {

        has_target = false;
        is_enabled = (current_mode != RadarMode.Off);
        radar_collider.enabled = is_enabled;

        Game.Navigator.Radar_pointer.SetActive( false );
        if( !Game.Navigator.Is_enabled ) Game.Navigator.Target_pointer.SetActive( false );

        Game.Navigator.Main_ring.SetActive( Game.Navigator.Use_main_ring ? (is_enabled || Game.Navigator.Is_enabled) : false );
        Game.Navigator.Cross_ring.SetActive( false );

        Game.Navigator.RefreshNavigatorScale();

        Game.Navigator.Panel_navigator.SetActive( is_enabled || Game.Navigator.Is_enabled );
    }

    public void Disable() {

        has_target = false;
        is_enabled = false;
        radar_collider.enabled = is_enabled;

        detecting_list.Clear();

        radar_collider.enabled = is_enabled;

        Game.Navigator.Radar_pointer.SetActive( false );

        Game.Navigator.Main_ring.SetActive( Game.Navigator.Use_main_ring ? (is_enabled || Game.Navigator.Is_enabled) : false );
        Game.Navigator.Cross_ring.SetActive( false );

        Game.Navigator.Panel_navigator.SetActive( is_enabled || Game.Navigator.Is_enabled );
    }

    private RadarMode current_mode = RadarMode.Off;
    public RadarMode Current_mode { get { return current_mode; } }

    public string Mode_name { get {

            return (current_mode == RadarMode.Off) ? 
                Game.Localization.GetTextValue( off_mode_key ) : ((current_mode == RadarMode.Obstacle) ? 
                Game.Localization.GetTextValue( obstacle_mode_key ) : 
                Game.Localization.GetTextValue( mineral_mode_key ) );
        }
    }

    public void RefreshRange() { radar_collider.radius = available_radius = Game.Player.Ship.Radar_range.Available * radius_rate; }
    public void RefreshRower() { available_power = Game.Player.Ship.Radar_power.Available; }

    // Starting initialization #################################################################################################################################################
    void Awake() {

        layer_detecting = LayerMask.NameToLayer( "Detecting" );

        if( detect_zones ) {

            Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Radar" ), LayerMask.NameToLayer( "Zone" ), false );
            Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Ignore Raycast" ), LayerMask.NameToLayer( "Zone" ), false );
        }

        else {

            Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Radar" ), LayerMask.NameToLayer( "Zone" ), true );
            Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Ignore Raycast" ), LayerMask.NameToLayer( "Zone" ), true );
        }
    }
    
    // Initialize components ###################################################################################################################################################
	void Start() {

        available_radius = Game.Player.Ship.Radar_range.Available;
        available_power = Game.Player.Ship.Radar_power.Available;

        // Определяем радиус коллайдера в зависимости от текущей дистанции радара
        radar_collider = GetComponent<SphereCollider>();
        radar_collider.radius = available_radius * radius_rate;
        radar_collider.enabled = is_enabled;

        // Если от мощности радара зависят не только типы минералов, но и типы зон и препятствий, здесь нужно проинициализироватьэти параметры
        max_found_zone = ZoneType.Gamma_radiation;
        max_found_mineral = Game.Control.MaxMineralType( available_power );
        max_found_obstacle = ObstacleType.Structure;
    }

    // Collision processing ####################################################################################################################################################
    void OnTriggerEnter( Collider collider ) {

        IDetecting target = null;

        obstacle = collider.GetComponent<ObstacleControl>();

        // Если у объекта есть компонент <Obstacle>, то с зоной мы уже не работаем
        if( obstacle != null ) {
            
            if( obstacle.Is_obstacle ) target = obstacle;
        }

        // Иначе проверяем, что возможно это зона (если разрешено обнаружение зон)
        else if( detect_zones ) {

            zone = collider.GetComponent<ZoneControl>();
            if( (zone != null) && zone.Is_zone ) target = zone;
        }

        // Если найденный объект не подходит по параметрам, не включаем его в список
        if( target == null ) return;

        // Если объект уже попал в список, повторно его не помещаем
        if( detecting_list.Contains( target ) ) return;

        // Если объект прошёл полную проверку, меняем его слой и помещаем в список (зоны остаются в своём слое)
        if( !target.Is_zone ) collider.gameObject.layer = layer_detecting;
        detecting_list.Add( target );
    }

    // Collision processing ####################################################################################################################################################
    void OnTriggerExit( Collider collider ) {

        IDetecting target = null;

        obstacle = collider.GetComponent<ObstacleControl>();

        // Если у объекта есть компонент <Obstacle>, то с зоной мы уже не работаем
        if( obstacle != null ) {
            
            if( obstacle.Is_obstacle ) target = obstacle;
        }

        // Иначе проверяем, что возможно это зона (если разрешено обнаружение зон)
        else if( detect_zones ) {

            zone = collider.GetComponent<ZoneControl>();
            if( (zone != null) && zone.Is_zone ) target = zone;
        }

        // Если найденный объект не подходит по параметрам, мы его оставляем в покое
        if( target == null ) return;

        // Если объект уже был исключён из списка, не пытаемся повторно его исключить
        if( !detecting_list.Contains( target ) ) return;

        // Если объект прошёл полную проверку, восстанавливаем его слой и удаляем из списка
        collider.gameObject.layer = target.Layer_original;
        detecting_list.Remove( target );
    }
                
    // Detect the nearest danger ###############################################################################################################################################
    public IDetecting DetectNearestObject() {

        IDetecting body = null;
        IDetecting target = null;

        // Для повышения производительност при входе объекта в триггер он переназначается в слой <Detected>
        // По выходу из триггера у объекта восстанавливается их родной слой (<Default>, например, или другой, какой был у объекта изначально)
        // При этом здесь можно использовать Linecast( Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask, QueryTriggerInteraction queryTriggerInteraction );

        Vector2 distance;
        Vector2 position;

        Vector2 player_position;
        player_position.x = Game.Player_transform.position.x;
        player_position.y = Game.Player_transform.position.y;

        float magnitude = 0f;
        float min_magnitude = float.MaxValue;

        // Проверяем все объекты в списке радара
        for( int i = 0; i < detecting_list.Count; i++ ) {

            // Если объект вдруг оказался уничтоженным (например, защитой станции), его нужно исключить из списка
            if( detecting_list[i].Cached_transform == null ) {

                detecting_list.RemoveRange( i--, 1 );
                continue;
            }

            // Если объект вдруг стал неактивным, его тоже нужно исключить из списка, но вначале вернуть ему родной слой
            else if( !detecting_list[i].Cached_transform.gameObject.activeInHierarchy ) {

                detecting_list[i].Cached_transform.gameObject.layer = detecting_list[i].Layer_original;
                detecting_list.RemoveRange( i--, 1 );
                continue;
            }

            // Кэшируем текущий объект
            body = detecting_list[i];
            position = body.Cached_transform.position;

            // Если данный объект не подлежит регистрации (из-за настроек, характера цели или режима радара), берём следующий
            if( !detect_zones && body.Is_zone ) continue;
            if( body.Is_station || body.Is_mission ) continue;
            if( (current_mode == RadarMode.Mineral) && !body.Is_mineral ) continue;

            // Если найденный объект из-за характеристик радара не может быть зарегистрирован, берём следующий
            if( body.Is_zone && detect_zones && (body.Zone_type > max_found_zone) ) continue;
            if( body.Is_mineral && (body.Mineral_type > max_found_mineral) ) continue;
            if( body.Is_obstacle && (body.Obstacle_type > max_found_obstacle) ) continue;

            // Находим вектор разности положений между радаром и объектом
            distance.x = player_position.x - position.x;
            distance.y = player_position.y - position.y;

            // Отпределяем точное расстояние до края объекта через магнитуду и средний радиус объекта
            // Этот вариант более точный, но если нужно повысить производительность, можно определять через квадрат магнитуды
            magnitude = distance.magnitude - body.Radius - Game.Player.Ship.Radius;

            if( magnitude < 0f ) magnitude = 0f;

            if( magnitude < min_magnitude ) {

                min_magnitude = magnitude;

                target = body;
                target.Magnitude = magnitude;
                target.Sqr_magnitude = magnitude * magnitude;
            }
        }

        // Если ближайший объект обнаружен, уточняем положение ближайшей точки объекта к кораблю, и устанавливаем флаг наличия цели
        if( target != null ) {

            Vector3 point;
            RaycastHit hitInfo;

            if( Physics.Linecast( player_position, target.Cached_transform.position, out hitInfo, LayerMask.GetMask(), QueryTriggerInteraction.Collide ) ) point = hitInfo.point;
            else point = target.Cached_transform.position;

            target.Detected_point = point;
        }

        // Если ближайший объект обнаружен, устанавливаем флаг наличия цели
        if( target == null ) has_target = false;
        else has_target = true;

        return target;
    }

    // Choose the next radar's mode ############################################################################################################################################
    public void SwitchToNextMode() {

        current_mode++;

        if( current_mode > RadarMode.Mineral ) current_mode = RadarMode.Off;

        if( current_mode == RadarMode.Obstacle ) {

            image_radar_arrow.color = object_mode_color;
            text_radar_distance.color = object_mode_color;
        }

        else if( current_mode == RadarMode.Mineral ) {

            image_radar_arrow.color = mineral_mode_color;
            text_radar_distance.color = mineral_mode_color;
        }
    }
    
    // Remove from radar's list the specified target ###########################################################################################################################
    public void RemoveAsTarget( Transform object_transform ) {

        IDetecting target = null;

        obstacle = object_transform.GetComponent<ObstacleControl>();

        // Если у объекта есть компонент <Obstacle>, то с зоной мы уже не работаем
        if( obstacle != null ) {
            
            if( obstacle.Is_obstacle ) target = obstacle;
        }

        // Иначе проверяем, что возможно это зона (если разрешено обнаружение зон)
        else if( detect_zones ) {

            zone = object_transform.GetComponent<ZoneControl>();
            if( (zone != null) && zone.Is_zone ) target = zone;
        }

        // Если найденный объект не подходит по параметрам, мы его оставляем в покое
        if( target == null ) return;

        // Если объект уже был исключён из списка, не пытаемся повторно его исключить
        if( !detecting_list.Contains( target ) ) return;

        // Если объект прошёл полную проверку, восстанавливаем его слой и удаляем из списка
        object_transform.gameObject.layer = target.Layer_original;
        detecting_list.Remove( target );
    }
}