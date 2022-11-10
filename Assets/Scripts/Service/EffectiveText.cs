using UnityEngine;
using UnityEngine.UI;
using System.Text;

[System.Serializable]
public class EffectiveText {

    private static StringBuilder _string = new StringBuilder( 100 );

    [SerializeField]
    [Tooltip( "Ссылка на тектовое поля компонента UI <Text>" )]
	private Text text;

    [SerializeField]
    [Tooltip( "Внутренний параметр текстовой строки: определяет максимально допустимую длину строки; по умолчанию = 100" )]
    [Range( 100, 5000 )]
	private int _capacity = 100;

	private StringBuilder _builder;

    public void SetColor( Color color ) { text.color = color; }
    public void SetActive( bool state ) { text.enabled = state; }

    public Color Get_color { get { return text.color; } }
    public Text Text_component { get { return text; } }
    public string Text_field { get { return text.text; } }
    public string Builder_string { get { return _builder.ToString(); } }
    public bool Empty { get { return (text == null); } }
    public int Length { get { return (text == null) ? 0 : text.text.Length; } }

    public EffectiveText RewriteSeparatedInt( int number ) { Clear(); return AppendSeparatedInt( number ); }
    public EffectiveText RewriteDottedFloat( float number, int decimal_signs = 1 ) { Clear(); return AppendDottedFloat( number, decimal_signs ); }
    public EffectiveText RewriteSeparatedFloat( float number, int decimal_signs = 1 ) { Clear(); return AppendSeparatedFloat( number, decimal_signs ); }

    // Формирует строку для целого числа, в которой отделены разряды по тысячам ################################################################################################
    public EffectiveText AppendSeparatedInt( int number ) {

 		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

        _string.Length = 0;
        _string.Append( number );

        for( int i = _string.Length - 1, count = 0, pos = i; i >= 0; i--, pos-- ) {

            if( (++count == 3) && (i > 0) ) {

                count = 0;
                _string.Insert( pos, Game.Separator_triad );
            }
        }

        _builder.Append( _string.ToString() );

        text.text = _builder.ToString();

        return this;
    }

    // Формирует строку для числа с плавающей запятой, в которой разряды отделены по тысячам ###################################################################################
    public EffectiveText AppendSeparatedFloat( float number, int decimal_signs = 1 ) {

		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

        AppendSeparatedInt( Mathf.FloorToInt( number ) );
        AppendDottedFloat( (number - Mathf.Floor( number )), decimal_signs, false );

        text.text = _builder.ToString();

        return this;
    }
    
    // Формирует строку для числа с плавающей запятой без разделения разрядов ##################################################################################################
    public EffectiveText AppendDottedFloat( float number, int decimal_signs = 1, bool use_integer_part = true ) {

        if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

        if( use_integer_part ) {

            Append( Mathf.FloorToInt( number ) );
            number -= Mathf.Floor( number );
        }

        if( decimal_signs > 0 ) Append( Game.Separator_float );

        for( int i = 0; i < decimal_signs; i++ ) {

            number *= 10f;
            Append( Mathf.FloorToInt( number ) );
            number -= Mathf.Floor( number );
        }

        text.text = _builder.ToString();

        return this;
    }
    
    // #########################################################################################################################################################################
	public EffectiveText Rewrite( int _value ) {

		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
		
		_builder.Length = 0;
		_builder.Append( _value );

        text.text = _builder.ToString();

        return this;
	}

    // #########################################################################################################################################################################
    public EffectiveText Rewrite( float _value ) {

		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
		
		_builder.Length = 0;
		_builder.Append( _value );

        text.text = _builder.ToString();

        return this;
	}

    // #########################################################################################################################################################################
    public EffectiveText Rewrite( string _str ) {

        if( _str == null ) return this;
		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
        if( _capacity < _str.Length ) _builder = new StringBuilder( _str, (_capacity = _str.Length * 2) );
		
		_builder.Length = 0;
		_builder.Append( _str );

        text.text = _builder.ToString();

        return this;
	}

    // #########################################################################################################################################################################
    public EffectiveText Append( int _value ) {

		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
        if( _capacity < (_builder.Length + 10) ) _builder = new StringBuilder( text.text, (_capacity *= 2) );

		_builder.Append( _value );

        text.text = _builder.ToString();

        return this;
	}

    // #########################################################################################################################################################################
    public EffectiveText Append( float _value ) {

		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
        if( _capacity < (_builder.Length + 10) ) _builder = new StringBuilder( text.text, (_capacity *= 2) );

		_builder.Append( _value );

        text.text = _builder.ToString();

        return this;
	}

    // #########################################################################################################################################################################
    public EffectiveText Append( string _str ) {

        if( _str == null ) return this;
		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
        if( _capacity < (_builder.Length + _str.Length) ) _builder = new StringBuilder( text.text, (_capacity = (_builder.Length + _str.Length) * 2) );

		_builder.Append( _str );

        text.text = _builder.ToString();

        return this;
	}

    // #########################################################################################################################################################################
    public EffectiveText Clear() {

		if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

        _builder.Length = 0;

        text.text = _builder.ToString();

        return this;
    }
}