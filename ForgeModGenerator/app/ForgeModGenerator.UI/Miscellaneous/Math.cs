namespace ForgeModGenerator
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

        public static float Clamp(float value, float min = 0.0f, float max = 1.0f)
        {
            if (value >= max)
            {
                return min;
            }
            else if (value <= min)
            {
                return max;
            }
            return value;
        }
    }
}
