namespace FakerTest
{
    public class MyGeneric<T>
    {
        public T _T;
        //private U _U;

        public MyGeneric(T field)//, U f)
        {
            _T = field;
            //_U = f;
        }

        public override string ToString()
        {
            return "MeGeneric: _T = " + _T;//+ ", _U = " + _U;
        }
    }
}