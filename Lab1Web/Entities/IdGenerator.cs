namespace Lab1Web.Entities
{
    public class IdGenerator<T> where T : Entity
    {
        private IdGenerator() { }
        public static IdGenerator<T> Instance { get; } = new IdGenerator<T>();
        static int count = 0;
        public int Genereate()
        {
            return count++;
        }
    }
}
