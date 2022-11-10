using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// /////////////////////////////////////////////////////////////////////
// Позволяет отобразить корабль и его характеристики в главном меню игры
// /////////////////////////////////////////////////////////////////////

public class ShipWindow : MonoBehaviour {

    [Header( "ОСНОВНЫЕ ЭЛЕМЕНТЫ ИНТЕРФЕЙСА" )]
    [SerializeField]
    [Tooltip( "Кнопка, на которой оботражается модель корабля: по её нажатию данный корабль становится активным (к ней прикрепляется модель корабля)" )]
    private Button button_take_ship;
    public Button Button_take_ship { get { return button_take_ship; } }

    [SerializeField]
    [Tooltip( "Цвет кнопки для активного корабля" )]
    private Color active_color = new Color( 1f, 1f, 1f, 1f );

    [SerializeField]
    [Tooltip( "Цвет кнопки для неактивного или некупленного корабля" )]
    private Color inactive_color = new Color( 0.135f, 0.555f, 0.605f, 1f );

    [SerializeField]
    [Tooltip( "Основа изображения кнопки продажи или покупки корабля (ссылка нужна, чтобы при необходимости скрывать/показывать кнопку)" )]
    private GameObject image_buy_sell_ship;

    [SerializeField]
    [Tooltip( "Копка продажи или покупки корабля (в зависимости от текущего состояния игры и наличия средств у игрока)" )]
    private Button button_buy_sell_ship;

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

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Объект-контейнер кнопки выбора корабля, в который помещается модель корабля" )]
    private Transform ship_model_transform;
    public Transform Ship_model_transform { get { return ship_model_transform; } }

    [Header( "НАСТРОЙКИ МЕНЮ ПОДТВЕРЖДЕНИЯ ПОКУПКИ / ПРОДАЖИ" )]
    [SerializeField]
    private GameObject panel_choice;

    [SerializeField]
    private EffectiveText text_yes;

    [SerializeField]
    private EffectiveText text_no;

    [Header( "ТЕКСТОВЫЕ ПОЛЯ И ИНДИКАТОРЫ ОКНА КОРАБЛЯ" )]
    [SerializeField]
    private EffectiveText
        text_grade_and_type; [SerializeField] private EffectiveText
        text_description,
        text_price,
        text_buy_sell,
        text_hull,
        text_fuel,
        text_engine,
        text_hold,
        text_shield_time,
        text_shield_power,
        text_charge_time,
        text_radar_range,
        text_radar_power,
        text_autolanding;

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

    private bool choice_panel_is_activated = false;

    public bool Is_available { get { return Ship.Is_available; } }
    public void SetAvailable( bool state ) { Ship.SetAvailable( state ); }

    public bool Is_active { get { return Ship.Is_active; } }
    public void SetActive( bool state ) { Ship.SetActive( state ); }

    private bool is_buy_accessible = false;

    [HideInInspector]
    public Ship Ship;

    private CanvasMenu canvas_menu;
    public void SetMenuReference( CanvasMenu canvas_menu ) { this.canvas_menu = canvas_menu; }

    private CanvasScrollControl scroll_control;
    public void SetScrollReference( CanvasScrollControl scroll_control ) { this.scroll_control = scroll_control; }

    public void EventButtonUpPressed() { button_arrow_up.enabled = false; button_arrow_up.enabled = true; scroll_control.EventButtonUpPressed( ScrollingSource.Ship ); }
    public void EventButtonDownPressed() { button_arrow_down.enabled = false; button_arrow_down.enabled = true; scroll_control.EventButtonDownPressed( ScrollingSource.Ship ); }
    public void EventButtonLeftPressed() { button_arrow_left.enabled = false; button_arrow_left.enabled = true; scroll_control.EventButtonLeftPressed( ScrollingSource.Ship ); }
    public void EventButtonRightPressed() { button_arrow_right.enabled = false; button_arrow_right.enabled = true; scroll_control.EventButtonRightPressed( ScrollingSource.Ship ); }

