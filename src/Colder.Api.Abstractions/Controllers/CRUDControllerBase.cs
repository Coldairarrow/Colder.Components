using AutoMapper;
using AutoMapper.QueryableExtensions;
using Colder.Common;
using Colder.DistributedId;
using Colder.EFCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Colder.Api.Abstractions.Controllers;

/// <summary>
/// 通用增删改查接口
/// 注：增删改查原始Controller封装
/// </summary>
public abstract class CRUDControllerBase<TKey, TEntity, TDto, TExtendDto, TSearchDto, TDbContext> : ApiControllerBase
    where TEntity : class
    where TSearchDto : new()
    where TDbContext : DbContext
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// 
    /// </summary>
    protected readonly TDbContext Db;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IDistributedId DistributedId;

    /// <summary>
    /// 
    /// </summary>
    protected CRUDControllerBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        Db = serviceProvider.GetRequiredService<TDbContext>();
        Mapper = serviceProvider.GetRequiredService<IMapper>();
        DistributedId = serviceProvider.GetRequiredService<IDistributedId>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual TKey NewId()
    {
        var keyType = typeof(TKey);

        if (keyType == typeof(int) || keyType == typeof(long))
        {
            return (TKey)(object)DistributedId.NewLongId();
        }
        else if (keyType == typeof(Guid))
        {
            return (TKey)(object)DistributedId.NewGuid();
        }
        else
        {
            throw new NotImplementedException("请重写NewId");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    protected virtual Task<IQueryable<TExtendDto>> GetQuery(TSearchDto queryParams)
    {
        return Task.FromResult(Db.Set<TEntity>().ProjectTo<TExtendDto>(Mapper.ConfigurationProvider));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    protected virtual Task Append(TExtendDto[] items)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="save"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    protected virtual async Task OnSaving(Func<Task> save, params SaveContext<TDto, TEntity>[] items)
    {
        await save();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected virtual TKey GetKeyValue(object obj)
    {
        return (TKey)obj.GetType().GetProperty("Id").GetValue(obj);
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <param name="input">参数</param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<ApiResult<PageList<TExtendDto>>> List(PageInput<TSearchDto> input)
    {
        var q = await GetQuery(input.Search);
        var pageList = await q.ToPageList(input);

        await Append(pageList.Items.ToArray());

        return Success(pageList);
    }

    /// <summary>
    /// 获取单条记录
    /// </summary>
    /// <param name="id">主键id</param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<ApiResult<TExtendDto>> Info(IdInput<TKey> id)
    {
        var data = await (await GetQuery(new TSearchDto())).Where($"Id == @0", id.Id).FirstOrDefaultAsync();

        var dto = Mapper.Map<TExtendDto>(data);
        await Append(new TExtendDto[] { dto });

        return Success(dto);
    }

    /// <summary>
    /// 保存（添加或更新）
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<ApiResult<TKey>> Save(TDto data)
    {
        if (GetKeyValue(data).Equals(default(TKey)))
        {
            data.SetPropertyValue("Id", NewId());
        }

        var entity = await Db.Set<TEntity>().Where($"Id == @0", GetKeyValue(data)).FirstOrDefaultAsync();
        TEntity entitySnap = null;
        if (entity == null)
        {
            entity = Mapper.Map<TEntity>(data);
            Db.Add(entity);
        }
        else
        {
            entitySnap = entity.DeepClone();

            Mapper.Map(data, entity);
        }

        await OnSaving(async () => await Db.SaveChangesAsync(),
            new SaveContext<TDto, TEntity>
            {
                OriginalEntity = entitySnap,
                Dto = data,
                CurrentEntity = entity
            });

        return Success(GetKeyValue(entity));
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids">id集合</param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<ApiResult> Delete(params TKey[] ids)
    {
        var deleteList = await Db.Set<TEntity>().Where($"@0.Contains(Id)", ids).ToListAsync();

        var deleteListSnap = deleteList.DeepClone();

        Db.RemoveRange(deleteList);

        await OnSaving(async () => await Db.SaveChangesAsync(),
            deleteListSnap.Select(x => new SaveContext<TDto, TEntity> { OriginalEntity = x }).ToArray());

        return Success();
    }
}