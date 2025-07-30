namespace MineSweeper.Views.Shared
{
    public static class ViewHelpers
    {
        public static string GetNumberColor(int number)
        {
            switch (number)
            {
                case 1: return "blue";
                case 2: return "green";
                case 3: return "red";
                case 4: return "darkblue";
                case 5: return "maroon";
                case 6: return "teal";
                case 7: return "black";
                case 8: return "grey";
                default: return "black";
            }
        }
    }
}
