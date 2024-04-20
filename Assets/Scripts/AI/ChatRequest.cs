namespace AI
{
    [System.Serializable]
    public class ChatRequest
    {
        public string model="gpt-3.5-turbo";
        public Message[] messages;
    }
}