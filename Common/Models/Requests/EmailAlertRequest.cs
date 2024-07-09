using Common.Interfaces.Requests;

namespace Common.Models.Requests
{
    public class EmailAlertRequest : IRequest
    {
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
    }
}
