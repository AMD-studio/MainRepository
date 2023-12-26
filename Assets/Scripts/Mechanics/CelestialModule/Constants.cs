using System;

namespace Celestial
{
    public static class EARTH_CONSTS
    {
        public const float EARTH_DAY = 24.0f; // Продолжительность реального дня на Земле в часах
        public const float EARTH_YEAR = 365.25f; // Продолжительность реального года на Земле в днях

        public const float ROTATION_PERIOD = 1.0f; // Период оборота Земли вокруг своей оси в сутках
    }

    public static class SUN_CONSTS
    {
        public const float SUN_YEAR = 1.0f; // Для упрощения, год на солнце примем равным году на Земле

        public const float DAY_NIGHT_CYCLE = 1.0f; // Один полный цикл дня и ночи в игре (может быть изменен для скорости)
    }
}
