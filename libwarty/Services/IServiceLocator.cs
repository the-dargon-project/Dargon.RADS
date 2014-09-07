using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Services
{
   public interface IServiceLocator
   {
      void RegisterService(Type type, object provider);
      T GetService<T>();
   }
}
