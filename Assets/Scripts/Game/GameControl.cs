using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class SubjectValue {

    public SubjectType Type = SubjectType.Unknown;

    [Tooltip( "Ключ названия типа объекта: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Name_key;

    [Tooltip( "Ключ описания типа объекта: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Description_key;
}

[System.Serializable]
public class MineralValue {

    [SerializeField]
    [Tooltip( "Тип минерала: стандартный, опасный, скоропортящийся, комбинированный" )]
    public SubjectType Subject_type = SubjectType.Unknown;

    public MineralType Type = MineralType.Unknown;

    [Tooltip( "Ключ названия вида объекта (например, <Минерал> или <Неизвестная субстанция>): оригинальное значение в базе локализаций нужно писать с большой буквы" )]
    public string Kind_key;

    [Tooltip( "Ключ названия минерала: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Name_key;

    [Tooltip( "Ключ описания минерала: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Description_key;

    [Tooltip( "Базовая цена продажи за килограмм данного минерала" )]
    public float Price_per_kilo = 0f;

    [Tooltip( "Повышение уровня опыта за проданный минерал" )]
    public float Experience_increase = 0f;
}

[System.Serializable]
public class FreightValue {

    [SerializeField]
    [Tooltip( "Тип груза: стандартный, опасный, скоропортящийся, комбинированный" )]
    public SubjectType Subject_type = SubjectType.Unknown;

    public FreightType Type = FreightType.Unknown;

    [Tooltip( "Ключ названия груза: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Name_key;

    [Tooltip( "Ключ описания груза: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Description_key;

    [Tooltip( "Базовая цена продажи за килограмм данного вида груза" )]
    public float Price_per_kilo = 0f;

    [Tooltip( "Повышение уровня опыта за проданный груз" )]
    public float Experience_increase = 0f;
}

[System.Serializable]
public class MissionValue {

    public MissionType Type = MissionType.Unknown;

    [Tooltip( "Ключ названия миссии: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Name_key;

    [Tooltip( "Ключ описания миссии: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Description_key;

    [Tooltip( "Размер награды за выполнение миссии" )]
    public float Award = 0f;

    [Tooltip( "Повышение уровня опыта за выполнение миссии" )]
    public float Experience_increase = 0f;
}

[System.Serializable]
public class ActValue {

    public ActType Type = ActType.Unknown;

    [Tooltip( "Ключ названия действия: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Name_key;

    [Tooltip( "Ключ описания действия: оригинальное значение в базе локализаций нужно писать с маленькой буквы" )]
    public string Description_key;

    [Tooltip( "Размер награды за произведённое действие" )]
    public float Award = 0f;

    [Tooltip( "Повышение уровня опыта за произведённое действие" )]
    public float Experience_increase = 0f;
}

[System.Serializable]
public class PowerValue {

    [SerializeField]
    [Tooltip( "Мощность радара, которой соответствует количество указанных ниже минералов" )]
    [Range( 5f, 100f )]
    public float Power = 10f;

    [SerializeField]
    [Tooltip( "Максимально возможный минерал, которые радар может обнаружить при данной мощности" )]
    public MineralType Max_mineral_type = MineralType.Unknown;
}

public class GameControl : MonoBehaviour {

    [System.Serializable]
    private class PhysicalPause {

        public Rigidbody[] physics;
        public Vector3[] velocity;
        public bool[] useGravity;
        public bool[] isKinematic;
        public bool[] isMoving;
        public bool[] isRotating;

        public Collider[] colliders;
        public bool[] enabled;

        public void InitializeRigidbody() {

            // Получаем все активные физические объекты
            physics = FindObjectsOfType<Rigidbody>();
            // Если этот способ будет работать медленно, его следует заменить на gameObject.GetComponentsInChildren<Rigidbody>( true );
            // Кроме того, данным вариантом можно собрать компоненты также и у неактивных объектов

            velocity = new Vector3[ physics.Length ];
            useGravity = new bool[ physics.Length ];
            isKinematic = new bool[ physics.Length ];
            isMoving = new bool[ physics.Length ];
            isRotating = new bool[ physics.Length ];
        }

        public void InitializeColliders() {

            // Получаем все коллайдеры у активных объектов
            colliders = FindObjectsOfType<Collider>();
            // Если этот способ будет работать медленно, его следует заменить на gameObject.GetComponentsInChildren<Collider>( true );
            // Кроме того, данным вариантом можно собрать компоненты также и у неактивных объектов

            enabled = new bool[ colliders.Length ];
        }

        public void Pause( bool total_pause ) {

            InitializeRigidbody();

            ObstacleControl obstacle;

            for( int i = 0; i < physics.Length; i++ ) {

                obstacle = physics[i].GetComponent<ObstacleControl>();

                velocity[i] = physics[i].velocity;
                physics[i].velocity = Vector3.zero;

                useGravity[i] = physics[i].useGravity;
                physics[i].useGravity = physics[i].useGravity;
                isKinematic[i] = physics[i].isKinematic;
                isMoving[i] = ((obstacle != null) && obstacle.Is_moving) ? obstacle.DisableMovement() : false;
                isRotating[i] = ((obstacle != null) && obstacle.Is_rotating) ? obstacle.DisableRotation() : false;

                if( total_pause ) physics[i].isKinematic = true;
                else physics[i].isKinematic = ((obstacle != null) && obstacle.Is_non_kinematic ) ? false : true;
            }
        }

        public void Resume() {

            if( physics == null ) return;

            ObstacleControl obstacle;

            for( int i = 0; i < physics.Length; i++ ) {

                obstacle = physics[i].GetComponent<ObstacleControl>();

                physics[i].velocity = velocity[i];
                physics[i].useGravity = useGravity[i];
                physics[i].isKinematic = isKinematic[i];
                if( (obstacle != null) && obstacle.Is_moving && isMoving[i] ) obstacle.EnableMovement();
                if( (obstacle != null) && obstacle.Is_rotating && isRotating[i] ) obstacle.EnableRotation();
            }

            physics = null;
        }
    }

    private PhysicalPause physical_pause = new PhysicalPause();
    public void PauseKinematic() { physical_pause.Pause( false ); Game.AddState( GameState.Paused ); DisabeAudioExceptMusic(); }
    public void PauseKinematicTotal() { physical_pause.Pause( true ); Game.AddState( GameState.Paused ); DisabeAudioExceptMusic(); }
    public void ResumeKinematic() { physical_pause.Resume(); Game.ResetState( GameState.Paused ); EnabeAudioAll(); }

    [Header( "СПИСКИ ВСЕХ ИГРОВЫХ УРОВНЕЙ И КОРАБЛЕЙ" )]
    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Список всех игровых уровней игры" )]
    private Level[] level_prefabs;
    public Level[] Level_prefabs { get { return level_prefabs; } }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Список всех типов кораблей для игры" )]
    private Ship[] ship_prefabs;
    public Ship[] Ship_prefabs { get { return ship_prefabs; } }

    [Header( "СПИСОК СКОРОПОРТЯЩИХСЯ И ВЗРЫВООПАСНЫХ ГРУЗОВ" )]
    [SerializeField]
    private SubjectType[] perishable_loads;
    public SubjectType[] Perishable_loads { get { return perishable_loads; } }
    public bool IsPerishable( SubjectType type ) { for( int i = 0; i < perishable_loads.Length; i++ ) if( type == perishable_loads[i] ) return true; return false; }

    [SerializeField]
    private SubjectType[] explosive_loads;
    public SubjectType[] Explosive_loads { get { return explosive_loads; } }
    public bool IsExplosive( SubjectType type ) { for( int i = 0; i < explosive_loads.Length; i++ ) if( type == explosive_loads[i] ) return true; return false; }

    [Header( "СПИСОК ОСНОВНЫХ ОБЪЕКТОВ И ДЕЙСТВИЙ В ИГРЕ" )]
    [SerializeField]
    private SubjectValue[] subject_values;
    public string SubjectName( SubjectType type ) {
        for( int i = 0; i < subject_values.Length; i++ ) if( subject_values[i].Type == type ) return Game.Localization.GetTextValue( subject_values[i].Name_key ); return string.Empty; }
    public string SubjectDescription( SubjectType type ) {
        for( int i = 0; i < subject_values.Length; i++ ) if( subject_values[i].Type == type ) return Game.Localization.GetTextValue( subject_values[i].Description_key ); return string.Empty; }

    [SerializeField]
    [Space( 10 )]
    private MineralValue[] mineral_values;
    public MineralValue GetMineralValue( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return mineral_values[i]; return null; }
    public string MineralName( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return Game.Localization.GetTextValue( mineral_values[i].Name_key ); return string.Empty; }
    public string MineralDescription( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return Game.Localization.GetTextValue( mineral_values[i].Description_key ); return string.Empty; }
    public SubjectType MineralSubjectType( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return mineral_values[i].Subject_type; return SubjectType.Unknown; }
    public float MineralPricePerGram( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return mineral_values[i].Price_per_kilo * 0.001f; return 0f; }
    public float MineralPricePerKilo( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return mineral_values[i].Price_per_kilo; return 0f; }
    public float MineralPricePerTon( MineralType type ) {
        for( int i = 0; i < mineral_values.Length; i++ ) if( mineral_values[i].Type == type ) return mineral_values[i].Price_per_kilo * 1000f; return 0f; }

    [SerializeField]
    [Space( 10 )]
    private FreightValue[] freight_values;
    public FreightValue GetFreightValue( FreightType type ) {
        for( int i = 0; i < freight_values.Length; i++ ) if( freight_values[i].Type == type ) return freight_values[i]; return null; }
    public string FreightName( FreightType type ) {
        for( int i = 0; i < freight_values.Length; i++ ) if( freight_values[i].Type == type ) return Game.Localization.GetTextValue( freight_values[i].Name_key ); return string.Empty; }
    public string FreightDescription( FreightType type ) {
        for( int i = 0; i < freight_values.Length; i++ ) if( freight_values[i].Type == type ) return Game.Localization.GetTextValue( freight_values[i].Description_key ); return string.Empty; }
    public SubjectType FreightSubjectType( FreightType type ) {
        for( int i = 0; i < freight_values.Length; i++ ) if( freight_values[i].Type == type ) return freight_values[i].Subject_type; return SubjectType.Unknown; }

    [SerializeField]
    [Space( 10 )]
    private MissionValue[] mission_values;
    public string MissionName( MissionType type ) {
        for( int i = 0; i < mission_values.Length; i++ ) if( mission_values[i].Type == type ) return Game.Localization.GetTextValue( mission_values[i].Name_key ); return string.Empty; }
    public string MissionDescription( MissionType type ) {
        for( int i = 0; i < mission_values.Length; i++ ) if( mission_values[i].Type == type ) return Game.Localization.GetTextValue( mission_values[i].Description_key ); return string.Empty; }

    [SerializeField]
    [Space( 10 )]
    private ActValue[] act_values;
    public string ActName( ActType type ) {
        for( int i = 0; i < act_values.Length; i++ ) if( act_values[i].Type == type ) return Game.Localization.GetTextValue( act_values[i].Name_key ); return string.Empty; }
    public string ActDescription( ActType type ) {
        for( int i = 0; i < act_values.Length; i++ ) if( act_values[i].Type == type ) return Game.Localization.GetTextValue( act_values[i].Description_key ); return string.Empty; }

    [SerializeField]
    [Space( 10 )]
    private PowerValue[] power_values;
    public MineralType MaxMineralType( float radar_power ) {
        for( int i = 0; i < power_values.Length; i++ ) if( radar_power >= power_values[i].Power ) return power_values[i].Max_mineral_type; return MineralType.Unknown; }

    [Header( "ВЫБОР ТОПЛИВА И ЕГО СТОИМОСТИ" )]
    [SerializeField]
    [Tooltip( "Ключевой минерал, применяемый в игре в качестве основы для производства топлива; по умолчанию = Water" )]
    private MineralType fuel_mineral = MineralType.Water;

    [SerializeField]
    [Tooltip( "Коэффициент производства: во сколько раз готовое топливо дороже самого минерала; по умолчанию = 2" )]
    [Range( 1f, 10f )]
    private float production_rate = 2f;

    private float fuel_price_per_ton = 0f;
    public float Fuel_price_per_ton { get {

            if( fuel_price_per_ton != 0f ) return fuel_price_per_ton;
            else return (fuel_price_per_ton = MineralPricePerTon( fuel_mineral ) * production_rate);
    } }

    [Header( "СПИСОК ОБЩИХ СООБЩЕНИЙ ДЛЯ СТАНЦИЙ" )]
    [SerializeField]
    private ComplexMessage service_message;
    public ComplexMessage Service_message { get { return service_message; } }

    [SerializeField]
    private ComplexMessage trade_message;
    public ComplexMessage Trade_message { get { return trade_message; } }

    [SerializeField]
    private ComplexMessage upgrade_message;
    public ComplexMessage Upgrade_message { get { return upgrade_message; } }

    [SerializeField]
    private ComplexMessage landing_soft_message;
    public ComplexMessage Landing_soft_message { get { return landing_soft_message; } }

    [SerializeField]
    private ComplexMessage landing_hard_message;
    public ComplexMessage Landing_hard_message { get { return landing_hard_message; } }

    [SerializeField]
    private ComplexMessage landing_wrong_message;
    public ComplexMessage Landing_wrong_message { get { return landing_wrong_message; } }

    private float last_int_experience = 0f;
  
    private GameData game_data;

    [System.NonSerialized]
    private bool was_saved = false;

    [HideInInspector, SerializeField]
    private AudioSource sound_audio_source;
    public AudioSource Sound_audio_source { get { return sound_audio_source; } }

    [HideInInspector, SerializeField]
    private AudioSource voice_audio_source;
    public AudioSource Voice_audio_source { get { return voice_audio_source; } }

    [HideInInspector, SerializeField]
    private AudioSource music_audio_source;
    public AudioSource Music_audio_source { get { return music_audio_source; } }

    [HideInInspector, SerializeField]
    private AudioSource engine_audio_source;
    public AudioSource Engine_audio_source { get { return engine_audio_source; } }

    public void DisabeAudioSound() { if( sound_audio_source != null ) sound_audio_source.mute = true; }
    public void DisabeAudioVoice() { if( voice_audio_source != null ) voice_audio_source.mute = true; }
    public void DisabeAudioMusic() { if( music_audio_source != null ) music_audio_source.mute = true; }
    public void DisabeAudioEngine() { if( engine_audio_source != null ) engine_audio_source.mute = true; }
    public void DisabeAudioAll() { DisabeAudioSound(); DisabeAudioVoice(); DisabeAudioMusic(); DisabeAudioEngine(); }
    public void DisabeAudioExceptMusic() { DisabeAudioSound(); DisabeAudioVoice(); DisabeAudioEngine(); }

    public void EnabeAudioSound() { if( sound_audio_source != null ) sound_audio_source.mute = false; }
    public void EnabeAudioVoice() { if( voice_audio_source != null ) voice_audio_source.mute = false; }
    public void EnabeAudioMusic() { if( music_audio_source != null ) music_audio_source.mute = false; }
    public void EnabeAudioEngine() { if( engine_audio_source != null ) engine_audio_source.mute = false; }
    public void EnabeAudioAll() { EnabeAudioSound(); EnabeAudioVoice(); EnabeAudioMusic(); EnabeAudioEngine(); }
    public void EnabeAudioExceptMusic() { EnabeAudioSound(); EnabeAudioVoice(); EnabeAudioEngine(); }

    public bool Global_sound_enabled { get { return (sound_audio_source != null) ? sound_audio_source.mute : false; } }
    public bool Global_voice_enabled { get { return (voice_audio_source != null) ? voice_audio_source.mute : false; } }
    public bool Global_music_enabled { get { return (music_audio_source != null) ? music_audio_source.mute : false; } }

    private float
        screen_width = 0f,
        screen_height = 0f,
        half_screen_width = 0f,
        half_screen_height = 0f;

    public float Screen_width { get { return screen_width; } }
    public float Screen_height { get { return screen_height; } }
    public float Half_screen_width { get { return half_screen_width; } }
    public float Half_screen_height { get { return half_screen_height; } }
            
    private float 
        screen_rate_x = 0f,
        screen_rate_y = 0f,
        target_screen_resolution_x = 0f,
        target_screen_resolution_y = 0f;

    public float Screen_rate_x { get { return screen_rate_x; } }
    public float Screen_rate_y { get { return screen_rate_y; } }
    public float Target_screen_resolution_x { get { return target_screen_resolution_x; } }
    public float Target_screen_resolution_y { get { return target_screen_resolution_y; } }

    private WaitForSeconds update_wait_for_seconds = new WaitForSeconds( 0.3f );
   
    // Use this for initialization #############################################################################################################################################
    private void Awake() {

        if( !Application.isEditor ) Game.Is_genuine = Application.genuine;

        // Здесь необходимо воспользоваться медленным методом, чтобы он одинаково работал и в главном меню, и на игровых уровнях
        CanvasScaler canvas_scaler = GameObject.FindObjectOfType<CanvasScaler>();
        target_screen_resolution_x = canvas_scaler.referenceResolution.x;
        target_screen_resolution_y = canvas_scaler.referenceResolution.y;

        screen_rate_x = Screen.width / target_screen_resolution_x;
        screen_rate_y = Screen.height / target_screen_resolution_y;

        screen_width = Screen.width;
        screen_height = Screen.height;

        half_screen_width = screen_width * 0.5f;
        half_screen_height = screen_height * 0.5f;
        
        #if UNITY_IOS
        Screen.orientation = ScreenOrientation.Landscape;
        Handheld.SetActivityIndicatorStyle( UnityEngine.iOS.ActivityIndicatorStyle.Gray );
        Handheld.StartActivityIndicator();
        #elif UNITY_ANDROID
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Handheld.SetActivityIndicatorStyle( AndroidActivityIndicatorStyle.Small );
        Handheld.StartActivityIndicator();
        #endif

        // Получаем сохранённое игровое состояние
        game_data = new GameData();
        LoadGame.Load( game_data );

        Game.Localization.ChangeLanguage( Game.Language );

        Game.ResetSumCalculation();
        Game.RefreshLocalizedUnits();
        Game.RefreshLocalizedPhrases();
        Game.RefreshLocalizedSeparators();

        sound_audio_source = GameObject.Find( "Game_sound" ).GetComponent<AudioSource>();
        voice_audio_source = GameObject.Find( "Game_voice" ).GetComponent<AudioSource>();
        music_audio_source = GameObject.Find( "Game_music" ).GetComponent<AudioSource>();
        engine_audio_source = GameObject.Find( "Game_effects" ).GetComponent<AudioSource>();

        ChangeSoundVolume();
    }

	// Use this for initialization #############################################################################################################################################
	private void Start() {

        if( !Application.isEditor && !Game.Is_genuine ) QuitGame();

        last_int_experience = Mathf.Floor( Game.Experience );

        // Если это не игровой уровень, там нет игрока, поэтому обходим методы, связанные с игроком
        if( Game.Current_level > LevelType.Level_Menu ) {

            // Регулируем управление исходя из загруженных настроек
            if( Game.Use_vertical_control ) Game.Input_control.EnableVerticalControl();
            else Game.Input_control.DisableVerticalControl();

            if( Game.Use_horizontal_control ) Game.Input_control.EnableHorizontalControl();
            else Game.Input_control.DisableHorizontalControl();
        
            // Запускаем контроль показателей корабля игрока
            engine_audio_source = GameObject.Find( "Player" ).GetComponentInChildren<Ship>().GetComponent<AudioSource>();
            StartCoroutine( UpdateCheck() );

            // Запускаем генератор случайных объектов
            StartCoroutine( UpdateGenerator() );
        }
	}

    // Здесь метод переинициализирует глобальные экранные параметры, и необходим на случай изменения размеров окна #############################################################
    #if UNITY_EDITOR
    private void Update() {

        // На случай динамического изменения размера окна (актуально для режима редактора, а также для оконного режима Windows)
        // Если будет необходим оконный режим Windows, то нужно завести по паре переменных для ширины и высоты: одна - копия, другая - инверсия
        // Если копии совпадают, то умножаем на инверсию копии; если нет, то делим на текущие Screen.width или Screen.height

        if( screen_width != Screen.width ) {

            screen_width = Screen.width;
            half_screen_width = screen_width * 0.5f;
            screen_rate_x = target_screen_resolution_x / screen_width;
        }

        if( screen_height != Screen.height ) {

            screen_height = Screen.height;
            half_screen_height = screen_height * 0.5f;
            screen_rate_y = target_screen_resolution_y / screen_height;
        }
    }
    #endif

    // Более редкая проверка на качество ключевых индикаторов корабля ##########################################################################################################
    private IEnumerator UpdateCheck() {

        while( !Game.Is( GameState.Complete ) ) {

            // При обнулени одного из ключевых индикаторов, если корабль не на станции, он уничтожается (рестарт уровня); исключение - топливо, его можно заправить
            if( !Game.Player.At_station && (Game.Player.Ship.Fuel_capacity.Available <= Game.Player.Ship.Fuel_capacity.Restart_limit) ) Game.Player.DestroyShip( Vector3.zero );
            else if( Game.Player.Ship.Hull_durability.Available <= Game.Player.Ship.Hull_durability.Restart_limit ) Game.Player.DestroyShip( Vector3.zero );
            else if( Game.Player.Ship.Engine_thrust.Available <= Game.Player.Ship.Engine_thrust.Restart_limit ) Game.Player.DestroyShip( Vector3.zero );

            // Если достигнут новый уровень опыта, игрок получает бонус
            if( (Game.Experience - last_int_experience) >= 1f ) {

                ExperienceAward( Mathf.FloorToInt( last_int_experience ) );

                last_int_experience = Mathf.Floor( Game.Experience );
            }

            // Если топливо закончилось на станции при наличии утечки, то необходимо прервать визуальный эффект утечки
            // При появлени топлива в баках, и если утечка ещё не устранена, эффект снова возобновляется
            if( Game.Player.Has_fuel_leaks && Game.Player.At_station ) {
             
                if( (Game.Player.Ship.Fuel_capacity.Available == 0f ) && Game.Player.Leaks_effect_is_playing ) Game.Player.StopLeaksEffect();
                else if( (Game.Player.Ship.Fuel_capacity.Available > 0f ) && !Game.Player.Leaks_effect_is_playing ) Game.Player.ResumeLeaksEffect();
            }

            yield return update_wait_for_seconds;
        }

        yield break;
    }

    // Генератор случайных объектов ############################################################################################################################################
    private IEnumerator UpdateGenerator() {

        yield break;
    }

    // Change the game sound's volume ##########################################################################################################################################
    public void ChangeSoundVolume() {

        sound_audio_source.volume = Game.Sound_volume;
        voice_audio_source.volume = Game.Voice_volume;
        music_audio_source.volume = Game.Music_volume;
        engine_audio_source.volume = Game.Engine_volume;
    }

    // Play this message #######################################################################################################################################################
    public void PlayAudioMessage( ComplexMessage message ) {

        if( message.Sound_key != null ) {

            if( sound_audio_source.isPlaying ) sound_audio_source.Stop();
            sound_audio_source.clip = Game.Localization.GetAudioClip( message.Sound_key );
            sound_audio_source.volume = Game.Sound_volume;
            sound_audio_source.Play();
        }

        if( message.Voice_key != null ) {

            if( voice_audio_source.isPlaying ) voice_audio_source.Stop();
            voice_audio_source.clip = Game.Localization.GetAudioClip( message.Voice_key );
            voice_audio_source.volume = Game.Voice_volume;
            voice_audio_source.Play();
        }
    }

    // Stop this message #######################################################################################################################################################
    public void StopAudioMessage( ComplexMessage message ) {

        if( sound_audio_source.clip == Game.Localization.GetAudioClip( message.Sound_key ) ) sound_audio_source.Stop();
        if( voice_audio_source.clip == Game.Localization.GetAudioClip( message.Voice_key ) ) voice_audio_source.Stop();
    }

    // Check for playing of this message's sound ###############################################################################################################################
    public bool IsPlayingSound( ComplexMessage message ) {

        if( sound_audio_source.clip == Game.Localization.GetAudioClip( message.Sound_key ) ) return sound_audio_source.isPlaying;
        else return false;
    }

    // Check for playing of this message's sound ###############################################################################################################################
    public bool IsPlayingVoice( ComplexMessage message ) {

        if( voice_audio_source.clip == Game.Localization.GetAudioClip( message.Voice_key ) ) return voice_audio_source.isPlaying;
        else return false;
    }

    // Returns a price for the specified mineral ###############################################################################################################################
    public float GetMineralPricePerKilo( MineralType type ) {

        for( int i = 0; i < mineral_values.Length; i++ )
            if( mineral_values[i].Type == type )
                return mineral_values[i].Price_per_kilo;

        return 0f;
    }
    
    // Returns a price for the specified freight ###############################################################################################################################
    public float GetFreightPricePerKilo( FreightType type ) {

        for( int i = 0; i < freight_values.Length; i++ )
            if( freight_values[i].Type == type )
                return freight_values[i].Price_per_kilo;

        return 0f;
    }

    // Метод вручения бонуса игроку за достижение нового уровня опыта ##########################################################################################################
    private void ExperienceAward( int experience_level ) {

        #if UNITY_EDITOR
        Debug.Log( "Метод получения приза игроком: пока пустой" );
        #endif
    }

    // Quit game ###############################################################################################################################################################
    private void QuitGame() {

        Application.Quit();
    }
    
    // Сохранение данных перед выходом из игры #################################################################################################################################
    private void OnApplicationQuit() {

        if( was_saved ) return;
        else was_saved = true;
        
        // Сохраняется только конфигурация: уровни и корабли сохраняются либо в главном меню, либо на уровне (на уровне есть проверки, нужно ли сохранять или нет)
        SaveGame.Save( game_data );
    }
    
    // Сохранение данных перед загрузкой уровня ################################################################################################################################
    private void OnDisable() {

        if( was_saved ) return;
        else was_saved = true;

        // Сохраняется только конфигурация: уровни и корабли сохраняются либо в главном меню, либо на уровне (на уровне есть проверки, нужно ли сохранять или нет)
        SaveGame.Save( game_data );
    }
}