using System;

namespace Extensions {
    public static class IntExt {
        public static bool InRange(this int n, int min, int max, bool inclusive = true) => inclusive ? (n >= min && n <= max) : (n > min && n < max);

        public static void Clear(this int[] a) => Array.Clear(a, 0, a.Length);

        public static int IndexOf(this int[] a, int n) => Array.IndexOf(a, n);
        public static int LastIndexOf(this int[] a, int n) => Array.LastIndexOf(a, n);

        public static bool Eq(this int[] l, int[] r) {
            if (l == null && r == null) return true;
            if (l == null || r == null) return false;

            if (l.Length != r.Length) return false;

            for (int i = 0; i < l.Length; i++)
                if (l[i] != r[i]) return false;
            
            return true;
        }
    }
}