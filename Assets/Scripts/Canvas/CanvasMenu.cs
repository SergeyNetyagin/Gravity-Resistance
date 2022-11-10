using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasMenu : MonoBehaviour {

    [Header( "НАСТРОЙКА ОКОН ДЛЯ ВЫБОРА УРОВНЕЙ" )]
    [SerializeField]
    [Tooltip( "Панель, внутри которой создаются окна для выбора уровней" )]
    private RectTransform panel_levels_content;

    [SerializeField]
    [Tooltip( "Шаблон окна меню для выбора уровня" )]
    private GameObject level_window_prefab;

    private Level[] level_prefabs;
    private LevelWindow[] level_windows;
    public int Levels_amount { get { return level_windows.Length; } }

    [Header( "НАСТРОЙКА ОКОН ДЛЯ ВЫБОРА КОРАБЛЕЙ" )]
    [SerializeField]
    [Tooltip( "Панель, внутри которой создаются окна для выбора кораблей" )]
    private RectTransform panel_ships_content;

    [SerializeField]
    [Tooltip( "Шаблон окна меню для выбора корабля" )]
    private GameObject ship_window_prefab;

    private Ship[] ship_prefabs;
    private ShipWindow[] ship_windows;
    public int Ships_amount { get { return ship_windows.Length; } }

    [Header( "НАСТРОЙКА ОКНА ГЛАВНОГО МЕНЮ" )]
    [SerializeField]
    private MenuWindow menu_window;

    [SerializeField]
    [Tooltip( "Точка, к которой прикрепляется модель текущего активного корабля (он постоянно виден в правом верхнем углу экрана рядом с названием активного корабля)" )]
    private Transform ship_model_transform;

    [SerializeField]
    [Tooltip( "Скорость вращения корабля во время демонстрации вокруг вертикальной оси; по умолчанию = 5" )]
    private float ship_rotation_speed = 5f;

    [SerializeField]
    [Tooltip( "Поле для отображения текущих средств, которыми располагает игрок в данный момент" )]
    private EffectiveText text_money;

    [SerializeField]
    [Tooltip( "Поле для отображения текущего активного корабля игрока, на котором он будет выполнять полёты после выбора уровня" )]
    private EffectiveText text_ship;

    [Header( "НАСТРОЙКА ПАНЕЛИ МЕНЮ ВЫБОРА" )]
    [SerializeField]
    private GameObject panel_choice;

    [SerializeField]
    private Button button_yes;

    [SerializeField]
    private Button button_no;

    [Space( 10 )]
    [SerializeField]
    private EffectiveText 
        text_question; [SerializeField] private EffectiveText 
        text_description,
        text_yes,
        text_no;

    private bool choice_panel_is_activated = false;
    private bool escape_key_pressed = false;

    private Ship current_ship;

    public int Available_ships { get {

            int amount = 0;

            for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Is_available ) amount++;

            return amount;
    } }

    public bool Has_current_ship { get {

            for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Is_active ) return true;

            return false;
    } }

    public int Current_ship_index { get {

            for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Is_active ) return i;

            return 0;
    } }

    public ShipWindow Current_ship_window { get {

            for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Is_active ) return ship_windows[i];

            return null;
    } }

    public Ship Current_ship { get {

            for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Is_active ) return ship_windows[i].Ship;

            return null;
    } }

    public ShipType Current_ship_type { get {

            for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Is_active ) return ship_windows[i].Ship.Type;

            return ShipType.Ship_unknown;
    } }

    public int ShipIndex( ShipType type ) {

        for( int i = 0; i < ship_prefabs.Length; i++ ) if( ship_prefabs[i].Type == type ) return i;

        return 0;
    }

    public string ShipTypeKey( ShipType type ) {

        for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Ship.Type == type ) return ship_windows[i].Ship.Type_key;

        return string.Empty;
    }

    public void RefreshAllShips() {

        for( int i = 0; i < ship_windows.Length; i++ ) ship_windows[i].Refresh();
    }

    public void ShowAllShips() {

        for( int i = 0; i < ship_windows.Length; i++ ) ship_windows[i].Ship.gameObject.SetActive( true );
    }

    public void HideAllShips() {

        for( int i = 0; i < ship_windows.Length; i++ ) ship_windows[i].Ship.gameObject.SetActive( false );
    }

    public int Opened_levels { get {

            int amount = 0;

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Is_opened ) amount++;

            return amount;
    } }

    public bool Has_current_level { get {

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Is_active ) return true;

            return false;
    } }

    public int Current_level_index { get {

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Is_active ) return i;

            return 0;
    } }

    public LevelWindow Current_level_window { get {

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Is_active ) return level_windows[i];

            return null;
    } }

    public Level Current_level { get {

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Is_active ) return level_windows[i].Level;

            return null;
    } }

    public LevelType Current_level_type { get {

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Is_active ) return level_windows[i].Level.Type;

            return LevelType.Level_Quit;
    } }

    public void RefreshAllLevels() {

        for( int i = 0; i < level_windows.Length; i++ ) level_windows[i].Refresh();
    }

    public int LevelIndex( LevelType type ) {

            for( int i = 0; i < level_prefabs.Length; i++ ) if( level_prefabs[i].Type == type ) return i;

            return 0;
    }

    public string LevelTypeKey( LevelType type ) {

            for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Level.Type == type ) return level_windows[i].Level.Type_key;

            return string.Empty;
    }

    public void RefreshShipAdvertising() { CreateShipAdvertising( ref current_ship, Current_ship_type, ship_model_transform ); }
    public void RefreshCurrentShipName() { text_ship.Rewrite( (current_ship != null) ? Game.Localization.GetTextValue( current_ship.Type_key ) : string.Empty); }
    public void RefreshMoneyValue() { text_money.RewriteSeparatedInt( Mathf.FloorToInt( Game.Money ) ); }

    private float slide_width = 1920f;
    private float slide_height = 1080f;

    private Transform panel_levels_transform;
    private Transform panel_ships_transform;
 
    private CanvasScrollControl scroll_control;

    private bool is_saved = false;
               
    // Use this for initialization #############################################################################################################################################
	void Awake() {

        level_prefabs = Game.Control.Level_prefabs;
        ship_prefabs = Game.Control.Ship_prefabs;

        scroll_control = GetComponentInChildren<CanvasScrollControl>();

        // /////////////////////////////////////////////////
        // Готовим канвас к принятию окон кораблей и уровней
        // /////////////////////////////////////////////////

        level_windows = new LevelWindow[ level_prefabs.Length ];
        ship_windows = new ShipWindow[ ship_prefabs.Length ];

        panel_levels_transform = panel_levels_content.GetComponent<Transform>();
        panel_ships_transform = panel_ships_content.GetComponent<Transform>();

        slide_width = level_window_prefab.GetComponent<RectTransform>().rect.width;
        slide_height = level_window_prefab.GetComponent<RectTransform>().rect.height;

        panel_levels_content.sizeDelta = new Vector2( slide_width * level_prefabs.Length, slide_height );
        panel_ships_content.sizeDelta = new Vector2( slide_width * ship_prefabs.Length, slide_height );

        // /////////////////////////////////
        // Инициализируем окно главного меню
        // /////////////////////////////////

        menu_window.SetMenuReference( this );
        menu_window.SetScrollReference( scroll_control );
        menu_window.Refresh();

        // ///////////////////////////////////////////////
        // Создаём окна для уровней из префаба для уровней
        // ///////////////////////////////////////////////

        for( int i = 0; i < level_prefabs.Length; i++ ) {

            LevelWindow level_window = (Instantiate( level_window_prefab, panel_levels_transform ) as GameObject).GetComponent<LevelWindow>();
            RectTransform window_rect = level_window.GetComponent<RectTransform>();

            level_window.Image_arrow_up.SetActive( false );
            if( i == 0 ) level_window.Image_arrow_left.SetActive( false );
            if( i == (level_prefabs.Length - 1) ) level_window.Image_arrow_right.SetActive( false );

            window_rect.localScale = Vector3.one;
            window_rect.localPosition = new Vector3( (i * slide_width), (- slide_height * 0.5f), 0f );

            level_window.Level = level_prefabs[i].GetComponent<Level>();
            level_window.Button_take_level.GetComponent<Image>().sprite = level_window.Level.Picture;

            LoadGame.Load( level_window.Level );

            level_windows[i] = level_window;
            level_windows[i].SetMenuReference( this );
            level_windows[i].SetScrollReference( scroll_control );
        }

        // /////////////////////////////////////////////////
        // Создаём окна для кораблей из префаба для кораблей
        // /////////////////////////////////////////////////

        for( int i = 0; i < ship_prefabs.Length; i++ ) {

            ShipWindow ship_window = (Instantiate( ship_window_prefab, panel_ships_transform ) as GameObject).GetComponent<ShipWindow>();
            RectTransform window_rect = ship_window.GetComponent<RectTransform>();

            ship_window.Image_arrow_down.SetActive( false );
            if( i == 0 ) ship_window.Image_arrow_left.SetActive( false );
            if( i == (ship_prefabs.Length - 1) ) ship_window.Image_arrow_right.SetActive( false );

            window_rect.localScale = Vector3.one;
            window_rect.localPosition = new Vector3( (i * slide_width), (- slide_height * 0.5f), 0f );

            CreateShipAdvertising( ref ship_window.Ship, ship_prefabs[i].Type, ship_window.Ship_model_transform );

            LoadGame.Load( ship_window.Ship );

            ship_windows[i] = ship_window;
            ship_windows[i].SetMenuReference( this );
            ship_windows[i].SetScrollReference( scroll_control );
        }

        // Инициализируем рекламный образец текущего корабля (находится в правом верхнем углу экрана)
        CreateShipAdvertising( ref current_ship, Current_ship_type, ship_model_transform );

        // Для случая первого запуска инициализируем активные уровень и корабль
        if( !Has_current_level ) FindCurrentLevel();
        if( !Has_current_ship ) FindCurrentShip();

        // Обновляем все окна
        RefreshAllLevels();
        RefreshAllShips();

        // Обновляем данные: денежные средства и название текущего корабля
        RefreshMoneyValue();
        RefreshCurrentShipName();
    }
    
    // Use this for initialization #############################################################################################################################################
	void Start() {

        DeactivateChoicePanel();
	}
	
	// Update is called once per frame #########################################################################################################################################
	void Update () {

        escape_key_pressed = Input.GetKeyUp( KeyCode.Escape );

        // Проверка нажатия клавиш ESC: если нечётный раз - меню подтверждения
        if( escape_key_pressed && !choice_panel_is_activated ) ActivateChoicePanel();

        // Проверка нажатия клавиш ESC: если чётный раз - возврат в игру
        else if( escape_key_pressed && choice_panel_is_activated ) DeactivateChoicePanel();
	}
    
    // Find new active level, if former active level was changed ###############################################################################################################
    public void FindCurrentLevel() {

        for( int i = level_windows.Length - 1; i >= 0; i-- ) {

            if( level_windows[i].Is_opened ) {

                level_windows[i].Level.SetActive( true );
                break;
            }
        }
    }

    // Find new active ship, if former active ship was sold ####################################################################################################################
    public void FindCurrentShip() {

        for( int i = ship_windows.Length - 1; i >= 0; i-- ) {

            if( ship_windows[i].Is_available ) {

                ship_windows[i].Ship.SetActive( true );
                break;
            }
        }
    }
    
    // Обновляет модель корабля у заданного родителя ###########################################################################################################################
    private void CreateShipAdvertising( ref Ship ship, ShipType type, Transform model_transform ) {

        // Уничтожаем модель прежнего активного корабля, и создаём новую модель
        if( ship != null ) Destroy( ship.gameObject );

        ship = (Instantiate( ship_prefabs[ ShipIndex( type ) ].gameObject, model_transform ) as GameObject).GetComponent<Ship>();
        ship.transform.localEulerAngles = Vector3.zero;
        ship.transform.localPosition = Vector3.zero;
        ship.transform.localScale = Vector3.one;

        // Выключаем звук двигателя корабля и убираем лишние компоненты
        if( ship.GetComponent<ShieldControl>() != null ) Destroy( ship.GetComponent<ShieldControl>() );
        if( ship.GetComponent<SoundEffects>() != null ) Destroy( ship.GetComponent<SoundEffects>() );
        if( ship.GetComponent<SpaceBody>() != null ) Destroy( ship.GetComponent<SpaceBody>() );
        if( ship.GetComponent<AudioSource>() != null ) Destroy( ship.GetComponent<AudioSource>() );

        // Убираем компоненты двигателей, чтобы их частицы не сталкивались с объектами
        JetCollision[] jets = ship.GetComponentsInChildren<JetCollision>( true );
        for( int i = 0; i < jets.Length; i++ ) Destroy( jets[i] );

        // Добавляем скрипт вращения к модели корабля
        ship.gameObject.AddComponent<AnimationRotation>();
        ship.GetComponent<AnimationRotation>().SetSpeedOnY( ship_rotation_speed );

        // Проводим стартовую инициализацию корабля, чтобы затем включить сопло двигателя вертикалной тяги
        ship.StartingInitialization( model_transform.gameObject.layer );
        ship.ControlJetVertical( 50f );
    }

    // Активация окна выбора ###################################################################################################################################################
    private void ActivateChoicePanel() {

        HideAllShips();

        text_question.Rewrite( Game.Localization.GetTextValue( "Game.Quit.Question" ) );
        text_description.Rewrite( Game.Localization.GetTextValue( "Game.Quit.Description" ) );
        text_yes.Rewrite( Game.Localization.GetTextValue( "Game.Quit.Exit" ) );
        text_no.Rewrite( Game.Localization.GetTextValue( "Game.Quit.Return" ) );

        choice_panel_is_activated = true;
        panel_choice.SetActive( true );
    }

    // Активация окна выбора ###################################################################################################################################################
    private void DeactivateChoicePanel() {

        ShowAllShips();

        choice_panel_is_activated = false;
        panel_choice.SetActive( false );
    }
    
    // Событие, вызываемое по кнопке YES меню подтверждения выбора #############################################################################################################
    public void EventButtonYesPressed() {

        // Перед запуском анимации затемнения экрана указываем признак завершения приложения
        Game.Current_level = LevelType.Level_Quit;
        GetComponent<Animator>().SetBool( "Menu_show", false );
    }

    // Событие, вызываемое по кнопке NO меню подтверждения выбора ##############################################################################################################
    public void EventButtonNoPressed() {

        DeactivateChoicePanel();
    }
            
    // Запуск процесса загрузки игрового уровня или завершения приложения ######################################################################################################
    public void AnimationEventMenuFadeOutIsComplete() {

        // Была нажата клавиша ESC: завершаем игру
        if( Game.Current_level == LevelType.Level_Quit ) {

            #if UNITY_EDITOR
            if( Application.isEditor ) UnityEditor.EditorApplication.isPlaying = false;
            else
            #endif

            Application.Quit();
        }

        // Иначе был выбран какой-то игровой уровень: мы его загружаем
        else SceneManager.LoadScene( (int) LevelType.Level_Loading, LoadSceneMode.Single );
    }

    // Сохранение данных перед загрузкой уровня ################################################################################################################################
    void OnDisable() {

        if( is_saved ) return;
        else is_saved = true;

        // Далее сохраняются уровни и корабли (на уровне есть проверки, нужно ли сохранять или нет); а общие параметры сохраняются в GameControl
        for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Level.Was_changed ) SaveGame.Save( level_windows[i].Level );
        for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Ship.Was_changed ) SaveGame.Save( ship_windows[i].Ship );
    }
    
    // Сохранение данных перед выходом из игры #################################################################################################################################
    void OnApplicationQuit() {

        if( is_saved ) return;
        else is_saved = true;

        // Далее сохраняются уровни и корабли (на уровне есть проверки, нужно ли сохранять или нет); а общие параметры сохраняются в GameControl
        for( int i = 0; i < level_windows.Length; i++ ) if( level_windows[i].Level.Was_changed ) SaveGame.Save( level_windows[i].Level );
        for( int i = 0; i < ship_windows.Length; i++ ) if( ship_windows[i].Ship.Was_changed ) SaveGame.Save( ship_windows[i].Ship );
    }
}