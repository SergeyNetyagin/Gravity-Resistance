#define DISABLE_SCREEN_JOYSTICK

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FlightIndicator {

    [Tooltip( "Назначенная кнопка: пустое поле, если кнопка не используется" )]
    public Button button;

    [HideInInspector]
    public Shadow button_shadow;

    [Tooltip( "Основной постоянный цвет иконки" )]
    public Color icon_color = Color.white;
        
    [Tooltip( "Цвет для неактивного состояния иконки индикатора (если представляет собой кнопку, используется её неактивный цвет)" )]
    public Color disabled_color = Color.gray;

    [Tooltip( "Цвет для иконки индикатора на случай необходимости его изменения в особых ситуациях" )]
    public Color special_color = Color.red;

    [Tooltip( "Индикатор прогресса. Задействуется также для иконки персонажа. Пустое поле если не используется" )]
    public Image progress_bar;

    [Tooltip( "Объект-иконка на кнопке или панели (мигает, если задано значение alarm_percent, сигнализируя о завершени ресурса)" )]
    public GameObject icon;

    [HideInInspector]
    public Image icon_image;

    [HideInInspector]
    public AnimationColorAlpha icon_animation;

    [Tooltip( "Предельный процент наличия ресурса по отношению к текущему муксимуму, когда индикатор начинает мигать; если = 0, то мигание не используется" )]
    [Range( 0f, 1f )]
    public float alarm_rate = 0.25f;

    [Tooltip( "Кривая скорости мерцания иконки: на отрезке [0, 1] строится кривая, значения по Y которой соответствуют частоте мерцания (рекомендуемый максимум по Y = 5)" )]
    public AnimationCurve blinking_curve;

    [Tooltip( "Поле названия индикатора или сервиса" )]
    public EffectiveText name_field;

    [Tooltip( "Поле значения индикатора или сервиса" )]
    public EffectiveText value_field;

    [Tooltip( "Ключ для локализации названия индикатора (деньги, автопосадка, топливо и т.д.)" )]
    public string title_key;

    [Tooltip( "Ключ для локализации названия единицы измерения при отображении индикатора (проценты, тонны и т.п.)" )]
    public string unit_key;

    [HideInInspector]
    public Color 
        name_text_color = Color.white,
        value_text_color = Color.white;

    private bool is_blinking = false;
    public bool Is_blinking { get { return is_blinking; } set { is_blinking = value; } }

    private bool is_interactable = false;
    public bool Is_interactable { get { return is_interactable; } }

    private bool is_initialized = false;
    public bool Is_initialized { get { return is_initialized; } }

    public void SetProgressBar( float value ) { if( !is_initialized ) Initialize(); if( progress_bar != null ) progress_bar.fillAmount = value; }

    // Инициализация основных полей индикатора #################################################################################################################################
    public void Initialize() {

        is_initialized = true;

        if( icon != null ) icon_image = icon.GetComponent<Image>();
        if( icon_image != null ) icon_color = icon_image.color;
        if( icon != null ) icon_animation = icon.GetComponent<AnimationColorAlpha>();
        if( icon_animation != null ) icon_animation.enabled = false;

        if( button != null ) button_shadow = button.GetComponent<Shadow>();
        if( !name_field.Empty ) name_text_color = name_field.Get_color;
        if( !value_field.Empty ) value_text_color = value_field.Get_color;

        is_blinking = (alarm_rate > 0f) ? true : false;
        is_interactable = (button == null) ? false : button.interactable;
    }

    // Set the UI button in active or inactive state with sound ################################################################################################################
    public void SetButton( bool activate, AudioSource audio_source = null, AudioClip audio_clip = null ) {

        if( !is_initialized ) Initialize();

        if( button_shadow != null ) button_shadow.enabled = activate;

        if( button == null ) return;
        else button.interactable = is_interactable ? activate : false;

        if( (audio_source != null) && (audio_clip != null) ) audio_source.PlayOneShot( audio_clip, Game.Sound_volume );
    }

    // Включает/выключает мигание иконки и её цветовые параметры ###############################################################################################################
    public void SetBlinking( bool activate, bool change_color = false, float speed = 1f ) {

        if( !is_initialized ) Initialize();

        if( icon_image != null ) {

            if( change_color ) icon_image.color = special_color;
            else icon_image.color = icon_color;
        }

        if( !is_blinking ) return;
        if( icon_animation == null ) return;

        if( !icon_animation.enabled && activate ) icon_animation.enabled = true;
        else if( icon_animation.enabled && !activate ) icon_animation.enabled = false;

        if( icon_animation.enabled ) icon_animation.SetSpeed( speed );
    }
}

public class CanvasGame : MonoBehaviour {

    #region MAIN_PANELS
    [Header( "КЛЮЧЕВЫЕ ПАНЕЛИ ДЛЯ УПРАВЛЕНИЯ ИНТЕРФЕЙСАМИ" )]
    [SerializeField]
    private GameObject  
        panel_flight; [SerializeField] private GameObject  
        panel_station,
        panel_message,
        panel_joystick,
        panel_navigator,
        panel_menu_pause,
        panel_menu_choice,
        panel_brief,
        panel_trainig,
        panel_examination,
        panel_inventory,
        panel_upgrade,
        panel_trade,
        panel_map;
    #endregion

    #region PANEL_PAUSE
    [Header( "ССЫЛКИ ДЛЯ РАБОТЫ С ПАНЕЛЯМИ ПАУЗЫ" )]
    [SerializeField]
    [Tooltip( "Панель настроек, которая появляется в обычном окне паузы в игре" )]
    private GameObject panel_pause_settings;

    [SerializeField]
    [Tooltip( "Панель сообщения о причине рестарта уровня, которая появляется в окне паузы при фатальном повреждении корабля" )]
    private GameObject panel_pause_restart;

    [SerializeField]
    [Tooltip( "Строка, в которой указывается причина рестарта уровня (какое фатальное повреждение получил корабль)" )]
    private EffectiveText text_restart_causes;

    [SerializeField]
    [Tooltip( "Строка, в которой сообщается о возможости рестарта и даются добрые пожелания игроку" )]
    private EffectiveText text_restart_advices;
    #endregion

    #region FLIGHT_INDICATORS
    [Header( "ПОСТОЯННЫЕ ИГРОВЫЕ ИНДИКАТОРЫ ИНТЕРФЕЙСА" )]
    [SerializeField, Space( 0 )]  private FlightIndicator pause;
    [SerializeField, Space( 10 )] private FlightIndicator person;
    [SerializeField, Space( 10 )] private FlightIndicator money;
    [SerializeField, Space( 10 )] private FlightIndicator combo;
    [SerializeField, Space( 10 )] private FlightIndicator thrust;
    [SerializeField, Space( 10 )] private FlightIndicator hull;
    [SerializeField, Space( 10 )] private FlightIndicator fuel;
    [SerializeField, Space( 10 )] private FlightIndicator engine;
    [SerializeField, Space( 10 )] private FlightIndicator hold;
    [SerializeField, Space( 10 )] private FlightIndicator inventory;
    [SerializeField, Space( 10 )] private FlightIndicator timer;
    [SerializeField, Space( 10 )] private FlightIndicator navigator;
    [SerializeField, Space( 10 )] private FlightIndicator map;
    [SerializeField, Space( 10 )] private FlightIndicator shield;
    [SerializeField, Space( 10 )] private FlightIndicator radar;
    [SerializeField, Space( 10 )] private FlightIndicator autolanding;

