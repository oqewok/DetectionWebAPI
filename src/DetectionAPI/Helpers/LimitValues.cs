namespace DetectionAPI.Helpers
{
    /// <summary>
    /// Класс, описывающий ограничения по максимальному количеству запросов глобально
    /// </summary>
    public static class LimitValues
    {
        public static long ImageCountLimit = 100;
        public static long PlatesCountLimit = 150;
    }
}
