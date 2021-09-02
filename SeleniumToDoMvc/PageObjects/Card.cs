namespace SeleniumToDoMvc.PageObjects
{
    public class Card
    {
        public string Id { get; internal set; }
        public bool IsCompleted { get; internal set; }
        public string Text { get; internal set; }
    }
}