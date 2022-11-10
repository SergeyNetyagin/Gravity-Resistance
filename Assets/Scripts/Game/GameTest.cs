using UnityEngine;
using System.Collections;

public class GameTest : MonoBehaviour {

    public bool Clear_saves = false;
    public bool Use_immortal_mode = false;

    #region GAME_VALUES
    [Header( "ТЕСТОВЫЕ ИГРОВЫЕ ПАРАМЕТЫ" )]
    public bool Use_game_testing_values = false;

        [Space( 10 )]
        public Language Language = Language.Autodetect;

        [Range( 0f, 1000000000000f )]
        public float Money = 150000000f;

        [Range( 0, 20 )]
        public float Experience = 0.9f;

        [Space( 10 )]
        [Range( 0f, 1f )]
        public float Sound_volume = 1f;
    
        [Range( 0f, 1f )]
        public float Music_volume = 1f;
    
        [Range( 0f, 1f )]
        public float Voice_volume = 1f;

        [Range( 0f, 1f )]
        public float Engine_volume = 1f;

    #endregion

    #region LEVEL_VALUES
    [Header( "ТЕСТОВЫЕ ПАРАМЕТЫ УРОВНЯ" )]
    public bool Use_level_testing_values = false;

        [Space( 10 )]
        public bool Use_brief = true;
        public bool Use_examination = true;
        public bool Use_training = true;

        [Range( 1f, 2f )]
        public float Combo = 1.5f;

        [Range( 1f, 10f )]
        public float Complication = 2f;
    
    #endregion

    #region SHIP_VALUES
    [Header( "ТЕСТОВЫЕ НАСТРОЙКИ ДЛЯ КОРАБЛЯ" )]
    public bool Use_ship_testing_values = false;

        [Space( 10 )]
        public bool test_hull = false;
        public Indicator Hull_durability;

        [Space( 10 )]
        public bool test_fuel = false;
        public Indicator Fuel_capacity;

        [Space( 10 )]
        public bool test_engine = false;
        public Indicator Engine_thrust;

        [Space( 10 )]
        public bool test_hold = false;
        public Value[] value_prefabs;
        public Indicator Hold_capacity;
        [System.NonSerialized] private Value[] values;

        [Space( 10 )]
        public bool test_shield_time = false;
        public Indicator Shield_time;

        [Space( 10 )]
        public bool test_shield_power = false;
        public Indicator Shield_power;

        [Space( 10 )]
        public bool test_charge_time = false;
        public Indicator Charge_time;

        [Space( 10 )]
        public bool test_radar_range = false;
        public Indicator Radar_range;

        [Space( 10 )]
        public bool test_radar_power = false;
        public Indicator Radar_power;

        [Space( 10 )]
        public bool test_autolanding = false;
        public Indicator Autolanding_amount;

    #endregion

    void Awake() {

    }

    void Start() {

        if( Use_ship_testing_values && test_hold ) StartCoroutine( LoadToHold() );
    }

    public void Load() {

        if( !enabled ) return;

        Game.Language = Language;
        Game.LoadMoney( Money );
        Game.LoadExperience( Experience );

        Game.Sound_volume = Sound_volume;
        Game.Music_volume = Music_volume;
        Game.Voice_volume = Voice_volume;
        Game.Engine_volume = Engine_volume;
    }

    public void Load( Level level ) {

        if( !enabled ) return;

        level.SetUseBrief( Use_brief );
        level.SetUseExamination( Use_examination );
        level.SetUseTraining( Use_training );

        level.SetCombo( Combo );
        level.SetComplication( Complication );
    }

    public void Load( Ship ship ) {

        if( !enabled ) return;

        if( test_hull ) ship.Hull_durability.CopyFrom( Hull_durability );
        if( test_fuel ) ship.Fuel_capacity.CopyFrom( Fuel_capacity );
        if( test_engine ) ship.Engine_thrust.CopyFrom( Engine_thrust );
        if( test_hold ) ship.Hold_capacity.CopyFrom( Hold_capacity );
        if( test_shield_time ) ship.Shield_time.CopyFrom( Shield_time );
        if( test_shield_power ) ship.Shield_power.CopyFrom( Shield_power );
        if( test_charge_time ) ship.Charge_time.CopyFrom( Charge_time );
        if( test_radar_range ) ship.Radar_range.CopyFrom( Radar_range );
        if( test_radar_power ) ship.Radar_power.CopyFrom( Radar_power );
        if( test_autolanding ) ship.Autolanding_amount.CopyFrom( Autolanding_amount );
    }

    // Загрузка тестовых грузов в трюм корабля #################################################################################################################################
    private IEnumerator LoadToHold() {

        // Создаём объекты из их префабов
        values = new Value[ value_prefabs.Length ];

        for( int i = 0; i < value_prefabs.Length; i++ ) {

            values[i] = Instantiate( value_prefabs[i].gameObject ).GetComponent<Value>();
            values[i].gameObject.name = value_prefabs[i].gameObject.name;
        }

        // Ждём, пока не инициализирована ссылка на корабль игрока
        while( Game.Player.Ship == null ) yield return null;

        // Полностью освобождаем трюм от грузов, если они там есть
        Game.Player.Ship.ClearHold();

        for( int i = 0; i < values.Length; i++ ) {

            Game.Player.Ship.LoadToHold( values[i] );
        }

        yield break;
    }
}
