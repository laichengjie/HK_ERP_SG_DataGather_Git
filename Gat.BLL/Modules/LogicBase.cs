using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gat.DAL;
using Gat.Utils;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace Gat.BLL.Modules 
{
    /// <summary>
    /// 业务逻辑模型基类
    /// <remarks>为了能体现主从关系，模型类可使用数据仓库来实例化</remarks>
    /// </summary>
    public class LogicBase
    {
        #region 受保护属性

        /// <summary>
        /// 数据提报数据层服务
        /// </summary>
        public GatRepository Owner { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public LogicBase() { }

        public LogicBase(ModuleBase moduleBase) 
        {
            if (moduleBase != null) this.CopyPropertyFrom(moduleBase); //moduleBase.CopyPropertyTo(this);
        }

        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public LogicBase(ModuleBase moduleBase,GatRepository owner)
        {
            if (moduleBase != null) this.CopyPropertyFrom(moduleBase); //moduleBase.CopyPropertyTo(this);
            this.Owner = owner;
        }

        #endregion

        #region 公开属性

        #endregion

        #region 实体查询

        #endregion

        #region 增删改查


        #endregion

        #region 重写方法

        #endregion

        #region 受保护方法


        /// <summary>
        /// 将指定类型的属性拷贝到本身的同名属性上
        /// <remarks>用于使用主键查询和更新实体时拷贝属性</remarks>
        /// </summary>
        /// <typeparam name="T">属性值来源类型</typeparam>
        /// <param name="pkcopy">是否拷贝主键值</param>
        /// <returns></returns>
        public virtual void CopyPropertyFrom(object TSource)
        {
            if (TSource == null) TSource = new object { };//default(T);

            Type typeSelf = this.GetType();         //本身类型，做为拷贝目标i
            Type typeSource = TSource.GetType();    //属性值来源类型

            PropertyInfo[] infos = typeSource.GetProperties(Extension.Public);
            PropertyInfo property = null;

            foreach (PropertyInfo info in infos)
            {
                property = typeSelf.GetProperty(info.Name, Extension.Public);
                if (property == null ||
                    property.PropertyType != info.PropertyType ||
                    !property.CanWrite) continue;

                //赋值属性
                object value1 = property.GetValue(this, null);
                object value2 = info.GetValue(TSource, null);
                if (value1 != value2)
                    property.SetValue(this, info.GetValue(TSource, null), null);
            }
        }

        /// <summary>
        /// 将本身的属性拷贝到指定类型的同名属性上
        /// <remarks>多用于BLL层属性复制</remarks>
        /// </summary>
        /// <typeparam name="T">属性的拷贝目标</typeparam>
        /// <param name="pkcopy">是否拷贝主键值</param>
        /// <returns></returns>
        public virtual void CopyPropertyTo(object TTarget)
        {
            if (TTarget == null) TTarget = new object { };

            Type typeSelf = this.GetType();         //属性值来源类型本身
            Type typeTarget = TTarget.GetType();    //类型，做为拷贝目标i

            PropertyInfo[] infos = typeSelf.GetProperties(Extension.Public);
            PropertyInfo property = null;

            foreach (PropertyInfo info in infos)
            {
                property = typeTarget.GetProperty(info.Name, Extension.Public);
                if (property == null ||
                    property.PropertyType != info.PropertyType ||
                    !property.CanWrite) continue;


                //赋值属性
                object value1 = property.GetValue(TTarget, null);
                object value2 = info.GetValue(this, null);
                if (value1 != value2)
                    property.SetValue(TTarget, info.GetValue(this, null), null);
            }
        }

        #endregion

        #region 私有方法

        #endregion
    }
}
