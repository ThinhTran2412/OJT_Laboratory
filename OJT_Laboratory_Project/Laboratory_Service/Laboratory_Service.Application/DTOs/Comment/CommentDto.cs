namespace Laboratory_Service.Application.DTOs.Comment
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public Guid? TestOrderId { get; set; }
        public int? TestResultId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
