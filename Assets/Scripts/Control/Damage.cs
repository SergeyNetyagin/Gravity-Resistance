using UnityEngine;

[System.Serializable]
public class Damage {

    [Tooltip( "Ресурс корабля, который подвергается повреждениям" )]
    public IndicatorType Indicator_type = IndicatorType.Unknown;

    [Tooltip( "Коэффициент повреждения на одну единицу ресурса без учёта скорости столкновения: 1) от однократного столкновения с опасным объектом; 2) либо коэффициент повреждения за секунду при попадании в опасные зоны" )]
    [Range( 0.01f, 1.0f )]
    public float Strength = 0.1f;
}
