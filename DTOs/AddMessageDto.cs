namespace ChatRealTime.DTOs
{
    public class AddMessageDto
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public required string Message { get; set; }
    }
}