using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colder.Common
{
    /// <summary>
    /// 管道
    /// </summary>
    public class Pipeline<TContext> where TContext : IPipelineContext
    {
        private readonly IServiceProvider _serviceProvider;
        private List<Type> _valveTypes = new List<Type>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public Pipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValve"></typeparam>
        public void Add<TValve>() where TValve : IAbstractValve<TContext>
        {
            _valveTypes.Add(typeof(TValve));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Handle(TContext context)
        {
            foreach (var aType in _valveTypes)
            {
                var valve = ActivatorUtilities.CreateInstance(_serviceProvider, aType) as IAbstractValve<TContext>;
                await valve.Handle(context);
                if (context.Break)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 抽象阀门
    /// </summary>
    public interface IAbstractValve<TContext> where TContext : IPipelineContext
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task Handle(TContext context);
    }

    /// <summary>
    /// 管道上下文
    /// </summary>
    public interface IPipelineContext
    {
        /// <summary>
        /// 退出
        /// </summary>
        public bool Break { get; set; }
    }
}
