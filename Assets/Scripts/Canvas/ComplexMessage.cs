using UnityEngine;
using System;

[System.Serializable]
public class ComplexMessage : IComparable<ComplexMessage> {

    [SerializeField]
    private string text_key;
    public string Text_key { get { return text_key; } }

    [SerializeField]
    private string sound_key;
    public string Sound_key { get { return sound_key; } }

    [SerializeField]
    private string voice_key;
    public string Voice_key { get { return voice_key; } }

    [SerializeField]
    [Tooltip( "Будет ли мигать текст сообщения, или нет" )]
    private bool use_blinking = true;
    public bool Use_blinking { get { return use_blinking; } }

    [SerializeField]
    [Tooltip( "Скорость мерцания текста; по умолчанию = 1" )]
    [Range( 0.1f, 10f )]
    private float blinking_speed = 1f;
    public float Blinking_speed { get { return blinking_speed; } }

    [SerializeField]
    [Tooltip( "Цвет текста сообщения" )]
    private Color color = new Color( 1f, 1f, 1f, 1f );
    public Color Color { get { return color; } }

    [SerializeField]
    [Tooltip( "Изображение персонажа, передающего сообщение" )]
    private Sprite character;
    public Sprite Character { get { return character; } }

    [SerializeField]
    [Tooltip( "Приоритет сообщения от 1 до 10 (чем выше, тем позднее оно появится)" )]
    [Range( 1, 10 )]
    private int priority = 5;
    public int Priority { get { return priority; } }

    [SerializeField]
    [Tooltip( "Максимальное время отображения текста сообщения в секундах; по умолчанию = 5" )]
    [Range( 1, 60f )]
    private float max_time = 5f;
    public float Max_time { get { return max_time; } }

    public float Usage_time { get; set; }

    // Interface method ########################################################################################################################################################
    public int CompareTo( ComplexMessage other ) {

        if( Usage_time < other.Usage_time ) return 1;
        else if( Usage_time > other.Usage_time ) return -1;
   
        if( Priority < other.Priority ) return 1;
        else if( Priority > other.Priority ) return -1;

        return 0;
    }
}