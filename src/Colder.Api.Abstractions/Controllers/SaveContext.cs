namespace Colder.Api.Abstractions.Controllers;

/// <summary>
/// 保存上下文
/// </summary>
/// <typeparam name="TDto">Dto数据类型</typeparam>
/// <typeparam name="TEntity">数据库表实体类型</typeparam>
public class SaveContext<TDto, TEntity>
{
    /// <summary>
    /// Dto数据
    /// </summary>
    public TDto Dto { get; set; }

    /// <summary>
    /// 原实体(更新之前的数据，更新后不会改变)
    /// </summary>
    public TEntity OriginalEntity { get; set; }

    /// <summary>
    /// 当前实体(跟踪中的数据库实体数据，会随着更新而改变，若数据删除则为null)
    /// </summary>
    public TEntity CurrentEntity { get; set; }
}
