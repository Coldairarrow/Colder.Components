using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Colder.Common
{
    /// <summary>
    /// 程序集帮助类
    /// </summary>
    public static class AssemblyHelper
    {
        static AssemblyHelper()
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var assemblies = Directory.GetFiles(rootPath, "*.dll")
                .Where(x => !new FileInfo(x).Name.StartsWith("System")
                    && !new FileInfo(x).Name.StartsWith("Microsoft"))
                .Select(x => Assembly.LoadFrom(x))
                .Where(x => !x.IsDynamic)
                .ToList();

            assemblies.ForEach(aAssembly =>
            {
                try
                {
                    AllTypes.AddRange(aAssembly.GetTypes());
                }
                catch
                {

                }
            });
        }

        public static readonly List<Type> AllTypes = new List<Type>();
    }
}
