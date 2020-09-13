using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Colder.CommonUtil
{
    /// <summary>
    /// 程序集帮助类
    /// </summary>
    public static class AssemblyHelper
    {
        static AssemblyHelper()
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assemblies = Directory.GetFiles(rootPath, "*.dll")
                .Where(x => !new FileInfo(x).Name.StartsWith("System")
                    && !new FileInfo(x).Name.StartsWith("Microsoft"))
                .Select(x => Assembly.LoadFrom(x))
                .Where(x => !x.IsDynamic)
                .Concat(new Assembly[] { Assembly.GetEntryAssembly() })
                .Distinct()
                .ToList();

            List<Assembly> allAssemblies = new List<Assembly>();
            List<Type> allTypes = new List<Type>();
            assemblies.ForEach(aAssembly =>
            {
                try
                {
                    allTypes.AddRange(aAssembly.GetTypes());
                    allAssemblies.Add(aAssembly);
                }
                catch
                {

                }
            });

            AllTypes = allTypes.ToArray();
            AllAssemblies = allAssemblies.ToArray();
        }

        /// <summary>
        /// 所有类型
        /// </summary>
        public static readonly Type[] AllTypes;

        /// <summary>
        /// 所有程序集
        /// </summary>
        public static readonly Assembly[] AllAssemblies;
    }
}
