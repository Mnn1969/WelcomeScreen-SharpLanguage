namespace WelcomeScreenСSharpLanguage.Extensions
{
    internal static class RectangleEx
    {
        public static void Deconstruct(this System.Drawing.Rectangle rect, 
            out int left, out int top, out int width, out int height) =>
            (left, top, width, height) = (rect.Left, rect.Top, rect.Width, rect.Height);
        
    }
}
