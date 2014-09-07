//via http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace ItzWarty
{
   public class SingleGlobalInstance : IDisposable
   {
      public bool hasHandle = false;
      Mutex mutex;

      private void InitMutex(Assembly assembly)
      {
         string appGuid = ((GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
         string mutexId = string.Format("Global\\{{{0}}}", appGuid);
         mutex = new Mutex(false, mutexId);

         var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
         var securitySettings = new MutexSecurity();
         securitySettings.AddAccessRule(allowEveryoneRule);
         mutex.SetAccessControl(securitySettings);
      }

      public SingleGlobalInstance(Assembly assembly, int TimeOut)
      {
         InitMutex(assembly);
         try
         {
            if (TimeOut <= 0)
               hasHandle = mutex.WaitOne(Timeout.Infinite, false);
            else
               hasHandle = mutex.WaitOne(TimeOut, false);

            if (hasHandle == false)
               throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
         }
         catch (AbandonedMutexException)
         {
            hasHandle = true;
         }
      }


      public void Dispose()
      {
         if (hasHandle && mutex != null)
            mutex.ReleaseMutex();
      }

      /// <summary>
      /// Attempts to acquire a reference to a singleglobalinstance object.
      /// If the reference cannot be created, our app is already being run, and we return null.
      /// Otherwise, the reference must be held until our application exits.  
      /// </summary>
      /// <returns></returns>
      public static SingleGlobalInstance Acquire(Assembly assembly, int timeout)
      {
         try
         {
            return new SingleGlobalInstance(assembly, timeout);
         }
         catch (Exception e)
         {
            return null;
         }
      }
   }
}
