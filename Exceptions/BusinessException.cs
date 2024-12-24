namespace Easy.Net.Starter.Exceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; set; } = string.Empty;

        public string ErrorTitle { get; set; } = string.Empty;

        public string ErrorText { get; set; } = string.Empty;

        public string ErrorDetails { get; set; } = string.Empty;

        public BusinessException(string errorTitle, string errorText, string errorCode)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
            ErrorTitle = errorTitle;
        }

        public BusinessException()
        {

        }
    }
}