// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("pVDkl1f7G+Djgo2trJch8C7M6iOgIy0iEqAjKCCgIyMijAMkNgSywxL77aMDThytQIL1iPndCdoXoVi2VtJicXYHEJEOWp+LgXwYEeZyO2u+apvltvNOEowNGEk1tLG6wa/vHp7SBcQ+PJrsTJ+8xyzEsEd/iteL1URs8SnEH9NJL3rdPnNVxGn3PCTYRh2NeRY5sRxDfQtgdDnWc1IvAZuM0jcBcr6GlPNHB+sVJukGNDsLDYDqcwu7u3r+EQMiprk7O0bgrnS6q1raaQ/SRjv9flhlgtYWE9FTmBKgIwASLyQrCKRqpNUvIyMjJyIh3IJfmLfYKPkhiOo3GcZUnmD4ZKz2bYJSxjM/Yf9qcj2zC7EJ6Zji576Ze5AxmmH3nSAhIyIj");
        private static int[] order = new int[] { 5,1,7,10,11,6,12,9,8,9,10,12,12,13,14 };
        private static int key = 34;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
