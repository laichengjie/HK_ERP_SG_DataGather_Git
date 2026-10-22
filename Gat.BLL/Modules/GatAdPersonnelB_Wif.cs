using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.BLL.Modules
{
    public class GatAdPersonnelB_Wif : LogicBase
    {

        #region 私有|保护属性



        #endregion

        #region 公开属性
        /// <summary>
        ///  
        ///</summary>
        ///
       
        public int Id { get; set; }
        /// <summary>
        ///  方案id
        ///</summary>
        public int SolutionID { get; set; }

        /// <summary>
        ///  店员编号
        ///</summary>
        public string staff_code { get; set; }


        /// <summary>
        ///  姓名
        ///</summary>
        public string name { get; set; }

        /// <summary>
        ///  店铺编号
        ///</summary>
        public string store_code { get; set; }

        /// <summary>
        ///  在职状态
        ///</summary>
        public Boolean active { get; set; }

        /// <summary>
        ///  是否导购
        ///</summary>
        public Boolean saler { get; set; }

        /// <summary>
        ///  入职时间
        ///</summary>
        public DateTime? entry_date { get; set; }


        /// <summary>
        ///  性别
        ///</summary>
        public string sex { get; set; }


        /// <summary>
        ///  年龄
        ///</summary>
        public string age { get; set; }


        /// <summary>
        ///  职位
        ///</summary>
        public string position { get; set; }

        /// <summary>
        ///  店员快照 日期
        ///</summary>
        public DateTime stats_date { get; set; }


        /// <summary>
        ///  公司ID
        ///</summary>
        public string CompanyID { get; set; }

        /// <summary>
        ///  品牌ID
        ///</summary>
        public string CardID { get; set; }


        /// <summary>
        ///  职员ID
        ///</summary>
        public string PersonnelID { get; set; }


        /// <summary>
        ///  修改时间
        ///</summary>
        public DateTime ModifyTime { get; set; }
        #endregion

        #region 构造函数

        /// <summary>
        /// 默认的空构造函数，如果只想使用属性，可以使用此构造 函数
        /// </summary>
        public GatAdPersonnelB_Wif() : base() { }

        /// <summary>
        /// 指定LINQ模型来初始化类 
        /// </summary>
        public GatAdPersonnelB_Wif(Gat.DAL.ModuleBase moduleBase) : base(moduleBase) { }
        
        /// <summary>
        /// 使用数据服务类来使用化
        /// </summary>
        /// <param name="moduleBase"></param>
        /// <param name="owner">实体隶属于哪个数据仓库</param>
        public GatAdPersonnelB_Wif(Gat.DAL.ModuleBase moduleBase, GatRepository owner) : base(moduleBase, owner) { }

        #endregion 

        #region 私有|保护方法



        #endregion

        #region 重写方法



        #endregion

        #region 公开方法



        #endregion

    }
}
