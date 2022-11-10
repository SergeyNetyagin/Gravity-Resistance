using UnityEngine;
using System.Collections;

public class InventoryTrade : MonoBehaviour {

    [System.NonSerialized]
    private bool is_active = false;
    public bool Is_active { get { return is_active; } }

	// Use this for initialization #################################################################################################################################################
	private void OnEnabe() {
	
        is_active = true;
	}

	// Меняет индикатор режима работы инвентаря с торговли на обычный режим ########################################################################################################
	private void OnDisable() {
	
        is_active = false;
	}
}
