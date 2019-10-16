namespace FakerTest
{
    public class Bar
    {
        public int isS;

        public Bar(int isS)
        {
            this.isS = isS;
        }

        public override string ToString()
        {
            return "Bar: isS = " + isS;
        }
    }
}