using Common.Interfaces.Requests;

namespace Common.Models.Requests
{
    public class AlertRequest : IRequest
    {
        public string ClientName { get ; set ; }
        public IList<string> AlertTypes { get ; set; }
    }
}
