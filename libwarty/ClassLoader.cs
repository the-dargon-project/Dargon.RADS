using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ItzWarty
{
   public static class ClassLoader
   {
      public static List<Assembly> EnumerateAssemblies(string directory, Func<string, bool> exclude = null)
      {
         var results = new List<Assembly>();
         foreach (var path in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
         {
            if (exclude != null && exclude(path))
               continue;

            try
            {
               results.Add(Assembly.LoadFile(path));
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               if (e is ReflectionTypeLoadException)
               {
                  var typeLoadException = e as ReflectionTypeLoadException;
                  var loaderExceptions = typeLoadException.LoaderExceptions;
                  loaderExceptions.ToList().ForEach(Console.WriteLine);
               }
            }
         }
         return results;
      }

      public static List<T> InstantiateClasses<T>(IEnumerable<Assembly> assemblies, Func<Type, T> instantiator = null)
         where T : class //or an interface
      {
         Debug.Assert(typeof(T).IsInterface || typeof(T).IsClass);

         if (instantiator == null)
            instantiator = (type) => (T)Activator.CreateInstance(type);

         var results = new List<T>();
         var genericPluginType = typeof(T);
         foreach (var assembly in assemblies)
         {
            try
            {
               foreach (var type in assembly.ExportedTypes.Where((t) => !t.IsAbstract && genericPluginType.IsAssignableFrom(t) && genericPluginType != t))
               {
                  try
                  {
                     results.Add(instantiator(type));
                  }
                  catch (ReflectionTypeLoadException ex)
                  {
                     ex.LoaderExceptions.ToList().ForEach(Console.WriteLine);
                  }
               }
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
         }
         return results;
      }

      public class AppDomainResolver : IDisposable
      {
         private IEnumerable<Assembly> m_assemblies;

         public AppDomainResolver(IEnumerable<Assembly> assemblies)
         {
            m_assemblies = assemblies;
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
         }

         private Assembly Resolver(object s, ResolveEventArgs e)
         {
            if (FastParseIsSatelliteAssembly(e.Name))
               return null;

            Console.WriteLine("Resolving Location of Assembly: " + e.Name);

            foreach (var assembly in m_assemblies)
               if (assembly.FullName == e.Name)
                  return assembly;
            Console.WriteLine("    Resolve Failed");
            return null;
         }

         /// <summary>
         /// http://connect.microsoft.com/VisualStudio/feedback/details/526836/wpf-appdomain-assemblyresolve-being-called-when-it-shouldnt
         /// </summary>
         /// <param name="assemblyName"></param>
         /// <returns></returns>
         public static bool FastParseIsSatelliteAssembly(string assemblyName)
         {
            int firstComma = assemblyName.IndexOf(',');

            if (string.Equals(assemblyName.Substring(firstComma - 10, 10), ".resources"))
            {
               int cultureStart = assemblyName.IndexOf("Culture=", firstComma) + 8;
               int cultureEnd = assemblyName.IndexOf(',', cultureStart);

               if (!string.Equals(assemblyName.Substring(cultureStart, cultureEnd - cultureStart), "neutral"))
                  return true;
            }

            return false;
         }

         /// <summary>
         /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
         /// </summary>
         public void Dispose()
         {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolver;
         }
      }
   }

   public class Plugin<T>
   {
   }
}
