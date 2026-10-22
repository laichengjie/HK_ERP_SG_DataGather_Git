using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Customer
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Gat.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;

namespace Gat.DAL
{
    /// <summary>
    /// 数据模型基类
    /// </summary>
    /// <typeparam name="T">提供有关与数据库映射的信息，求解</typeparam>
    public abstract class  ModuleBase : IDataErrorInfo
    {
        #region 私有成员

        private string _dataError = string.Empty;
        private Dictionary<string, string> _dataErrors = new Dictionary<string, string>();
        private IList<string> _primaryKeys = null;               //主键集合
        private IDictionary<string, object> _primaryKeyValues;   //主键值集合
        private Type _type;                                      //类型声明            
        
        #endregion

        #region 公开成员

        /// <summary>
        /// 模型数据是否合法
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.Validate();
            }
        }

        /// <summary>
        /// 错误集合
        /// </summary>
        public IDictionary<string, string> Errors
        {
            get
            {
                return _dataErrors;
            }
        }

        /// <summary>
        /// 主键集合
        /// </summary>
        public IList<string> PrimaryKeys
        {
            get
            {
                if (_primaryKeys != null) return _primaryKeys;

                _primaryKeys = new List<string>();
                ColumnAttribute attribute = null;
                PropertyInfo[] propertyInfo = _type.GetProperties(Extension.Public);
                foreach (PropertyInfo property in propertyInfo)
                {
                    attribute = this.FetchColumnAttribute(property); //property.GetColumnAttribute();
                    if (attribute != null &&
                        attribute.IsPrimaryKey) _primaryKeys.Add(property.Name);
                }
                return _primaryKeys;
            }
        }

        /// <summary>
        /// 主键值
        /// </summary>
        public IDictionary<string, object> PrimaryKeyValues
        {
            get
            {
                if (_primaryKeyValues != null) return _primaryKeyValues;
                
                _primaryKeyValues = new Dictionary<string, object>();
                if (PrimaryKeys.Count == 0) return _primaryKeyValues;
                
                PropertyInfo property = null;
                foreach (string pk in PrimaryKeys)
                {
                    property = _type.GetProperty(pk, Extension.Public);
                    if (property != null) _primaryKeyValues.Add(pk, property.GetValue(this, null));
                }
                return _primaryKeyValues;
            }
        }

        #endregion

        #region 构造函数

        protected ModuleBase()
        {
            _type = this.GetType();
            //注册元数据的映射关系
            foreach (MetadataTypeAttribute m in _type.GetCustomAttributes(typeof(MetadataTypeAttribute), true))
            {
                //PropertyDescriptor p = new 
                TypeDescriptor.AddProviderTransparent(
                        new AssociatedMetadataTypeTypeDescriptionProvider(_type, m.MetadataClassType), _type
                    );
            }
        }

        #endregion

        #region IDataErrorInfo成员

        /// <summary>
        /// 获取指示对象何处出错的错误信息。
        /// </summary>
        public string Error
        {
            get { return _dataError; }
        }

        /// <summary>
        /// 获取具有给定名称的属性的错误信息。
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                if (_dataErrors.ContainsKey(columnName))
                    return _dataErrors[columnName];
                return null;
            }
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 验证数据是否合法
        /// </summary>
        /// <returns></returns>
        protected virtual bool Validate()
        {
            var results = new List<ValidationResult>();
            this.ClearError();

            if (!Validator.TryValidateObject(this, new ValidationContext(this, null, null), results, true))
            {
                results.ForEach(r => this.AddError(r.MemberNames.FirstOrDefault(), r.ErrorMessage));
                return false;
            }
            return true;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 清空错误消息
        /// </summary>
        private void ClearError()
        {
            var keys = new string[_dataErrors.Count];
            _dataErrors.Keys.CopyTo(keys, 0);
            foreach (var key in keys)
            {
                this.RemoveError(key);
            }
        }

        /// <summary>
        /// 添加一条错误消息
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="message"></param>
        private void AddError(string columnName, string message)
        {
            if (!_dataErrors.ContainsKey(columnName))
                _dataErrors.Add(columnName, message);
            else
                _dataErrors[columnName] = message;
            //this.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 删除一条错误消息
        /// </summary>
        /// <param name="columnName"></param>
        private void RemoveError(string columnName)
        {
            if (_dataErrors.ContainsKey(columnName))
            {
                _dataErrors.Remove(columnName);
                //this.OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// 将指定类型的属性拷贝到本身的同名属性上
        /// <remarks>用于使用主键查询和更新实体时拷贝属性</remarks>
        /// </summary>
        /// <typeparam name="T">属性值来源类型</typeparam>
        /// <param name="pkcopy">是否拷贝主键值</param>
        /// <returns></returns>
        public virtual void CopyPropertyFrom( object TSource) //where T : ModuleBase
        {
            if (TSource == null) TSource = new object { };//default(T);

            //Type typeSelf = this.GetType();         //本身类型，做为拷贝目标i
            Type typeSource = TSource.GetType();    //属性值来源类型

            PropertyInfo[] infos = typeSource.GetProperties(Extension.Public);
            PropertyInfo property = null;

            foreach (PropertyInfo info in infos)
            {
                property = _type.GetProperty(info.Name, Extension.Public);
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
        public virtual void CopyPropertyTo(object TTarget) //where T : ModuleBase
        {
            if (TTarget == null) TTarget = new object { };

            //Type typeSelf = this.GetType();         //属性值来源类型本身
            Type typeTarget = TTarget.GetType();    //类型，做为拷贝目标i

            PropertyInfo[] infos = _type.GetProperties(Extension.Public);
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

        /// <summary>
        /// 取属性的映射特性
        /// </summary>
        /// <param name="property">属性对象</param>
        /// <returns></returns>
        public virtual ColumnAttribute FetchColumnAttribute(PropertyInfo property)
        {
            if (property == null) return null;

            return Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;
        }

        /// <summary>
        /// 取属性的映射特性
        /// </summary>
        /// <param name="property">属性对象</param>
        /// <returns></returns>
        public virtual ColumnAttribute FetchColumnAttribute(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            PropertyInfo property = this.GetType().GetProperty(propertyName,Extension.Public);
            if (property == null) return null;

            return Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;
        }

        /// <summary>
        /// 取自增列名称
        /// </summary>
        /// <returns></returns>
        public string FetchIdentityName()
        {
            if (this.PrimaryKeys == null || this.PrimaryKeys.Count != 1) return null;

            Regex regex = new Regex(@"IDENTITY", RegexOptions.IgnoreCase); //主键映射特性
            var pi = this.GetType().GetProperty(PrimaryKeys[0]);
            if (pi == null) return null;

            ColumnAttribute attribute = this.FetchColumnAttribute(pi);
            if (pi == null) return null;
            if (!regex.IsMatch(attribute.DbType)) return null;
            return PrimaryKeys[0];
        }

        #endregion
    }
}
