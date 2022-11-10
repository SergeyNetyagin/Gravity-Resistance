using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ScrollingSource {

    Level   = 1,
    Menu    = 2,
    Ship    = 3
}

public enum StopIndexChange {

    Zero = 0,
    Increase = 1,
    Decrease = -1
}

[RequireComponent( typeof( ScrollRect ) )]
public class CanvasScrollControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [System.Serializable]
    private class ScrollingContent {

        [Tooltip( "Тип данной линейки меню (для скроллинга каких элементов она предназначена)" )]
        public ScrollingSource type = ScrollingSource.Menu;

        [Tooltip( "Ссылка на ScrollRect необходимой группы меню" )]
        public ScrollRect scroll_rect;
            
        [Tooltip( "Скорость скролинга в единицах канваса; по умолчанию = 3" )]
        public float scroll_speed = 3f;

        [HideInInspector]
        // Положения окон в относительных координатах канваса (на отрезке от 0 до 1)
        public float[] stops;

        [HideInInspector]
        // Индекс текущего окна
        public int stop_index = 0;

        [HideInInspector]
        // Направление скроллинга: нулевое (0), положительное (1) или отрицательное (-1)
        public int scroll_direction = 0;
    }

    [SerializeField]
    [Tooltip( "Значение позиции канваса, выше которой считается, что мы находимся в слое меню уровней; по умолчанию = 0.99 на отрезке [0, 1]" )]
    [Range( 0.75f, 1.0f )]
    private float levels_layer_position = 0.99f;

    [SerializeField]
    [Tooltip( "Значение позиции канваса, выше которой считается, что мы находимся в слое меню уровней; по умолчанию = 0.01 на отрезке [0, 1]" )]
    [Range( 0.0f, 0.25f )]
    private float ships_layer_position = 0.01f;

    [SerializeField]
    [Tooltip( "Скорость скролинга, выше которой происходит постоянное пролистывание окон без остановки на каждом новом появившемся окне; по умолчанию = 3000" )]
    [Range( 2000f, 5000f )]
    private float nonstop_scroll_speed = 3000f;

    [Space( 10 )]
    [SerializeField]
    private ScrollingContent menu;

    [SerializeField]
    private ScrollingContent levels;

    [SerializeField]
    private ScrollingContent ships;

    private Vector2 drag_length = Vector2.zero;
    private Vector2 start_drag_position = Vector2.zero;

    private bool is_drag = false;
    private bool is_one_touch = false;
    private bool is_horizontal_scrolling = false;

    private ScrollRect scroll_rect;
    private ScrollingSource current_scrolling_layer = ScrollingSource.Menu;

    private CanvasMenu canvas_menu;

    private int touch_count { get {

        #if UNITY_STANDALONE
        return Input.mousePresent ? 1 : 0;
        #elif UNITY_IOS
        return Input.touchCount;
        #elif UNITY_ANDROID
        return Input.touchCount;
        #endif

    } }

	// Use this for initialization #############################################################################################################################################
	void Start() {

        canvas_menu = GameObject.Find( "Canvas_menu" ).GetComponent<CanvasMenu>();

        // Подготовка позиций для скроллинга главного меню
        menu.stops = new float[ menu.scroll_rect.content.childCount ];

        for( int i = 0; i < menu.scroll_rect.content.childCount; i++ )
            menu.stops[i] = ((menu.scroll_rect.content.childCount - 1 - i) / (float) ((menu.scroll_rect.content.childCount > 1) ? (menu.scroll_rect.content.childCount - 1) : 1));

        menu.stop_index = 1;
        menu.scroll_direction = 0;
        menu.scroll_speed /= menu.scroll_rect.content.childCount;
        menu.scroll_rect.verticalNormalizedPosition = menu.stops[ menu.stop_index ];

        // Подготовка позиций для скроллинга окон уровней
        levels.stops = new float[ levels.scroll_rect.content.childCount ];

        for( int i = 0; i < levels.scroll_rect.content.childCount; i++ )
            levels.stops[i] = ((float) i) / ((levels.scroll_rect.content.childCount > 1) ? (levels.scroll_rect.content.childCount - 1) : 1);

        levels.stop_index = canvas_menu.Current_level_index;
        levels.scroll_direction = 0;
        levels.scroll_speed /= levels.scroll_rect.content.childCount;
        levels.scroll_rect.horizontalNormalizedPosition = levels.stops[ levels.stop_index ];

        // Подготовка позиций для скроллинга окон кораблей
        ships.stops = new float[ ships.scroll_rect.content.childCount ];

        for( int i = 0; i < ships.scroll_rect.content.childCount; i++ )
            ships.stops[i] = ((float) i) / ((ships.scroll_rect.content.childCount > 1) ? (ships.scroll_rect.content.childCount - 1) : 1);

        ships.stop_index = canvas_menu.Current_ship_index;
        ships.scroll_direction = 0;
        ships.scroll_speed /= ships.scroll_rect.content.childCount;
        ships.scroll_rect.horizontalNormalizedPosition = ships.stops[ ships.stop_index ];
    }
    
    // Update is called once per frame #########################################################################################################################################
	void Update () {

        if( menu.scroll_rect.verticalNormalizedPosition >= levels_layer_position ) current_scrolling_layer = ScrollingSource.Level;
        else if( menu.scroll_rect.verticalNormalizedPosition <= ships_layer_position ) current_scrolling_layer = ScrollingSource.Ship;
        else current_scrolling_layer = ScrollingSource.Menu;

        if( !is_drag ) {

            MoveToStop( menu );

            // Если скорость скроллинга уровней после отпускания указателя меньше ограничения, то успокаиваем скроллинг в центре ближайшего окна
            if(  Mathf.Abs( levels.scroll_rect.velocity.x ) > nonstop_scroll_speed ) FindNearestStop( levels );
            else MoveToStop( levels );

            // Если скорость скроллинга кораблей после отпускания указателя меньше ограничения, то успокаиваем скроллинг в центре ближайшего окна
            if( Mathf.Abs( ships.scroll_rect.velocity.x ) > nonstop_scroll_speed ) FindNearestStop( ships );
            else MoveToStop( ships );
        }
    }

    // Находит ближайшую точку остановки при быстром скроллинге окон ###########################################################################################################
    private void FindNearestStop( ScrollingContent content ) {

        content.scroll_direction = (content.scroll_rect.velocity.x > 0f) ? 1 : -1;

        content.stop_index = Mathf.FloorToInt( content.scroll_rect.horizontalNormalizedPosition * content.stops.Length );

        if( content.stop_index < 0 ) content.stop_index = 0;
        else if( content.stop_index >= content.stops.Length ) content.stop_index = content.stops.Length - 1;
    }

    // Scroll window to the target position ####################################################################################################################################
    private void MoveToStop( ScrollingContent content ) {

        // Если это меню, то перемещаем содержимое в вертикальном направлении
        if( content == menu ) {

            if( content.scroll_direction < 0 ) {

                content.scroll_rect.verticalNormalizedPosition += content.scroll_speed * Time.deltaTime;

                if( content.scroll_rect.verticalNormalizedPosition >= content.stops[ content.stop_index ] ) {

                    content.scroll_direction = 0;
                    content.scroll_rect.verticalNormalizedPosition = content.stops[ content.stop_index ];
                }
            }

            else if( content.scroll_direction > 0 ) {

                content.scroll_rect.verticalNormalizedPosition -= content.scroll_speed * Time.deltaTime;

                if( content.scroll_rect.verticalNormalizedPosition <= content.stops[ content.stop_index ] ) {

                    content.scroll_direction = 0;
                    content.scroll_rect.verticalNormalizedPosition = content.stops[ content.stop_index ];
                }
            }
        }

        // Иначе перемещаем содержимое в горизонтальном направлении
        else {

            if( content.scroll_direction > 0 ) {

                content.scroll_rect.horizontalNormalizedPosition += content.scroll_speed * Time.deltaTime;

                if( content.scroll_rect.horizontalNormalizedPosition >= content.stops[ content.stop_index ] ) {

                    content.scroll_direction = 0;
                    content.scroll_rect.horizontalNormalizedPosition = content.stops[ content.stop_index ];
                }
            }

            else if( content.scroll_direction < 0 ) {

                content.scroll_rect.horizontalNormalizedPosition -= content.scroll_speed * Time.deltaTime;

                if( content.scroll_rect.horizontalNormalizedPosition <= content.stops[ content.stop_index ] ) {

                    content.scroll_direction = 0;
                    content.scroll_rect.horizontalNormalizedPosition = content.stops[ content.stop_index ];
                }
            }
        }
    }
        
    // #########################################################################################################################################################################
    public void OnBeginDrag( PointerEventData eventData ) {

        is_drag = true;

        if( touch_count == 1 ) is_one_touch = true;
        else is_one_touch = false;

        start_drag_position = eventData.position;
    }

    // #########################################################################################################################################################################
    public void OnDrag( PointerEventData eventData ) {

        is_drag = true;
    }

    // #########################################################################################################################################################################
    public void OnEndDrag( PointerEventData eventData ) {

        // Определяем, из какого слоя поступило событие (меню, уровни или корабли)
        scroll_rect = eventData.pointerDrag.GetComponent<ScrollRect>();

        is_drag = false;

        if( !is_one_touch ) return;

        drag_length.x = eventData.position.x - start_drag_position.x;
        drag_length.y = eventData.position.y - start_drag_position.y;

        if( Mathf.Abs( drag_length.x ) > Mathf.Abs( drag_length.y ) ) is_horizontal_scrolling = true;
        else is_horizontal_scrolling = false;

        // Если событие поступило от слоя уровней
        if( scroll_rect == levels.scroll_rect ) {

            if( (drag_length.x == 0f) && (drag_length.y == 0f) ) return;

            if( is_horizontal_scrolling ) {

                DetectHorizontalStop( levels, (drag_length.x > 0f) ? StopIndexChange.Decrease : StopIndexChange.Increase );
                menu.scroll_direction = -1;
            }
            
            else {

                DetectVerticalStop( menu, (drag_length.y > 0f) ? StopIndexChange.Increase : StopIndexChange.Decrease );
                levels.scroll_direction = (drag_length.x < 0f) ? 1 : -1;
            }
        }

        // Если событие поступило от слоя кораблей
        else if( scroll_rect == ships.scroll_rect ) {

            if( (drag_length.x == 0f) && (drag_length.y == 0f) ) return;

            if( is_horizontal_scrolling ) {

                DetectHorizontalStop( ships, (drag_length.x > 0f) ? StopIndexChange.Decrease : StopIndexChange.Increase );
                menu.scroll_direction = 1;
            }
            
            else {

                DetectVerticalStop( menu, (drag_length.y > 0f) ? StopIndexChange.Increase : StopIndexChange.Decrease );
                ships.scroll_direction = (drag_length.x < 0f) ? 1 : -1;
            }
        }

        // Если событие поступило от главного меню
        else if( scroll_rect == menu.scroll_rect ) {

            if( drag_length.y == 0f ) return;
            if( is_horizontal_scrolling ) return;

            DetectVerticalStop( menu, (drag_length.y > 0f) ? StopIndexChange.Increase : StopIndexChange.Decrease );
        }
    }

    // Находит очередную цель для вертикального скроллинга #####################################################################################################################
    private void DetectVerticalStop( ScrollingContent content, StopIndexChange direction ) {

        content.scroll_direction = (int) direction;

        if( (content.scroll_direction < 0) && (content.stop_index == 0) ) return;
        if( (content.scroll_direction > 0) && (content.stop_index == (content.stops.Length - 1)) ) return;

        content.stop_index += content.scroll_direction;
    }
    
    // Находит очередную цель для вертикального скроллинга #####################################################################################################################
    private void DetectHorizontalStop( ScrollingContent content, StopIndexChange direction ) {

        content.scroll_direction = (int) direction;

        if( (content.scroll_direction < 0) && (content.stop_index == 0) ) return;
        if( (content.scroll_direction > 0) && (content.stop_index == (content.stops.Length - 1)) ) return;

        content.stop_index += content.scroll_direction;
    }

    // #########################################################################################################################################################################
    public void EventButtonUpPressed( ScrollingSource layer ) {

        DetectVerticalStop( menu, StopIndexChange.Decrease );
    }

    // #########################################################################################################################################################################
    public void EventButtonDownPressed( ScrollingSource layer ) {

        DetectVerticalStop( menu, StopIndexChange.Increase );
    }

    // #########################################################################################################################################################################
    public void EventButtonLeftPressed( ScrollingSource layer ) {

        if( layer == ScrollingSource.Level ) DetectHorizontalStop( levels, StopIndexChange.Decrease );
        else if( layer == ScrollingSource.Ship ) DetectHorizontalStop( ships, StopIndexChange.Decrease );
    }

    // #########################################################################################################################################################################
    public void EventButtonRightPressed( ScrollingSource layer ) {

        if( layer == ScrollingSource.Level ) DetectHorizontalStop( levels, StopIndexChange.Increase );
        else if( layer == ScrollingSource.Ship ) DetectHorizontalStop( ships, StopIndexChange.Increase );
    }
}