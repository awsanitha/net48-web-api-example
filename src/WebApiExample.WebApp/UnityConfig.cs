using Unity;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore;
using WebApiExample.WebApp.Services;

namespace WebApiExample.WebApp
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            container
                .RegisterType<IUserService, UserService>()
                .RegisterType<IUnitOfWork, UserContext>();
        }
    }
}
