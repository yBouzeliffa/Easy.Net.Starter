using Newtonsoft.Json;

namespace Easy.Net.Starter.Exceptions
{
    public class ApiExceptionDto
    {
        public string ErrorCode { get; set; } = "";

        public string ErrorTitle { get; set; } = "";

        public string ErrorText { get; set; } = "";

        public string StackTrace { get; set; } = "";

        public string Message { get; set; } = "";

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
