using UnityEngine;

// Этот класс служит в качестве признака того, что данный объект содержит подсказку с информацией об объекте
public class InventoryTooltip : MonoBehaviour {

    [Tooltip( "Строка, в которой сообщается название груза в отсеке трюма" )]
    public EffectiveText Text_item_name;

    [Tooltip( "Строка, в которой отображается краткое описание груза в отсеке трюма" )]
    public EffectiveText Text_item_description;

    [Tooltip( "Строка, в которой отображается масса груза в отсеке трюма" )]
    public EffectiveText Text_item_mass;

    [Tooltip( "Строка, в которой отображается стоимость груза в отсеке трюма" )]
    public EffectiveText Text_item_cost;
    
    // Use this for initialization #################################################################################################################################################
	void OnEnable() {
	
	}
}