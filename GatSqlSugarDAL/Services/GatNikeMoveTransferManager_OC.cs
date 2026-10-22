using SqlSugar;
using System;
using System.Collections.Generic;
using GatEntities.DbModels;
namespace GatSqlSugarDAL.Services
{
    public class GatNikeMoveTransferManager_OC : Repository<GatNikeMoveTransfer_OC>
    {
        //当前类已经继承了 Repository 增、删、查、改的方法

        #region 教学方法
        /// <summary>
        /// 仓储方法满足不了复杂业务需求，业务代码请在这里面定义方法
        /// </summary>
        public void Study()
        {
            /*********查询*********/

            var data1 = base.GetById(1);                    // 根据ID查询
            var data2 = base.GetList();                     // 查询所有
            var data3 = base.GetList(it => 1 == 1);         // 根据条件查询  
                                                            // var data4 = base.GetSingle(it => 1 == 1);//根据条件查询一条,如果超过一条会报错

            var p = new PageModel() { PageIndex = 1, PageSize = 2 };// 分页查询
            var data5 = base.GetPageList(it => 1 == 1, p);
            Console.Write(p.TotalCount);                    // 返回总数

            var data6 = base.GetPageList(it => 1 == 1, p, it => SqlFunc.GetRandom(), OrderByType.Asc);// 分页查询加排序
            Console.Write(p.TotalCount);                    // 返回总数

            List<IConditionalModel> conModels = new List<IConditionalModel>(); // 组装条件查询作为条件实现 分页查询加排序
            conModels.Add(new ConditionalModel() { FieldName = typeof(GatNikeMoveTransfer_OC).GetProperties()[0].Name, ConditionalType = ConditionalType.Equal, FieldValue = "1" });// id=1
            var data7 = base.GetPageList(conModels, p, it => SqlFunc.GetRandom(), OrderByType.Asc);

            base.AsQueryable().Where(x => 1 == 1).ToList(); // 支持了转换成queryable,我们可以用queryable实现复杂功能

            /*********插入*********/
            var insertData = new GatNikeMoveTransfer_OC() { };// 测试参数
            var insertArray = new GatNikeMoveTransfer_OC[] { insertData };
            base.Insert(insertData);                        // 插入
            base.InsertRange(insertArray);                  // 批量插入
            var id = base.InsertReturnIdentity(insertData); // 插入返回自增列
            base.AsInsertable(insertData).ExecuteCommand(); // 我们可以转成 Insertable实现复杂插入

            /*********更新*********/
            var updateData = new GatNikeMoveTransfer_OC() { };// 测试参数
            var updateArray = new GatNikeMoveTransfer_OC[] { updateData };// 测试参数
            base.Update(updateData);                    // 根据实体更新
            base.UpdateRange(updateArray);              // 批量更新
                                                        // base.Update(it => new GatNikeMoveTransfer_OC() { StockTransferStatus = "Completed" }, it => it.TransID == 1);// 只更新StockTransferStatus列，其它列不更新，条件TransID=1
            base.AsUpdateable(updateData).ExecuteCommand(); // 转成Updateable可以实现复杂的插入

            /*********删除*********/
            var deldata = new GatNikeMoveTransfer_OC() { };// 测试参数
            base.Delete(deldata);                   // 根据实体删除
            base.DeleteById(1);                     // 根据主键删除
            base.DeleteById(new int[] { 1, 2 });    // 根据主键数组删除
            base.Delete(it => 1 == 2);              // 根据条件删除
            base.AsDeleteable().Where(it => 1 == 2).ExecuteCommand();// 转成Deleteable实现复杂的操作
        }
        #endregion
    }



}