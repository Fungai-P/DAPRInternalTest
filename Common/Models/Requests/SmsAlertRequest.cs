using Common.Interfaces.Requests;

namespace Common.Models.Requests
{
    public class SmsAlertRequest : IRequest
    {
        public string PhoneNumber { get; set; }
    }
}