    // Use this for initialization #############################################################################################################################################
	void Start() {

        button_buy_sell_ship = image_buy_sell_ship.GetComponentInChildren<Button>();

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
        text_grade_and_type.Rewrite( Game.Localization.GetTextValue( Ship.Grade_key ) ).Append( Game.Separator_space ).Append( Game.Localization.GetTextValue( Ship.Type_key ) );
        text_description.Rewrite( Game.Localization.GetTextValue( Ship.Description_key ) );

        // /////////////////////////////////////////////////////////////////////////////////
        // Обновление состояния корабля (активность и наличие управляется из других методов)
        // /////////////////////////////////////////////////////////////////////////////////
    
        is_buy_accessible = (Is_active || Is_available) ? false : (Ship.Price_buy <= Game.Money);

        // //////////////////////////////
        // Обновление индикаторов корабля
        // //////////////////////////////

        Game.ReportIndicator( Ship.Hull_durability, text_hull, available_bar_hull, maximum_bar_hull, true, false );
        Game.ReportIndicator( Ship.Fuel_capacity, text_fuel, available_bar_fuel, maximum_bar_fuel, true, true );
        Game.ReportIndicator( Ship.Engine_thrust, text_engine, available_bar_engine, maximum_bar_engine, true, false );
        Game.ReportIndicator( Ship.Hold_capacity, text_hold, available_bar_hold, maximum_bar_hold, true, true );
        Game.ReportIndicator( Ship.Shield_time, text_shield_time, available_bar_shield_time, maximum_bar_shield_time, true, true );
        Game.ReportIndicator( Ship.Shield_power, text_shield_power, available_bar_shield_power, maximum_bar_shield_power, true, true );
        Game.ReportIndicator( Ship.Charge_time, text_charge_time, available_bar_charge_time, maximum_bar_charge_time, true, true );
        Game.ReportIndicator( Ship.Radar_range, text_radar_range, available_bar_radar_range, maximum_bar_radar_range, true, true );
        Game.ReportIndicator( Ship.Radar_power, text_radar_power, available_bar_radar_power, maximum_bar_radar_power, true, true );
        Game.ReportIndicator( Ship.Autolanding_amount, text_autolanding, available_bar_autolanding, maximum_bar_autolanding, false, false );

        // /////////////////////////////////////////////////////////////////////
        // Обновление панелей и кнопок окна в соответствии с флагами доступности
        // /////////////////////////////////////////////////////////////////////

        // Если корабль активен, подсвечиваем его кнопку (кнопка остаётся интерактивной)
        if( Is_active ) {

            if( Ship.Price_buy == 0f ) {

                text_price.Rewrite( Game.Phrase_ship_free );
                text_buy_sell.Rewrite( Game.Phrase_ship_gift );
            }

            else {

                text_price.Rewrite( Game.Phrase_sell_price ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Ship.Price_sell ) );
                text_buy_sell.Rewrite( Game.Phrase_sell_ship );
            }

            button_take_ship.GetComponent<Image>().color = active_color;
            button_take_ship.interactable = true;

            button_buy_sell_ship.interactable = (Ship.Price_buy == 0f) ? false : true;

            lock_frame.gameObject.SetActive( false );
            lock_icon.gameObject.SetActive( false );
        }

        // Если корабль не является активным, но куплен игроком ранее и есть в наличии, меняем только цвет кнопки (кнопка остаётся интерактивной)
        else if( Is_available ) {

            if( Ship.Price_buy == 0f ) {

                text_price.Rewrite( Game.Phrase_ship_free );
                text_buy_sell.Rewrite( Game.Phrase_ship_gift );
            }

            else {

                text_price.Rewrite( Game.Phrase_sell_price ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Ship.Price_sell ) );
                text_buy_sell.Rewrite( Game.Phrase_sell_ship );
            }

            button_take_ship.GetComponent<Image>().color = inactive_color;
            button_take_ship.interactable = true;

            button_buy_sell_ship.interactable = (Ship.Price_buy == 0f) ? false : true;

