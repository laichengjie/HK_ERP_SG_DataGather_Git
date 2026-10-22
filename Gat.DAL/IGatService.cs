using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Linq.Expressions;
using Gat.Utils;

namespace Gat.DAL
{
    /// <summary>
    /// 数据提报服务接口
    /// </summary>
    public interface IGatService
    {
        #region 数据增删改

        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <typeparam name="T">ModuleBase</typeparam>
        /// <typeparam name="TTable">实体映射的Table</typeparam>
        /// <returns>Boolean</returns>
        HandleArgs InsertEntity<T>(T TEntity,bool submitChanges = true) where T : ModuleBase;

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T">ModuleBase</typeparam>
        /// <returns>Boolean</returns>
        HandleArgs UpdateEntity<T>(T TEntity, bool submitChanges = true) where T : ModuleBase;

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        HandleArgs DeleteEntity<T>(Expression<Func<T, bool>> predicate) where T : ModuleBase;

        /// <summary>
        /// 根据实体主键删除单个记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        HandleArgs DeleteEntity<T>(T TEntity, bool submitChanges = true) where T : ModuleBase;


        /// <summary>
        /// 取单个实体记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEntity">主要用来提供主键属性值</param>
        /// <returns></returns>
        T FetchEntity<T>(T TEntity) where T : ModuleBase;


        /// <summary>
        /// 查询所有记录，返回数据迭代变量，分页等其它操作在BLL层实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> QueryEntity<T>(Expression<Func<T, bool>> predicate) where T : ModuleBase;

        /// <summary>
        /// 取单个实体记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int CountEntity<T>(Expression<Func<T, bool>> predicate) where T : ModuleBase;

        #endregion
    }
}
