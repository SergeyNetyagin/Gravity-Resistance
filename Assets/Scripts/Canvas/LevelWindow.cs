using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// //////////////////////////////////////////////////////////////////
// Позволяет отобразить параметры игрового уровня в главном меню игры
// //////////////////////////////////////////////////////////////////

public class LevelWindow : MonoBehaviour {

    [Header( "ОСНОВНЫЕ ЭЛЕМЕНТЫ ИНТЕРФЕЙСА" )]
    [SerializeField]
    [Tooltip( "Кнопка выбора игрового уровня: по её нажатию игрок попадает на игровой уровень" )]
    private Button button_take_level;
    public Button Button_take_level { get { return button_take_level; } }

    [SerializeField]
    [Tooltip( "Цвет кнопки для активного уровня" )]
    private Color active_color = new Color( 1f, 1f, 1f, 1f );

    [SerializeField]
    [Tooltip( "Цвет кнопки для неактивного или некупленного уровня" )]
    private Color inactive_color = new Color( 0.4f, 0.4f, 0.4f, 1f );

    [SerializeField]
    [Tooltip( "Кнопка покупки доступного уровня: по её нажатию игрок приобретает лицензию на работу на данном уровне и может после этого играть на нём" )]
    private GameObject image_button_buy_license;
    private Button button_buy_license;

    [SerializeField]
    [Tooltip( "Рамка (окно) для предотвращения доступа к управлению интерфейсом (необходима, когда корабль недоступен для покупки)" )]
    private Image lock_frame;

    [SerializeField]
    [Tooltip( "Значок замка (необходим, когда корабль нельзя купить из-за нехватки средств)" )]
    private Image lock_icon;

    [SerializeField]
    [Tooltip( "Объект управления кнопкой <Стрелка вверх> (активация/деактивация производится контроллером канваса)" )]
    private GameObject image_arrow_up;
    public GameObject Image_arrow_up { get { return image_arrow_up; } }
    private Button button_arrow_up;

    [SerializeField]
    [Tooltip( "Объект управления кнопкой <Стрелка вниз> (активация/деактивация производится контроллером канваса)" )]
    private GameObject image_arrow_down;
    public GameObject Image_arrow_down { get { return image_arrow_down; } }
    private Button button_arrow_down;

    [SerializeField]
    [Tooltip( "Объект управления кнопкой <Стрелка влево> (активация/деактивация производится контроллером канваса)" )]
    private GameObject image_arrow_left;
    public GameObject Image_arrow_left { get { return image_arrow_left; } }
    private Button button_arrow_left;

    [SerializeField]
    [Tooltip( "Объект управления кнопкой <Стрелка вправо> (активация/деактивация производится контроллером канваса)" )]
    private GameObject image_arrow_right;
    public GameObject Image_arrow_right { get { return image_arrow_right; } }
    private Button button_arrow_right;

    [Header( "НАСТРОЙКИ МЕНЮ ПОДТВЕРЖДЕНИЯ ПОКУПКИ / ПРОДАЖИ" )]
    [SerializeField]
    private GameObject panel_choice;

    [SerializeField]
    private EffectiveText text_yes;

    [SerializeField]
    private EffectiveText text_no;

    [Header( "ТЕКСТОВЫЕ ПОЛЯ ОКНА УРОВНЯ" )]
    [SerializeField]
    private EffectiveText
        text_name; [SerializeField] private EffectiveText
        text_description,
        text_gravity_type,
        text_gravity_force,
        text_recommended_ship,
        text_opening_cost,
        text_buy_license;

    private bool choice_panel_is_activated = false;

    public bool Is_opened { get { return Level.Is_opened; } }
    public void SetOpened( bool state ) { Level.SetOpened( state ); } 

    public bool Is_active { get { return Level.Is_active; } }
    public void SetActive( bool state ) { Level.SetActive( state ); }

    private bool is_buy_accessible;

    [HideInInspector]
    public Level Level;

