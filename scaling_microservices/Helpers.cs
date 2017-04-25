namespace scaling_microservices
{
    public class Helpers
    {
        private Helpers() { }

        static public bool EqualOrNone(string left, string right)
        {
            if (left == "" || right == "")
                return true;
            return left == right;
        }
    }
}
