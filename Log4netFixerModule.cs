using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Log4netFixer
{
  public class Log4netFixerModule : IHttpModule
  {
    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      FixNewLog4net();
    }


    /// <summary>
    /// We use a hack to allow our software to use a newer version.
    /// http://ponifaax.wordpress.com/2009/01/05/dll-version-conflict-with-log4net/
    /// </summary>
    public static void FixNewLog4net()
    {
      AppDomain current = AppDomain.CurrentDomain;
      current.AssemblyResolve += new ResolveEventHandler(ForwardLog4NetResolver);
    }

    // handler is called only when the CLR tries to bind
    // to the assembly and fails.
    private static Assembly ForwardLog4NetResolver(object sender, ResolveEventArgs args)
    {
      string assemblyName = args.Name.Substring(0, args.Name.IndexOf(","));
      if (assemblyName.StartsWith("log4net"))
      {
        var thisAssembly = new Uri(typeof(Log4netFixerModule).Assembly.CodeBase).AbsolutePath;
        string binFolder = Path.GetDirectoryName(thisAssembly);
        string log4netPath = Path.Combine(binFolder, "log4net-1.2.11", "log4net.dll");
        return Assembly.LoadFrom(log4netPath);
      }
      return null;
    }
  }
}