    [Header( "СКОРОСТЬ ОБНОВЛЕНИЯ ОТДЕЛЬНЫХ ИГРОВЫХ ИНДИКАТОРОВ" )]
    [SerializeField]
    [Range( 1, 300 )]
    [Tooltip( "Скорость обновления индикатора работающего щита, сколько раз в секунду (обновляется только при разрядке); по умолчанию = 60" )]
    private int protect_refresh_speed = 60;
    public int Protect_refresh_speed { get { return protect_refresh_speed; } }

    [SerializeField]
    [Range( 1, 300 )]
    [Tooltip( "Скорость обновления индикатора щита в процессе зарядки, сколько раз в секунду (обновляется только при зарядке); по умолчанию = 30" )]
    private int charge_refresh_speed = 30;
    public int Charge_refresh_speed { get { return charge_refresh_speed; } }

    [SerializeField]
    [Range( 0, 10 )]
    [Tooltip( "Скорость обновления индикатора обшивки, сколько раз в секунду" )]
    private int hull_refresh_speed = 2;
    public int Hull_refresh_speed { get { return hull_refresh_speed; } }

    [SerializeField]
    [Range( 0, 10 )]
    [Tooltip( "Скорость обновления индикатора топливного бака, сколько раз в секунду" )]
    private int fuel_refresh_speed = 2;
    public int Fuel_refresh_speed { get { return fuel_refresh_speed; } }

    [SerializeField]
    [Range( 0, 10 )]
    [Tooltip( "Скорость обновления индикатора двигателя, сколько раз в секунду" )]
    private int engine_refresh_speed = 2;
    public int Engine_refresh_speed { get { return engine_refresh_speed; } }
    #endregion

    #region STATION_INDICATORS
    [Header( "ЭЛЕМЕНТЫ ИНДИКАЦИИ СЕРВИСА НА СТАНЦИЯХ" )]
    [SerializeField, Space( 10 )] private FlightIndicator service_station;
    [SerializeField, Space( 10 )] private FlightIndicator service_hull;
    [SerializeField, Space( 10 )] private FlightIndicator service_fuel;
    [SerializeField, Space( 10 )] private FlightIndicator service_engine;
    [SerializeField, Space( 10 )] private FlightIndicator service_upgrade;
    [SerializeField, Space( 10 )] private FlightIndicator service_trade;
    #endregion

    #region SOUNDS_REFERENCES
    [Header( "ЗВУКОВЫЕ ЭФФЕКТЫ ДЛЯ ИНТЕРФЕЙСА" )]
    [SerializeField]
    [Tooltip( "Приглушает звук перемещения ползунков слайдеров в меню; по умолчанию = 0.3" )]
    [Range( 0.0f, 1.0f )]
    private float silence_slider_rate = 0.3f;

    [SerializeField]
    [Tooltip( "Время прослушивания звуков в секундах в момент изменения настроек громкости; по умолчанию = 2" )]
    [Range( 0.5f, 5.0f )]
    private float audio_check_time = 2f;

    [SerializeField]
    [Space( 10 )]
    private AudioClip 
        clip_check_engine; [SerializeField] private AudioClip 
        clip_check_voice,
        clip_check_sound,
        clip_check_music;
        
    [SerializeField]
    [Space( 10 )]
    private AudioClip 
        clip_drag_slider; [SerializeField] private AudioClip 
        clip_press_button,
        clip_enable_button,
        clip_disable_button,
        clip_press_repair,
        clip_press_fill,
        clip_press_buy,
        clip_press_sell;
    #endregion

    #region LOCAL_MENU
    [Header( "НАСТРОЙКИ И ССЫЛКИ ДЛЯ ЛОКАЛЬНОГО МЕНЮ" )]
    [SerializeField]
    private EffectiveText 
        text_pause_title; [SerializeField] private EffectiveText
        text_combo,
        text_combo_description,
        text_experience,
        text_experience_description,
        text_earned,
        text_earned_description,
        text_spent,
        text_spent_description,
        text_total,
        text_total_description;

    [Space( 10 )]
    [SerializeField]
    private EffectiveText 
        text_control_vertical; [SerializeField] private EffectiveText
        text_control_horizontal,
        ______________________________________text_radar_using,   // Вместо этих двух настроект будем использовать что-то другое (звук в вакууме и т.п.)
        ______________________________________text_detector_using,
        text_engine_volume,
        text_sound_volume,
        text_voice_volume,
        text_music_volume;

    [Space( 10 )]
    [SerializeField]
    private EffectiveText 
        text_ship_state; [SerializeField] private EffectiveText
        text_ship_name,
        text_menu_hull,
        text_menu_fuel,
        text_menu_engine,
        text_menu_hold,
        text_menu_shield_time,
        text_menu_shield_power,
        text_menu_charge_time,
        text_menu_radar_range,
        text_menu_radar_power,
        text_menu_autolanding;
        
    [Space( 10 )]
    [SerializeField]
    private Image
        available_bar_hull; [SerializeField] private Image
        available_bar_fuel,
        available_bar_engine,
        available_bar_hold,
        available_bar_shield_time,
        available_bar_shield_power,
        available_bar_charge_time,
        available_bar_radar_range,
        available_bar_radar_power,
        available_bar_autolanding;

    [Space( 10 )]
    [SerializeField]
    private Image
        maximum_bar_hull; [SerializeField] private Image
        maximum_bar_fuel,
        maximum_bar_engine,
        maximum_bar_hold,
        maximum_bar_shield_time,
        maximum_bar_shield_power,
        maximum_bar_charge_time,
        maximum_bar_radar_range,
        maximum_bar_radar_power,
        maximum_bar_autolanding;

    [Space( 10 )]       
    [SerializeField]
    private Color 
        pause_title_color = Color.white; [SerializeField] private Color 
        restart_title_color = Color.red,
        toggle_on_color = Color.white,
        toggle_off_color = Color.clear;

    [SerializeField]
    private Button
        pause_button_home; [SerializeField] private Button
        pause_button_play,
        pause_button_restart;

    [Space( 10 )]
    [SerializeField]
    private Slider
        pause_slider_engine; [SerializeField] private Slider
        pause_slider_voice,
        pause_slider_sound,
        pause_slider_music;

    [SerializeField]
    private Image
        image_on_off_engine; [SerializeField] private Image
        image_on_off_voice,
        image_on_off_sound,
        image_on_off_music;

    [Space( 10 )]
    [SerializeField]
    private Toggle
        toggle_vertical_control; [SerializeField] private Toggle
        toggle_horizontal_control,
        ________________________________________________________________toggle_radar_using,
        ________________________________________________________________toggle_detector_using;

    [SerializeField]
    private Image
        image_on_off_vertical; [SerializeField] private Image
        image_on_off_horizontal,
        ________________________________________________________________image_on_off_radar,
        ________________________________________________________________image_on_off_detector;
    #endregion

    #region ANIMATIONS_REFERENCES
    [Header( "НАСТРОЙКИ ССЫЛОК ДЛЯ АНИМАЦИЙ" )]
    [SerializeField]
    [Tooltip( "Аниматор затемнения экрана" )]
    private Animator animator_canvas;

    [SerializeField]
    [Tooltip( "Аниматор игровых полётных индикаторов" )]
    private Animator animator_flight;

    [SerializeField]
    [Tooltip( "Аниматор индикаторов сервиса на станциях" )]
    private Animator animator_station;

    private int
        animator_ID_canvas_show = 0,
        animator_ID_flight_show = 0,
        animator_ID_station_show = 0,
        animator_ID_autolanding_show = 0;
    #endregion

