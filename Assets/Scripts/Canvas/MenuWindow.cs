using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// ///////////////////////////////////////////////////////////////////
// Позволяет отобразить окно настроек и управления в главном меню игры
// ///////////////////////////////////////////////////////////////////

public class MenuWindow : MonoBehaviour {

    [Header( "ОСНОВНЫЕ ЭЛЕМЕНТЫ ИНТЕРФЕЙСА" )]
    [SerializeField]
    [Tooltip( "Кнопка выбора уровня" )]
    private Button button_begin_flight;
    public Button Button_begin_flight { get { return button_begin_flight; } }

    [SerializeField]
    [Tooltip( "Кнопка настроек" )]
    private Button button_settings;
    public Button Button_settings { get { return button_settings; } }

    [SerializeField]
    [Tooltip( "Кнопка Gravity Resistance" )]
    private Button button_Gravity_Resistance;
    public Button Button_Gravity_Resistance { get { return button_Gravity_Resistance; } }

    [SerializeField]
    [Tooltip( "Кнопка показа интродукции" )]
    private Button button_introduction;
    public Button Button_introduction { get { return button_introduction; } }

    [SerializeField]
    [Tooltip( "Кнопка выбора корабля" )]
    private Button button_select_ship;
    public Button Button_select_ship { get { return button_select_ship; } }

    [SerializeField]
    [Tooltip( "Рамка (окно) для предотвращения доступа к управлению интерфейсом (необходима, когда выбраны пункты настроек или интродукции)" )]
    private Image lock_frame;

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

    [Header( "ТЕКСТОВЫЕ ПОЛЯ ОКНА ГЛАВНОГО МЕНЮ" )]
    [SerializeField]
    private EffectiveText
        text_begin_flight; [SerializeField] private EffectiveText
        text_settings,
        text_Gravity_Resistance,
        text_introduction,
        text_select_ship;

    private CanvasMenu canvas_menu;
    public void SetMenuReference( CanvasMenu canvas_menu ) { this.canvas_menu = canvas_menu; }

    private CanvasScrollControl scroll_control;
    public void SetScrollReference( CanvasScrollControl scroll_control ) { this.scroll_control = scroll_control; }

    public void EventButtonBeginFlightPressed() { button_begin_flight.enabled = false; button_begin_flight.enabled = true; scroll_control.EventButtonUpPressed( ScrollingSource.Menu ); }
    public void EventButtonSelectShipPressed() { button_select_ship.enabled = false; button_select_ship.enabled = true; scroll_control.EventButtonDownPressed( ScrollingSource.Menu ); }
    public void EventButtonUpPressed() { button_arrow_up.enabled = false; button_arrow_up.enabled = true; scroll_control.EventButtonUpPressed( ScrollingSource.Menu ); }
    public void EventButtonDownPressed() { button_arrow_down.enabled = false; button_arrow_down.enabled = true; scroll_control.EventButtonDownPressed( ScrollingSource.Menu ); }
    
    // Use this for initialization #############################################################################################################################################
	void Start() {

        button_arrow_up = image_arrow_up.GetComponentInChildren<Button>( true );
        button_arrow_down = image_arrow_down.GetComponentInChildren<Button>( true );
    }

    // Check the level conditions ##############################################################################################################################################
    public void Refresh() {

        text_begin_flight.Rewrite( Game.Localization.GetTextValue( "Menu.Main.Levels" ) );
        text_settings.Rewrite( Game.Localization.GetTextValue( "Menu.Main.Settings" ) );
        text_Gravity_Resistance.Rewrite( Game.Localization.GetTextValue( "Menu.Main.Game" ) );
        text_introduction.Rewrite( Game.Localization.GetTextValue( "Menu.Main.Introduction" ) );
        text_select_ship.Rewrite( Game.Localization.GetTextValue( "Menu.Main.Ships" ) );
    }

	// Нажата кнопка основных игровых настроек #################################################################################################################################
	public void EventButtonSettingsPressed() {

        button_settings.enabled = false;
        button_settings.enabled = true;

        Refresh();
    }

	// Нажата кнопка названия игры (по этой кнопке должны проигрываться титры) #################################################################################################
	public void EventButtonGravityResistancePressed() {

        button_Gravity_Resistance.enabled = false;
        button_Gravity_Resistance.enabled = true;

        Refresh();
    }

	// Нажата кнопка показа интродукции ########################################################################################################################################
	public void EventButtonIntroductionPressed() {

        button_introduction.enabled = false;
        button_introduction.enabled = true;

        Refresh();
    }
}