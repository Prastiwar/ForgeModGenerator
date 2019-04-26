namespace ForgeModGenerator
{
    public static class Math
    {
        public static float Lerp(float from, float to, float time) => time * (to - from) + from;

        public static double Lerp(double from, double to, double time) => time * (to - from) + from;

        public static float Clamp(float value, float min = 0.0f, float max = 1.0f)
        {
            if (value >= max)
            {
                return max;
            }
            else if (value <= min)
            {
                return min;
            }
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value >= max)
            {
                return max;
            }
            else if (value <= min)
            {
                return min;
            }
            return value;
        }
    }
}
