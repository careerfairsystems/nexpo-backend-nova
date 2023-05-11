using Nexpo.DTO;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;



namespace Nexpo.Services
{
    public class NotifyService
    {
        protected readonly IConfig _config;
        private readonly INotyfService _notyf;



        NotifyService(IConfig iConfig, INotyfService notyf)
        {
            _config = iConfig;
            _notyf = notyf;

            
        }

        public void NotifyAll(NotificationDTO dto)
        {
            _notyf.Custom(dto.Message, 5);
        }

        // public async NotificationDTO GetPastNNotifications(int n)
        // {
            
        // }

    }
}