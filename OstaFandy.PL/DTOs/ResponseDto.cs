namespace OstaFandy.PL.DTOs
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; } 
        public string? Message { get; set; }  
        public T? Data { get; set; }           
        public int StatusCode { get; set; }
    }
}
