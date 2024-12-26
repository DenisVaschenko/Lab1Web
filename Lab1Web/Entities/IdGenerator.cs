namespace Lab1Web.Entities
{
    public class IdGenerator<T>
    {
        private IdGenerator() { }
        public static IdGenerator<T> Instance { get; } = new IdGenerator<T>();
        int count = 0;
        public int Genereate()
        {
            return count++;
        }
    }
}
