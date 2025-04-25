namespace AttechServer.Shared.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public int ErrorCode { get; set; }
        public UserFriendlyException(int errorCode) : base(Consts.Exceptions.ErrorCode.GetMessage(errorCode))
        {
        }
    }
}
