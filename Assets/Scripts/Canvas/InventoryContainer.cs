using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Этот класс служит для подсветки "пуговки" (кнопки на вид, но как таковую кнопку мы не используем) при наведении указателя на отсек инвентаря
public class InventoryContainer : MonoBehaviour, 
                                  IPointerEnterHandler, 
                                  IPointerExitHandler,
                                  IBeginDragHandler, 
                                  IDragHandler, 
                                  IEndDragHandler,
                                  IPointerDownHandler,
                                  IPointerUpHandler,
                                  IPointerClickHandler,
                                  IDropHandler {

    [SerializeField]
    private Color normal_color = Color.white;

    [SerializeField]
    private Color highlighted_color = Color.yellow;

    [System.NonSerialized]
    private Image button_image;
    public Image Button_image { get { return button_image; } }

    private InventoryItem item;
    private Inventory inventory;

    private Transform picture_transform;
    private Transform tooltip_transform;
    private Transform container_transform;
    private Transform drag_over_transform;

    private CanvasGroup canvas_group;
    
    [System.NonSerialized]
    private Vector3 
        touch_position = Vector3.zero,
        reset_position = Vector3.zero;

    public void SetButtonColor( Color color ) { button_image.color = color; }

    // Use this for initialization #################################################################################################################################################
	void OnEnable() {

        if( button_image == null ) {

            button_image = GetComponent<Image>();

            item = GetComponentInParent<InventoryItem>();
            inventory = GetComponentInParent<Inventory>();

            picture_transform = GetComponentInChildren<InventoryPicture>().GetComponent<Transform>();
            tooltip_transform = GetComponentInChildren<InventoryTooltip>().GetComponent<Transform>();
            container_transform = GetComponent<Transform>();
            drag_over_transform = inventory.Drag_picture_transform;

            canvas_group = GetComponent<CanvasGroup>();
        }
    }

    // Отслеживание перемещения указателя (мыши, джойстика, тача) ##################################################################################################################
    void Update() {

        // Положение подсказки пересчитывается только если сама подсказка активна
        if( inventory.Tooltip_is_active ) {

            if( Input.mousePresent ) touch_position = Input.mousePosition;
            else touch_position = Input.touches[0].position;

            touch_position.x += inventory.Pointer_offset_x;
            touch_position.y += inventory.Pointer_offset_y;

            inventory.Tooltip_transform.position = touch_position;
        }
    }
    
    // Подсвечиваем отсек при наведении на него указателя ##########################################################################################################################
    public void OnPointerEnter( PointerEventData eventData ) {

        if( !inventory.Is_drag ) {

            tooltip_transform.parent = drag_over_transform;
            inventory.ShowTooltip( tooltip_transform );
        }

        if( !item.Is_absent ) button_image.color = highlighted_color;
    }

    // Восстанавливаем цвет отсека при уходе с него указателя ######################################################################################################################
    public void OnPointerExit( PointerEventData eventData ) {

        tooltip_transform.parent = item.Cached_transform;
        inventory.HideTooltip();

        if( !item.Is_absent ) button_image.color = normal_color;
    }

    // Когда указатель нажат, также выключаем подсказку ############################################################################################################################
    public void OnPointerDown( PointerEventData eventData ) {

        inventory.HideTooltip();
    }

    // Когда указатель снова отпустили, вновь включаем подсказку ###################################################################################################################
    public void OnPointerUp( PointerEventData eventData ) {

        inventory.ShowTooltip( item.Tooltip_transform );
    }

    // Когда указатель снова отпустили, вновь включаем подсказку ###################################################################################################################
    public void OnPointerClick( PointerEventData eventData ) {

        inventory.ShowTooltip( item.Tooltip_transform );
    }

    // Когда начинается драг, перемещаем рисунок в специальный контейнер, чтобы он отображался поверх любых элементов ##############################################################
    public void OnBeginDrag( PointerEventData eventData ) {

        canvas_group.blocksRaycasts = false;

        // Если ячейка пустая или недоступна, не инициируем драг
        if( !item.Is_absent && !item.Is_empty ) {
        
            inventory.BeginDrag( item.Index );
            picture_transform.parent = drag_over_transform;
        }

        inventory.HideTooltip();
    }

    // Пока продолжается драг, рисунок следует за указателем #######################################################################################################################
    public void OnDrag( PointerEventData eventData ) {

        // Если ячейка была пустой или недоступной, то это не считается драгом
        if( inventory.Is_drag ) picture_transform.position = Input.mousePosition;
    }

    // По завершении драга восстанавливаем рисунку прежнего родителя ###############################################################################################################
    public void OnEndDrag( PointerEventData eventData ) {
        
        canvas_group.blocksRaycasts = true;

        // Вариант 1: когда указатель находится над недоступным отсеком, никакие действия не выполняются, кроме исчезновения подсказки
        if( item.Is_absent ) {

            inventory.HideTooltip();
        }

        // Вариант 2: когда указатель находится над пустым отсеком, просто перемещаем груз в этот отсек
        else if( item.Is_empty ) {

             inventory.ShowTooltip( item.Tooltip_transform );
        }

        // Вариант 3: меняем местами содержимое отсеков
        else if( (inventory.Begin_drag_item_index != inventory.End_drag_item_index) && (inventory.Begin_drag_item_index != -1) && (inventory.End_drag_item_index != -1) ) {

            inventory.ExchangeItems();
            inventory.RefreshHold();

            item.Picture_shadow.enabled = true;
        }

        // Вариант 4: выбрасываем груз из трюма
        else if( (inventory.Begin_drag_item_index != -1) && (inventory.End_drag_item_index == -1) && inventory.OutOfInventory( Input.mousePosition ) ) {

            Game.Player.Ship.UnloadFromHold( item.Value, true );
            item.Picture_image.sprite = inventory.Base_sprite;
            item.Picture_image.color = inventory.Absent_picture_color;
            item.Picture_shadow.enabled = false;

            item.Value = null;

            inventory.HideTooltip();
            inventory.RefreshHold();

            Game.Canvas.RefreshHoldIndicator( true );
        }

        // Вариант 5: все остальные случаи, при которых ничего менять не нужно - груз возвращается обратно в тот же отсек
        else {

            inventory.HideTooltip();
        }

        // Сбрасываем позицию рисунка в исходное состояние
        picture_transform.parent = container_transform;
        picture_transform.localPosition = reset_position;

        // Сбрасываем индексы отсеков в исходное состояние
        inventory.ResetDrags();
    }

    // Событие, поступающие при бросании рисунка в отсек #######################################################################################################################
    public void OnDrop( PointerEventData eventData ) {

        // Сброс возможен только в доступный пустой или заполненный контейнер
        if( !item.Is_absent ) inventory.EndDrag( item.Index );

        inventory.RefreshTooltip( item );
        inventory.ShowTooltip( item.Tooltip_transform );
    }
}