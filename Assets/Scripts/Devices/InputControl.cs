#define DISABLE_SCREEN_JOYSTICK

using UnityEngine;
using UnityEngine.Networking;

public enum TouchType {

    Touch_up,
    Touch_down,
    Touch_single_drag,
    Touch_double_drag
}

public class InputControl : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Использовать контроль горизонтальной тяги (если отключен, разрешено управление только вертикальной тягой, если оно включено)" )]
    private bool use_horizontal_control = true;
    public void EnableHorizontalControl() { use_horizontal_control = true; }
    public void DisableHorizontalControl() { use_horizontal_control = false; }

    [SerializeField]
    [Tooltip( "Использовать контроль вертикальной тяги (если отключен, разрешено управление только горизонтальной тягой, если оно включено)" )]
    private bool use_vertical_control = true;
    public void EnableVerticalControl() { use_vertical_control = true; }
    public void DisableVerticalControl() { use_vertical_control = false; }

    [SerializeField]
    [Tooltip( "Нижняя панель или основа джойстика, по которому перемещается ползунок управления" )]
    private RectTransform joystick_box_transform;
    private GameObject joystick_box;

    [SerializeField]
    [Tooltip( "Ползунок управления" )]
    private RectTransform joystick_control_transform;

    [SerializeField]
    [Tooltip( "Максимальное расстояние, на которое может перемещаться ползунок джойстика" )]
    private Vector2 max_delta_rect = new Vector2( 25f, 25f );

    private Vector2 
        thrust = Vector2.zero,
        thrust_direction = Vector2.zero,
        thrust_acceleration = Vector2.zero,
        touch_position = Vector2.zero;

    [System.NonSerialized]
    private Vector3 current_position = Vector3.zero;

    [System.NonSerialized]
    private bool space_key_pressed = false;
    public bool Space_key_pressed { get { bool key_value = space_key_pressed; space_key_pressed = false; return key_value; } }

    [System.NonSerialized]
    private bool mouse_button_pressed = false;
    public bool Mouse_button_pressed { get { bool mouse_value = mouse_button_pressed; mouse_button_pressed = false; return mouse_value; } }

    private bool control_is_disabled = false;
    public bool Control_is_disabled { get { return control_is_disabled; } }
    public void EnableControl() { control_is_disabled = false; Joystick_control_is_disabled = false; }
    public void DisableControl() { control_is_disabled = true; Joystick_control_is_disabled = true; joystick_box.SetActive( false ); }

    public bool Joystick_control_is_disabled { get; set; }

    // Awake ###################################################################################################################################################################
    void Awake() {

        joystick_box = joystick_box_transform.gameObject;
        joystick_box.SetActive( false );
    }

    // Starting initialization #################################################################################################################################################
    void Start() {

        thrust_acceleration = Game.Player.Thrust_acceleration;
    }

    // Input control ###########################################################################################################################################################
    void Update() {

        // Обработка нажатия любой клавиши при включённом брифинге (нужно также "съесть" клавишу ESC, чтобы она не сработала сразу после окончания брифинга)
        if( Game.Level.Is_brief_mode ) {

            if( Input.GetMouseButtonUp( 0 ) || Input.GetMouseButtonUp( 1 ) || Input.GetMouseButtonUp( 2 ) ) mouse_button_pressed = true;
            else if( Input.GetKeyUp( KeyCode.Space ) ) space_key_pressed = true;
            return;
        }

        // Обработка нажатий клавиш во время игрового процесса
        else if( !Game.Is( GameState.Loading ) && !Game.Is( GameState.Starting ) && !Game.Is( GameState.Restarting ) ) {
            
            if( Input.GetKeyUp( KeyCode.Escape ) ) {

                Game.Canvas.EventKeyEscapePressed();
                return;
            }
        }

        // Если полный запрет на любое управление
        if( control_is_disabled ) return;

        /////////////////////////////////////////////////////////////////////
        // Здесь далее можно обрабатывать все остальные клавиши, кроме ESC //
        /////////////////////////////////////////////////////////////////////

        thrust = Game.Player.Thrust;

        #if UNITY_IOS
        InputFromIPhone();
        #elif UNITY_ANDROID
        InputFromAndroid();
        #elif UNITY_EDITOR_WIN
        InputFromWindows();
        #elif UNITY_STANDALONE_WIN
        InputFromWindows();
        #elif UNITY_EDITOR_OSX
        InputFromMac();
        #elif UNITY_STANDALONE_OSX
        InputFromMac();
        #endif

        Game.Player.Thrust = thrust;
    }

    // Inuput from Windows #####################################################################################################################################################
    void InputFromWindows() {

        #if DISABLE_SCREEN_JOYSTICK
        ThrustFormKeyboard();
        return;

        #else
        // Если экранный джойстик запрещён, управляем только от клавиатуры
        if( Joystick_control_is_disabled ) {

            ThrustFormKeyboard();
        }

        // Полное управление от различных устройств
        else {

            if( Input.GetMouseButtonDown( 0 ) ) ThrustFromTouch( TouchType.Touch_down );
            else if( Input.GetMouseButtonUp( 0 ) ) ThrustFromTouch( TouchType.Touch_up );
            else if( Input.GetMouseButton( 0 ) ) ThrustFromTouch( TouchType.Touch_single_drag );
            else ThrustFormKeyboard();
        }
        #endif
    }

    // Inuput from Mac OS ######################################################################################################################################################
    void InputFromMac() {

        #if DISABLE_SCREEN_JOYSTICK
        ThrustFormKeyboard();
        return;

        #else
        // Если экранный джойстик запрещён, управляем только от клавиатуры
        if( Joystick_control_is_disabled ) {

            ThrustFormKeyboard();
        }

        // Полное управление от различных устройств
        else {

            if( Input.GetMouseButtonDown( 0 ) ) ThrustFromTouch( TouchType.Touch_down );
            else if( Input.GetMouseButtonUp( 0 ) ) ThrustFromTouch( TouchType.Touch_up );
            else if( Input.GetMouseButton( 0 ) ) ThrustFromTouch( TouchType.Touch_single_drag );
            else ThrustFormKeyboard();
        }
        #endif
    }
    
    // Input from Android ######################################################################################################################################################
    void InputFromAndroid() {

        // Using only for one touch
        if( Input.touchCount != 1 ) return;
        
        // Detecting of touch (if it will be works when touch emulate a mouse)
        if( Input.GetMouseButtonDown( 0 ) ) ThrustFromTouch( TouchType.Touch_down );
        else if( Input.GetMouseButtonUp( 0 ) ) ThrustFromTouch( TouchType.Touch_up );
        else if( Input.GetMouseButton( 0 ) ) ThrustFromTouch( TouchType.Touch_single_drag );
    }

    // Input from iPhone #######################################################################################################################################################
    void InputFromIPhone() {

        // Using only for one touch
        if( Input.touchCount != 1 ) return;
        
        // Detecting of touch (if it will be works when touch emulate a mouse)
        if( Input.GetMouseButtonDown( 0 ) ) ThrustFromTouch( TouchType.Touch_down );
        else if( Input.GetMouseButtonUp( 0 ) ) ThrustFromTouch( TouchType.Touch_up );
        else if( Input.GetMouseButton( 0 ) ) ThrustFromTouch( TouchType.Touch_single_drag );
    }

    // Use the joystick for control of the ship ################################################################################################################################
    void ThrustFromTouch( TouchType touch_type ) {

        if( !use_horizontal_control && !use_vertical_control ) return;

        if( Input.mousePresent ) touch_position = Input.mousePosition;
        else touch_position = Input.touches[0].position;

        switch( touch_type ) {

            // Activate the joystick -------------------------------------------------------------------------------------------------------------------------------------------
            case TouchType.Touch_down:

                if( Game.Player.Is_autolanding ) Game.Player.ResetState( PlayerState.Autolanding );

                touch_position.x *= Game.Control.Screen_rate_x;
                touch_position.y *= Game.Control.Screen_rate_y;

                joystick_box_transform.anchoredPosition3D = touch_position;
                joystick_control_transform.anchoredPosition3D = current_position;

                if( !joystick_box.activeInHierarchy ) joystick_box.SetActive( true );

                break;

            // Deactivate the joystick -----------------------------------------------------------------------------------------------------------------------------------------
            case TouchType.Touch_up:

                if( joystick_box_transform.gameObject.activeInHierarchy ) joystick_box_transform.gameObject.SetActive( false );

                current_position = Vector3.zero;

                break;

            // Change the joystick position ------------------------------------------------------------------------------------------------------------------------------------
            case TouchType.Touch_single_drag:

                if( use_horizontal_control ) current_position.x = (touch_position.x * Game.Control.Screen_rate_x) - joystick_box_transform.anchoredPosition3D.x;
                if( use_vertical_control ) current_position.y = (touch_position.y * Game.Control.Screen_rate_y) - joystick_box_transform.anchoredPosition3D.y;

                if( Mathf.Abs( current_position.x ) > max_delta_rect.x ) current_position.x = max_delta_rect.x * ((current_position.x > 0f) ? 1f : -1f);
                if( Mathf.Abs( current_position.y ) > max_delta_rect.y ) current_position.y = max_delta_rect.y * ((current_position.y > 0f) ? 1f : -1f);

                joystick_control_transform.anchoredPosition3D = current_position;

                // Calculate the thrust in depending of the joystick position
                if( use_horizontal_control ) thrust.x = current_position.x / max_delta_rect.x * Game.Player.Current_thrust_max_x;
                if( use_vertical_control ) thrust.y = current_position.y / max_delta_rect.y * Game.Player.Current_thrust_max_y;

                break;

            default: break;
        }
    }

    // Set the new thrust value ################################################################################################################################################
    void ThrustFormKeyboard() {

        thrust_direction.x = 0f;
        thrust_direction.y = 0f;

        // /////////////////////////////// //
        // Анализ клавиш вертикальной тяги // 
        // /////////////////////////////// //
        if( Input.GetKeyUp( KeyCode.UpArrow ) || 
            Input.GetKeyUp( KeyCode.DownArrow ) ||
            Input.GetKeyUp( KeyCode.W ) ||
            Input.GetKeyUp( KeyCode.S ) ) thrust_direction.y = 0f;

        else if( Input.GetKey( KeyCode.UpArrow ) ||
                 Input.GetKey( KeyCode.W ) ) thrust_direction.y = 1f;

        else if( Input.GetKey( KeyCode.DownArrow ) ||
                 Input.GetKey( KeyCode.S ) ) thrust_direction.y = Game.Player.At_station ? 0f : -1f;

        // ///////////////////////////////// //
        // Анализ клавиш горизонтальной тяги // 
        // ///////////////////////////////// //
        if( Game.Player.At_station ||
            Input.GetKeyUp( KeyCode.RightArrow ) || 
            Input.GetKeyUp( KeyCode.LeftArrow ) || 
            Input.GetKeyUp( KeyCode.A ) || 
            Input.GetKeyUp( KeyCode.D ) ) thrust_direction.x = 0f;

        else if( Input.GetKey( KeyCode.LeftArrow ) ||
                 Input.GetKey( KeyCode.A ) ) thrust_direction.x = Game.Player.Is( PlayerState.On_surface | PlayerState.Landing_zone ) ? 0f : -1f;

        else if( Input.GetKey( KeyCode.RightArrow ) ||
                 Input.GetKey( KeyCode.D ) ) thrust_direction.x = Game.Player.Is( PlayerState.On_surface | PlayerState.Landing_zone ) ? 0f : 1f;
 
        // ///////////////////////////// //
        // Управление вертикальной тягой // 
        // ///////////////////////////// //
        if( thrust_direction.y == 0f ) {

            if( !Game.Player.Is_autolanding ) thrust.y = 0f;
        }

        else { 

            if( Game.Player.Is_autolanding ) Game.Player.ResetState( PlayerState.Autolanding );

            thrust.x = Game.Player.Thrust_x;
            thrust.y = (((Game.Player.Thrust_y < 0f) && (thrust_direction.y > 0f)) || ((Game.Player.Thrust_y > 0f) && (thrust_direction.y < 0f))) ? 0f : Game.Player.Thrust_y;
            thrust.y += thrust_direction.y * ((Mathf.Abs( Game.Player.Thrust_y ) >= Mathf.Abs( Game.Player.Current_thrust_max_y )) ? 0f : thrust_acceleration.y * Time.deltaTime);
        }

        // /////////////////////////////// //
        // Управление горизонтальной тягой // 
        // /////////////////////////////// //
        if( thrust_direction.x == 0f ) {

            if( !Game.Player.Is_autolanding ) thrust.x = 0f;
        }

        else {

            if( Game.Player.Is_autolanding ) Game.Player.ResetState( PlayerState.Autolanding );

            thrust.x = (((Game.Player.Thrust_x < 0f) && (thrust_direction.x > 0f)) || ((Game.Player.Thrust_x > 0f) && (thrust_direction.x < 0f))) ? 0f : Game.Player.Thrust_x;
            thrust.x += thrust_direction.x * ((Mathf.Abs( Game.Player.Thrust_x ) >= Mathf.Abs( Game.Player.Current_thrust_max_x )) ? 0f : thrust_acceleration.x * Time.deltaTime);
            thrust.y = (thrust.y == 0f) ? Game.Player.Thrust_y : thrust.y;
        }

        // Отключить экранный джойстик, чтобы не было коллизии с клавиатурой
        if( joystick_box.activeInHierarchy ) joystick_box.SetActive( false );
    }
}