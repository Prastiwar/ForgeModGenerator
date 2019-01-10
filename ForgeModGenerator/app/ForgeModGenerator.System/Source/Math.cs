namespace ForgeModGenerator.Logic
{
    public static class Math
    {
        public static float Lerp(float from, float to, float time)
        {
            return time * (to - from) + from;
        }

        public static double Lerp(double from, double to, double time)
        {
            return time * (to - from) + from;
        }
    }
}
