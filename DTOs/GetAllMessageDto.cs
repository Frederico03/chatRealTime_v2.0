namespace ChatRealTime.DTOs
{
    public class GetAllMessageDto
    {
        public required string From { get; set; }
        public required string To { get; set; }
    }
}