    public void ShowAutolandingButton() { animator_flight.SetBool( animator_ID_autolanding_show, true ); }
    public void HideAutolandingButton() { animator_flight.SetBool( animator_ID_autolanding_show, false ); }

    private Vector2 screen_rate;
    private Vector2 target_screen_resolution;
    private Vector3 operation;

    private AudioSource audio_source;

    private const float inversed_60_seconds = 1f / 60f;

    // Use this for initialization #############################################################################################################################################
	void Start () {

        Game.ResetSumCalculation();
        Game.RefreshLocalizedUnits();
        Game.RefreshLocalizedPhrases();
        Game.RefreshLocalizedSeparators();

        audio_source = GetComponent<AudioSource>();

        // Очищаем значения сервисов (иначе они на мгновение будут видны при анимации сервиса)
        RefreshServiceStation( false, true );
        RefreshServiceHullIndicator( false, true );
        RefreshServiceFuelIndicator( false, true );
        RefreshServiceEngineIndicator( false, true );
        RefreshServiceTradeIndicator( false, true );
        RefreshServiceUpgradeIndicator( false, true );

        animator_canvas.enabled = true;
        animator_ID_canvas_show = Animator.StringToHash( "Canvas_show" );
        animator_canvas.SetBool( animator_ID_canvas_show, false );

        animator_flight.enabled = true;
        animator_ID_flight_show = Animator.StringToHash( "Flight_show" );
        animator_flight.SetBool( animator_ID_flight_show, true );

        animator_ID_autolanding_show = Animator.StringToHash( "Autolanding_show" );
        animator_flight.SetBool( animator_ID_autolanding_show, false );

        animator_station.enabled = true;
        animator_ID_station_show = Animator.StringToHash( "Station_show" );
        animator_station.SetBool( animator_ID_station_show, false );

        target_screen_resolution.x = GetComponent<CanvasScaler>().referenceResolution.x;
        target_screen_resolution.y = GetComponent<CanvasScaler>().referenceResolution.y;

        screen_rate.x = target_screen_resolution.x / Screen.width;
        screen_rate.y = target_screen_resolution.y / Screen.height;
        
        if( Game.Level.Use_brief ) Game.SetState( GameState.Briefing );
        else Game.SetState( GameState.Starting );

        // Начальная визуализация основных панелей интерфейса
        panel_flight.SetActive( true );
        panel_station.SetActive( true );
        panel_message.SetActive( true );
        panel_navigator.SetActive( false );
        panel_menu_pause.SetActive( false );
        panel_menu_choice.SetActive( false );
        panel_pause_settings.SetActive( false );
        panel_pause_restart.SetActive( false );

        #if DISABLE_SCREEN_JOYSTICK
        panel_joystick.SetActive( false );
        #else
        panel_joystick.SetActive( true );
        #endif

        panel_brief.SetActive( false );
        panel_trainig.SetActive( false );
        panel_examination.SetActive( false );
                  
        panel_inventory.SetActive( false );
        panel_upgrade.SetActive( false );
        panel_trade.SetActive( false );
        panel_map.SetActive( false );
        
        // Подготовка панели брифа
        if( Game.Level.Use_brief ) {

            panel_brief.SetActive( true );

            BriefControl[] brief_controls = panel_brief.GetComponentsInChildren<BriefControl>( true );

            for( int i = 0; i < brief_controls.Length; i++ ) {

                if( brief_controls[i].Level_type == Game.Level.Type ) brief_controls[i].gameObject.SetActive( true );
                else brief_controls[i].gameObject.SetActive( true );
            }
        }

        // Иначе сразу включаются полётные индикаторы и кнопки
        else Game.Canvas.RefreshFlightIndicators( true );

        // Инициализация слайдеров меню
        pause_slider_engine.normalizedValue = Game.Engine_volume;
        pause_slider_voice.normalizedValue = Game.Voice_volume;
        pause_slider_sound.normalizedValue = Game.Sound_volume;
        pause_slider_music.normalizedValue = Game.Music_volume;
        toggle_vertical_control.isOn = Game.Use_vertical_control;
        toggle_horizontal_control.isOn = Game.Use_horizontal_control;
        //____________________________________________________________________________________toggle_radar_using.isOn = Game.Use_radar;

        image_on_off_engine.color = (Game.Engine_volume == 0f) ? toggle_off_color : toggle_on_color;
        image_on_off_voice.color = (Game.Voice_volume == 0f) ? toggle_off_color : toggle_on_color;
        image_on_off_sound.color = (Game.Sound_volume == 0f) ? toggle_off_color : toggle_on_color;
        image_on_off_music.color = (Game.Music_volume == 0f) ? toggle_off_color : toggle_on_color;
        image_on_off_vertical.color = (Game.Use_vertical_control) ? toggle_on_color : toggle_off_color;
        image_on_off_horizontal.color = (Game.Use_horizontal_control) ? toggle_on_color : toggle_off_color;
        //____________________________________________________________________________________image_on_off_radar.color = (Game.Use_radar) ? toggle_on_color : toggle_off_color;
    }

    // Refresh the flight's indicators #########################################################################################################################################
    #region FLIGHT_REFRESH_INDICATORS
    public void RefreshFlightIndicators( bool activate_button ) {

        RefreshPersonIndicator( activate_button );
        RefreshMoneyIndicator( activate_button );
        RefreshComboIndicator( activate_button );
        RefreshThrustIndicator( activate_button );
        RefreshTimerIndicator( activate_button );
        RefreshNavigatorIndicator( activate_button );
        RefreshMapIndicator( activate_button );
        RefreshRadarIndicator( activate_button );
        RefreshHullIndicator( activate_button );
        RefreshFuelIndicator( activate_button );
        RefreshEngineIndicator( activate_button );
        RefreshHoldIndicator( activate_button );
        RefreshShieldIndicator( activate_button );
        RefreshAutolandingIndicator( activate_button );
        RefreshInventoryIndicator( activate_button );
        RefreshPauseIndicator( activate_button );
    }

    public void RefreshPauseIndicator( bool activate_button ) {

        pause.SetButton( activate_button );
    }

    public void RefreshPersonIndicator( bool activate_button ) {

    }

