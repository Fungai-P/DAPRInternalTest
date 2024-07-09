using Common.Interfaces.Requests;

namespace Common.Models.Requests
{
    public class PublishAlertRequest : IRequest
    {
        public string AlertType { get; set; }
        public DateTime PublishRequestTime { get; set; }
    }
}