            lock_frame.gameObject.SetActive( false );
            lock_icon.gameObject.SetActive( false );
        }

        // Если корабля нет в наличии, но он готов к покупке, открываем окно, но кнопка выбора корабля пока неактивна
        else if( is_buy_accessible ) {

            text_price.Rewrite( Game.Phrase_buy_price ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Ship.Price_buy ) );
            text_buy_sell.Rewrite( Game.Phrase_buy_ship );

            button_take_ship.GetComponent<Image>().color = inactive_color;
            button_take_ship.interactable = false;

            button_buy_sell_ship.interactable = true;

            lock_frame.gameObject.SetActive( false );
            lock_icon.gameObject.SetActive( false );
        }

        // В остальных случаях корабль пока нельзя приобрести, и поэтому кнопка заблокирована, окно затемнено
        else {

            text_price.Rewrite( Game.Phrase_buy_price ).Append( Game.Separator_colon ).Append( Game.Separator_space ).AppendSeparatedInt( Mathf.FloorToInt( Ship.Price_buy ) );
            text_buy_sell.Rewrite( Game.Phrase_ship_money );

            button_take_ship.GetComponent<Image>().color = inactive_color;
            button_take_ship.interactable = false;

            button_buy_sell_ship.interactable = false;

            lock_frame.gameObject.SetActive( true );
            lock_icon.gameObject.SetActive( true );
        }
    }

	// If the take ship button is pressed and enabled, make this ship an active ################################################################################################
	public void EventButtonActivateShipPressed() {

        button_take_ship.enabled = false;
        button_take_ship.enabled = true;

        if( Ship != canvas_menu.Current_ship ) {

            ShipWindow old_ship_window = canvas_menu.Current_ship_window;

            canvas_menu.Current_ship.SetActive( false );
            SetActive( true );
            SetAvailable( true );

            old_ship_window.Refresh();
            Refresh();

            canvas_menu.RefreshShipAdvertising();
            canvas_menu.RefreshCurrentShipName();
        }
    }

	// Buy ship button pressed #################################################################################################################################################
	public void EventButtonBuySellShipPressed() {

        button_buy_sell_ship.enabled = false;
        button_buy_sell_ship.enabled = true;

        ActivateChoicePanel();
    }

    // Активация окна выбора ###################################################################################################################################################
    private void ActivateChoicePanel() {

        // Если продаём корабль
        if( Is_available ) {

            text_yes.Rewrite( Game.Pharse_ship_sell_confirm );
            text_no.Rewrite( Game.Pharse_ship_sell_decline );
        }

        // Если покупаем корабль
        else {

            text_yes.Rewrite( Game.Pharse_ship_buy_confirm );
            text_no.Rewrite( Game.Pharse_ship_buy_decline );
        }

        image_buy_sell_ship.SetActive( false );
        choice_panel_is_activated = true;
        panel_choice.SetActive( true );
    }

    // Активация окна выбора ###################################################################################################################################################
    private void DeactivateChoicePanel() {

        choice_panel_is_activated = false;
        panel_choice.SetActive( false );
        image_buy_sell_ship.SetActive( true );
    }

    // Событие, вызываемое по кнопке YES меню подтверждения выбора #############################################################################################################
    public void EventButtonYesPressed() {

        // Если продаём корабль
        if( Is_available ) {

            ShipWindow old_ship_window = canvas_menu.Current_ship_window;

            SetActive( false );
            SetAvailable( false );

            Game.Money += Ship.Price_sell;
            canvas_menu.RefreshMoneyValue();

            if( Ship == old_ship_window.Ship ) {

                canvas_menu.FindCurrentShip();
                canvas_menu.RefreshShipAdvertising();
                canvas_menu.RefreshCurrentShipName();
            }

            DeactivateChoicePanel();
            canvas_menu.RefreshAllShips();
            canvas_menu.RefreshAllLevels();

            SaveGame.RemoveShipFile( Ship );
        }

        // Если покупаем корабль
        else {

            ShipWindow old_ship_window = canvas_menu.Current_ship_window;
            old_ship_window.Ship.SetActive( false );

            SetActive( true );
            SetAvailable( true );

            Game.Money -= Ship.Price_buy;
            canvas_menu.RefreshMoneyValue();

            DeactivateChoicePanel();
            canvas_menu.RefreshAllShips();
            canvas_menu.RefreshAllLevels();

            canvas_menu.RefreshShipAdvertising();
            canvas_menu.RefreshCurrentShipName();
        }
    }

    // Событие, вызываемое по кнопке NO меню подтверждения выбора ##############################################################################################################
    public void EventButtonNoPressed() {

        DeactivateChoicePanel();
    }
}