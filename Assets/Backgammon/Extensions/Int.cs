namespace Extensions {
    public static class IntExt {
        public static bool InRange(this int n, int min, int max, bool inclusive = true) => inclusive ? (n >= min && n <= max) : (n > min && n < max);
    }
}