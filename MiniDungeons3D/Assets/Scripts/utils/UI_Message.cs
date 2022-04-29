public class UI_Message
{

    public string Title { get; set; }
    public string Message { get; set; }
    public UI_Message(string Title, string Message)
    {
        this.Title = Title;
        this.Message = Message;
    }
}