    public void RefreshMoneyIndicator( bool activate_button ) {

        if( Game.Money < 1000000f ) {

            money.name_field.Rewrite( Game.Phrase_money );
            money.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Money ) );
        }

        else if( Game.Money >= 1000000000f ) {

            int digits = (Game.Money >= 100000000000f) ? 1 : ((Game.Money >= 10000000000f) ? 2 : 3);

            money.name_field.Rewrite( Game.Phrase_money );
            money.value_field.RewriteDottedFloat( Game.Money * 0.000000001f, digits );
            money.value_field.Append( Game.Separator_space ).Append( Game.Unit_money_bln );
        }

        else {

            int digits = (Game.Money >= 100000000f) ? 1 : ((Game.Money >= 10000000f) ? 2 : 3);

            money.name_field.Rewrite( Game.Phrase_money );
            money.value_field.RewriteDottedFloat( Game.Money * 0.000001f, digits );
            money.value_field.Append( Game.Separator_space ).Append( Game.Unit_money_mln );
        }
    }

    public void RefreshComboIndicator( bool activate_button ) {

        combo.progress_bar.fillAmount = (Game.Level.Combo - 1f) * Game.Level.Max_combo_inversed;
    }

    public void RefreshThrustIndicator( bool activate_button ) {

        if( Mathf.Abs( Game.Player.Thrust_x ) > Mathf.Abs( Game.Player.Thrust_y ) ) {

            thrust.progress_bar.fillAmount = Mathf.Abs( Game.Player.Thrust.x ) * Game.Player.Ship_thrust_maximum_x_inversed * Game.Player.Thrust_x_to_y_rate;
        }

        else {

            thrust.progress_bar.fillAmount = Mathf.Abs( Game.Player.Thrust.y ) * Game.Player.Ship_thrust_maximum_y_inversed * Game.Player.Thrust_y_to_x_rate;
        }
    }

    public void RefreshTimerIndicator( bool activate_button ) {

        int minutes = Mathf.FloorToInt( Game.Timer.Current_time * inversed_60_seconds );
        int seconds = Mathf.FloorToInt( Game.Timer.Current_time - (minutes * 60) );

        if( minutes < 10 ) timer.value_field.Rewrite( Game.Separator_zero ).Append( minutes );
        else timer.value_field.Rewrite( minutes );
        timer.value_field.Append( Game.Separator_colon );
        if( seconds < 10 ) timer.value_field.Append( Game.Separator_zero ).Append( seconds );
        else timer.value_field.Append( seconds );

        float available_rate = Game.Timer.Current_time * Game.Timer.Total_time_inversed;

        if( Game.Timer.Is_enabled && (available_rate < timer.alarm_rate) ) timer.SetBlinking( true, true, timer.blinking_curve.Evaluate( available_rate ) );
        else timer.SetBlinking( false );

        timer.SetProgressBar( available_rate );
        timer.SetButton( activate_button );
    }

    public void RefreshNavigatorIndicator( bool activate_button ) {

        navigator.value_field.Rewrite( Game.Navigator.Mode_name );

        navigator.SetProgressBar( Game.Navigator.Is_enabled ? 0f : 1f );
        navigator.SetButton( activate_button );
    }

    public void RefreshMapIndicator( bool activate_button ) {

    }

    public void RefreshRadarIndicator( bool activate_button ) {

        radar.value_field.Rewrite( Game.Radar.Mode_name );

        radar.SetProgressBar( Game.Radar.Is_enabled ? 0f : 1f );
        radar.SetButton( activate_button );
    }

    public void RefreshHullIndicator( bool activate_button ) {

        hull.name_field.Rewrite( Game.Phrase_hull ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        hull.name_field.Append( Mathf.FloorToInt( Game.Player.Ship.Hull_durability.Available ) );
        hull.name_field.Append( Game.Separator_space ).Append( Game.Unit_pressure_MPa ).Append( Game.Separator_space ).Append( Game.Separator_slash ).Append( Game.Separator_space );
        hull.name_field.Append( Mathf.FloorToInt( Game.Player.Ship.Hull_durability.Maximum ) );
        hull.name_field.Append( Game.Separator_space ).Append( Game.Unit_pressure_MPa );

        float available_rate = Game.Player.Ship.Hull_durability.Available * Game.Player.Ship.Hull_durability.Maximum_inversed;

        if( available_rate < hull.alarm_rate ) hull.SetBlinking( true, false, hull.blinking_curve.Evaluate( available_rate ) );
        else hull.SetBlinking( false );

        hull.SetProgressBar( available_rate );
    }

    public void RefreshFuelIndicator( bool activate_button ) {

        fuel.name_field.Rewrite( Game.Phrase_fuel ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        fuel.name_field.AppendDottedFloat( Game.Player.Ship.Fuel_capacity.Available );
        fuel.name_field.Append( Game.Separator_space ).Append( Game.Unit_mass_t ).Append( Game.Separator_space ).Append( Game.Separator_slash ).Append( Game.Separator_space );
        fuel.name_field.AppendDottedFloat( Game.Player.Ship.Fuel_capacity.Maximum );
        fuel.name_field.Append( Game.Separator_space ).Append( Game.Unit_mass_t );

        float available_rate = Game.Player.Ship.Fuel_capacity.Available * Game.Player.Ship.Fuel_capacity.Maximum_inversed;

        if( available_rate < fuel.alarm_rate ) fuel.SetBlinking( true, Game.Player.Has_fuel_leaks, fuel.blinking_curve.Evaluate( available_rate ) );
        else fuel.SetBlinking( false, Game.Player.Has_fuel_leaks );

        fuel.SetProgressBar( available_rate );
    }

    public void RefreshEngineIndicator( bool activate_button ) {

        engine.name_field.Rewrite( Game.Phrase_engine ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        engine.name_field.Append( Mathf.FloorToInt( Game.Player.Ship.Engine_thrust.Available ) );
        engine.name_field.Append( Game.Separator_space ).Append( Game.Unit_thrust_kN ).Append( Game.Separator_space ).Append( Game.Separator_slash ).Append( Game.Separator_space );
        engine.name_field.Append( Mathf.FloorToInt( Game.Player.Ship.Engine_thrust.Maximum ) );
        engine.name_field.Append( Game.Separator_space ).Append( Game.Unit_thrust_kN );

        float available_rate = Game.Player.Ship.Engine_thrust.Available * Game.Player.Ship.Engine_thrust.Maximum_inversed;

        if( available_rate < engine.alarm_rate ) engine.SetBlinking( true, false, engine.blinking_curve.Evaluate( available_rate ) );
        else engine.SetBlinking( false );

        engine.SetProgressBar( available_rate );
    }

    public void RefreshShieldIndicator( bool activate_button ) {

        if( Game.Player.Shield.Is_active || Game.Player.Shield.Is_ready ) {

            shield.value_field.RewriteDottedFloat( Game.Player.Ship.Shield_time.Available );
        }

        else {

            shield.value_field.Rewrite( Mathf.FloorToInt( Game.Player.Ship.Charge_time.Maximum - Game.Player.Ship.Charge_time.Available ) );
        }

        shield.SetProgressBar( 1f - Game.Player.Ship.Shield_time.Available * Game.Player.Ship.Shield_time.Maximum_inversed );
        shield.SetButton( Game.Player.Shield.Is_ready ? activate_button : false );
    }

    public void RefreshHoldIndicator( bool activate_button ) {

        float available_rate = Game.Player.Ship.Compartments_loaded * Game.Player.Ship.Compartments_available_inversed;

        if( available_rate < 0f ) available_rate = 0f;
        else if( available_rate > 1f ) available_rate = 1f;

        hold.name_field.Rewrite( Game.Localization.GetTextValue( hold.title_key ) ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        hold.name_field.AppendSeparatedInt( Mathf.FloorToInt( available_rate * 100f ) ).Append( Game.Separator_space ).Append( Game.Unit_percent );

        hold.SetProgressBar( available_rate );
        hold.SetButton( activate_button );
    }

    public void RefreshAutolandingIndicator( bool activate_button ) {

        autolanding.value_field.Rewrite( Game.Player.Ship.Autolanding_amount.Available );
        autolanding.value_field.Append( Game.Separator_slash );
        autolanding.value_field.Append( Game.Player.Ship.Autolanding_amount.Maximum );

        if( (Game.Player.Ship.Autolanding_amount.Available > 0f) && !Game.Player.At_station && !Game.Player.Is_autolanding ) autolanding.SetButton( activate_button );
        else autolanding.SetButton( false );
    }

    public void RefreshInventoryIndicator( bool activate_button ) {

        inventory.SetButton( activate_button );
    }
    #endregion

    // Refresh the service buttons and indicators ##############################################################################################################################
    #region STATION_REFRESH_INDICATORS
    public void RefreshServiceIndicators( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        RefreshServiceStation( activate_button, clear_values );
        RefreshServiceHullIndicator( activate_button, clear_values );
        RefreshServiceFuelIndicator( activate_button, clear_values );
        RefreshServiceEngineIndicator( activate_button, clear_values );
        RefreshServiceUpgradeIndicator( activate_button, clear_values );
        RefreshServiceTradeIndicator( activate_button, clear_values );
    }

    public void RefreshServiceStation( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        if( clear_values ) {

            service_station.name_field.Clear();
            service_station.value_field.Clear();
            service_station.SetButton( false );
            return;
        }

        service_station.name_field.Rewrite( Game.Phrase_station );
        service_station.value_field.Rewrite( Game.Separator_quote ).Append( Game.Localization.GetTextValue( Game.Player.Station.Name_key ) ).Append( Game.Separator_quote );

        service_station.SetButton( activate_button );
    }

    public void RefreshServiceHullIndicator( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        if( clear_values ) {

            service_hull.name_field.Clear();
            service_hull.value_field.Clear();
            service_hull.SetButton( false );
            return;
        }

        bool enable_button = false;

        service_hull.name_field.Rewrite( Game.Phrase_hull );
     
        // Если станция не ремонтирует обшивку
        if( !Game.Player.Station.Repair_hull ) {

            service_hull.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_no );
            service_hull.value_field.Rewrite( 0 );
            goto button_label;
        }

        // Если обшивка корабля в полном порядке
        else if( Game.Player.Ship.Hull_durability.Available >= Game.Player.Ship.Hull_durability.Maximum ) {

            service_hull.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_norm );
            service_hull.value_field.Rewrite( 0 );
            goto button_label;
        }
        
        // Если денег нет вообще
        else if( Game.Money <= 0f ) {

            service_hull.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_money_no );
            service_hull.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Hull_price ) );
            goto button_label;
        }

        // Если не хватает денег на полный ремонт
        else if( Game.Player.Station.Hull_partial_repair ) {

            service_hull.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_restore_partial );
            service_hull.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Hull_price ) );
            enable_button = true;
            goto button_label;
        }

        // Если нужен ремонт, станция оказывает такой сервис и хватает средств для этого
        else {

            service_hull.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_restore_full );
            service_hull.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Hull_price ) );
            enable_button = true;
        }

        button_label:
        service_hull.SetButton( enable_button ? activate_button : false );
    }

    public void RefreshServiceFuelIndicator( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        if( clear_values ) {

            service_fuel.name_field.Clear();
            service_fuel.value_field.Clear();
            service_fuel.SetButton( false );
            return;
        }

        bool enable_button = false;

        service_fuel.name_field.Rewrite( Game.Phrase_fuel );
     
        // Если станция не торгует топливом
        if( !Game.Player.Station.Sells_fuel ) {

            service_fuel.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_no );
            service_fuel.value_field.Rewrite( 0 );
            goto button_label;
        }
        
        // Если корабль полностью заправлен
        else if( Game.Player.Ship.Fuel_capacity.Available >= Game.Player.Ship.Fuel_capacity.Maximum ) {

            service_fuel.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_norm );
            service_fuel.value_field.Rewrite( 0 );
            goto button_label;
        }

        // Если денег нет вообще
        else if( Game.Money <= 0f ) {

            service_fuel.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_money_no );
            service_fuel.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Fuel_price ) );
            goto button_label;
        }

        // Если не хватает денег на полную заправку
        else if( Game.Player.Station.Fuel_partial_filling ) {

            service_fuel.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_filling_partial );
            service_fuel.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Fuel_price ) );
            enable_button = true;
            goto button_label;
        }

        // Если нужна заправка, станция оказывает такой сервис и хватает средств для этого
        else {

            service_fuel.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_filling_full );
            service_fuel.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Fuel_price ) );
            enable_button = true;
        }

        button_label:
        service_fuel.SetButton( enable_button ? activate_button : false );
    }

    public void RefreshServiceEngineIndicator( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        if( clear_values ) {

            service_engine.name_field.Clear();
            service_engine.value_field.Clear();
            service_engine.SetButton( false );
            return;
        }

        bool enable_button = false;

        service_engine.name_field.Rewrite( Game.Phrase_engine );
     
        // Если станция не ремонтирует двигатели
        if( !Game.Player.Station.Repairs_engine ) {

            service_engine.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_no );
            service_engine.value_field.Rewrite( 0 );
            goto button_label;
        }

        // Если двигатель корабля в полном порядке
        else if( Game.Player.Ship.Engine_thrust.Available >= Game.Player.Ship.Engine_thrust.Maximum ) {

            service_engine.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_norm );
            service_engine.value_field.Rewrite( 0 );
            goto button_label;
        }
        
        // Если денег нет вообще
        else if( Game.Money <= 0f ) {

            service_engine.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_money_no );
            service_engine.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Engine_price ) );
            goto button_label;
        }

        // Если не хватает денег на полный ремонт двигателя
        else if( Game.Player.Station.Engine_partial_repair ) {

            service_engine.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_restore_partial );
            service_engine.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Engine_price ) );
            enable_button = true;
            goto button_label;
        }

        // Если нужен ремонт двигателя, станция оказывает такой сервис и хватает средств для этого
        else {

            service_engine.name_field.Append( Game.Separator_colon ).Append( Game.Separator_space ).Append( Game.Phrase_service_restore_full );
            service_engine.value_field.RewriteSeparatedInt( Mathf.FloorToInt( Game.Player.Station.Engine_price ) );
            enable_button = true;
        }

        button_label:
        service_engine.SetButton( enable_button ? activate_button : false );
    }

    public void RefreshServiceUpgradeIndicator( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        if( clear_values ) {

            service_upgrade.name_field.Clear();
            service_upgrade.value_field.Clear();
            service_upgrade.SetButton( false );
            return;
        }

        bool enable_button = false;
        
        // Если станция оказывает услуги апгрэйда
        if( Game.Player.Station.Is_upgrading ) {

            service_upgrade.name_field.Rewrite( Game.Localization.GetTextValue( service_upgrade.title_key ) );
            service_upgrade.value_field.Rewrite( Game.Localization.GetTextValue( service_upgrade.title_key ) );
            enable_button = true;
        }

        else {

            service_upgrade.name_field.Rewrite( Game.Localization.GetTextValue( service_upgrade.unit_key ) );
            service_upgrade.value_field.Rewrite( Game.Localization.GetTextValue( service_upgrade.unit_key ) );
        }

        service_upgrade.SetButton( enable_button ? activate_button : false );
    }

    public void RefreshServiceTradeIndicator( bool activate_button, bool clear_values = false ) {

        if( !clear_values && !Game.Player.At_station ) return;

        if( clear_values ) {

            service_trade.name_field.Clear();
            service_trade.value_field.Clear();
            service_trade.SetButton( false );
            return;
        }

        bool enable_button = false;
        
        // Если станция вообще не оказывает ни услуг апгрэйда, ни продажи автопосадок
        if( Game.Player.Station.Is_trading ) {

            service_trade.name_field.Rewrite( Game.Localization.GetTextValue( service_trade.title_key ) );
            service_trade.value_field.Rewrite( Game.Localization.GetTextValue( service_trade.title_key ) );
            enable_button = true;
        }

        else {

            service_trade.name_field.Rewrite( Game.Localization.GetTextValue( service_trade.unit_key ) );
            service_trade.value_field.Rewrite( Game.Localization.GetTextValue( service_trade.unit_key ) );
        }

        service_trade.SetButton( enable_button ? activate_button : false );
    }
    #endregion

    // Check and enable the station service ####################################################################################################################################
    public void SetStationPanel( bool activate ) {

        if( activate && !Game.Player.At_station ) return;
        if( !activate && !panel_station.activeInHierarchy ) return;

        RefreshServiceIndicators( activate );

        if( activate ) animator_station.SetBool( animator_ID_station_show, true );
        else animator_station.SetBool( animator_ID_station_show, false );
    }
    
    // Check and enable the flight cintrol panel ################################################################################################################################
    public void SetFlightPanel( bool activate ) {

        if( !activate && !panel_flight.activeInHierarchy ) return;

        RefreshFlightIndicators( activate );

        if( activate ) animator_flight.SetBool( "Exit", false );
        else animator_flight.SetBool( "Exit", true );
        animator_flight.enabled = true;
    }

    // Activate restart window #################################################################################################################################################
    public void ActivateRestartWindow() {

        Game.AddState( GameState.Restarting );

        Indicator death_indicator = null;

        if( Game.Player.Ship.Hull_durability.Available <= Game.Player.Ship.Hull_durability.Restart_limit ) death_indicator = Game.Player.Ship.Hull_durability;
        else if( Game.Player.Ship.Fuel_capacity.Available <= Game.Player.Ship.Fuel_capacity.Restart_limit ) death_indicator = Game.Player.Ship.Fuel_capacity;
        else if( Game.Player.Ship.Engine_thrust.Available <= Game.Player.Ship.Engine_thrust.Restart_limit ) death_indicator = Game.Player.Ship.Engine_thrust;

        if( death_indicator != null ) {

            text_restart_causes.Rewrite( Game.Localization.GetTextValue( death_indicator.Cause_restart_key ) );
            text_restart_advices.Rewrite( Game.Localization.GetTextValue( death_indicator.Advice_restart_key ) );
        }

        else {

            text_restart_causes.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Restart.Cause" ) );
            text_restart_advices.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Restart.Advice" ) );
        }

        EventButtonPausePressed();
    }

    // Event: pause button pressed #############################################################################################################################################
    public void EventButtonPausePressed() {

        if( Game.Is( GameState.Restarting ) ) Game.Control.PauseKinematicTotal();
        else Game.Control.PauseKinematic();

        if( !Game.Is( GameState.Restarting ) && (audio_source != null) ) audio_source.PlayOneShot( clip_press_button, Game.Sound_volume );

        if( Game.Player.Has_fuel_leaks ) Game.Player.PauseLeaksEffect();

        Game.AddState( GameState.Paused );

        Game.Player.DisableEngineSound();
        Game.Input_control.DisableControl();
        Game.Player.StopThrust();

        RefreshFlightIndicators( false );
        RefreshServiceIndicators( false );

        text_pause_title.SetColor( Game.Is( GameState.Restarting ) ? restart_title_color : pause_title_color );
        text_pause_title.Text_component.GetComponent<AnimationColorAlpha>().enabled = Game.Is( GameState.Restarting ) ? true : false;
        text_pause_title.Rewrite( Game.Localization.GetTextValue( Game.Is( GameState.Restarting ) ? "Menu.Local.Restart" : "Menu.Local.Paused" ) );
        panel_pause_settings.SetActive( Game.Is( GameState.Restarting ) ? false : true );
        panel_pause_restart.SetActive( Game.Is( GameState.Restarting ) ? true : false );

        text_combo.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Combo" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendDottedFloat( Game.Level.Combo, 2 );
        text_combo_description.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Combo.Description" ) );
        text_experience.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Experience" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendDottedFloat( Game.Experience, 2 );
        text_experience_description.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Experience.Description" ) );
        text_earned.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Money.Earned" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Game.Sum_earned ) );
        text_earned_description.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Money.Earned.Description" ) );
        text_spent.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Money.Spent" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Game.Sum_spent ) );
        text_spent_description.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Money.Spent.Description" ) );
        text_total.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Money.Total" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Game.Sum_earned - Game.Sum_spent ) );
        text_total_description.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Money.Total.Description" ) );

        text_control_vertical.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Control.Vertical" ) );
        text_control_horizontal.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Control.Horizontal" ) );

        text_engine_volume.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Audio.Engine" ) );
        text_sound_volume.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Audio.Sound" ) );
        text_voice_volume.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Audio.Voice" ) );
        text_music_volume.Rewrite( Game.Localization.GetTextValue( "Menu.Local.Audio.Music" ) );

        text_ship_state.Rewrite( Game.Localization.GetTextValue( Game.Is( GameState.Restarting ) ? "Menu.Local.Ship.Restart" : "Menu.Local.Ship.State" ) );
        text_ship_name.Rewrite( Game.Localization.GetTextValue( Game.Player.Ship.Type_key ) ).Append( Game.Separator_colon );

        Game.ReportIndicator( Game.Player.Ship.Hull_durability, text_menu_hull, available_bar_hull, maximum_bar_hull, true, false );
        Game.ReportIndicator( Game.Player.Ship.Fuel_capacity, text_menu_fuel, available_bar_fuel, maximum_bar_fuel, true, true );
        Game.ReportIndicator( Game.Player.Ship.Engine_thrust, text_menu_engine, available_bar_engine, maximum_bar_engine, true, false );
        Game.ReportIndicator( Game.Player.Ship.Hold_capacity, text_menu_hold, available_bar_hold, maximum_bar_hold, true, true );
        Game.ReportIndicator( Game.Player.Ship.Shield_time, text_menu_shield_time, available_bar_shield_time, maximum_bar_shield_time, true, true );
        Game.ReportIndicator( Game.Player.Ship.Shield_power, text_menu_shield_power, available_bar_shield_power, maximum_bar_shield_power, true, true );
        Game.ReportIndicator( Game.Player.Ship.Charge_time, text_menu_charge_time, available_bar_charge_time, maximum_bar_charge_time, true, true );
        Game.ReportIndicator( Game.Player.Ship.Radar_range, text_menu_radar_range, available_bar_radar_range, maximum_bar_radar_range, true, true );
        Game.ReportIndicator( Game.Player.Ship.Radar_power, text_menu_radar_power, available_bar_radar_power, maximum_bar_radar_power, true, true );
        Game.ReportIndicator( Game.Player.Ship.Autolanding_amount, text_menu_autolanding, available_bar_autolanding, maximum_bar_autolanding, false, false );

        // В режиме обычной паузы кнопка продолжения активна
        pause_button_play.interactable = Game.Is( GameState.Restarting ) ? false : true;
        panel_menu_pause.SetActive( true );
    }
    
    // Event: play button is pressed ###########################################################################################################################################
    public void EventButtonHomePressed() {

        Game.AddState( GameState.Complete );

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_button, Game.Sound_volume );

        // Предотвращаем использование брифа при повторном запуске уровне
        Game.Level.SetUseBrief( false );

        RefreshFlightIndicators( false );
        RefreshServiceIndicators( false );

        panel_flight.SetActive( false );
        if( Game.Player.At_station ) panel_station.SetActive( false );

        animator_canvas.SetBool( animator_ID_canvas_show, false );
        panel_menu_pause.SetActive( false );
    }

    // Event: play button is pressed ###########################################################################################################################################
    public void EventButtonPlayPressed() {

        if( audio_source != null ) {

            audio_source.Stop();
            audio_source.PlayOneShot( clip_press_button, Game.Sound_volume );
        }

        Game.ResetState( GameState.Paused );
        Game.Player.EnableEngineSound();

        if( Game.Player.Has_fuel_leaks ) Game.Player.ResumeLeaksEffect();

        RefreshFlightIndicators( true );
        RefreshServiceIndicators( true );

        panel_menu_pause.SetActive( false );

        Game.Input_control.EnableControl();
        Game.Control.ResumeKinematic();
    }
    
    // Event: play button is pressed ###########################################################################################################################################
    public void EventButtonRestartPressed() {

        Game.AddState( GameState.Restarting );

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_button, Game.Sound_volume );

        // Предотвращаем использование брифа при повторном запуске уровне
        Game.Level.SetUseBrief( false );

        RefreshFlightIndicators( false );
        RefreshServiceIndicators( false );

        panel_flight.SetActive( false );
        if( Game.Player.At_station ) panel_station.SetActive( false );

        animator_canvas.SetBool( animator_ID_canvas_show, false );
        panel_menu_pause.SetActive( false );
    }

    // Event: engine slider dragged ############################################################################################################################################
    public void EventSliderEngineDragged() {

        if( !Game.Is( GameState.Paused ) ) return;

        pause_slider_engine.enabled = false;
        pause_slider_engine.enabled = true;

        Game.Engine_volume = pause_slider_engine.normalizedValue;

        if( Game.Control != null ) Game.Control.ChangeSoundVolume();

        if( Game.Engine_volume == 0f ) image_on_off_engine.color = toggle_off_color;
        else image_on_off_engine.color = toggle_on_color;
    }
    
    // Event: voice slider dragged #############################################################################################################################################
    public void EventSliderVoiceDragged() {

        if( !Game.Is( GameState.Paused ) ) return;

        pause_slider_voice.enabled = false;
        pause_slider_voice.enabled = true;

        Game.Voice_volume = pause_slider_voice.normalizedValue;

        if( Game.Control != null ) Game.Control.ChangeSoundVolume();

        if( Game.Voice_volume == 0f ) image_on_off_voice.color = toggle_off_color;
        else image_on_off_voice.color = toggle_on_color;
    }
        
    // Event: sound slider dragged #############################################################################################################################################
    public void EventSliderSoundDragged() {

        if( !Game.Is( GameState.Paused ) ) return;

        pause_slider_sound.enabled = false;
        pause_slider_sound.enabled = true;

        Game.Sound_volume = pause_slider_sound.normalizedValue;

        if( Game.Control != null ) Game.Control.ChangeSoundVolume();

        if( Game.Sound_volume == 0f ) image_on_off_sound.color = toggle_off_color;
        else image_on_off_sound.color = toggle_on_color;
    }
    
    // Event: music slider dragged #############################################################################################################################################
    public void EventSliderMusicDragged() {

        if( !Game.Is( GameState.Paused ) ) return;

        pause_slider_music.enabled = false;
        pause_slider_music.enabled = true;

        Game.Music_volume = pause_slider_music.normalizedValue;

        if( Game.Control != null ) Game.Control.ChangeSoundVolume();

        if( Game.Music_volume == 0f ) image_on_off_music.color = toggle_off_color;
        else image_on_off_music.color = toggle_on_color;
    }

    // Event: vertical control's toggle pressed ################################################################################################################################
    public void EventToggleVerticalPressed() {

        if( !Game.Is( GameState.Paused ) ) return;

        toggle_vertical_control.enabled = false;
        toggle_vertical_control.enabled = true;

        Game.Use_vertical_control = toggle_vertical_control.isOn;

        if( toggle_vertical_control.isOn ) {

            image_on_off_vertical.color = toggle_on_color;
            Game.Input_control.EnableVerticalControl();
        }

        else {

            image_on_off_vertical.color = toggle_off_color;
            Game.Input_control.DisableVerticalControl();
        }
    }

    // Event: horizontal control's toggle pressed ##############################################################################################################################
    public void EventToggleHorizontalPressed() {

        if( !Game.Is( GameState.Paused ) ) return;

        toggle_horizontal_control.enabled = false;
        toggle_horizontal_control.enabled = true;

        Game.Use_horizontal_control = toggle_horizontal_control.isOn;

        if( toggle_horizontal_control.isOn ) {

            image_on_off_horizontal.color = toggle_on_color;
            Game.Input_control.EnableHorizontalControl();
        }

        else {

            image_on_off_horizontal.color = toggle_off_color;
            Game.Input_control.DisableHorizontalControl();
        }
    }
    
    // Event: radar control's toggle pressed ###################################################################################################################################
    public void EventToggleRadarPressed() {

        
        // Нужно изменить название, убрать радар, и по нижеследующей аналогии заменить на другую кнопку (радар мы перенесли на панель полёта)
/*

        if( !Game.Is( GameState.Paused ) ) return;

        toggle_radar_using.enabled = false;
        toggle_radar_using.enabled = true;

        Game.Use_radar = toggle_radar_using.isOn;
        if( !Game.Use_radar ) Game.Player_radar.Disable();

        if( toggle_radar_using.isOn ) image_on_off_radar.color = toggle_on_color;
        else image_on_off_radar.color = toggle_off_color;*/
    }
    
    // Event: detector control's toggle pressed ################################################################################################################################
    public void EventToggleDetectorPressed() {

        // Нужно заменить название и назначение этой кнопки, поскольку освободилось место для детектора
    }
    
    // Событие: когда кнопка мыши отпущена от ползунка слайдера ################################################################################################################
    public void EventSliderUp( int audio_type ) {

        if( audio_source == null ) return;

        AudioType type = (AudioType) audio_type;

        switch( type ) {

            case AudioType.Engine:

                audio_source.clip = Game.Control.Engine_audio_source.clip;
                if( audio_source.clip == null ) audio_source.clip = clip_check_engine;
                audio_source.volume = Game.Engine_volume * silence_slider_rate;
                break;

            case AudioType.Music:

                audio_source.clip = Game.Control.Music_audio_source.clip;
                if( audio_source.clip == null ) audio_source.clip = clip_check_music;
                audio_source.volume = Game.Music_volume * silence_slider_rate;
                break;

            case AudioType.Sound:

                audio_source.clip = Game.Control.Sound_audio_source.clip;
                if( audio_source.clip == null ) audio_source.clip = clip_check_sound;
                audio_source.volume = Game.Sound_volume * silence_slider_rate;
                break;

            case AudioType.Voice:

                audio_source.clip = Game.Control.Voice_audio_source.clip;
                if( audio_source.clip == null ) audio_source.clip = clip_check_voice;
                audio_source.volume = Game.Voice_volume * silence_slider_rate;
                break;
        }

        audio_source.time = audio_source.clip.length - ((audio_check_time >= audio_source.clip.length) ? audio_source.clip.length : audio_check_time);
        audio_source.Play();
    }

    // Event: shield button pressed ############################################################################################################################################
    public void EventButtonShieldPressed() {

        // Исключаем зависание подсветки
        shield.button.interactable = false;

        if( Game.Player.Shield.Is_active ) Game.Player.Shield.DeactivateProtection();
        else if( Game.Player.Shield.Is_ready ) Game.Player.Shield.ActivateProtection();

        // При любом раскладе кнопка неактивна (либо работает щит, либо он дозаряжается)
        RefreshShieldIndicator( false );
    }
    
    // Event: radar button pressed #############################################################################################################################################
    public void EventButtonRadarPressed() {

        // Исключаем зависание подсветки
        radar.button.interactable = false;

        Game.Radar.SwitchToNextMode();
        Game.Radar.Enable();

        // При любом раскладе кнопка активна, только переключился режим (поскольку нажали, значит кнопки в целом активны)
        RefreshRadarIndicator( true );
    }
    
    // Event: navigation button is pressed #####################################################################################################################################
    public void EventButtonNavigatorPressed() {

        // Исключаем зависание подсветки
        navigator.button.interactable = false;

        Game.Navigator.SwitchToNextMode();
        Game.Navigator.Enable();

        // При любом раскладе кнопка активна, только переключился режим (поскольку нажали, значит кнопки в целом активны)
        RefreshNavigatorIndicator( true );
    }

    // Event: inventory button pressed #########################################################################################################################################
    public void EventButtonMapPressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        map.button.interactable = false;
        map.button.interactable = true;
       
        RefreshMapIndicator( true );
    }
    
    // Event: inventory button pressed #########################################################################################################################################
    public void EventButtonInventoryPressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        inventory.button.interactable = false;
        inventory.button.interactable = true;

        panel_inventory.SetActive( true );
        
        Game.Pause();
    }
    
    // Event: station button pressed ###########################################################################################################################################
    public void EventButtonStationPressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        service_station.button.interactable = false;
        service_station.button.interactable = true;
              
        RefreshServiceIndicators( true );
    }
    
    // Event: repair button is pressed #########################################################################################################################################
    public void EventButtonRepairHullPressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        service_hull.button.interactable = false;
        service_hull.button.interactable = true;
        RefreshServiceIndicators( true );

        if( !Game.Player.At_station ) return;

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_repair, Game.Sound_volume );

        Game.Money -= Game.Player.Station.Hull_price;

        Game.Player.Ship.Hull_durability.Available += Game.Player.Ship.Hull_durability.Unit_size * Game.Player.Station.Hull_units;

        RefreshMoneyIndicator( true );
        RefreshServiceIndicators( true );
        RefreshHullIndicator( true );
    }
       
    // Event: fill button is pressed ###########################################################################################################################################
    public void EventButtonBuyFuelPressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        service_fuel.button.interactable = false;
        service_fuel.button.interactable = true;
        RefreshServiceIndicators( true );

        if( !Game.Player.At_station ) return;

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_fill, Game.Sound_volume );

        Game.Money -= Game.Player.Station.Fuel_price;

        Game.Player.Ship.Fuel_capacity.Available += Game.Player.Ship.Fuel_capacity.Unit_size * Game.Player.Station.Fuel_units;

        RefreshMoneyIndicator( true );
        RefreshServiceIndicators( true );
        RefreshFuelIndicator( true );
    }

    // Event: repair button is pressed #########################################################################################################################################
    public void EventButtonTradePressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        service_trade.button.interactable = false;
        service_trade.button.interactable = true;
        RefreshServiceIndicators( true );

        if( !Game.Player.At_station ) return;

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_sell, Game.Sound_volume );

        Game.Canvas.RefreshMoneyIndicator( true );

        RefreshHoldIndicator( true );
        RefreshMoneyIndicator( true );
        RefreshServiceIndicators( true );
    }
    
    // Event: repair button is pressed #########################################################################################################################################
    public void EventButtonRepairEnginePressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        service_engine.button.interactable = false;
        service_engine.button.interactable = true;
        RefreshServiceIndicators( true );

        if( !Game.Player.At_station ) return;

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_repair, Game.Sound_volume );

        Game.Money -= Game.Player.Station.Engine_price;

        Game.Player.Ship.Engine_thrust.Available += Game.Player.Ship.Engine_thrust.Unit_size * Game.Player.Station.Engine_units;

        RefreshMoneyIndicator( true );
        RefreshServiceIndicators( true );
        RefreshEngineIndicator( true );
    }
    
    // Event: repair button is pressed #########################################################################################################################################
    public void EventButtonUpgradePressed() {

        // Исключаем зависание подсветки и обновляем индикаторы
        service_upgrade.button.interactable = false;
        service_upgrade.button.interactable = true;
        RefreshServiceIndicators( true );

        if( !Game.Player.At_station ) return;

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_buy, Game.Sound_volume );

        RefreshFlightIndicators( true );
        RefreshServiceIndicators( true );
        RefreshAutolandingIndicator( false );
    }
    
    // Event: autolanding button pressed #######################################################################################################################################
    public void EventButtonAutolandingPressed() {

        // Исключаем зависание подсветки
        autolanding.button.interactable = false;

        // If need to on autolanding mode
        if( Game.Player.At_station || Game.Player.Is_autolanding ) return;

        Game.Player.Ship.Autolanding_amount.Available -= Game.Player.Ship.Autolanding_amount.Unit_size;

        if( audio_source != null ) audio_source.PlayOneShot( clip_press_button, Game.Sound_volume );

        Game.Canvas.RefreshMoneyIndicator( true );

        RefreshAutolandingIndicator( false );

        StartCoroutine( Game.Player.AutoLanding() );
    }
    
    // Animation event: make pause if using brief ##############################################################################################################################
    public void EventAnimationFadeInRunPause() {

        // Запускается пауза, чтобы при необходимости показать бриф (если брифа нет, то она далее сразу выключается по новому событию)
        if( Game.Is( GameState.Briefing ) ) {

            Game.Input_control.DisableControl();
            Game.Pause();
        }
    }
    
    // Animation event: <fade in> is started ###################################################################################################################################
    public void EventAnimationFadeInStarted() {

        RefreshFlightIndicators( false );

        // Если брифа нет, то сразу же отключаем паузу
        if( !Game.Is( GameState.Briefing ) ) Game.Resume();

        // Запускаем анимацию выхода из затемнения
        animator_canvas.SetBool( animator_ID_canvas_show, true );
    }

    // Animation event: <fade in> is complete ##################################################################################################################################
    public void EventAnimationFadeInComplete() {

        Game.ResetState( GameState.Loading | GameState.Starting);

        RefreshFlightIndicators( true );
    }
    
    // Animation event: <fade out> is started ##################################################################################################################################
    public void EventAnimationFadeOutStarted() {

        Game.Input_control.DisableControl();

        SetFlightPanel( false );
        SetStationPanel( false );
    }
    
    // Animation event: <fade out> is complete #################################################################################################################################
    public void EventAnimationFadeOutComplete() {

        if( Game.Is( GameState.Complete ) ) LoadLevel( (int) LevelType.Level_Menu );
        else if( Game.Is( GameState.Restarting ) ) LoadLevel( SceneManager.GetActiveScene().buildIndex );
    }
    
    // Restart the level #######################################################################################################################################################
    private void LoadLevel( int level ) {

        SceneManager.LoadScene( level, LoadSceneMode.Single );
    }
    
    // Обработка события нажатия клавиши ECS на клавиатуре #####################################################################################################################
    public void EventKeyEscapePressed() {

        // Если игра уже на паузе, то действие выбирается в зависимости от текущей включенной панели, вызвавшей паузу
        if( Game.Is( GameState.Paused ) ) {

            if( panel_menu_pause.activeInHierarchy ) EventButtonPlayPressed();
            else if( panel_inventory.activeInHierarchy ) panel_inventory.GetComponentInChildren<Inventory>().EventButtonReturnPressed();
        }

        // Если игра не на паузе, то по ESC можно вызвать только меню паузы
        else EventButtonPausePressed();
    }

    // Обработка событий нажатия клавиш на клавиатуре ##########################################################################################################################
    public void EventKeySpacePressed() { }
    public void EventKeyMinusPressed() { }
    public void EventKeyPlusPressed() { }
    public void EventKeyTabPressed() { }
    public void EventKeyDPressed() { }
    public void EventKeyZPressed() { }
    public void EventKeyPPressed() { }
}