    private CanvasMenu canvas_menu;
    public void SetMenuReference( CanvasMenu canvas_menu ) { this.canvas_menu = canvas_menu; }

    private CanvasScrollControl scroll_control;
    public void SetScrollReference( CanvasScrollControl scroll_control ) { this.scroll_control = scroll_control; }

    public void EventButtonUpPressed() { button_arrow_up.enabled = false; button_arrow_up.enabled = true; scroll_control.EventButtonUpPressed( ScrollingSource.Level ); }
    public void EventButtonDownPressed() { button_arrow_down.enabled = false; button_arrow_down.enabled = true; scroll_control.EventButtonDownPressed( ScrollingSource.Level ); }
    public void EventButtonLeftPressed() { button_arrow_left.enabled = false; button_arrow_left.enabled = true; scroll_control.EventButtonLeftPressed( ScrollingSource.Level ); }
    public void EventButtonRightPressed() { button_arrow_right.enabled = false; button_arrow_right.enabled = true; scroll_control.EventButtonRightPressed( ScrollingSource.Level ); }

	// Use this for initialization #############################################################################################################################################
	void Start() {

        button_buy_license = image_button_buy_license.GetComponentInChildren<Button>( true );

        button_arrow_up = image_arrow_up.GetComponentInChildren<Button>( true );
        button_arrow_down = image_arrow_down.GetComponentInChildren<Button>( true );
        button_arrow_left = image_arrow_left.GetComponentInChildren<Button>( true );
        button_arrow_right = image_arrow_right.GetComponentInChildren<Button>( true );
	}

