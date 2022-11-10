using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelData {

    public bool Use_brief = true;
    public bool Use_examination = true;
    public bool Use_training = true;

    public float Combo = 1f;
    public float Complication = 1f;

    public bool Starting_autolanding = false;
    public string Starting_station_key = "Station.Name.WindRose";

    public bool Is_opened = false;
    public bool Is_active = false;

    public LevelData() { }

    public void Load( Level level ) {

        level.SetUseBrief( Use_brief );
        level.SetUseExamination( Use_examination );
        level.SetUseTraining( Use_training );

        level.SetCombo( Combo );
        level.SetComplication( Complication );

        level.SetStartingAutolanding( Starting_autolanding );
        level.SetStartingStationKey( Starting_station_key );

        level.SetOpened( Is_opened );
        level.SetActive( Is_active );
    }

    public void Save( Level level ) {

        Use_brief = level.Use_brief;
        Use_examination = level.Use_examination;
        Use_training = level.Use_training;

        Combo = level.Combo;
        Complication = level.Complication;

        Starting_autolanding = level.Starting_autolanding;
        Starting_station_key = level.Starting_station_key;

        Is_opened = level.Is_opened;
        Is_active = level.Is_active;
    }
}

public class Level : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Использовать ли на данном уровне краткое описание уровня перед первым стартом уровня?" )]
    private bool use_brief = true;
    public bool Use_brief { get { return use_brief; } }
    public void SetUseBrief( bool state ) { use_brief = state; }

    [SerializeField]
    [Tooltip( "Использовать ли на данном уровне краткий тест на профессиональную пригодность перед первым стартом уровня (при прохождении теста игроку начисляется премия)?" )]
    private bool use_examination = true;
    public bool Use_examination { get { return use_examination; } }
    public void SetUseExamination( bool state ) { use_examination = state; }

    [SerializeField]
    [Tooltip( "Использовать ли на данном уровне обучающие подсказки?" )]
    private bool use_training = true;
    public bool Use_training { get { return use_training; } }
    public void SetUseTraining( bool state ) { use_training = state; }

    [Header( "КЛЮЧЕВЫЕ ПАРАМЕТРЫ УРОВНЯ" )]
    [SerializeField]
    [Tooltip( "Название (тип) данного уровня" )]
    private LevelType type;
    public LevelType Type { get { return type; } }
     
    [SerializeField]
    [Tooltip( "Рисунок данного уровня, который будет затем отображаться в меню (необходимы размеры 128 x 128 пикселей)" )]
    private Sprite picture;
    public Sprite Picture { get { return picture; } }

    [SerializeField]
    [Tooltip( "Ключ названия уровня (полное название уровня, например: <ЭКОНОМИЧЕСКАЯ ЗОНА 'ОКОЛОЗЕМНАЯ ОРБИТА'>)" )]
    private string type_key;
    public string Type_key { get { return type_key; } }

    [SerializeField]
    [Tooltip( "Ключ описания уровня: сжатое описание особенностей уровня в одну строку (не более 100 символов)" )]
    private string description_key;
    public string Description_key { get { return description_key; } }

    [SerializeField]
    [Tooltip( "Коэффициент сложности данного уровня (влияет как на повреждающие, так и на призовые факторы); по умолчанию = 1 для стартового уровня" )]
    [Range( 1f, 10f )]
    private float complication = 1f;
    public float Complication { get { return complication; } }
    public void SetComplication( float value ) { complication = value; }

    [SerializeField]
    [Tooltip( "Рекомендуемый для данного уровня тип корабля" )]
    private ShipType recommended_ship_type = ShipType.Ship_unknown;
    public ShipType Recommended_ship_type { get { return recommended_ship_type; } }

    [SerializeField]
    [Tooltip( "Стоимость открытия уровня (как назовём это: либо покупки лицензии для работы на уровне; либо стоимость перелёта)" )]
    private float opening_cost = 100000000f;
    public float Opening_cost { get { return opening_cost; } }

    [SerializeField]
    [Tooltip( "Максимально возможное значение комбо-коэффициента для данного уровня; по умолчанию = 2" )]
    [Range( 1f, 2f )]
    private float max_combo = 2f;
    public float Max_combo { get { return max_combo; } }
    [System.NonSerialized]
    private float max_combo_inversed = -1f;
    public float Max_combo_inversed { get { if( max_combo_inversed == -1f ) max_combo_inversed = 1f / (max_combo - 1f); return max_combo_inversed; } }

    [HideInInspector, SerializeField]
    private float combo = 1f;
    public float Combo { get { return combo; } }
    public void SetCombo( float value ) { combo = value; }

    [SerializeField]
    [Tooltip( "Шаг повышения комбо-коэффициента при каждом успешном выполнении задания; по умолчанию = 0.5" )]
    [Range( 0.05f, 0.2f )]
    private float combo_step = 0.05f;
    public float Combo_step { get { return combo_step; } }

    [SerializeField]
    [Tooltip( "Фраза для каждого отдельного уровня: выглядит в виде <Добро пожаловать на околоземную орбиту>, или <Добро пожаловать в окрестности Венеры> и т.п." )]
    private ComplexMessage welcome_level_message;
    public ComplexMessage Welcome_level_message { get { return welcome_level_message; } }

    [Header( "НАСТРОЙКИ, СВЯЗАННЫЕ С КОРАБЛЁМ ИГРОКА" )]
    [SerializeField]
    [Tooltip( "Начать уровень с автопосадки на станцию (или иначе корабль будет стоять на посадочной площадке станции)" )]
    private bool starting_autolanding = false;
    public bool Starting_autolanding { get { return starting_autolanding; } }
    public void SetStartingAutolanding( bool state ) { starting_autolanding = state; }

    [SerializeField]
    [Tooltip( "Ключ названия станции, на которой появляется корабль при старте уровня" )]
    private string starting_station_key = "Station.Name.WindRose";
    public string Starting_station_key { get { return starting_station_key; } }
    public void SetStartingStationKey( string key ) { starting_station_key = key; }

    [Header( "НАСТРОЙКИ ГРАВИТАЦИИ НА УРОВНЕ" )]
    [SerializeField]
    private GravityType gravity_type = GravityType.Stable;
    public GravityType Gravity_type { get { return gravity_type; } }

    [Space( 10 )]
    [SerializeField]
    private float normal_drag = 0.05f;

    [SerializeField]
    private float delta_drag = 0.2f;

    [Space( 10 )]
    [SerializeField]
    private Vector3 normal_gravity = new Vector3( 0f, -9.81f, 0f );

    private const float earth_gravity = 9.81f;
    private const float earth_gravity_inversed = 1f / earth_gravity;
    public float Gravity_force { get { return (- (normal_gravity.y * earth_gravity_inversed)); } }

    [SerializeField]
    private Vector3 delta_gravity = new Vector3( 0f, 5f, 0f );

    [SerializeField]
    [Range( 10f, 300f )]
    private float gravity_change_cycle = 10f;

    private Vector3 current_gravity = new Vector3( 0f, (- earth_gravity), 0f );

    [Header( "ССЫЛКИ НА ГРАНИЦЫ УРОВНЯ (ГРАНИЧНЫЕ КОЛЛАЙДЕРЫ)" )]
    [SerializeField]
    private Transform
        edge_top_transform; [SerializeField] private Transform
        edge_bottom_transform,
        edge_left_transform,
        edge_right_transform;

    public Transform Edge_top_transform { get { return edge_top_transform; } }
    public Transform Edge_bottom_transform { get { return edge_bottom_transform; } }
    public Transform Edge_left_transform { get { return edge_left_transform; } }
    public Transform Edge_right_transform { get { return edge_right_transform; } }

    private WaitForSeconds gravity_change_wait_for_seconds;

    private const float activation_offset = 5f;
        
    private float 
        activation_top_position,
        activation_bottom_position,
        activation_left_position,
        activation_right_position,
        activation_near_position,
        activation_far_position;

    public float Activation_top_position { get { return activation_top_position; } }
    public float Activation_bottom_position { get { return activation_bottom_position; } }
    public float Activation_left_position { get { return activation_left_position; } }
    public float Activation_right_position { get { return activation_right_position; } }
    public float Activation_near_position { get { return activation_near_position; } }
    public float Activation_far_position { get { return activation_far_position; } }

    [System.NonSerialized]
    private bool is_brief_mode = false;
    public bool Is_brief_mode { get { return is_brief_mode; } }
    public void SetBriefMode( bool state ) { is_brief_mode = state; }

    // Истина, если уровень был открыт игроком
    [System.NonSerialized]
    private bool is_opened = false;
    public bool Is_opened { get { return (type == LevelType.Level_00_Earth_Low_Orbit) ? true : is_opened; } }
    public void SetOpened( bool state ) { is_opened = state; was_changed = true; } 

    // Истина, если последний раз именно он использовался в игре или был выбран в главном меню
    [System.NonSerialized]
    private bool is_active = false;
    public bool Is_active { get { return is_active; } }
    public void SetActive( bool state ) { is_active = state; was_changed = true; }

    // Служит для предотвращения большого объёма записей уровней в главном меню (если в главном меню уровень не изменялся, его не нужно снова перезаписывать)
    [System.NonSerialized]
    private bool was_changed = false;
    public bool Was_changed { get { return was_changed; } }

	// Use this for initialization #############################################################################################################################################
	void Awake() {

    }
        
    // Use this for initialization #############################################################################################################################################
	void Start() {

        activation_top_position    = edge_top_transform.position.y - edge_top_transform.GetComponent<BoxCollider>().size.x - activation_offset;
        activation_bottom_position = edge_bottom_transform.position.y + edge_bottom_transform.GetComponent<BoxCollider>().size.x + activation_offset;
        activation_left_position   = edge_left_transform.position.x + edge_left_transform.GetComponent<BoxCollider>().size.x + activation_offset;
        activation_right_position  = edge_right_transform.position.x - edge_right_transform.GetComponent<BoxCollider>().size.x - activation_offset;
        activation_near_position   = Game.Camera_transform.position.z + Game.Camera.nearClipPlane - activation_offset;
        activation_far_position    = Game.Camera_transform.position.z + Game.Camera.farClipPlane + activation_offset;

        delta_drag = Mathf.Abs( delta_drag );
        delta_gravity.x = Mathf.Abs( delta_gravity.x );
        delta_gravity.y = Mathf.Abs( delta_gravity.y );
        delta_gravity.z = Mathf.Abs( delta_gravity.z );

        gravity_change_wait_for_seconds = new WaitForSeconds( gravity_change_cycle / Game.Level.Complication );

        if( Game.Current_level > LevelType.Level_Menu ) {

            // Настраиваем гравитацию
            if( gravity_type == GravityType.Unstable ) StartCoroutine( DestabilizeGravity() );
            else SetNormalGravity();

            // Загружаем данные о состоянии уровня
            LoadGame.Load( this );
        }
    }

    // Coroutine for changing of gravity parameters ############################################################################################################################
    IEnumerator DestabilizeGravity() {

        Vector3 random_gravity;

        while( gravity_type == GravityType.Unstable ) {

            if( !Game.Player.Is_autolanding ) {

                random_gravity.x = Random.Range( normal_gravity.x - delta_gravity.x, normal_gravity.x + delta_gravity.x );
                random_gravity.y = Random.Range( normal_gravity.y - delta_gravity.y, normal_gravity.y + delta_gravity.y );
                random_gravity.z = Random.Range( normal_gravity.z - delta_gravity.z, normal_gravity.z + delta_gravity.z );

                SetSpecialGravity( ref random_gravity, Random.Range( normal_drag - delta_drag, normal_drag + delta_drag ) );
            }

            yield return gravity_change_wait_for_seconds;
        }

        yield break;
    }

    // Set starting gravity parameters #########################################################################################################################################
    public void SetNormalGravity() {

        current_gravity = normal_gravity;
        UnityEngine.Physics.gravity = current_gravity;

        if( Game.Player != null ) {

            if( Game.Player.Physics != null ) Game.Player.Physics.drag = normal_drag;
            else Game.Player.GetComponent<Rigidbody>().drag = normal_drag;
        }
    }

    // Set starting gravity parameters #########################################################################################################################################
    public void SetSpecialGravity( ref Vector3 gravity, float drag ) {

        current_gravity = gravity;
        UnityEngine.Physics.gravity = gravity;

        if( Game.Player != null ) {

            if( Game.Player.Physics != null ) Game.Player.Physics.drag = drag;
            else Game.Player.GetComponent<Rigidbody>().drag = drag;
        }
    }

    // Сохранение состояния уровня перед выходов из уровня или из игры #########################################################################################################
    private void OnDisable() {

        if( Game.Current_level > LevelType.Level_Menu ) SaveGame.Save( this );
    }
}