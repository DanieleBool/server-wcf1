using System.ServiceModel;

namespace Wcf.Common
{
    [ServiceContract]
    public interface Iestore
    {
        [OperationContract]
        void Ping(string message);
    }

}