    // Check the level conditions ##############################################################################################################################################
    public void Refresh() {

        DeactivateChoicePanel();

        // ///////////////////////////////
        // Обновление текстовой информации
        // ///////////////////////////////

        text_name.Rewrite( Game.Localization.GetTextValue( Level.Type_key ) );
        text_description.Rewrite( Game.Localization.GetTextValue( Level.Description_key ) );

        text_gravity_type.Rewrite( Game.Phrase_gravity_type ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        text_gravity_type.Append( (Level.Gravity_type == GravityType.Stable) ? Game.Phrase_gravity_stable : Game.Phrase_gravity_unstable );

        text_gravity_force.Rewrite( Game.Phrase_gravity_force ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        text_gravity_force.AppendDottedFloat( Level.Gravity_force ).Append( Game.Separator_space ).Append( "g" );

        text_recommended_ship.Rewrite( Game.Phrase_ship_recommended ).Append( Game.Separator_colon ).Append( Game.Separator_space );
        text_recommended_ship.Append( Game.Localization.GetTextValue( canvas_menu.ShipTypeKey( Level.Recommended_ship_type ) ) );

        // //////////////////////////////////////////////////////////////////////////////////
        // Обновление доступности уровня (активность и наличие управляется из других методов)
        // //////////////////////////////////////////////////////////////////////////////////

        is_buy_accessible = (Is_active || Is_opened) ? false : (Level.Opening_cost <= Game.Money);

        // /////////////////////////////////////////////////////////////////////
        // Обновление панелей и кнопок окна в соответствии с флагами доступности
        // /////////////////////////////////////////////////////////////////////

        // Если уровень активен, подсвечиваем его кнопку (кнопка остаётся интерактивной)
        if( Is_active ) {

            text_opening_cost.Rewrite( Game.Phrase_cost_free );

            button_take_level.GetComponent<Image>().color = active_color;
            button_take_level.interactable = true;

            text_buy_license.Rewrite( Game.Phrase_buy_license );
            image_button_buy_license.SetActive( false );

            lock_frame.gameObject.SetActive( false );
            lock_icon.gameObject.SetActive( false );
        }

        // Если уровень не является активным, но был открыт игроком ранее (уже известен), меняем только цвет кнопки (кнопка остаётся интерактивной)
        else if( Is_opened ) {

            text_opening_cost.Rewrite( Game.Phrase_cost_free );

            button_take_level.GetComponent<Image>().color = inactive_color;
            button_take_level.interactable = true;

            text_buy_license.Rewrite( Game.Phrase_buy_license );
            image_button_buy_license.SetActive( false );

            lock_frame.gameObject.SetActive( false );
            lock_icon.gameObject.SetActive( false );
        }

        // Если уровень ещё не открыт, но он готов к покупке, открываем окно, но кнопка выбора уровня пока неактивна
        else if( is_buy_accessible ) {

            text_opening_cost.Rewrite( Game.Phrase_cost_opening ).Append( Game.Separator_colon ).Append( Game.Separator_space );
            text_opening_cost.AppendSeparatedInt( Mathf.FloorToInt( Level.Opening_cost ) );

            button_take_level.GetComponent<Image>().color = inactive_color;
            button_take_level.interactable = false;

            text_buy_license.Rewrite( Game.Phrase_buy_license );
            image_button_buy_license.SetActive( true );

            lock_frame.gameObject.SetActive( false );
            lock_icon.gameObject.SetActive( false );
        }

        // В остальных случаях уровень пока нельзя открыть, и поэтому кнопка заблокирована, окно затемнено
        else {

            text_opening_cost.Rewrite( Game.Phrase_cost_opening ).Append( Game.Separator_colon ).Append( Game.Separator_space );
            text_opening_cost.AppendSeparatedInt( Mathf.FloorToInt( Level.Opening_cost ) );

            button_take_level.GetComponent<Image>().color = inactive_color;
            button_take_level.interactable = false;

            text_buy_license.Rewrite( Game.Phrase_buy_license );
            image_button_buy_license.SetActive( false );

            lock_frame.gameObject.SetActive( true );
            lock_icon.gameObject.SetActive( true );
        }
    }

	// If the button is pressed and enabled, load the appropriate level ########################################################################################################
	public void EventButtonOpenLevelPressed() {

        button_buy_license.enabled = false;
        button_buy_license.enabled = true;

        ActivateChoicePanel();
    }
    
    // If the button is pressed and enabled, load the appropriate level ########################################################################################################
	public void EventButtonStartLevelPressed() {

        if( Level != canvas_menu.Current_level ) {

            canvas_menu.Current_level.SetActive( false );
            SetActive( true );
            SetOpened( true );
            Refresh();
        }

        Game.Loading_level = Level.Type;
        Game.Playing_ship = canvas_menu.Current_ship.Type;

        canvas_menu.GetComponent<Animator>().SetBool( "Menu_show", false );
    }

    // Активация окна выбора ###################################################################################################################################################
    private void ActivateChoicePanel() {

        text_yes.Rewrite( Game.Pharse_level_buy_confirm );
        text_no.Rewrite( Game.Pharse_level_buy_decline );

        image_button_buy_license.SetActive( false );
        choice_panel_is_activated = true;
        panel_choice.SetActive( true );
    }

    // Активация окна выбора ###################################################################################################################################################
    private void DeactivateChoicePanel() {

        choice_panel_is_activated = false;
        panel_choice.SetActive( false );
        image_button_buy_license.SetActive( true );
    }

    // Событие, вызываемое по кнопке YES меню подтверждения выбора #############################################################################################################
    public void EventButtonYesPressed() {

        LevelWindow old_level_window = canvas_menu.Current_level_window;
        old_level_window.Level.SetActive( false );

        Game.Money -= Level.Opening_cost;
        canvas_menu.RefreshMoneyValue();

        SetActive( true );
        SetOpened( true );

        DeactivateChoicePanel();
        canvas_menu.RefreshAllShips();
        canvas_menu.RefreshAllLevels();
    }

    // Событие, вызываемое по кнопке NO меню подтверждения выбора ##############################################################################################################
    public void EventButtonNoPressed() {

        DeactivateChoicePanel();
    }
}