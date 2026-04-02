using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public ErrorTypeEnum Type { get; }

        public DomainException(string message, ErrorTypeEnum type) : base(message)
        {
            Type = type;
        }
    }
}
