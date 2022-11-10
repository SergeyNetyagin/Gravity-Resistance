using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeIndicator : MonoBehaviour {

    [SerializeField]
    [Header( "SOURCE PREFAB WITH REAL PARAMETERS" )]
    private GameObject source_prefab;

    [SerializeField]
    [Header( "KEY ATTRIBUTE FOR LOADING SHIP'S INDICATOR" )]
    private IndicatorType indicator_type;

    [SerializeField]
    [Header( "CHILD ELEMENTS OF INDICATION" )]
    private RectTransform 
        panel_items; [SerializeField] private RectTransform
        upgrade_pointer;

    [SerializeField]
    private Color
        free_color = Color.gray,
        upgraded_color = Color.red;
    
    [SerializeField]
    [Header( "FIELD TO PUT A CURRENT VALUE OF INDICATOR" )]
    private EffectiveText text_cost_field;

    [HideInInspector]
    private Button button;

    [HideInInspector]
    private Indicator indicator;
    public Indicator Indicator { get { return indicator; } }

    private Image[] items_image = new Image[ 20 ];
    private RectTransform[] items_transform = new RectTransform[ 20 ];

    private Vector3 position;

    //public bool Is_saved { get { return indicator.Is_saved; } set { indicator.Is_saved = value; } }
    //public bool Is_loaded { get { return indicator.Is_loaded; } set { indicator.Is_loaded = value; } }

    public bool Is_full { get { return indicator.Maximum >= indicator.Upgrade_max_ship; } }
    public bool Is_pressed { get; set; }

    private bool is_enabled = false;
    private bool is_enabled_outside = false;
        
    private WaitForSeconds refresh_button_wait_for_seconds = new WaitForSeconds( 0.3f );

    public void EnableButton() { is_enabled_outside = true; }
    public void DisableButton() { is_enabled_outside = false; }

    public Indicator GetIndicator() { return indicator; }

	// Use this for initialization ##############################################################################################################################################
	void Start() {

        //Game.Level = LevelType._03_Main_menu;
	
        button = gameObject.GetComponent<Button>() as Button;

        indicator = new Indicator();
//        if( source_prefab != null ) indicator.CopyAll( source_prefab.GetComponent<Ship>().GetIndicator( indicator_type ) );
        InitItems().Refresh().RefreshResourceIndicator();

        StartCoroutine( RefreshBuyResourceButton() );
    }

    // Update is called once per frame ##########################################################################################################################################
	void Update() {
	
	}

    // Refresh enable or disable the button #####################################################################################################################################
    IEnumerator RefreshBuyResourceButton() {

        while( Game.Current_level == LevelType.Level_Menu ) {

            RefreshResourceIndicator();

            yield return refresh_button_wait_for_seconds;
        }

        yield break;
    }

    // Check for enable or disable button #######################################################################################################################################
    public void RefreshResourceIndicator() {

        if( is_enabled_outside ) {

            if( Is_full || (indicator.Upgrade_cost > Game.Money) ) if( button.interactable ) button.interactable = false;
            if( !Is_full && (indicator.Upgrade_cost <= Game.Money) ) if( !button.interactable ) button.interactable = true;
        }

        else {

            if( button.interactable ) button.interactable = false;
        }

        is_enabled = button.interactable;

        Refresh();
    }

    // Button buy resource pressed ##############################################################################################################################################
    public void EventButtonBuyResourcePressed() {

        RefreshResourceIndicator();

        if( !is_enabled ) return;

        Is_pressed = true;
        Increase().Refresh();

        Game.Money -= indicator.Upgrade_cost;

        RefreshResourceIndicator();
    }

    // Initialize image items ##################################################################################################################################################
    public UpgradeIndicator InitItems() {

        for( int i = 0; i < panel_items.childCount; i++ ) {

            items_image[i] = panel_items.GetChild( i ).GetComponent<Image>() as Image;
            items_transform[i] = panel_items.GetChild( i ).GetComponent<RectTransform>() as RectTransform;
        }

        position = upgrade_pointer.position;

        return this;
    }

    // Update the indicator ####################################################################################################################################################
    public UpgradeIndicator Refresh() {

        for( int i = 0; i < items_image.Length; i++ ) {

            if( i < ((int) (indicator.Maximum / indicator.Unit_size)) ) items_image[i].color = upgraded_color;
            else items_image[i].color = free_color;
        }

        position = upgrade_pointer.position;
        position.x = items_transform[ (int) (indicator.Upgrade_max_ship / indicator.Unit_size) - 1 ].position.x + (items_transform[1].position.x - items_transform[0].position.x) / 2;
        upgrade_pointer.position = position;

        text_cost_field.Rewrite( indicator.Upgrade_cost );

        return this;
    }

    // Increase the indicator ##################################################################################################################################################
    public UpgradeIndicator Increase() {

        indicator.Maximum += indicator.Unit_size;
        if( indicator.Maximum > indicator.Upgrade_max_ship) indicator.Maximum = indicator.Upgrade_max_ship;

        return this;
    }
}
