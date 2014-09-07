using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Services
{
   public class ServiceLocator : IServiceLocator
   {
      private Dictionary<Type, object> providerByType = new Dictionary<Type, object>(); 

      public void RegisterService(Type type, object provider) { providerByType.Add(type, provider); }

      public T GetService<T>() { return (T)providerByType[typeof(T)]; }
   }
}
