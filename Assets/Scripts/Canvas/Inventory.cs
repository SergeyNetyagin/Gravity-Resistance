using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    [System.NonSerialized]
    public static ShipType ship_type = ShipType.Ship_unknown;

    [SerializeField]
    [Tooltip( "Тип инвентаря: 1) просто инвентарь корабля; 2) инвентарь корабля во время торговли; 3) инвентарь станции во время торговли; 4) просто инвентарь станции" )]
    private InventoryType inventory_type = InventoryType.Ship_inventory_only;
    public InventoryType Inventory_type { get { return inventory_type; } }

    [SerializeField]
    [Tooltip( "Корневая родительская панель, которая сожержит в себе данный набор элементов интерфейса; необходима, чтобы полностью закрыть окно и выйти из режима инвентаря" )]
    private GameObject main_parental_panel;

    [SerializeField]
    [Tooltip( "Префаб элемента отсека трюма (отсеки автоматически располагаются в окне в зависимости от параметров трюма корабля)" )]
    private GameObject item_prefab;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Вспомогательный объект инвентаря, через который осуществляется перетаскивания рисунков между отсеками (иначе рисунки будут то видны, то скрыты за отсеками)" )]
    private RectTransform drag_picture_transform;
    public RectTransform Drag_picture_transform { get { return drag_picture_transform; } }

    [SerializeField]
    [Tooltip( "Окошко заголовка инвентаря (если отсутствует, то положение заголовка считается постоянным, поскольку размеры окна не меняются)" )]
    private RectTransform title_transform;

    [SerializeField]
    [Tooltip( "Ключ локализации заголовка" )]
    private string title_key;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Кнопка закрытия окна: её приходится подстраивать под размеры окна и инициализировать ссылку отдельно, чтобы она не попала в разметку Grid Layout Group" )]
    private Transform button_close_transform;

    [SerializeField]
    private float button_close_offset_x = -35f;

    [SerializeField]
    private float button_close_offset_y = -35f;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Количество пикселей смещения окна подсказки относительно указателя мыши по оси X; по умолчанию = 10" )]
    [Range( -100f, 100f )]
    private float pointer_offset_x = 10f;
    public float Pointer_offset_x { get { return pointer_offset_x; } }

    [SerializeField]
    [Tooltip( "Количество пикселей смещения окна подсказки относительно указателя мыши по оси Y; по умолчанию = -5" )]
    [Range( -50f, 50f )]
    private float pointer_offset_y = -5f;
    public float Pointer_offset_y { get { return pointer_offset_y; } }

    [SerializeField]
    [Tooltip( "Вертикальное смещение окошка заголовка; по умолчанию = -5" )]
    [Range( -50f, 50f )]
    private float title_offset_y = -5;

    [SerializeField]
    [Tooltip( "Максимальное количество колонок в инвентаре (если больше, то инвентарь располагаетс в два или более рядов); по умолчанию = 6" )]
    [Range( 5, 10 )]
    private int max_columns = 6;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Строка заголовка инвентаря" )]
    private EffectiveText text_title;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Цвет рамки для имеющегося в наличии отсека" )]
    private Color available_frame_color = Color.white;
    public Color Available_frame_color { get { return available_frame_color; } }

    [SerializeField]
    [Tooltip( "Цвет подсветки для имеющегося в наличии отсека" )]
    private Color available_container_color = Color.white;
    public Color Available_container_color { get { return available_container_color; } }

    [SerializeField]
    [Tooltip( "Цвет рисунка для имеющегося в наличии отсека" )]
    private Color available_picture_color = Color.white;
    public Color Available_picture_color { get { return available_picture_color; } }

    [SerializeField]
    [Tooltip( "Цвет рамки для отсека, которого нет в наличии, но до которого можно выполнить апгрэйд" )]
    private Color absent_frame_color = Color.gray;
    public Color Absent_frame_color { get { return absent_frame_color; } }

    [SerializeField]
    [Tooltip( "Цвет подсветки для отсека, которого нет в наличии, но до которого можно выполнить апгрэйд" )]
    private Color absent_container_color = Color.gray;
    public Color Absent_container_color { get { return absent_container_color; } }

    [SerializeField]
    [Tooltip( "Цвет рисунка для отсека, которого нет в наличии, но до которого можно выполнить апгрэйд" )]
    private Color absent_picture_color = new Color( 1f, 1f, 1f, 0f );
    public Color Absent_picture_color { get { return absent_picture_color; } }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Ключ текста для сообщения при наведении на пустой доступный отсек инвентаря" )]
    private string empty_compartment_key;
    
    [SerializeField]
    [Tooltip( "Ключ текста для сообщения при наведении на недоступный отсек инвентаря" )]
    private string absent_compartment_key;

    [System.NonSerialized]
    private Transform tooltip_transform;
    public Transform Tooltip_transform { get { return tooltip_transform; } }

    [System.NonSerialized]
    private RectTransform tooltip_rect_transform;
    public Transform Tooltip_rect_transform { get { return tooltip_rect_transform; } }

    public void ShowTooltip( Transform item_transform ) { tooltip_transform = item_transform; if( tooltip_transform != null ) tooltip_transform.gameObject.SetActive( true ); }
    public void HideTooltip() { if( tooltip_transform != null ) tooltip_transform.gameObject.SetActive( false ); tooltip_transform = null; }
    public bool Tooltip_is_active { get { return (tooltip_transform != null) ? tooltip_transform.gameObject.activeInHierarchy : false; } }

    [System.NonSerialized]
    private int begin_drag_item_index = -1;
    public int Begin_drag_item_index { get { return begin_drag_item_index; } }

    [System.NonSerialized]
    private int end_drag_item_index = -1;
    public int End_drag_item_index { get { return end_drag_item_index; } }

    [System.NonSerialized]
    private bool is_drag = false;
    public bool Is_drag { get { return is_drag; } }
    public void BeginDrag( int i ) { begin_drag_item_index = i; is_drag = true; }
    public void EndDrag( int i ) { end_drag_item_index = i; is_drag = false; }
    public void ResetDrags() { begin_drag_item_index = end_drag_item_index = -1; is_drag = false; }

    [System.NonSerialized]
    private Sprite base_sprite;
    public Sprite Base_sprite { get { return base_sprite; } }

    [System.NonSerialized]
    private InventoryPanel[] panels;

    [System.NonSerialized]
    private Vector3 
        picture_position = Vector3.zero,
        reset_postion = Vector3.zero,
        reset_scale = Vector3.one;

    private Vector2
        window_min_point = Vector2.zero,
        window_max_point = Vector2.zero;

    // Сетка, которая автоматически позволяет правильно выстраивать элементы
    private GridLayoutGroup grid_group;
    public GridLayoutGroup Grid_group { get { return grid_group; } }

    // Корневой объект инвентаря, в котором отображаются отсеки трюма - окно изменяемого размера (в зависимости от числа отсеков трюма)" )]
    private RectTransform rect_transform;
    public RectTransform Rect_transform { get { return rect_transform; } }
    
    // Отображает инвентарь при включении панели инвентаря в канвасе ###############################################################################################################
    void OnEnable() {

        if( rect_transform == null ) {

            grid_group = GetComponent<GridLayoutGroup>();
            rect_transform = GetComponent<RectTransform>();
        }

        // Это означает, что панель была принудительно включена в редакторе, поэтому ссылки ещё не успели проинициализироваться
        if( (Game.Player == null) || (Game.Player.Ship == null) ) return;

        // На всякий случай скрываем подсказку
        HideTooltip();

        // Если у игрока корабль не тот, который был при старте, полностью меняем инвентарь перед отображением, удаляя прежний
        if( Game.Player.Ship.Type != ship_type ) {

            // Уничтожаем предыдущий инвентарь, если он существует
            for( int i = 0; i < rect_transform.childCount; i++ ) Destroy( rect_transform.GetChild( i ) );

            // Создаём новый инвентарь
            Create();
        }

        // Отображаем заранее подготовленный инвентарь
        Show();
    }

    // Скрывает инвентарь ##########################################################################################################################################################
    void OnDisable() {

        // На всякий случай скрываем подсказку
        HideTooltip();

        Game.Resume();
        Game.Canvas.RefreshHoldIndicator( true );
    }

    // Содаёт элементы (отсеки) инвентаря ##########################################################################################################################################
    private void Create() {

        // Запоминаем тип корабля, с которым связан данный инвентарь
        ship_type = Game.Player.Ship.Type;

        // До формирования окна инвентаря создаём собственно ячейки инвентаря: их количество равно количеству отсеков трюма
        panels = new InventoryPanel[ Game.Player.Ship.Compartments_maximum ];
        
        // Если инвентарь не служит для торговли, то его окно имеет переменные размеры, и их нужно менять в зависимости от количества элементов инвентаря
        if( (inventory_type == InventoryType.Ship_inventory_only) || (inventory_type == InventoryType.Station_inventory_only) ) {

            int columns = (panels.Length < max_columns) ? panels.Length : 
                (((panels.Length % 2) == 0) ? panels.Length / 2 : 
                (((panels.Length % 3) == 0) ? panels.Length / 3 : 
                (((panels.Length % 4) == 0) ? panels.Length / 4 : max_columns)));

            int rows = ((panels.Length % columns) == 0) ? panels.Length / columns : panels.Length / columns + 1;

            // Определяем размеры окна инвентаря
            rect_transform.sizeDelta = new Vector2( 
                grid_group.cellSize.x * columns + (columns - 1) * grid_group.spacing.x + grid_group.padding.left * 2 + grid_group.padding.right * 2, 
                grid_group.cellSize.y * rows + (rows - 1) * grid_group.spacing.y + grid_group.padding.top * 2 + grid_group.padding.bottom * 2 );
        }

        // Для любого типа инвентаря вычисляем крайние точки окна (чтобы правильно определять, выброшен груз или нет)
        window_min_point.x = rect_transform.position.x - rect_transform.sizeDelta.x * 0.5f * Game.Control.Screen_rate_x;
        window_max_point.x = rect_transform.position.x + rect_transform.sizeDelta.x * 0.5f * Game.Control.Screen_rate_x;
        window_min_point.y = rect_transform.position.y - rect_transform.sizeDelta.y * 0.5f * Game.Control.Screen_rate_y;
        window_max_point.y = rect_transform.position.y + rect_transform.sizeDelta.y * 0.5f * Game.Control.Screen_rate_y;

        button_close_transform.position = new Vector3( 
            window_max_point.x + button_close_offset_x * Game.Control.Screen_rate_x, 
            window_max_point.y + button_close_offset_y * Game.Control.Screen_rate_y, 0f );

        // Если окно инвентаря имеет изменяемые размеры, то устанавливаем положение окна заголовка
        if( (inventory_type == InventoryType.Ship_inventory_only) || (inventory_type == InventoryType.Station_inventory_only) ) {

            title_transform.position = new Vector3( rect_transform.position.x, window_max_point.y + title_offset_y, 0f );
        }

        // Инициализируем каждый элемент инвентаря в соответствии с тем, как объекты размещены в трюме
        for( int i = 0; i < panels.Length; i++ ) {

            panels[i] = (Instantiate( item_prefab, rect_transform ) as GameObject).GetComponent<InventoryPanel>();
            panels[i].transform.localScale = reset_scale;
            panels[i].transform.localPosition = reset_postion;

            panels[i].Panel_transform = panels[i].GetComponent<Transform>();
            panels[i].Item = panels[i].GetComponentInChildren<InventoryItem>();
            panels[i].Item.Index = i;

            // Базовый спрайт, заменяющий рисунок груза - он будет восстанавливаться, когда ячейка отсека будет снова освобождаться от груза
            if( base_sprite == null ) base_sprite = panels[i].Item.Picture_image.sprite;

            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Устанавливаем тип подсказок ///////////////////////////////////////////////////////////////////////////////////////////////
            // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Обновляем данные текущей ячейки
            tooltip_transform = panels[i].Item.Container_image.GetComponentInChildren<InventoryTooltip>().GetComponent<Transform>();
            tooltip_rect_transform = tooltip_transform.GetComponent<RectTransform>();

            // Если это обычный (контекстный) показ инвентаря, то подсказка контекстная
            if( inventory_type == InventoryType.Ship_inventory_only ) {

                tooltip_rect_transform.anchorMin = new Vector2( 0f, 0f );
                tooltip_rect_transform.anchorMax = new Vector2( 0f, 0f );
                tooltip_rect_transform.pivot = new Vector2( 0f, 1f );
                tooltip_rect_transform.localPosition = new Vector3( 0f, 0f, 0f );
                tooltip_rect_transform.offsetMin = new Vector2( -1f, -1f );
                tooltip_rect_transform.offsetMax = new Vector2( 0f, 1f );

                HideTooltip();
            }
            
            // Если это торговля со стороны корабля, то подсказка постоянно отображается рядом с собственным отсеком корабля слева от него
            else if( inventory_type == InventoryType.Ship_inventory_trade ) {

                tooltip_rect_transform.anchorMin = new Vector2( 0.5f, 0.5f );
                tooltip_rect_transform.anchorMax = new Vector2( 0.5f, 0.5f );
                tooltip_rect_transform.pivot = new Vector2( 0.5f, 0.5f );
                tooltip_rect_transform.localPosition = new Vector3( -281f, -3f, 0f );
                tooltip_rect_transform.offsetMin = new Vector2( 0f, 0f );
                tooltip_rect_transform.offsetMax = new Vector2( 0f, 0f );

                ShowTooltip( tooltip_transform  );
            }

            // Если это торговля со стороны станции, то подсказка постоянно отображается рядом с торговым отсеком станции справа от него
            else if( inventory_type == InventoryType.Station_inventory_trade ) {

                tooltip_rect_transform.anchorMin = new Vector2( 0.5f, 0.5f );
                tooltip_rect_transform.anchorMax = new Vector2( 0.5f, 0.5f );
                tooltip_rect_transform.pivot = new Vector2( 0.5f, 0.5f );
                tooltip_rect_transform.localPosition = new Vector3( 277f, -3f, 0f );
                tooltip_rect_transform.offsetMin = new Vector2( 0f, 0f );
                tooltip_rect_transform.offsetMax = new Vector2( 0f, 0f );

                ShowTooltip( tooltip_transform  );
            }

            // Если нужно просто отобразить инвентарь станции
            else if( inventory_type == InventoryType.Station_inventory_only ) {

                tooltip_rect_transform.anchorMin = new Vector2( 0f, 0f );
                tooltip_rect_transform.anchorMax = new Vector2( 0f, 0f );
                tooltip_rect_transform.pivot = new Vector2( 0f, 1f );
                tooltip_rect_transform.localPosition = new Vector3( 0f, 0f, 0f );
                tooltip_rect_transform.offsetMin = new Vector2( -1f, -1f );
                tooltip_rect_transform.offsetMax = new Vector2( 0f, 1f );

                HideTooltip();
            }

            // Корректировка единых параметров окошка подсказки и обновление локализованных данных
            tooltip_rect_transform.sizeDelta = new Vector2( 360f, 169f );
            for( int j = 0; j < tooltip_rect_transform.childCount; j++ ) tooltip_rect_transform.GetChild( j ).GetComponent<Text>().transform.localScale = new Vector3( 0.2f, 0.2f, 1f );
        }
    }

    // Отображает актуальное состояние инвентаря ###################################################################################################################################
    private void Show() {

        // Отображаем локализованный заголовок
        text_title.Rewrite( Game.Localization.GetTextValue( "Compartment.Title" ) );

        // Отображаем актуальное состояние отсеков (цвета и рисунки)
        for( int i = 0; i < panels.Length; i++ ) {

            panels[i].Item.Value = Game.Player.Ship.GetCompartmentValue( i );

            panels[i].Item.Is_empty = Game.Player.Ship.IsEmptyCompartment( i );
            panels[i].Item.Is_absent = Game.Player.Ship.IsAbsentCompartment( i );

            panels[i].Item.Frame_image.color = panels[i].Item.Is_absent ? absent_frame_color : available_frame_color;
            panels[i].Item.Container_image.color = panels[i].Item.Is_absent ? absent_container_color : available_container_color;
            panels[i].Item.Picture_image.sprite = (panels[i].Item.Is_absent || panels[i].Item.Is_empty) ? base_sprite : Game.Player.Ship.GetCompartmentPicture( i );
            panels[i].Item.Picture_image.color = (panels[i].Item.Is_absent || panels[i].Item.Is_empty) ? absent_picture_color : available_picture_color;

            panels[i].Item.Frame_shadow.enabled = panels[i].Item.Is_absent ? false : true;
            panels[i].Item.Picture_shadow.enabled = (panels[i].Item.Is_absent || panels[i].Item.Is_empty) ? false : true;

            // Обновление локализованных данных
            RefreshTooltip( panels[i].Item );
        }
    }

    // Обновляет данные для подсказки в соответствии с атрибутами текущего указанного отсека #######################################################################################
    public void RefreshTooltip( InventoryItem item ) {

        // Если отсек недоступен, выводим подсказку о возможном апгрэйде
        if( item.Is_absent ) {

            item.Inventory_tooltip.Text_item_name.Rewrite( string.Empty );
            item.Inventory_tooltip.Text_item_description.Rewrite( Game.Localization.GetTextValue( "Compartment.Absent" ) );
            item.Inventory_tooltip.Text_item_mass.Rewrite( string.Empty );
            item.Inventory_tooltip.Text_item_cost.Rewrite( string.Empty );
        }

        // Если отсек пустой, но доступный, просто сообщаем, сколько туда можно поместить
        else if( item.Is_empty ) {

            item.Inventory_tooltip.Text_item_name.Rewrite( string.Empty );

            item.Inventory_tooltip.Text_item_description.Rewrite( Game.Localization.GetTextValue( "Compartment.Empty" ) ).Append( Game.Separator_space );
            item.Inventory_tooltip.Text_item_description.AppendDottedFloat( Game.Player.Ship.Hold_capacity.Unit_size, 2 ).Append( Game.Separator_space ).Append( Game.Unit_mass_t );

            item.Inventory_tooltip.Text_item_mass.Rewrite( string.Empty );
            item.Inventory_tooltip.Text_item_cost.Rewrite( string.Empty );
        }

        // Если в отсеке есть груз, формируем полную информацию о грузе
        else {

            FreightValue freight = item.Value.Is_freight ? Game.Control.GetFreightValue( item.Value.Obstacle.Freight.Type ) : null;
            MineralValue mineral = item.Value.Is_mineral ? Game.Control.GetMineralValue( item.Value.Obstacle.Mineral.Type ) : null;

            if( freight != null ) {

                item.Inventory_tooltip.Text_item_name.Rewrite( Game.Localization.GetTextValue( freight.Name_key ) );
            }

            else if( mineral != null ) {

                item.Inventory_tooltip.Text_item_name.Rewrite( Game.Localization.GetTextValue( mineral.Kind_key ) ).
                    Append( Game.Separator_colon).Append( Game.Separator_space ).
                    Append( Game.Localization.GetTextValue( mineral.Name_key ) );
            }

            else {

                item.Inventory_tooltip.Text_item_name.Rewrite( string.Empty );
            }

            item.Inventory_tooltip.Text_item_description.Rewrite( (freight != null) ? Game.Localization.GetTextValue( freight.Description_key ) : 
                ((mineral != null) ? Game.Localization.GetTextValue( mineral.Description_key ) : string.Empty) );

            item.Inventory_tooltip.Text_item_mass.Rewrite( Game.Localization.GetTextValue( "Compartment.Mass" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).
                AppendDottedFloat( item.Value.Total_mass_in_tons, 2 ).Append( Game.Separator_space ).Append( Game.Unit_mass_t );

            item.Inventory_tooltip.Text_item_cost.Rewrite( Game.Localization.GetTextValue( "Compartment.Cost" ) ).Append( Game.Separator_colon ).Append( Game.Separator_space ).
                AppendSeparatedInt( Mathf.FloorToInt( item.Value.Total_cost ) );
        }
    }
   
    // Определяет, находится ли указанная точка за пределами инвентаря, или нет ####################################################################################################
    public bool OutOfInventory( Vector2 position ) {

        if( position.x < window_min_point.x ) return true;
        if( position.x > window_max_point.x ) return true;
        if( position.y < window_min_point.y ) return true;
        if( position.y > window_max_point.y ) return true;

        return false;
    }
    
    // Меняет местонахождение грузов в отсеках #####################################################################################################################################
    public void ExchangeItems() {

        // Меняем элементы местами
        InventoryItem temp_item = panels[ begin_drag_item_index ].Item;
        panels[ begin_drag_item_index ].Item = panels[ end_drag_item_index ].Item;
        panels[ end_drag_item_index ].Item = temp_item;

        panels[ begin_drag_item_index ].Item.Cached_transform.parent = panels[ begin_drag_item_index ].Panel_transform;
        panels[ begin_drag_item_index ].Item.Cached_transform.localPosition = reset_postion;
        panels[ begin_drag_item_index ].Item.Index = begin_drag_item_index;

        panels[ end_drag_item_index ].Item.Cached_transform.parent = panels[ end_drag_item_index ].Panel_transform;
        panels[ end_drag_item_index ].Item.Cached_transform.localPosition = reset_postion;
        panels[ end_drag_item_index ].Item.Index = end_drag_item_index;

        // Сразу же обновляем данные трюма на случай экстренного выхода из игры
        Game.Player.Ship.RefreshHold( panels );
    }

    // Event: exit button pressed ##################################################################################################################################################
    public void EventButtonReturnPressed() {

        // Перед выходом сообщаем кораблю новый порядок расположения грузов в трюме
        RefreshHold();

        // Деактивируем панель инвентаря
        main_parental_panel.SetActive( false );
    }

    // Обновляет содержимое отсеков трюма и свойства ячеек #########################################################################################################################
    public void RefreshHold() {

        Game.Player.Ship.RefreshHold( panels );

        // Определяем свойства ячеек
        for( int i = 0; i < panels.Length; i++ ) {

            panels[i].Item.Is_empty = Game.Player.Ship.IsEmptyCompartment( i );
            panels[i].Item.Is_absent = Game.Player.Ship.IsAbsentCompartment( i );
        }
    }
}