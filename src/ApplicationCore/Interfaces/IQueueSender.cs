using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IQueueSender
    {
        Task SendPendingOrderMessage(int orderId);
    }
}
