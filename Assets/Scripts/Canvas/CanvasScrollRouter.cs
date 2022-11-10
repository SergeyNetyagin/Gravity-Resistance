using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent( typeof( ScrollRect ) )]
public class CanvasScrollRouter : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    // Передаем родителям событие, которое отправляется перед возможным началом перемещения ####################################################################################
    public void OnInitializePotentialDrag( PointerEventData eventData ) {

        Transform parent = transform.parent;

        while( parent != null ) {

            foreach( var handler in parent.GetComponents<IInitializePotentialDragHandler>() ) handler.OnInitializePotentialDrag( eventData );
            parent = parent.parent;
        }
    }

    // Передаем родителям событие начала перемещения ###########################################################################################################################
    public void OnBeginDrag( PointerEventData eventData ) {

        Transform parent = transform.parent;

        while( parent != null ) {

            foreach( var handler in parent.GetComponents<IBeginDragHandler>() ) handler.OnBeginDrag( eventData );
            parent = parent.parent;
        }
    }

    // Передаем родителям событие перемещения ##################################################################################################################################
    public void OnDrag( PointerEventData eventData ) {

        Transform parent = transform.parent;

        while( parent != null ) {

            foreach( var handler in parent.GetComponents<IDragHandler>() ) handler.OnDrag( eventData );
            parent = parent.parent;
        }
    }

    // Передаем родителям событие завершения перемещения #######################################################################################################################
    public void OnEndDrag( PointerEventData eventData ) {

        Transform parent = transform.parent;

        while( parent != null ) {

            foreach( var handler in parent.GetComponents<IEndDragHandler>() ) handler.OnEndDrag( eventData );
            parent = parent.parent;
        }
    }
}