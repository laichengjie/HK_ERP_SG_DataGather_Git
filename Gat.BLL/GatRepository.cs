using Gat.BLL.Modules;
using Gat.DAL;
using Gat.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;

namespace Gat.BLL
{
    /// <summary>
    /// 数据提报逻辑层统管类
    /// </summary>
    public partial class GatRepository : IDisposable
    {

        #region 公开属性

        /// <summary>
        /// 数据服务
        /// </summary>
        protected GatService _Service = null;

        /// <summary>
        /// 运行环境
        /// </summary>
        public ConnnectType ConnnectType;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认WEB正式库
        /// </summary>
        public GatRepository() : this(ConnnectType.Web) { }

        /// <summary>
        /// 指定的运行环境
        /// </summary>
        public GatRepository(ConnnectType connType)
        {
            if (_Service == null ||
                connType != ConnnectType) _Service = new GatService(connType);
            ConnnectType = connType;
        }

        #endregion

        #region 公用API（CUID）

        ///// <summary>
        ///// 添加一条实体记录
        ///// </summary>
        ///// <typeparam name="T">实体类型</typeparam>
        ///// <param name="TEntity">实体对象</param>
        ///// <param name="submitChanges">true：自动保存；fase：调用SaveChanges保存</param>
        ///// <returns></returns>
        //public virtual HandleArgs InsertEntity<T>(T TEntity, bool submitChanges = true) where T : LogicBase
        //{
        //    HandleArgs handle = this.ValidEntity(TEntity);
        //    if (handle.Status == HandleStatus.Fail) return handle;


        //    ModuleBase moduleBase = Activator.CreateInstance(TEntity.ModuleType) as ModuleBase;
        //    moduleBase.CopyPropertyFrom(TEntity);

        //    return _Service.InsertEntity(moduleBase);
        //}

        ///// <summary>
        ///// 此部分主要供PageBase及其子类使用，用以查询单个实体
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="TEntity">主要用来提供主键值</param>
        ///// <returns></returns>
        //private T FetchEntity<T>(T TEntity) where T : class
        //{


        //    Vendor vendor = _Service.FetchEntity(new Vendor { ID = vendorID });
        //    return vendor == null ? null : new VendorB(vendor);

        //    if (TEntity == null || TEntity.ModuleType == null) return null;
        //    ModuleBase moduleBase = Activator.CreateInstance(TEntity.ModuleType) as ModuleBase;
        //    moduleBase.PropertyFrom(TEntity);
        //    moduleBase = _Service.FetchEntity(moduleBase);
        //    if (moduleBase == null) return null;
        //    moduleBase.CopyPropertyTo(TEntity);
        //    return TEntity;
        //}

        /// <summary>
        /// 在进行增删改时对实参进行检验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEntity"></param>
        /// <returns></returns>
        private HandleArgs ValidEntity<T>(T TEntity) where T : LogicBase
        {

            HandleArgs args = new HandleArgs();
            if (TEntity == null)
            {
                args.Message = "实体引用为空！";
                args.Status = HandleStatus.Fail;
                return args;
            }

            //if (TEntity.ModuleType == null)
            //{
            //    args.Message = string.Format("在类型【{0}】上没有找到模型描述信息！", TEntity.GetType().Name);
            //    args.Status = HandleStatus.Fail;
            //    return args;
            //}
            return args;
        }

        /// <summary>
        /// 保存
        /// <remarks>公开此方法便于使用LINQ的隐式事务</remarks>
        /// </summary>
        /// <returns></returns>
        public virtual HandleArgs SaveChanges()
        {
            return _Service.SaveChanges();
        }

        #endregion

        #region 供应商部分

        /// <summary>
        /// 新增一条供应商记录
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs InsertVendor(VendorB vendor)
        {
            HandleArgs handle = this.ValidEntity(vendor);
            if (handle.Status == HandleStatus.Fail) return handle;

            Vendor vendorBase = new Vendor();
            vendor.CopyPropertyTo(vendorBase);
            handle = _Service.InsertEntity(vendorBase);
            if (handle.Status == HandleStatus.Completed) vendor.ID = vendorBase.ID;
            return handle;
        }

        /// <summary>
        /// 更新一条供应商记录
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateVendor(VendorB vendor)
        {
            HandleArgs handle = this.ValidEntity(vendor);
            if (handle.Status == HandleStatus.Fail) return handle;

            Vendor vendorBase = new Vendor();
            vendor.CopyPropertyTo(vendorBase);
            return _Service.UpdateEntity(vendorBase);
        }

        /// <summary>
        /// 删除一条供应商记录，将删除：1.店铺；2.品牌；3.提报方案；4.提报公司；5.店铺关联；6.权限设置
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs DeleteVendor(VendorB vendor)
        {
            //HandleArgs handle = this.ValidEntity(vendor);
            //Vendor vendorBase = new Vendor();
            //vendorBase.ID = vendor.ID;
            //return _Service.DeleteEntity(vendorBase);

            var command = @"
                            Delete From Bas_ShopMap Where VendShopID In ( Select ID From Bas_VendShop Where IsNull(VendorID,-1) = {0} ); /*删除店铺映射*/
                            Delete From Bas_VendShop Where VendorID={0}; /*删除店铺*/
                            Delete From Bas_VendorCard Where VendorID={0}; /*删除品牌*/
                            Delete From Gat_SolCompany Where SolutionID = (Select Top 1 ID From Gat_Solution Where IsNull(VendorID,-1) = {0});/*删除提报方案公司*/
                            Delete From Gat_Solution Where ID = (Select Top 1 ID From Gat_Solution Where IsNull(VendorID,-1) = {0});/*删除提报方案*/
                            Delete From Gat_ShopRight Where IsNull(VendorID,-1) = {0};/*删除提报店铺*/
                            Delete From Bas_Vendor Where ID={0};/*删除主表*/
            
                            ";
            command = string.Format(command, vendor.ID);
            return _Service.ExecuteCommand(command);
        }

        /// <summary>
        /// 查询单个供应商资料
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public VendorB FetchVendor(int vendorID)
        {
            Vendor vendor = _Service.FetchEntity(new Vendor { ID = vendorID });
            return vendor == null ? null : new VendorB(vendor);
        }

        /// <summary>
        /// 根据客户代码查找客户
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public VendorB FetchVendor(string code)
        {
            Expression<Func<Vendor, bool>> predicate = ExpHelper.True<Vendor>();
            var vendor = _Service
                        .QueryEntity(predicate)
                        .FirstOrDefault(u => u.VendorCode == code);
            return vendor != null ? new VendorB(vendor) : null;
        }

        /// <summary>
        /// 取所有供应商信息
        /// </summary>
        /// <returns></returns>
        public List<VendorB> QueryVendors()
        {
            var predicate = ExpHelper.True<Vendor>();
            return _Service
                    .QueryEntity(predicate)
                    .Select(v => new VendorB(v))
                    .ToList();
        }

        public List<VendorB> QueryAllowUsedVendors()
        {
            var predicate = ExpHelper.True<Vendor>();
            return _Service
                    .QueryEntity(predicate)
                    .Where(a => a.AllowUsed)
                    .Select(v => new VendorB(v))
                    .ToList();
        }

        /// <summary>
        /// 根据代码模糊查找供应商记录
        /// </summary>
        /// <returns></returns>
        public List<VendorB> QueryVendorsByCode(string vendorCode)
        {
            var predicate = ExpHelper.True<Vendor>();
            if (!string.IsNullOrEmpty(vendorCode))
            {
                predicate = predicate.AndAlso(v => v.VendorCode.Contains(vendorCode));
            }
            //.AndAlso(v => v.VendorCode.Contains(vendorCode));
            return _Service
                    .QueryEntity(predicate)
                    .Select(v => new VendorB(v))
                    .ToList();
        }

        /// <summary>
        /// 供应商记录计数
        /// </summary>
        /// <returns></returns>
        public int VendorCount()
        {
            var predicate = ExpHelper.True<Vendor>().AndAlso(v => v.AllowUsed); //可用的供应商
            return _Service.CountEntity(predicate);
        }

        /// <summary>
        /// 根据供应商ID取供应商品牌
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<VendorCardB> QueryVendorCards(int vendorID)
        {
            var cardPredicate = ExpHelper.True<Card>();
            var vcPredicate = ExpHelper.True<VendorCard>();
            var query = from a in _Service.QueryEntity(vcPredicate)
                        join b in _Service.QueryEntity(cardPredicate) on a.CardID equals b.CardID into vcc
                        from c in vcc.DefaultIfEmpty()
                        where a.VendorID == vendorID
                        select new VendorCardB
                        {
                            VendorID = a.VendorID,
                            VendorName = null,
                            VendorCode = null,
                            CardCode = c.CardCode,
                            CardName = c.CardName,
                            CardID = a.CardID,
                            AllowUsed = a.AllowUsed,
                            CreateDate = a.CreateDate,
                            ModifyDTM = a.ModifyDTM
                        };

            return query.ToList();
        }

        /// <summary>
        /// 根据供应商ID取供应商店铺
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<VendorShopB> QueryVendorShops(int vendorID, string shopCode = "")
        {
            var vcPredicate = ExpHelper.True<VendShop>();
            var query = from a in _Service.QueryEntity(vcPredicate)
                        where a.VendorID == vendorID && (string.IsNullOrEmpty(shopCode) ? true : a.ShopCode.Contains(shopCode))
                        select new VendorShopB(a, this);

            return query.ToList();
        }
        /// <summary>
        /// 根据供应商ID取供应商仓库
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<VendorShopB> QueryVendorStocks(int vendorID, string stockCode = "")
        {
            var vcPredicate = ExpHelper.True<VendShop>();
            var query = from a in _Service.QueryEntity(vcPredicate)
                        where a.VendorID == vendorID && (string.IsNullOrEmpty(stockCode) ? true : a.StockCode.Contains(stockCode))
                        select new VendorShopB(a, this);

            return query.ToList();
        }

        /// <summary>
        /// 根据公司ID取店铺资料
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<ShopB> QueryBasShops(string companyID, string shopCode = "")
        {
            var vcPredicate = ExpHelper.True<Shop>();
            var query = from a in _Service.QueryEntity(vcPredicate)
                        where a.CompanyID == companyID && (string.IsNullOrEmpty(shopCode) ? true : a.ShopCode.Contains(shopCode))
                        select new ShopB(a, this);
            return query.ToList();
        }


        /// <summary>
        /// 更新供应商品牌
        /// </summary>
        /// <param name="vendorID">供应商</param>
        /// <param name="cardList">添加的品牌ID</param>
        /// <returns></returns>
        public HandleArgs UpdateVendorCards(int vendorID, List<string> cardList, List<string> scopeList)
        {
            if (scopeList == null || scopeList.Count == 0)
                return new HandleArgs { Message = "没有任何数据写到数据库", Status = HandleStatus.Completed };

            cardList = cardList == null ? new List<string> { } : cardList;
            string scropeIDList = string.Empty;
            foreach (string cardID in scopeList)
            {
                scropeIDList += ",'" + cardID + "'";
            }
            scropeIDList = string.IsNullOrEmpty(scropeIDList) ? scropeIDList : scropeIDList.Substring(1);

            string command = @" Delete From Bas_VendorCard Where VendorID={0} And CardID In({1})";
            command = string.Format(command, vendorID, scropeIDList);

            //删除原来所有的品牌记录
            //var predicate = ExpHelper.True<VendorCard>()
            //                         .AndAlso(vc => vc.VendorID == vendorID);
            HandleArgs handle = _Service.ExecuteCommand(command, string.Empty);
            if (handle.Status == HandleStatus.Fail) return handle;

            foreach (string cardID in cardList)
            {
                if (string.IsNullOrEmpty(cardID)) continue;
                VendorCard vc = new VendorCard
                {
                    VendorID = vendorID,
                    CardID = cardID,
                    AllowUsed = true,
                    CreateDate = DateTime.Now.Date,
                    ModifyDTM = DateTime.Now
                };
                _Service.InsertEntity(vc, false);
            }

            return _Service.SaveChanges();
        }

        /// <summary>
        /// 取新增供应商，8条记录
        /// </summary>
        /// <returns></returns>
        public List<VendorB> QueryTopNewVendors()
        {
            var predicate = ExpHelper.True<Vendor>();
            return _Service
                    .QueryEntity(predicate)
                    .OrderByDescending(v => v.CreateDate)
                    .Skip(0)
                    .Take(8)
                    .Select(v => new VendorB(v, this))
                    .ToList();
        }

        /// <summary> 
        /// 更新-通过ID更新供应商店铺数量
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs UpdateShopNumber(string id, string ShopNumber)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(id))
            {
                return HANDLE;
            }
            string command = $" UPDATE Bas_Vendor set ShopNumber={ShopNumber} WHERE id={id}";
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }
        /// <summary> 
        /// 供应商店铺数量计算
        /// </summary>
        /// <returns></returns>
        public int VendorShopNumberCount(int vendorID,string cardID, int solutionID)
        {
            var QSolution = ExpHelper.True<Solution>().AndAlso(v => v.Enabled == true);
            if (solutionID>0)
            {
                QSolution = ExpHelper.True<Solution>().AndAlso(v => v.ID == solutionID);
            }
            var predicate = ExpHelper.True<ShopRight>().AndAlso(v => v.AllowUsed==true && v.VendorID == vendorID);
            if (!string.IsNullOrWhiteSpace(cardID) && solutionID == 0)
            {
                predicate = predicate.AndAlso(v => v.CardID != cardID);
            }
            if (!string.IsNullOrWhiteSpace(cardID) && solutionID > 0)
            {
                predicate = predicate.AndAlso(v => v.CardID == cardID);
            }
            var query = from a in _Service.QueryEntity(predicate)
                        join b in _Service.QueryEntity(QSolution) on new { a.VendorID,a.CardID } equals new {b.VendorID,b.CardID}
                        select new { ShopID = a.ShopID };
            int ShopNumber = query.Distinct().Count(); 
            return ShopNumber;
        }
        /// <summary> 
        /// 供应商店铺数量计算(方案启用时计算)
        /// </summary> 
        /// <returns></returns>
        public int SolutionShopNumberCount(int vendorID, string cardID,int solutionID)
        {
            var QSolution = ExpHelper.True<Solution>().AndAlso(v => (v.Enabled == true && v.ID!= solutionID) || v.ID == solutionID);
            var predicate = ExpHelper.True<ShopRight>().AndAlso(v => v.AllowUsed == true && v.VendorID == vendorID);
            if (!string.IsNullOrWhiteSpace(cardID))
            {
                predicate = predicate.AndAlso(v => v.CardID != cardID);
            }
            var query = from a in _Service.QueryEntity(predicate)
                        join b in _Service.QueryEntity(QSolution) on new { a.VendorID, a.CardID } equals new { b.VendorID, b.CardID }
                        select new { ShopID = a.ShopID };
            int ShopNumber = query.Distinct().Count();
            return ShopNumber;
        }


        #endregion  

        #region 用户部分

        /// <summary>
        /// 查询所有的用户
        /// </summary>
        /// <returns></returns>
        public List<UserB> QueryUsers()
        {
            Expression<Func<User, bool>> predicate = ExpHelper.True<User>();
            return _Service
                   .QueryEntity(predicate)
                   .Select(u => new UserB(u))
                   .ToList();
        }

        /// <summary>
        /// 根据用户代码查找用户
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public UserB FetchUser(string code)
        {
            Expression<Func<User, bool>> predicate = ExpHelper.True<User>();
            var user = _Service
                        .QueryEntity(predicate)
                        .FirstOrDefault(u => u.UserCode == code);
            return user != null ? new UserB(user) : null;
        }

        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public HandleArgs UpdateUser(UserB user)
        {
            HandleArgs handle = this.ValidEntity(user);
            if (handle.Status == HandleStatus.Fail) return handle;

            User userBase = new User();
            user.CopyPropertyTo(userBase);
            return _Service.UpdateEntity(userBase);
        }

        #endregion

        #region 品牌部分

        /// <summary>
        /// 取所有的品牌记录
        /// </summary>
        /// <returns></returns>
        public List<CardB> QueryCards()
        {
            return _Service
                   .QueryEntity(ExpHelper.True<Card>())
                   .Select(c => new CardB(c))
                   .ToList();
        }

        /// <summary>
        /// 根据品牌代码取得品牌
        /// </summary>
        /// <returns></returns>
        public List<CardB> QueryCards(string cardCode)
        {
            var predicate = ExpHelper.True<Card>();
            if (!string.IsNullOrEmpty(cardCode)) predicate = predicate.AndAlso(c => c.CardCode.Contains(cardCode));

            return _Service
                   .QueryEntity(predicate)
                   .Select(c => new CardB(c))
                   .ToList();
        }

        #endregion

        #region 供应商品牌部分

        /// <summary>
        /// 查询单个供应商的店铺资料
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public VendorShopB FetchVendorShop(int shopID)
        {
            VendShop vs = _Service.FetchEntity(new VendShop { ID = shopID });
            return vs == null ? null : new VendorShopB(vs);
        }

        /// <summary>
        /// 查询单个供应商的店铺资料
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public VendorShopB FetchVendorShop(string shopCode)
        {
            Expression<Func<VendShop, bool>> predicate = ExpHelper.True<VendShop>();
            var vendor = _Service
                        .QueryEntity(predicate)
                        .FirstOrDefault(u => u.ShopCode == shopCode);
            return vendor != null ? new VendorShopB(vendor) : null;
        }

        /// <summary>
        /// 插入一条供应商店铺
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public HandleArgs InsertVendorShop(VendorShopB vs)
        {
            HandleArgs handle = this.ValidEntity(vs);
            if (handle.Status == HandleStatus.Fail) return handle;

            VendShop vendorBase = new VendShop();
            vs.CopyPropertyTo(vendorBase);
            handle = _Service.InsertEntity(vendorBase);
            if (handle.Status == HandleStatus.Completed) vs.ID = vendorBase.ID;
            return handle;
        }


        /// <summary>
        /// 更新一条供应商店铺资料
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public HandleArgs UpdateVendorShop(VendorShopB vs)
        {
            HandleArgs handle = this.ValidEntity(vs);
            if (handle.Status == HandleStatus.Fail) return handle;

            VendShop vendorBase = new VendShop();
            vs.CopyPropertyTo(vendorBase);
            return _Service.UpdateEntity(vendorBase);
        }
        /// <summary>
        /// 删除一条供应商店铺资料
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public HandleArgs DeleteVendorShop(VendorShopB vs)
        {
            HandleArgs handle = this.ValidEntity(vs);
            if (handle.Status == HandleStatus.Fail) return handle;

            //查看店铺是否已经关联到ERP的店铺，如果关联则不能删除
            if (this.QueryMapShop(vs.ID).Count > 0)
            {
                handle.Status = HandleStatus.Fail;
                handle.Message = "店铺已经关联到ERP的店铺，不能删除！";
                return handle;
            }

            VendShop vendorBase = new VendShop();
            vendorBase.ID = vs.ID;
            return _Service.DeleteEntity(vendorBase);
        }

        #endregion

        #region 提报方案部分

        /* ----------------------------------
         * 公司、供应商、品牌唯一确定一条提报方案
         * 
         ----------------------------------*/

        /// <summary>
        /// 查询单条提报方案记录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SolutionB FetchSolution(int solutionID)
        {
            var solPredicate = ExpHelper.True<Solution>();
            var companyPredicate = ExpHelper.True<Company>();
            var cardPredicate = ExpHelper.True<Card>();
            var vendorPredicate = ExpHelper.True<Vendor>();

            var query = from a in _Service.QueryEntity(solPredicate)
                        join b in _Service.QueryEntity(companyPredicate) on a.CompanyID equals b.CompanyID into company
                        join c in _Service.QueryEntity(cardPredicate) on a.CardID equals c.CardID into card
                        join d in _Service.QueryEntity(vendorPredicate) on a.VendorID ?? -1 equals d.ID into vendor
                        from e in company.DefaultIfEmpty()
                        from f in card.DefaultIfEmpty()
                        from g in vendor.DefaultIfEmpty()
                        where a.ID == solutionID
                        select new SolutionB(a)
                        {
                            CardName = f.CardName,
                            VendorName = g.VendorName,
                            CompanyName = e.CompanyName
                        };
            return query.FirstOrDefault();
        }

        ///// <summary>
        ///// 查询单条提报方案记录，限：一个供应商只有一条提报方案
        ///// </summary>
        ///// <param name="code"></param>
        ///// <returns></returns>
        //public SolutionB FetchSolutionByVendorID(int vendorID)
        //{
        //    Expression<Func<Solution, bool>> predicate = ExpHelper.True<Solution>();
        //    var user = _Service
        //                .QueryEntity(predicate)
        //                .FirstOrDefault(u => (u.VendorID ?? 0) == vendorID);
        //    return user != null ? new SolutionB(user, this) : null;
        //}

        /// <summary>
        /// 查询单条提报方案记录，限：一个供应商+公司+品牌只有一条提报方案
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SolutionB FetchSolution(string companyID, int vendorID, string cardID)
        {

            var solPredicate = ExpHelper.True<Solution>();
            var companyPredicate = ExpHelper.True<Company>();
            var cardPredicate = ExpHelper.True<Card>();
            var vendorPredicate = ExpHelper.True<Vendor>();

            var query = from a in _Service.QueryEntity(solPredicate)
                        join b in _Service.QueryEntity(companyPredicate) on a.CompanyID equals b.CompanyID into company
                        join c in _Service.QueryEntity(cardPredicate) on a.CardID equals c.CardID into card
                        join d in _Service.QueryEntity(vendorPredicate) on a.VendorID ?? -1 equals d.ID into vendor
                        from e in company.DefaultIfEmpty()
                        from f in card.DefaultIfEmpty()
                        from g in vendor.DefaultIfEmpty()
                        where ((a.VendorID ?? 0) == vendorID) && a.CompanyID == companyID && a.CardID == cardID
                        select new SolutionB(a)
                        {
                            CardName = f.CardName,
                            VendorName = g.VendorName,
                            CompanyName = e.CompanyName
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 新增一条提报方案记录
        /// </summary>
        /// <param name="solution">解决方案基本信息</param>
        /// <param name="allowList">允许提报的店铺列表</param>
        /// <param name="shopCode">店铺代码</param>
        /// <returns></returns>
        public HandleArgs InsertSolution(SolutionB solution, List<ShopRightB> shopRight, List<string> allowList = null, List<string> allowListStock = null, List<string> shopList = null, List<string> stockList = null, string shopCode = null)
        {
            Func<HandleArgs> validate = () => this.ValidEntity(solution);
            Func<HandleArgs> insert = () =>
            {
                Solution solutionBase = new Solution();
                solution.CopyPropertyTo(solutionBase);
                HandleArgs h = _Service.InsertEntity(solutionBase);
                if (h.Status == HandleStatus.Completed) solution.ID = solutionBase.ID;
                return h;
            };
            Func<HandleArgs> command = () => this.SetShosAllowToGat(solution.VendorID ?? 0, solution.CompanyID, solution.CardID, allowList, allowListStock, shopList, stockList, shopRight, shopCode);
            return TransactionExtension.Excute(validate, insert, command);
        }

        /// <summary>
        /// 更新一条提报方案记录
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateSolution(SolutionB solution, List<ShopRightB> shopRight, List<string> allowList = null, List<string> allowListStock = null, List<string> shopList = null, List<string> stockList = null, string shopCode = null)
        {
            Func<HandleArgs> validate = () => this.ValidEntity(solution);
            Func<HandleArgs> update = () =>
            {
                Solution solutionBase = new Solution();
                solution.CopyPropertyTo(solutionBase);
                return _Service.UpdateEntity(solutionBase);
            };
            Func<HandleArgs> command = () => this.SetShosAllowToGat(solution.VendorID ?? 0, solution.CompanyID, solution.CardID, allowList, allowListStock, shopList, stockList, shopRight, shopCode);
            return TransactionExtension.Excute(validate, update, command);
        }

        /// <summary>
        /// 删除一条提报方案记录
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs DeleteSolution(SolutionB solution)
        {
            HandleArgs handle = this.ValidEntity(solution);
            if (handle.Status == HandleStatus.Fail) return handle;

            var command = @"
                            Delete From Gat_SolCard Where SolutionID={0}
                            Delete From Gat_SolCompany Where SolutionID={0}
                            Delete From Gat_Solution Where ID={0}
                            ";
            command = string.Format(command, solution.ID);
            return _Service.ExecuteCommand(command);
        }
        /// <summary>
        /// 设置方案开始和停用
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public HandleArgs UpdateSolutionEnabled(SolutionB solution)
        {
            HandleArgs handle = this.ValidEntity(solution);
            if (handle.Status == HandleStatus.Fail) return handle;
            var command = @"UPDATE Gat_Solution SET Enabled=~[Enabled] WHERE ID={0}";
            command = string.Format(command, solution.ID);
            return _Service.ExecuteCommand(command);
        }

        /// <summary>
        /// 取所有的提报方案记录
        /// </summary>
        /// <param name="title">提报方案标题</param>
        /// <returns></returns>
        public List<SolutionB> QuerySolutions(string title = null)
        {
            var predicate = ExpHelper.True<Solution>();
            var vendorPredicate = ExpHelper.True<Vendor>();
            var cardPredicate = ExpHelper.True<Card>();
            var companyPredicate = ExpHelper.True<Company>();
            if (!string.IsNullOrEmpty(title))
                predicate = predicate.AndAlso(item => item.Title.Contains(title));

            var query = from a in _Service.QueryEntity(predicate)
                        join b in _Service.QueryEntity(vendorPredicate) on (a.VendorID ?? 0) equals b.ID into vendor
                        join c in _Service.QueryEntity(cardPredicate) on a.CardID equals c.CardID into card
                        join d in _Service.QueryEntity(companyPredicate) on a.CompanyID equals d.CompanyID into company
                        from e in vendor.DefaultIfEmpty()
                        from f in card.DefaultIfEmpty()
                        from g in company.DefaultIfEmpty()
                        select new SolutionB(a, this)
                        {
                            VendorName = e.VendorName,
                            CardName = f.CardName,
                            CompanyName = g.CompanyName,

                            Title = "【" + g.CompanyName + "】-【" + e.VendorName + "】-【" + f.CardName + "】的提报方案" //【" + solution.CompanyName + "】
                        };

            return query.ToList();
        }
        /// <summary>
        /// 提取方案数据列表
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<SolutionB> QuerySolutionsForVendor(int vendorID = 0)
        {
            if (vendorID > 0)
            {
                var solutionItem = ExpHelper.True<Solution>();
                var vendorItem = ExpHelper.True<Vendor>();
                var cardItem = ExpHelper.True<Card>();
                var companyItem = ExpHelper.True<Company>();
                solutionItem = solutionItem.AndAlso(item => item.VendorID.Equals(vendorID));

                var query = from a in _Service.QueryEntity(solutionItem)
                            join b in _Service.QueryEntity(companyItem) on a.CompanyID equals b.CompanyID
                            join c in _Service.QueryEntity(cardItem) on a.CardID equals c.CardID
                            join d in _Service.QueryEntity(vendorItem) on a.VendorID equals d.ID
                            select new SolutionB(a, this)
                            {
                                VendorName = d.VendorName,
                                CardName = c.CardName,
                                CompanyName = b.CompanyName,
                                Title = b.CompanyName + "-" + d.VendorName + "-" + c.CardName
                            };
                return query.ToList();
            }
            return null;
        }
        /// <summary>
        /// 提报方案记录计数
        /// </summary>
        /// <returns></returns>
        public int SolutionCount()
        {
            var predicate = ExpHelper.True<Solution>();
            return _Service.CountEntity(predicate);
        }

        /// <summary>
        /// 查询指定公司能够为哪些公司提报
        /// <remarks>依据：公司经营品牌与供应商的品牌一致</remarks>
        /// </summary>
        /// <param name="compnayID"></param>
        /// <returns></returns>
        public List<VendorB> QueryVendorForCompany(string compnayID)
        {
            var comapnyCardPredicate = ExpHelper.True<CompanyCard>();
            var vendorCardPredicate = ExpHelper.True<VendorCard>();
            var vendorPredicate = ExpHelper.True<Vendor>();

            var query = from a in _Service.QueryEntity(comapnyCardPredicate)
                        join b in _Service.QueryEntity(vendorCardPredicate) on a.CardID equals b.CardID
                        join c in _Service.QueryEntity(vendorPredicate) on b.VendorID equals c.ID
                        where a.CompanyID == compnayID && c.AllowUsed
                        select new VendorB
                        {
                            ID = c.ID,
                            VendorName = c.VendorName,
                            VendorCode = c.VendorCode
                        };
            return query.Distinct(/*加DISTINCT是因为有可能公司给一个品牌供应商的多个品牌提报*/).ToList();
        }

        /// <summary>
        /// 查询指定公司能够为品牌公司的哪些品牌提报
        /// <remarks>依据：公司经营品牌与供应商的品牌一致</remarks>
        /// </summary>
        /// <param name="compnayID"></param>
        /// <returns></returns>
        public List<CardB> QueryCardForCompanyAndVendor(string compnayID, int vendorID)
        {
            var comapnyCardPredicate = ExpHelper.True<CompanyCard>();
            var vendorCardPredicate = ExpHelper.True<VendorCard>();
            var vendorPredicate = ExpHelper.True<Vendor>();
            var cardPredicate = ExpHelper.True<Card>();

            var query = from a in _Service.QueryEntity(comapnyCardPredicate)
                        join b in _Service.QueryEntity(vendorCardPredicate) on a.CardID equals b.CardID
                        join c in _Service.QueryEntity(vendorPredicate) on b.VendorID equals c.ID
                        join d in _Service.QueryEntity(cardPredicate) on a.CardID equals d.CardID
                        where a.CompanyID == compnayID && c.AllowUsed && b.VendorID == vendorID
                        select new CardB
                        {
                            CardID = d.CardID,
                            CardCode = d.CardCode,
                            CardName = d.CardName
                        };
            return query.ToList();
        }

        #endregion

        #region 提报方案品牌

        /// <summary>
        /// 更新提报方案品牌
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateSolutionCards(int solutionID, List<string> cardIDList)
        {
            string command = @"
                Delete From Gat_SolCard Where SolutionID={0}
            ";
            command = string.Format(command, solutionID);

            //删除原来所有的品牌记录
            //var predicate = ExpHelper.True<VendorCard>()
            //                         .AndAlso(vc => vc.VendorID == vendorID);
            HandleArgs handle = _Service.ExecuteCommand(command, string.Empty);
            if (handle.Status == HandleStatus.Fail) return handle;

            foreach (string cardID in cardIDList)
            {
                if (string.IsNullOrEmpty(cardID)) continue;
                SolCard vc = new SolCard
                {
                    SolutionID = solutionID,
                    CardID = cardID
                };
                _Service.InsertEntity(vc, false);
            }

            return _Service.SaveChanges();
        }

        /// <summary>
        /// 根据解决方案ID取解决方案品牌
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<SolCardB> QuerySolCards(int solutionID)
        {
            var vcPredicate = ExpHelper.True<SolCard>();
            var cardPredicate = ExpHelper.True<Card>();
            var query = from a in _Service.QueryEntity(vcPredicate)
                        join b in _Service.QueryEntity(cardPredicate) on a.CardID equals b.CardID into card
                        from c in card.DefaultIfEmpty()
                        where a.SolutionID == solutionID
                        select new SolCardB
                        {
                            CardID = a.CardID,
                            SolutionID = a.SolutionID,

                            CardCode = c.CardCode,
                            CardName = c.CardName,

                            Owner = this
                        };

            return query.ToList();
        }

        #endregion

        #region 提报方案公司

        /// <summary>
        /// 更新提报方案公司
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateSolutionCompanies(int solutionID, List<string> companyIDList)
        {
            string command = @"
                Delete From Gat_SolCompany Where SolutionID={0}
            ";
            command = string.Format(command, solutionID);

            HandleArgs handle = _Service.ExecuteCommand(command, string.Empty);
            if (handle.Status == HandleStatus.Fail) return handle;

            foreach (string companyID in companyIDList)
            {
                if (string.IsNullOrEmpty(companyID)) continue;
                SolCompany vc = new SolCompany
                {
                    SolutionID = solutionID,
                    CompanyID = companyID
                };
                _Service.InsertEntity(vc, false);
            }

            return _Service.SaveChanges();
        }

        /// <summary>
        /// 根据解决方案ID取解决方案品牌
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<SolCompanyB> QuerySolCompanies(int solutionID)
        {
            var vcPredicate = ExpHelper.True<SolCompany>();
            var cardPredicate = ExpHelper.True<Company>();
            var query = from a in _Service.QueryEntity(vcPredicate)
                        join b in _Service.QueryEntity(cardPredicate) on a.CompanyID equals b.CompanyID into company
                        from c in company.DefaultIfEmpty()
                        where a.SolutionID == solutionID
                        select new SolCompanyB
                        {
                            CompanyID = a.CompanyID,
                            SolutionID = a.SolutionID,

                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,

                            Owner = this
                        };

            return query.ToList();
        }

        #endregion

        #region 店铺权限

        /// <summary>
        /// 查找所有的仓库权限资料
        /// </summary>
        /// <returns></returns>
        public List<ShopRightB> QueryShopRights()
        {
            Expression<Func<ShopRight, bool>> shopRight = ExpHelper.True<ShopRight>();
            Expression<Func<Shop, bool>> shop = ExpHelper.True<Shop>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();

            var query = from a in _Service.QueryEntity(shopRight)
                        join b in _Service.QueryEntity(shop)
                            on new { ShopID = a.ShopID, CompanyID = a.CompanyID } equals new { ShopID = b.ShopID, CompanyID = b.CompanyID }
                        join c in _Service.QueryEntity(stock) on
                            new { StockID = b.StockID, CompanyID = b.CompanyID } equals new { StockID = c.StockID, CompanyID = c.CompanyID }
                        select new ShopRightB
                        {
                            CompanyID = a.CompanyID,
                            ShopID = a.ShopID,
                            ShopCode = b.ShopCode,
                            ShopName = b.ShopName,

                            VendorID = a.VendorID,
                            StockCode = c.StockCode,
                            StockName = c.StockName,

                            ID = a.ID,
                            CardID = a.CardID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM
                        };

            return query.ToList();
        }

        /// <summary>
        /// 查找指定供应商、品牌和公司的所有的店铺权限资料
        /// </summary>
        /// <returns></returns>
        public List<ShopRightB> QueryShopRights(int vendorID, string cardID, string companyID)
        {
            Expression<Func<ShopRight, bool>> shopRight = ExpHelper.True<ShopRight>();
            Expression<Func<Shop, bool>> shop = ExpHelper.True<Shop>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();

            var query = from a in _Service.QueryEntity(shopRight)
                        join b in _Service.QueryEntity(shop)
                            on new { ShopID = a.ShopID, CompanyID = a.CompanyID } equals new { ShopID = b.ShopID, CompanyID = b.CompanyID }
                        join c in _Service.QueryEntity(stock) on
                            new { StockID = b.StockID, CompanyID = b.CompanyID } equals new { StockID = c.StockID, CompanyID = c.CompanyID }
                        where (a.VendorID ?? 0) == vendorID && a.CardID == cardID && a.CompanyID == companyID
                        select new ShopRightB
                        {
                            CompanyID = a.CompanyID,
                            ShopID = a.ShopID,
                            ShopCode = b.ShopCode,
                            ShopName = b.ShopName,

                            VendorID = a.VendorID,
                            StockCode = c.StockCode,
                            StockName = c.StockName,
                            CardID = a.CardID,

                            ID = a.ID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM
                        };

            return query.ToList();
        }

        /// <summary>
        /// 查找指定供应商、品牌和公司的所有的店铺权限资料
        /// </summary>
        /// <returns></returns>
        public List<ShopRightB> QueryShopRights(int vendorID, string cardID, string companyID, string shopCode)
        {
            Expression<Func<ShopRight, bool>> shopRight = ExpHelper.True<ShopRight>();
            Expression<Func<Shop, bool>> shop = ExpHelper.True<Shop>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();
            Expression<Func<ShopMap, bool>> shopMap = ExpHelper.True<ShopMap>();
            Expression<Func<VendShop, bool>> vendorShop = ExpHelper.True<VendShop>();

            var query = from a in _Service.QueryEntity(shopRight)
                        join b in _Service.QueryEntity(shop)
                            on new { ShopID = a.ShopID, CompanyID = a.CompanyID } equals new { ShopID = b.ShopID, CompanyID = b.CompanyID }
                        join c in _Service.QueryEntity(stock) on
                            new { StockID = b.StockID, CompanyID = b.CompanyID } equals new { StockID = c.StockID, CompanyID = c.CompanyID }
                        join d in _Service.QueryEntity(shopMap) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = d.CompanyID, ShopID = d.ShopID, VendorID = d.VendorID ?? -1 } into map
                        from m in map.DefaultIfEmpty()
                        join e in _Service.QueryEntity(vendorShop) on m.VendShopID equals e.ID into vendshop
                        from vs in vendshop.DefaultIfEmpty()
                        where (a.VendorID ?? 0) == vendorID && a.CardID == cardID && a.CompanyID == companyID && (string.IsNullOrEmpty(shopCode) ? true : b.ShopCode.Contains(shopCode))
                        select new ShopRightB
                        {
                            CompanyID = a.CompanyID,
                            ShopID = a.ShopID,
                            ShopCode = b.ShopCode,
                            ShopName = b.ShopName,

                            VendorID = a.VendorID,
                            StockCode = c.StockCode,
                            StockName = c.StockName,

                            CardID = a.CardID,

                            ID = a.ID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM,
                            VendorShopID = vs != null ? vs.ID : 0,
                            VendorShopCode = vs.ShopCode,
                            VendorShopName = vs.ShopName,

                            ShopMapID = m != null ? m.ID : 0,
                            DefaultDiscount = a.DefaultDiscount,
                            Discount = a.Discount,
                            TradeCode = a.TradeCode
                        };

            return query.ToList();
        }

        /// <summary>
        /// 查找指定供应商、品牌和公司的所有的店铺权限资料
        /// </summary>
        /// <returns></returns>
        public List<ShopRightB> QueryShopRights(int vendorID, string cardID, string companyID, bool allowUsed)
        {
            Expression<Func<ShopRight, bool>> shopRight = ExpHelper.True<ShopRight>();
            Expression<Func<Shop, bool>> shop = ExpHelper.True<Shop>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();
            Expression<Func<ShopMap, bool>> shopMap = ExpHelper.True<ShopMap>();
            Expression<Func<VendShop, bool>> vendorShop = ExpHelper.True<VendShop>();

            var query = from a in _Service.QueryEntity(shopRight)
                        join b in _Service.QueryEntity(shop)
                            on new { ShopID = a.ShopID, CompanyID = a.CompanyID } equals new { ShopID = b.ShopID, CompanyID = b.CompanyID }
                        join c in _Service.QueryEntity(stock) on
                            new { StockID = b.StockID, CompanyID = b.CompanyID } equals new { StockID = c.StockID, CompanyID = c.CompanyID }
                        join d in _Service.QueryEntity(shopMap) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = d.CompanyID, ShopID = d.ShopID, VendorID = d.VendorID ?? -1 } into map
                        from m in map.DefaultIfEmpty()
                        join e in _Service.QueryEntity(vendorShop) on m.VendShopID equals e.ID into vendshop
                        from vs in vendshop.DefaultIfEmpty()
                        where (a.VendorID ?? 0) == vendorID && a.CardID == cardID && a.CompanyID == companyID && a.AllowUsed == allowUsed
                        select new ShopRightB
                        {
                            CompanyID = a.CompanyID,
                            ShopID = a.ShopID,
                            ShopCode = b.ShopCode,
                            ShopName = b.ShopName,

                            VendorID = a.VendorID,
                            StockCode = c.StockCode,
                            StockName = c.StockName,

                            CardID = a.CardID,

                            ID = a.ID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM,
                            VendorShopID = vs != null ? vs.ID : 0,
                            VendorShopCode = vs.ShopCode,
                            VendorShopName = vs.ShopName,

                            ShopMapID = m != null ? m.ID : 0
                        };

            return query.ToList();
        }

        /// <summary>
        /// 设置店铺为允许采集
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="allowList">允许采集的店铺ID</param>
        /// <param name="scopeList">作用范围</param>
        /// <returns></returns>
        public HandleArgs SetShosAllowToGat(int vendorID, string companyID, string cardID, List<string> allowList, List<string> allowListStock, List<string> shopAllList, List<string> stockAllList, List<ShopRightB> shopRight, string shopCode = "")
        {
            string allowIDList = string.Empty;
            string command = string.Empty;
            allowIDList = allowList == null || allowList.Count == 0 ? ",''" : allowList.Join();
            /*
             店铺资料，现在增加大仓
             */
            string sqlShop = @"IF OBJECT_ID('tempdb..#ShopIDList', 'U') IS NOT NULL 
                                DROP TABLE #ShopIDList
                                Create TABLE #ShopIDList (ShopID NVARCHAR(max))   
                             IF OBJECT_ID('tempdb..#StockIDList', 'U') IS NOT NULL 
                                DROP TABLE #StockIDList
                                Create TABLE #StockIDList (StockID NVARCHAR(max))" + Environment.NewLine;
            foreach (string item in shopAllList)//减少店铺范围
            {
                sqlShop += string.Format("INSERT  INTO #ShopIDList (ShopID) VALUES ('{0}')", item) + Environment.NewLine;
            }
            foreach (string item in stockAllList)//仓库
            {
                sqlShop += string.Format("INSERT  INTO #StockIDList (StockID) VALUES ('{0}')", item) + Environment.NewLine;
            }
            //command = sqlShop + Environment.NewLine;
            //command += @"Update Gat_ShopRight Set AllowUsed = 1,ModifyDTM=GETDATE()
            //            From Gat_ShopRight a
            //            Join Bas_Shop b On a.CompanyID = b.CompanyID And a.ShopID = b.ShopID 
            //            inner join #ShopIDList  c on a.ShopID=c.ShopID
            //            Where a.CompanyID={0} And a.VendorID={1} And a.CardID={2} And a.ShopID In(" + allowIDList + ") ";
            //command += string.IsNullOrEmpty(shopCode) ? ";" : "And b.ShopCode Like {3};" + Environment.NewLine;


            //command += @"Update Gat_ShopRight Set AllowUsed = 0,ModifyDTM=GETDATE()
            //            From Gat_ShopRight a
            //            Join Bas_Shop b On a.CompanyID = b.CompanyID And a.ShopID = b.ShopID 
            //           inner join #ShopIDList  c on a.ShopID=c.ShopID
            //            Where a.CompanyID={0} And a.VendorID={1} And a.CardID={2} And a.ShopID Not In(" + allowIDList + ") ";
            //command += string.IsNullOrEmpty(shopCode) ? ";" : "And b.ShopCode Like {3};" + Environment.NewLine;
            command = sqlShop + Environment.NewLine;
            foreach (var item in shopRight)
            {
                command += $@"Update Gat_ShopRight Set AllowUsed ={(item.AllowUsed ? 1 : 0)},DefaultDiscount={(item.DefaultDiscount ? 1 : 0)},Discount={item.Discount},ModifyDTM=GETDATE(),TradeCode='{item.TradeCode}'
                        From Gat_ShopRight a
                        Join Bas_Shop b On a.CompanyID = b.CompanyID And a.ShopID = b.ShopID 
                        Where a.CompanyID='{companyID}' And a.VendorID='{vendorID}' And a.CardID='{cardID}' and a.ShopID='{item.ShopID}';" + Environment.NewLine;
            }

            string allowStockIDList = string.Empty;
            allowStockIDList = allowListStock == null || allowListStock.Count == 0 || string.IsNullOrEmpty(allowListStock[0].ToString().Trim()) ? "''" : allowListStock.Join();
            command += @"update  a set AllowUsed=1 from Gat_StockRight a
                    JOIN  Bas_Stock b ON a.CompanyID=b.CompanyID AND a.StockID=b.StockID
                     inner join #StockIDList  c on a.StockID=c.StockID
                    WHERE  a.CompanyID={0}   AND a.VendorID={1}  AND a.StockID in (" + allowStockIDList + ")" + Environment.NewLine;

            command += @"update  a set AllowUsed=0 from Gat_StockRight a
                            JOIN  Bas_Stock b ON a.CompanyID=b.CompanyID AND a.StockID=b.StockID 
                            inner join #StockIDList  c on a.StockID=c.StockID
                            WHERE  a.CompanyID={0}   AND a.VendorID={1}  AND a.StockID not in (" + allowStockIDList + ")" + Environment.NewLine;

            HandleArgs HANDLE = _Service.ExecuteCommand(command, companyID, vendorID, cardID, "%" + shopCode + "%");
            return HANDLE;
        }

        /// <summary>
        /// 取还没有关联的店铺，允许采集数据并且未被关联的店铺才会被关联
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public List<ShopB> QueryUnMapShop(int vendorID, string companyID)
        {
            var shopRightPredicate = ExpHelper.True<ShopRight>();
            var shopMapPredicate = ExpHelper.True<ShopMap>();
            var shopPredicate = ExpHelper.True<Shop>();
            var stockPredicate = ExpHelper.True<Stock>();
            var vendorCardPredicate = ExpHelper.True<VendorCard>();

            var query = from a in _Service.QueryEntity(shopRightPredicate)
                        join b in _Service.QueryEntity(shopMapPredicate) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = b.CompanyID, ShopID = b.ShopID, VendorID = b.VendorID ?? 0 } into map
                        join c in _Service.QueryEntity(shopPredicate) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID } equals new { CompanyID = c.CompanyID, ShopID = c.ShopID }
                        join d in _Service.QueryEntity(stockPredicate) on
                            new { CompanyID = c.CompanyID, StockID = c.StockID } equals new { CompanyID = d.CompanyID, StockID = d.StockID }
                        join e in _Service.QueryEntity(vendorCardPredicate) on
                            new { VendorID = a.VendorID ?? 0, CardID = a.CardID } equals new { VendorID = e.VendorID, CardID = e.CardID }

                        from m in map.DefaultIfEmpty()
                        where a.CompanyID == companyID && a.VendorID == vendorID && a.AllowUsed && m.CompanyID == null//(m.CompanyID == null || m.CompanyID == "")
                        select new ShopB
                        {
                            CompanyID = a.CompanyID,
                            //VendorID = a.VendorID,
                            ShopID = a.ShopID,
                            ShopCode = c.ShopCode,
                            ShopName = c.ShopName,
                            StockID = d.StockID,
                            StockCode = d.StockCode,
                            StockName = d.StockName/*,
                            CardID = a.CardID*/
                        };



            return query.Distinct().ToList();
        }

        /// <summary>
        /// 取已经关联的店铺
        /// </summary>
        /// <param name="vendorShopID"></param>
        /// <returns></returns>
        public List<ShopB> QueryMapShop(int vendorShopID)
        {
            var shopMapPredicate = ExpHelper.True<ShopMap>();
            var shopPredicate = ExpHelper.True<Shop>();
            var stockPredicate = ExpHelper.True<Stock>();

            var query = from a in _Service.QueryEntity(shopMapPredicate)
                        join b in _Service.QueryEntity(shopPredicate) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID } equals new { CompanyID = b.CompanyID, ShopID = b.ShopID }
                        join c in _Service.QueryEntity(stockPredicate) on
                            new { CompanyID = b.CompanyID, StockID = b.StockID } equals new { CompanyID = c.CompanyID, StockID = c.StockID }

                        where a.VendShopID == vendorShopID
                        select new ShopB
                        {
                            CompanyID = a.CompanyID,
                            //VendorID = a.VendorID,
                            ShopID = a.ShopID,
                            ShopCode = b.ShopCode,
                            ShopName = b.ShopName,
                            StockID = c.StockID,
                            StockCode = c.StockCode,
                            StockName = c.StockName/*,
                            CardID = a.CardID*/
                        };

            return query.Distinct().ToList();
        }

        /// <summary>
        /// 取能用来关联的店铺
        /// </summary>
        /// <param name="vendorID">供应商</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="vendorShopID">供应商店铺ID</param>
        /// <param name="shopCode">ERP店铺代码</param>
        /// <returns></returns>
        public List<ShopB> QueryShopsForMap(int vendorID, string companyID, int vendorShopID, string shopCode = "")
        {
            var shopRightPredicate = ExpHelper.True<ShopRight>();
            var shopMapPredicate = ExpHelper.True<ShopMap>();
            var shopPredicate = ExpHelper.True<Shop>();
            var stockPredicate = ExpHelper.True<Stock>();
            var vendorCardPredicate = ExpHelper.True<VendorCard>();

            //未关联
            var unMapQuery = from a in _Service.QueryEntity(shopRightPredicate)
                             join b in _Service.QueryEntity(shopMapPredicate) on
                                 new { CompanyID = a.CompanyID, ShopID = a.ShopID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = b.CompanyID, ShopID = b.ShopID, VendorID = b.VendorID ?? 0 } into map
                             join c in _Service.QueryEntity(shopPredicate) on
                                 new { CompanyID = a.CompanyID, ShopID = a.ShopID } equals new { CompanyID = c.CompanyID, ShopID = c.ShopID }
                             join d in _Service.QueryEntity(stockPredicate) on
                                 new { CompanyID = c.CompanyID, StockID = c.StockID } equals new { CompanyID = d.CompanyID, StockID = d.StockID }
                             join e in _Service.QueryEntity(vendorCardPredicate) on
                                 new { VendorID = a.VendorID ?? 0, CardID = a.CardID } equals new { VendorID = e.VendorID, CardID = e.CardID }

                             from m in map.DefaultIfEmpty()
                             where a.CompanyID == companyID && a.VendorID == vendorID && a.AllowUsed && m.CompanyID == null//--(m.CompanyID == null || m.CompanyID == "")
                             select new ShopB
                             {
                                 CompanyID = a.CompanyID,
                                 //VendorID = a.VendorID,
                                 ShopID = a.ShopID,
                                 ShopCode = c.ShopCode,
                                 ShopName = c.ShopName,
                                 StockID = d.StockID,
                                 StockCode = d.StockCode,
                                 StockName = d.StockName,
                                 IsMapped = false
                                 /*,
                            CardID = a.CardID*/
                             };

            //已经关联
            var mapQuery = from a in _Service.QueryEntity(shopMapPredicate)
                           join b in _Service.QueryEntity(shopPredicate) on
                               new { CompanyID = a.CompanyID, ShopID = a.ShopID } equals new { CompanyID = b.CompanyID, ShopID = b.ShopID }
                           join c in _Service.QueryEntity(stockPredicate) on
                               new { CompanyID = b.CompanyID, StockID = b.StockID } equals new { CompanyID = c.CompanyID, StockID = c.StockID }

                           where a.VendShopID == vendorShopID
                           select new ShopB
                           {
                               CompanyID = a.CompanyID,
                               //VendorID = a.VendorID,
                               ShopID = a.ShopID,
                               ShopCode = b.ShopCode,
                               ShopName = b.ShopName,
                               StockID = c.StockID,
                               StockCode = c.StockCode,
                               StockName = c.StockName,
                               IsMapped = true/*,
                            CardID = a.CardID*/
                           };

            var query = unMapQuery.Union(mapQuery).Distinct().Where(m => string.IsNullOrEmpty(shopCode) ? true : m.ShopCode.Contains(shopCode));
            return query.ToList();
        }

        /// <summary>
        /// 关联店铺
        /// </summary>
        /// <param name="vendorShopID"></param>
        /// <param name="companyID"></param>
        /// <param name="mapList"></param>
        /// <param name="scopeList">删除店铺的作用范围，搜索时会搜索出符合条件的记录，因为设置只对此范围起起用</param>
        /// <returns></returns>
        public HandleArgs MapShops(int vendorID, int vendorShopID, string companyID, List<string> mapList, List<string> scopeList)
        {
            if (scopeList == null || scopeList.Count == 0)
                return new HandleArgs { Message = "没有任何数据写到数据库", Status = HandleStatus.Completed };

            mapList = mapList == null ? new List<string> { } : mapList;
            string scropeIDList = string.Empty;
            foreach (string shopID in scopeList)
            {
                scropeIDList += ",'" + shopID + "'";
            }
            scropeIDList = string.IsNullOrEmpty(scropeIDList) ? scropeIDList : scropeIDList.Substring(1);

            string command = @" Delete From Bas_ShopMap Where VendShopID={0} And ShopID In({1}) And CompanyID = '{2}' ";
            command = string.Format(command, vendorShopID, scropeIDList, companyID);

            HandleArgs handle = _Service.ExecuteCommand(command, string.Empty);
            if (handle.Status == HandleStatus.Fail) return handle;

            foreach (string shopID in mapList)
            {
                if (string.IsNullOrEmpty(shopID)) continue;
                ShopMap sm = new ShopMap
                {
                    VendShopID = vendorShopID,
                    CompanyID = companyID,
                    ShopID = shopID,
                    VendorID = vendorID
                };
                handle = _Service.InsertEntity(sm, false);
                if (handle.Status == HandleStatus.Fail) return handle;
            }

            return _Service.SaveChanges();
        }

        /// <summary>
        /// 保存店铺关联
        /// </summary>
        /// <param name="vendorID">供应商</param>
        /// <param name="companyID">公司</param>
        /// <param name="shopID">店铺</param>
        /// <param name="oldMapID">之前关联的供应商店铺ID</param>
        /// <param name="newMapID">新关联的供应商店铺ID</param>
        public HandleArgs MapShop(int vendorID, string companyID, string shopID, int oldMapID, string newMapID)
        {
            //string command = @"Delete From Bas_ShopMap Where VendShopID={0} And VendorID={1} And CompanyID='{2}' And ShopID='{3}';";
            //command += "Insert Into Bas_ShopMap(VendorShopID) Values()";
            //command = string.Format(command,oldMapID, vendorID, companyID, shopID);

            //首先删除原来的关联
            var predicate =
                ExpHelper
                .True<ShopMap>()
                .AndAlso(m => m.VendShopID == oldMapID)
                .AndAlso(m => (m.VendorID ?? 0) == vendorID)
                .AndAlso(m => m.CompanyID == companyID)
                .AndAlso(m => m.ShopID == shopID);

            ShopMap sm = _Service.QueryEntity(predicate).FirstOrDefault();
            if (sm != null) _Service.DeleteEntity(sm, false);

            if (!string.IsNullOrEmpty(newMapID))
            {
                ShopMap shopMap = new ShopMap
                {
                    VendShopID = Convert.ToInt32(newMapID),
                    CompanyID = companyID,
                    VendorID = vendorID,
                    ShopID = shopID
                };
                _Service.InsertEntity(shopMap, false);
            }

            return _Service.SaveChanges();
        }



        /// <summary>
        /// 查询电商店铺关联
        /// </summary>
        /// <param name="shopRightID">主表ID</param>
        /// <param name="companyID">公司</param>
        /// <param name="shopID">店铺</param>
        public List<ShopRightDSB> QueryShopRightDS(int[] shopRightID, string companyID, string[] shopID)
        {
            var shopRightDSPredicate = ExpHelper.True<ShopRightDS>();
            var shopPredicate = ExpHelper.True<Shop>();

            var mapQuery = from a in _Service.QueryEntity(shopRightDSPredicate)
                           join b in _Service.QueryEntity(shopPredicate) on
                               new { CompanyID = a.CompanyID, ShopID = a.DSShopID } equals new { CompanyID = b.CompanyID, ShopID = b.ShopID }
                           where shopRightID.Contains(a.ShopRightID) && a.CompanyID == companyID && shopID.Contains(a.ShopID)
                           select new ShopRightDSB
                           {
                               ID = a.ID,
                               ShopRightID = a.ShopRightID,
                               CompanyID = a.CompanyID,
                               ShopID = a.ShopID,
                               DSShopID = a.DSShopID,
                               DSShopCode = b.ShopCode,
                               DSShopName = b.ShopName
                           };
            return mapQuery.ToList();
        }

        /// <summary>
        /// 保存电商店铺关联
        /// </summary>
        /// <param name="shopRightID">主表ID</param>
        /// <param name="companyID">公司</param>
        /// <param name="shopID">店铺</param>
        /// <param name="oldMapID">之前关联的供应商店铺ID</param>
        /// <param name="newMapID">新关联的供应商店铺ID</param>
        public HandleArgs SaveShopRightDS(int shopRightID, string companyID, string shopID, string oldMapID, string newMapID)
        {
            var predicate =
            ExpHelper
            .True<ShopRightDS>()
            .AndAlso(m => m.ShopRightID == shopRightID)
            .AndAlso(m => m.CompanyID == companyID)
            .AndAlso(m => m.ShopID == shopID);

            var query = _Service.QueryEntity(predicate);

            var newMapIDs = newMapID.Split(',');

            var dbList = query.ToList();

            var oldMapIDs = dbList.Select(s => s.DSShopID).ToArray();
            //首先删除原来不要的关联
            if (oldMapIDs.Any(a => !newMapIDs.Contains(a)))
            {
                var deletes = oldMapIDs.Where(a => !newMapIDs.Contains(a)).ToArray();
                var deleteShopRightDS = dbList.Where(w => deletes.Contains(w.DSShopID)).ToList();
                foreach (var item in deleteShopRightDS)
                {
                    _Service.DeleteEntity(item, false);
                }
            }

            //增加新的关联
            foreach (var item in newMapIDs)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                if(dbList.Any(a => a.DSShopID == item))
                {
                    continue;
                }
                ShopRightDS shopMap = new ShopRightDS
                {
                    ShopRightID = shopRightID,
                    CompanyID = companyID,
                    ShopID = shopID,
                    DSShopID = item
                };
                _Service.InsertEntity(shopMap, false);
            }

            return _Service.SaveChanges();
        }


        #endregion

        #region 大仓
        /// <summary>
        /// 查找所有的大仓权限资料
        /// </summary>
        /// <returns></returns>
        public List<StockRightB> QueryStockRights()
        {
            Expression<Func<StockRight, bool>> stockRight = ExpHelper.True<StockRight>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();

            var query = from a in _Service.QueryEntity(stockRight)
                        join b in _Service.QueryEntity(stock) on
                         new { StockID = a.StockID, CompanyID = a.CompanyID } equals new { StockID = b.StockID, CompanyID = b.CompanyID }
                        select new StockRightB
                        {
                            CompanyID = a.CompanyID,
                            StockID = a.StockID,
                            StockCode = b.StockCode,
                            StockName = b.StockName,
                            ID = a.ID,
                            CardID = a.CardID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM
                        };

            return query.ToList();
        }
        /// <summary>
        /// 根据条件拿取对应的仓库数据
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<StockRightB> QueryStockRights(int vendorID, string cardID, string companyID)
        {
            Expression<Func<StockRight, bool>> stockRight = ExpHelper.True<StockRight>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();
            Expression<Func<StockMap, bool>> stockMap = ExpHelper.True<StockMap>();
            Expression<Func<VendShop, bool>> vendorShop = ExpHelper.True<VendShop>();

            var query = from a in _Service.QueryEntity(stockRight)
                        join b in _Service.QueryEntity(stock) on
                         new { StockID = a.StockID, CompanyID = a.CompanyID } equals new { StockID = b.StockID, CompanyID = b.CompanyID }
                        where (a.VendorID ?? 0) == vendorID && a.CompanyID == companyID
                        join d in _Service.QueryEntity(stockMap) on
                             new { CompanyID = a.CompanyID, ShopID = a.StockID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = d.CompanyID, ShopID = d.StockID, VendorID = d.VendorID ?? -1 }
                        into map
                        from m in map.DefaultIfEmpty()
                        join e in _Service.QueryEntity(vendorShop) on m.VendStockID equals e.ID into vendshop
                        from vs in vendshop.DefaultIfEmpty()
                        where (a.VendorID ?? 0) == vendorID && a.CompanyID == companyID && a.CardID == cardID && b.StockCode != "OOSS" && b.AllowUsed == true && b.IsObject == true
                        select new StockRightB
                        {
                            CompanyID = a.CompanyID,
                            StockID = a.StockID,
                            StockCode = b.StockCode,
                            StockName = b.StockName,
                            VendorID = a.VendorID,
                            ID = a.ID,
                            CardID = a.CardID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM,
                            VendorStockID = vs != null ? vs.ID : 0,
                            VendorStockCode = vs.StockCode,
                            VendorStockName = vs.StockName,
                            StockMapID = m != null ? m.ID : 0
                        };

            return query.ToList();
        }


        /// <summary>
        /// 根据条件拿取对应的仓库数据
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<StockRightB> QueryStockRights(int vendorID, string cardID, string companyID, bool allowUsed)
        {
            Expression<Func<StockRight, bool>> stockRight = ExpHelper.True<StockRight>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();
            Expression<Func<StockMap, bool>> stockMap = ExpHelper.True<StockMap>();
            Expression<Func<VendShop, bool>> vendorShop = ExpHelper.True<VendShop>();

            var query = from a in _Service.QueryEntity(stockRight)
                        join b in _Service.QueryEntity(stock) on
                         new { StockID = a.StockID, CompanyID = a.CompanyID } equals new { StockID = b.StockID, CompanyID = b.CompanyID }
                        where (a.VendorID ?? 0) == vendorID && a.CompanyID == companyID
                        join d in _Service.QueryEntity(stockMap) on
                             new { CompanyID = a.CompanyID, ShopID = a.StockID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = d.CompanyID, ShopID = d.StockID, VendorID = d.VendorID ?? -1 }
                        into map
                        from m in map.DefaultIfEmpty()
                        join e in _Service.QueryEntity(vendorShop) on m.VendStockID equals e.ID into vendshop
                        from vs in vendshop.DefaultIfEmpty()
                        where (a.VendorID ?? 0) == vendorID && a.CompanyID == companyID && a.AllowUsed == allowUsed && a.CardID == cardID && b.AllowUsed == true && b.IsObject == true
                        select new StockRightB
                        {
                            CompanyID = a.CompanyID,
                            StockID = a.StockID,
                            StockCode = b.StockCode,
                            StockName = b.StockName,
                            VendorID = a.VendorID,
                            ID = a.ID,
                            CardID = a.CardID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM,
                            VendorStockID = vs != null ? vs.ID : 0,
                            VendorStockCode = vs.StockCode,
                            VendorStockName = vs.StockName,
                            StockMapID = m != null ? m.ID : 0
                        };

            return query.ToList();
        }



        /// <summary>
        /// 根据条件拿取对应的仓库数据通过仓库代码模糊查询
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<StockRightB> QueryStockRights(int vendorID, string cardID, string companyID, string stockCode)
        {
            Expression<Func<StockRight, bool>> stockRight = ExpHelper.True<StockRight>();
            Expression<Func<Stock, bool>> stock = ExpHelper.True<Stock>();
            Expression<Func<StockMap, bool>> stockMap = ExpHelper.True<StockMap>();
            Expression<Func<VendShop, bool>> vendorShop = ExpHelper.True<VendShop>();

            var query = from a in _Service.QueryEntity(stockRight)
                        join b in _Service.QueryEntity(stock) on
                         new { StockID = a.StockID, CompanyID = a.CompanyID } equals new { StockID = b.StockID, CompanyID = b.CompanyID }
                        where (a.VendorID ?? 0) == vendorID && a.CompanyID == companyID
                        join d in _Service.QueryEntity(stockMap) on
                             new { CompanyID = a.CompanyID, ShopID = a.StockID, VendorID = a.VendorID ?? 0 } equals new { CompanyID = d.CompanyID, ShopID = d.StockID, VendorID = d.VendorID ?? -1 }
                        into map
                        from m in map.DefaultIfEmpty()
                        join e in _Service.QueryEntity(vendorShop) on m.VendStockID equals e.ID into vendshop
                        from vs in vendshop.DefaultIfEmpty()
                        where (a.VendorID ?? 0) == vendorID && a.CompanyID == companyID && a.CardID == cardID && b.StockCode != "OOSS" && b.AllowUsed == true && b.IsObject == true
                        && (string.IsNullOrEmpty(stockCode) ? true : b.StockCode.Contains(stockCode))
                        select new StockRightB
                        {
                            CompanyID = a.CompanyID,
                            StockID = a.StockID,
                            StockCode = b.StockCode,
                            StockName = b.StockName,
                            VendorID = a.VendorID,
                            ID = a.ID,
                            CardID = a.CardID,
                            AllowUsed = a.AllowUsed,
                            ModifyDTM = a.ModifyDTM,
                            VendorStockID = vs != null ? vs.ID : 0,
                            VendorStockCode = vs.StockCode,
                            VendorStockName = vs.StockName,
                            StockMapID = m != null ? m.ID : 0
                        };

            return query.ToList();
        }



        /// <summary>
        /// 保存仓库关联
        /// </summary>
        /// <param name="vendorID">供应商</param>
        /// <param name="companyID">公司</param>
        /// <param name="stockID">仓库</param>
        /// <param name="oldMapID">之前关联的供应商仓库ID</param>
        /// <param name="newMapID">新关联的供应商仓库ID</param>
        public HandleArgs MapStock(int vendorID, string companyID, string stockID, int oldMapID, string newMapID)
        {

            //首先删除原来的关联
            var predicate =
                ExpHelper
                .True<StockMap>()
                .AndAlso(m => m.VendStockID == oldMapID)
                .AndAlso(m => (m.VendorID ?? 0) == vendorID)
                .AndAlso(m => m.CompanyID == companyID)
                .AndAlso(m => m.StockID == stockID);

            StockMap sm = _Service.QueryEntity(predicate).FirstOrDefault();
            if (sm != null) _Service.DeleteEntity(sm, false);

            if (!string.IsNullOrEmpty(newMapID))
            {
                StockMap stockMap = new StockMap
                {
                    VendStockID = Convert.ToInt32(newMapID),
                    CompanyID = companyID,
                    VendorID = vendorID,
                    StockID = stockID
                };
                _Service.InsertEntity(stockMap, false);
            }

            return _Service.SaveChanges();
        }

        #endregion

        #region 公司部分

        /// <summary>
        /// 取所有的公司
        /// </summary>
        /// <returns></returns>
        public List<CompanyB> QueryCompanies()
        {
            var predicate = ExpHelper.True<Company>();
            return _Service.QueryEntity(predicate).Select(c => new CompanyB(c)).ToList();
        }

        /// <summary>
        /// 根据解决方案ID取公司记录
        /// <remarks>关系：供应商 -> 供应商品牌 －> 公司经营品牌－> 公司</remarks>
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public List<CompanyB> QueryCompanies(int vendorID)
        {
            var vendorPredicate = ExpHelper.True<Vendor>();
            var vendCardPredicate = ExpHelper.True<VendorCard>();
            var companyCardPredicate = ExpHelper.True<CompanyCard>();
            var companyPredicate = ExpHelper.True<Company>();

            var query = from b in _Service.QueryEntity(vendorPredicate)
                        join c in _Service.QueryEntity(vendCardPredicate) on b.ID equals c.VendorID
                        join d in _Service.QueryEntity(companyCardPredicate) on c.CardID equals d.CardID
                        join e in _Service.QueryEntity(companyPredicate) on d.CompanyID equals e.CompanyID
                        where b.ID == vendorID

                        select new CompanyB
                        {
                            CompanyID = e.CompanyID,
                            CompanyCode = e.CompanyCode,
                            CompanyName = e.CompanyName
                        };

            return query.Distinct().ToList();
        }

        #endregion

        #region 日志管理

        /// <summary>
        /// 更新一条日志记录
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public HandleArgs UpdateSysLog(SysLogB sysLog, bool submitChanges = true)
        {
            HandleArgs handle = this.ValidEntity(sysLog);
            if (handle.Status == HandleStatus.Fail) return handle;

            SysLog logBase = new SysLog();
            sysLog.CopyPropertyTo(logBase);
            return _Service.UpdateEntity(logBase, submitChanges);
        }


        /// <summary>
        /// 查询单条提报方案记录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SysLogB FetchSysLog(int logID)
        {
            var predicate = ExpHelper.True<SysLogB>();
            SysLog log = _Service.FetchEntity(new SysLog { ID = logID });
            return log == null ? null : new SysLogB(log);
        }

        /// <summary>
        /// 分页查询日志
        /// </summary>
        /// <returns></returns> 
        public List<SysLogB> QueryLogs(int currentPage, int itemsPerPage, DateTime start, DateTime end, string shopName = "", string cardName = "", string vendorCode = "")
        {
            Expression<Func<SysLog, bool>> logPredicate = ExpHelper.True<SysLog>();
            Expression<Func<Company, bool>> companyPredicate = ExpHelper.True<Company>();
            Expression<Func<Card, bool>> cardPredicate = ExpHelper.True<Card>();
            Expression<Func<Vendor, bool>> vendorPredicate = ExpHelper.True<Vendor>();
            Expression<Func<Shop, bool>> shopPredicate = ExpHelper.True<Shop>();


            //var logQuery = _Service.QueryEntity(logPredicate)
            //              .Skip((currentPage - 1) * itemsPerPage)
            //              .Take(itemsPerPage);
            var query = from a in _Service.QueryEntity(logPredicate)
                        join b in _Service.QueryEntity(companyPredicate) on a.CompanyID equals b.CompanyID into company
                        join c in _Service.QueryEntity(cardPredicate) on a.CardID equals c.CardID into card
                        join d in _Service.QueryEntity(vendorPredicate) on (a.VendorID ?? 0) equals d.ID into vendor
                        join e in _Service.QueryEntity(shopPredicate) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID } equals new { CompanyID = e.CompanyID, ShopID = e.ShopID } into shop

                        from com in company.DefaultIfEmpty()
                        from c in card.DefaultIfEmpty()
                        from d in vendor.DefaultIfEmpty()
                        from e in shop.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vendorCode) ? true : (d.VendorCode != null && d.VendorCode.Contains(vendorCode ?? ""))) &&
                              (string.IsNullOrEmpty(shopName) ? true : (e.ShopName != null && e.ShopName.Contains(shopName ?? ""))) &&
                              (string.IsNullOrEmpty(cardName) ? true : (c.CardName != null && c.CardName.Contains(cardName ?? ""))) &&
                              (a.ModifyDTM >= start && a.ModifyDTM <= end)
                        orderby a.ModifyDTM descending
                        select new SysLogB
                        {
                            ID = a.ID,
                            IsSucceed = a.IsSucceed,
                            LogType = (LogType)a.LogType,
                            ModifyDTM = a.ModifyDTM,
                            Remark = a.Remark,
                            Script = a.Script,

                            CompanyName = com.CompanyName,
                            CardName = c.CardName,
                            VendorName = d.VendorName,
                            ShopName = e.ShopName
                        };
            return query.OrderByDescending(l => l.ModifyDTM).ThenBy(l => l.CompanyName).ThenBy(l => l.CardName).Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();


        }

        /// <summary>
        /// 查询所有的日志
        /// </summary>
        /// <returns></returns>
        //public List<SysLogB> QueryLogs()
        //{
        //    Expression<Func<SysLog, bool>> predicate = ExpHelper.True<SysLog>();
        //    //var x = service.QueryEntity(predicate).Cast<SysLogl>().ToList();
        //    return _Service
        //        .QueryEntity(predicate)
        //        .ToList()
        //        .Select(item => new SysLogB(item))
        //        .ToList();
        //}

        /// <summary>
        /// 所有的日志记录数
        /// </summary>
        /// <returns></returns>
        public int LogsCount(DateTime start, DateTime end, string shopName = "", string cardName = "", string vendorCode = "")
        {
            Expression<Func<SysLog, bool>> logPredicate = ExpHelper.True<SysLog>();
            Expression<Func<Company, bool>> companyPredicate = ExpHelper.True<Company>();
            Expression<Func<Card, bool>> cardPredicate = ExpHelper.True<Card>();
            Expression<Func<Vendor, bool>> vendorPredicate = ExpHelper.True<Vendor>();
            Expression<Func<Shop, bool>> shopPredicate = ExpHelper.True<Shop>();


            var query = from a in _Service.QueryEntity(logPredicate)
                        join b in _Service.QueryEntity(companyPredicate) on a.CompanyID equals b.CompanyID into company
                        join c in _Service.QueryEntity(cardPredicate) on a.CardID equals c.CardID into card
                        join d in _Service.QueryEntity(vendorPredicate) on (a.VendorID ?? 0) equals d.ID into vendor
                        join e in _Service.QueryEntity(shopPredicate) on
                            new { CompanyID = a.CompanyID, ShopID = a.ShopID } equals new { CompanyID = e.CompanyID, ShopID = e.ShopID } into shop

                        from com in company.DefaultIfEmpty()
                        from c in card.DefaultIfEmpty()
                        from d in vendor.DefaultIfEmpty()
                        from e in shop.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vendorCode) ? true : (d.VendorCode != null && d.VendorCode.Contains(vendorCode ?? ""))) &&
                              (string.IsNullOrEmpty(shopName) ? true : (e.ShopName != null && e.ShopName.Contains(shopName ?? ""))) &&
                              (string.IsNullOrEmpty(cardName) ? true : (c.CardName != null && c.CardName.Contains(cardName ?? ""))) &&
                              (a.ModifyDTM >= start && a.ModifyDTM <= end)
                        select new SysLogB { ID = a.ID };
            return query.Count();
        }

        /// <summary>
        /// 取前8条错误日志
        /// </summary>
        /// <returns></returns>
        public List<SysLogB> QueryGatErrorLog()
        {
            Expression<Func<SysLog, bool>> logPredicate = ExpHelper.True<SysLog>();
            Expression<Func<Vendor, bool>> vendorPredicate = ExpHelper.True<Vendor>();
            Expression<Func<Company, bool>> companyPredicate = ExpHelper.True<Company>();
            Expression<Func<Card, bool>> cardPredicate = ExpHelper.True<Card>();

            var query = from a in _Service.QueryEntity(logPredicate)
                        join b in _Service.QueryEntity(vendorPredicate) on (a.VendorID ?? 0) equals b.ID into vendor
                        join c in _Service.QueryEntity(companyPredicate) on (a.CompanyID) equals c.CompanyID into company
                        join d in _Service.QueryEntity(cardPredicate) on (a.CardID) equals d.CardID into card
                        from e in vendor.DefaultIfEmpty()
                        from f in company.DefaultIfEmpty()
                        from g in card.DefaultIfEmpty()
                        orderby a.ModifyDTM descending
                        where a.LogType == (byte)(ConnnectType == Utils.ConnnectType.Web ? LogType.Gat : LogType.GatTest)
                              && a.IsSucceed == false
                        select new SysLogB
                        {
                            ID = a.ID,

                            VendorName = e.VendorName,
                            VendorCode = e.VendorCode,
                            VendorID = a.VendorID ?? 0,

                            CardCode = g.CardCode,
                            CardName = g.CardName,
                            CardID = a.CardID,

                            CompanyCode = f.CompanyCode,
                            CompanyName = f.CompanyName,
                            CompanyID = f.CompanyID,

                            ModifyDTM = a.ModifyDTM
                        };

            return query.Skip(0).Take(8).ToList();
        }

        #endregion

        #region IDisposable成员

        public void Dispose()
        {
            if (_Service != null) _Service.Dispose();
        }

        #endregion

        #region ERP存储过程部分

        /// <summary>
        /// 数据同步
        /// </summary>
        /// <param name="vendorID">供应商ID，该已弃用，存储过程会遍历表中的供应商</param>
        /// <returns></returns>
        public HandleArgs DataSynchron(int vendorID = 0)
        {
            return _Service.DataSynchron(vendorID);
        }

        /// <summary>
        /// 采集数据
        /// </summary>
        /// <param name="procName">执行的采集过程名称</param>
        /// <param name="vendorCode">品牌供应商代码</param>
        /// <param name="gatType">采集类型标志，1：正常采集；0：补采</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间，当采集类型为补采时，开始和结束时间方有效</param>
        /// <returns></returns>
        public HandleArgs ExecBatch(string vendorCode, int gatType, string companyID, string cardID, DateTime startTime, DateTime endTime)
        {
            return _Service.ExecBatch(vendorCode, gatType, companyID, cardID, startTime, endTime);
        }

        #endregion

        #region 报表

        #region Nike

        /// <summary>
        /// 查询所有的提报货号
        /// </summary>
        /// <param name="matCode"></param>
        /// <returns></returns>
        public List<GatMaterialB> QueryGatMaterials(int currentPage, int itemsPerPage, string matCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatMaterial>());
            if (!string.IsNullOrEmpty(matCode)) query = query.Where(g => g.MaterialCode.Contains(matCode));
            return query
                   .OrderBy(m => m.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatMaterialB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询所有的提报货号计数
        /// </summary>
        /// <param name="matCode"></param>
        /// <returns></returns>
        public int GatMaterialsCount(string matCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatMaterial>());
            if (!string.IsNullOrEmpty(matCode)) query = query.Where(g => g.MaterialCode.Contains(matCode));
            return query.Count();
        }

        /// <summary>
        /// 查询所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatMatInvB> QueryGatMatInvs(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopID.Contains(shopCode));
            return query
                   .OrderBy(m => m.MaterialID)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatMatInvB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopID.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 汇总
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatSummaryB> QueryGatSaleSummary(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatSaleSummary>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopID.Contains(shopCode));
            return query
                   .OrderBy(u => u.ShopID)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatSummaryB(g))
                   .ToList();
        }


        /// <summary>
        /// 汇总计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatSaleSummaryCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatSaleSummary>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopID.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 店铺
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatShopB> QueryGatShops(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatShop>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(p => p.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatShopB(g))
                   .ToList();
        }


        /// <summary>
        /// 店铺计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatShopCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatShop>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 销售
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        public List<GatPosSaleB> QueryGatPosSales(int currentPage, int itemsPerPage, string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.ShopID.Contains(shopID));
            return query
                   .OrderByDescending(p => p.BillDateTime)
                   .ThenBy(p => p.ShopID)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatPosSaleB(g))
                   .ToList();
        }


        /// <summary>
        /// 销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatPosSaleCount(string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.ShopID.Contains(shopID));
            return query.Count();
        }

        #endregion

        #region Adidas


        #endregion

        #region Kappa

        /// <summary>
        /// 查询卡帕所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatKappaMeterialINVB> QueryGatKpMatInvs(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatKappaMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatKappaMeterialINVB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询卡帕所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatKpMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatKappaMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 卡帕销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatKappaPosSaleB> QueryGatKpPosSales(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatKappaPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                    .OrderByDescending(p => p.BillDate)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.BarCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatKappaPosSaleB(g))
                   .ToList();
        }


        /// <summary>
        /// 卡帕销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatKpPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatKappaPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        #endregion

        #region Puma

        /// <summary>
        /// 查询所有的提报货号
        /// </summary>
        /// <param name="matCode"></param>
        /// <returns></returns>
        public List<GatPumaMaterialB> QueryGatPumaMaterials(int currentPage, int itemsPerPage, string matCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaMaterial>());
            if (!string.IsNullOrEmpty(matCode)) query = query.Where(g => g.MaterialCode.Contains(matCode));
            return query
                   .OrderBy(m => m.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatPumaMaterialB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询所有的提报货号计数
        /// </summary>
        /// <param name="matCode"></param>
        /// <returns></returns>
        public int GatPumaMaterialsCount(string matCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaMaterial>());
            if (!string.IsNullOrEmpty(matCode)) query = query.Where(g => g.MaterialCode.Contains(matCode));
            return query.Count();
        }

        /// <summary>
        /// 查询所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatPumaMatInvB> QueryGatPumaMatInvs(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopID.Contains(shopCode));
            return query
                   .OrderBy(m => m.MaterialID)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatPumaMatInvB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatPumaMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaMeterialINV>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopID.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 店铺
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatPumaShopB> QueryGatPumaShops(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaShop>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderBy(p => p.ShopCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatPumaShopB(g))
                   .ToList();
        }


        /// <summary>
        /// 店铺计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatPumaShopCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaShop>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 销售
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        public List<GatPumaPosSaleB> QueryGatPumaPosSales(int currentPage, int itemsPerPage, string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.ShopID.Contains(shopID));
            return query
                    .OrderByDescending(p => p.BillDateTime)
                   .ThenBy(p => p.ShopID)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatPumaPosSaleB(g))
                   .ToList();
        }


        /// <summary>
        /// 销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatPumaPosSaleCount(string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatPumaPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.ShopID.Contains(shopID));
            return query.Count();
        }

        #endregion

        #region Northland

        /// <summary>
        /// 查询诺斯兰所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatNorthMaterialInvB> QueryGatNorthMatInvs(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNorthMaterialInv>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.StockName.Contains(shopCode));
            return query
                   .OrderBy(m => m.StockName)
                   .ThenBy(m => m.MaterialName)
                   .ThenBy(m => m.SizeName)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatNorthMaterialInvB(g))
                   .ToList();
        }


        /// <summary>
        /// 查询诺斯兰所有的提报库存计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatNorthMatInvCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNorthMaterialInv>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.StockName.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 诺斯兰销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatNorthPosSaleB> QueryGatNorthPosSales(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNorthPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                   .OrderByDescending(p => p.BillTime)
                   .ThenBy(p => p.ShopName)
                   .ThenBy(p => p.BillNo)
                   .ThenBy(p => p.MaterialName)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatNorthPosSaleB(g))
                   .ToList();
        }


        /// <summary>
        /// 诺斯兰销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatNorthPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatNorthPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        #endregion

        #region Amer

        /// <summary>
        /// 查询萨洛蒙(亚玛芬)销售
        /// </summary>
        /// <returns></returns>
        public List<GatAmerPosSaleB> QueryGatAmerPosSale(int currentPage, int itemsPerPage, string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.ShopCode.Contains(shopID));
            return query
                  .OrderByDescending(p => p.BillDate)
                  .ThenBy(p => p.ShopCode)
                  .ThenBy(p => p.BillNo)
                  .ThenBy(p => p.ProductCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAmerPosSaleB(g))
                  .ToList();
        }
        /// <summary>
        /// 萨洛蒙(亚玛芬)销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAmerPosSaleCount(string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.ShopCode.Contains(shopID));
            return query.Count();
        }

        /// <summary>
        /// 查询萨洛蒙(亚玛芬)进货信息
        /// </summary>
        /// <returns></returns>
        public List<GatAmerReStockInfoB> QueryGatAmerReStockInfo(int currentPage, int itemsPerPage, string providerCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerReStockInfo>());
            if (!string.IsNullOrEmpty(providerCode)) query = query.Where(g => g.ProviderCode.Contains(providerCode));
            return query
                  .OrderByDescending(p => p.BillDate)
                  .ThenBy(p => p.ProviderCode)
                  .ThenBy(p => p.BillNo)
                  .ThenBy(p => p.ProductCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAmerReStockInfoB(g))
                  .ToList();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)进货计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAmerReStockInfoCount(string providerCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerReStockInfo>());
            if (!string.IsNullOrEmpty(providerCode)) query = query.Where(g => g.ProviderCode.Contains(providerCode));
            return query.Count();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)库存
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public List<GatAmerINVB> QueryGatAmerINV(int currentPage, int itemsPerPage, string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerINV>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ProductCode.Contains(productCode));
            return query
                  .OrderByDescending(p => p.ProductCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAmerINVB(g))
                  .ToList();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)库存记数
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public int GatAmerINVCount(string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerINV>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ProductCode.Contains(productCode));
            return query.Count();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)其他出入库
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public List<GatAmerOtherB> QueryGatAmerOther(int currentPage, int itemsPerPage, string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerOther>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ProductCode.Contains(productCode));
            return query
                  .OrderByDescending(p => p.ProductCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAmerOtherB(g))
                  .ToList();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)其他出入库记数
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public int GatAmerOtherCount(string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerOther>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ProductCode.Contains(productCode));
            return query.Count();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)产品
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public List<GatAmerMaterialB> QueryGatAmerMaterial(int currentPage, int itemsPerPage, string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerMaterial>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ProductCode.Contains(productCode));
            return query
                  .OrderByDescending(p => p.ProductCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAmerMaterialB(g))
                  .ToList();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)产品记数
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public int GatAmerMaterialCount(string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerMaterial>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ProductCode.Contains(productCode));
            return query.Count();
        }
        /// <summary>
        /// 萨洛蒙(亚玛芬)门店
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public List<GatAmerShopB> QueryGatAmerShop(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerShop>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query
                  .OrderByDescending(p => p.ShopCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAmerShopB(g))
                  .ToList();
        }

        /// <summary>
        /// 萨洛蒙(亚玛芬)产品门店
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public int GatAmerShopCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAmerShop>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        #endregion



        #region 亚瑟士

        /// <summary>
        /// 亚瑟士库存记数
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public int GatAsicsINVCount(string productCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAsicsMeterialINV>());
            if (!string.IsNullOrEmpty(productCode)) query = query.Where(g => g.ShopCode.Contains(productCode));
            return query.Count();
        }

        /// <summary>
        /// 查询萨洛蒙(亚玛芬)销售
        /// </summary>
        /// <returns></returns>
        public List<GatAsicsPosSaleB> QueryGatAsicsPosSale(int currentPage, int itemsPerPage, string shopCode, string billCode, string barCode, string startBillDate, string endBillDate, string materialCode, string sizeCode, ref int totalCount, ref decimal totalQty, ref decimal totalAmount)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAsicsPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.StoreCode.Contains(shopCode));
            if (!string.IsNullOrEmpty(billCode)) query = query.Where(g => g.BillCode.Contains(billCode));
            if (!string.IsNullOrEmpty(barCode)) query = query.Where(g => g.ProdBarCode.Contains(barCode));
            if (!string.IsNullOrEmpty(materialCode)) query = query.Where(g => g.ProdCode.Contains(materialCode));
            if (!string.IsNullOrEmpty(sizeCode)) query = query.Where(g => g.ProdSpec.Contains(sizeCode));

            if (!string.IsNullOrWhiteSpace(startBillDate))
            {
                DateTime sBillDate = Convert.ToDateTime(startBillDate);
                query = query.Where(s => s.BillDate >= sBillDate);
            } 
            if (!string.IsNullOrWhiteSpace(endBillDate))
            {
                DateTime eBillDate = Convert.ToDateTime(endBillDate);
                query = query.Where(e => e.BillDate <= eBillDate);
            }

            totalCount = query.Count();
            totalQty = 0;
            totalAmount = 0;

            if (totalCount > 0)
            {
                totalQty = (decimal)query.Sum(s => s.ProdQuantity);
                totalAmount = (decimal)query.Sum(s => s.ProdAmount);
            }
            return query
                  .OrderByDescending(p => p.BillDate)
                  .ThenBy(p => p.StoreCode)
                  .ThenBy(p => p.BillCode)
                  .ThenBy(p => p.ProdCode)
                  .Skip((currentPage - 1) * itemsPerPage)
                  .Take(itemsPerPage)
                  .Select(g => new GatAsicsPosSaleB(g))
                  .ToList();
        }

        /// <summary>
        /// 亚瑟士)销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatAsicsPosSaleCount(string shopID = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatAsicsPosSale>());
            if (!string.IsNullOrEmpty(shopID)) query = query.Where(g => g.StoreCode.Contains(shopID));
            return query.Count();
        }

        #endregion


        #region Ua

        /// <summary>
        /// 查询Ua所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatUaMaterialB> QueryGatUaMats(int currentPage, int itemsPerPage, string ShopCode, string BarCode, string MaterialCode , string SizeCode , out int Count, out decimal ActualQty, out decimal PathQty)
        {
            Count = 0;
            ActualQty = 0;
            PathQty = 0;
            var query = _Service.QueryEntity(ExpHelper.True<GatUaMaterial>());
            if (!string.IsNullOrEmpty(ShopCode)) query = query.Where(g => g.ShopCode.Contains(ShopCode));
            if (!string.IsNullOrEmpty(BarCode)) query = query.Where(g => g.BarCode.Contains(BarCode));
            if (!string.IsNullOrEmpty(MaterialCode)) query = query.Where(g => g.MaterialCode.Contains(MaterialCode));
            if (!string.IsNullOrEmpty(SizeCode)) query = query.Where(g => g.SizeName.Contains(SizeCode));
            Count = query.Count();
            if (Count > 0)
            {
                ActualQty = (decimal)query.Sum(s => s.ActualQty);
                PathQty = (decimal)query.Sum(s => s.PathQty);
            }
            return query
                   .OrderBy(m => m.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage) 
                   .Select(g => new GatUaMaterialB(g))
                   .ToList();
        }

        /// <summary>
        /// 查询Ua所有的提报库存 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public IQueryable<GatUaMaterialB> QueryGatUaMatInvsQyeryable(string ShopCode, string BarCode, string MaterialCode, string SizeCode)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaMaterial>());
            if (!string.IsNullOrEmpty(ShopCode)) query = query.Where(g => g.ShopCode.Contains(ShopCode));
            if (!string.IsNullOrEmpty(BarCode)) query = query.Where(g => g.BarCode.Contains(BarCode));
            if (!string.IsNullOrEmpty(MaterialCode)) query = query.Where(g => g.MaterialCode.Contains(MaterialCode));
            if (!string.IsNullOrEmpty(SizeCode)) query = query.Where(g => g.SizeName.Contains(SizeCode));
            return query
                   .OrderBy(m => m.ShopCode)
                   .Select(g => new GatUaMaterialB(g));
        }

        /// <summary>
        /// 查询Ua所有的提报库存
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatUaMatCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaMaterial>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// Ua销售
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public List<GatUaPosSaleB> QueryGatUaPosSales(int currentPage, int itemsPerPage, string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            query = query
                    .OrderByDescending(p => p.BillDate)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillCode)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage);
            List<GatUaPosSaleB> result = new List<GatUaPosSaleB>();
            if (query.Count() > 0)
            {
                result = query.Select(g => new GatUaPosSaleB(g)).ToList();
            }
            return result;
        }
        public decimal[] QueryGatUaTotal()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSale>());
            decimal Qty = 0;
            decimal prices = 0;
            if (query.Count()>0)
            {
                Qty = query.Select(s => s.Qty).Sum();
                prices = query.Select(s => s.Price).Sum() ?? 0;
            }
            decimal[] marks = new decimal[] { Qty, prices };
            return marks;
        }
        public decimal[] QueryGatUaTSTotal()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSaleTransfer>());
            decimal qty = 0;
            decimal prices = 0;
            if (query.Count() > 0)
            {
                qty = query.Select(s => s.Qty).Sum() ?? 0;
                prices = query.Select(s => s.Price).Sum() ?? 0;
            }
            decimal[] marks = new decimal[] { qty, prices };
            return marks;
        }
        public List<GatUaPosSaleB> QueryGatUaPosSalesData(int solutionID, string start_date, string end_date, string billDateStartDate, string billDateEndDate)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSale>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            if (!string.IsNullOrWhiteSpace(start_date))
            {
                DateTime dtToday = Convert.ToDateTime(start_date);
                query = query.Where(s => s.BillTime.Date >= dtToday);
            }
            if (!string.IsNullOrWhiteSpace(end_date))
            {
                DateTime dtendToday = Convert.ToDateTime(end_date);
                query = query.Where(e => e.BillTime.Date <= dtendToday);
            }
            if (!string.IsNullOrWhiteSpace(billDateStartDate))
            {
                DateTime date = Convert.ToDateTime(billDateStartDate);
                query = query.Where(e => e.BillDate.Date >= date);
            }
            if (!string.IsNullOrWhiteSpace(billDateEndDate))
            {
                DateTime date = Convert.ToDateTime(billDateEndDate);
                query = query.Where(e => e.BillDate.Date <= date);
            }
            return query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillCode)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatUaPosSaleB(g))
                .ToList();
        }
        public List<GatUaPosSaleB> QueryGatUaPosSalesALL()
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSale>());
            return query
                .OrderBy(p => p.BillTime)
                .ThenBy(p => p.ShopCode)
                .ThenBy(p => p.BillCode)
                .ThenBy(p => p.MaterialCode)
                .Select(g => new GatUaPosSaleB(g))
                .ToList();
        }
        /// <summary>
        /// Ua销售计数
        /// </summary>
        /// <param name="shopCode"></param>
        /// <returns></returns>
        public int GatUaPosSaleCount(string shopCode = "")
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSale>());
            if (!string.IsNullOrEmpty(shopCode)) query = query.Where(g => g.ShopCode.Contains(shopCode));
            return query.Count();
        }

        /// <summary>
        /// 中转数据（Ua的销售数据）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public List<GatUaPosSaletransferB> QueryGatUaPosSaleTransfer(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSaleTransfer>());
            if (solutionID > 0)
            {
                query = query.Where(q => q.SolutionID == solutionID);
                try
                {
                    query.ToList();
                }
                catch (InvalidCastException ex)
                {
                    query = query.Where(q => q.SolutionID == solutionID);
                }
            }

            return query
                    .OrderByDescending(p => p.ID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.BillCode)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatUaPosSaletransferB(g))
                   .ToList();
        }

        /// <summary>
        /// 中转数据总条数（Ua的销售数据总条数）
        /// </summary>
        /// <param name="solutionID"></param>
        /// <returns></returns>
        public int GatUaPosSaleTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaPosSaleTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }

        /// <summary>
        /// 保存中转数据到真实库
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs TransferUaSaveAs(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Ua_transferAs_Pos '{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        /// 删除中转库中的数据
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs DeleteUaTransfer(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = "";
                    if (TransIDstr == "C")
                    {
                        command = @" DELETE Gat_Ua_PosSale_transfer  ";
                    }
                    else
                    {
                        command = @" DELETE Gat_Ua_PosSale_transfer WHERE ID IN ({0}) ";
                    }
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }


        /// <summary>
        /// 删除UA所以库存
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs DeleteUaAllMaterial()
        { 
            HandleArgs HANDLE = new HandleArgs();
            string command = "";
            command = @" TRUNCATE TABLE Gat_Ua_Material ";
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs AddUaTransfer(GatUaPosSaleTransfer entity)
        {
            HandleArgs handle = new HandleArgs();
            if (entity == null)
            {
                handle.Status = HandleStatus.Fail;
                handle.Message = "不能为空";
                return handle;
            }
            handle = _Service.InsertEntity(entity);
            if (handle.Status == HandleStatus.Completed)
                return handle;
            return handle;
        }

        public HandleArgs InsertUaTransferQty(DataTable dt, string ddlSoule)
        {

            List<GatUaPosSaleTransfer> list = Utils.ConvertHelper.DataTableToList<GatUaPosSaleTransfer>(dt);
            var cardId = "";
            var companyId = "";
            if (!string.IsNullOrEmpty(ddlSoule))
            {
                var query = _Service.QueryEntity(ExpHelper.True<Solution>());
                var solution = query.Where(s => s.ID == int.Parse(ddlSoule)).FirstOrDefault();
                cardId = solution.CardID;
                companyId = solution.CompanyID;

            }

            HandleArgs handle = new HandleArgs();
            foreach (var item in list)
            {
                if (item.BillDate == null)
                {
                    item.BillDate = item.BillTime;
                }
                item.SolutionID = int.Parse(ddlSoule);
                item.BillTime = DateTime.Now;
                item.CardID = cardId;
                item.CompanyID = companyId;
                item.PromotionID = "";

                handle = AddUaTransfer(item);

            }
            return handle;
        }

        public HandleArgs UpdateUaTransferQty(int Qty, int TransID)
        {
            string command = @" UPDATE Gat_Ua_PosSale_transfer SET Amount=BalaPrice*{0},Qty={1} WHERE TransID={2} ";
            command = string.Format(command, Qty, Qty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        public HandleArgs InsertUaMaterialTransferQty_bak(DataTable dt, string ddlSoule)
        {

            List<GatUaMaterialTransfer> list = Utils.ConvertHelper.DataTableToList<GatUaMaterialTransfer>(dt);
            HandleArgs handle = new HandleArgs();
            foreach (var item in list)
            {
                if (item.UpdateDate == null)
                {
                    item.UpdateDate = DateTime.Now;
                    item.ModifyDTM = DateTime.Now;
                    item.CreateDTM = DateTime.Now;
                    item.Remark = "";
                }
                item.SolutionID = int.Parse(ddlSoule);

                handle = AddUaMaterialTransfer(item);

            }
            return handle;
        }

        public HandleArgs AddUaMaterialTransfer(GatUaMaterialTransfer entity)
        {
            HandleArgs handle = new HandleArgs();
            if (entity == null)
            {
                handle.Status = HandleStatus.Fail;
                handle.Message = "不能为空";
                return handle;
            }
            handle = _Service.InsertEntity(entity);
            if (handle.Status == HandleStatus.Completed)
                return handle;
            return handle;
        }


        public HandleArgs InsertUaMaterialTransferQty(DataTable dt, string ddlSoule)
        { 
            HandleArgs handle = new HandleArgs();

            if (dt == null || dt.Rows.Count == 0)
            {
                handle.Status = HandleStatus.Fail;
                handle.Message = "数据不能为空";
                return handle;
            }
            AddColumnIfNotExists(dt, "UpdateDate", typeof(DateTime));
            AddColumnIfNotExists(dt, "ModifyDTM", typeof(DateTime));
            AddColumnIfNotExists(dt, "CreateDTM", typeof(DateTime));
            AddColumnIfNotExists(dt, "Remark", typeof(string));
            AddColumnIfNotExists(dt, "SolutionID", typeof(int));
            // 1. 预处理 DataTable：添加默认值
            foreach (DataRow row in dt.Rows)
            {
                if (row["UpdateDate"] == DBNull.Value || row["UpdateDate"] == null)
                {
                    row["UpdateDate"] = DateTime.Now;
                    row["ModifyDTM"] = DateTime.Now;
                    row["CreateDTM"] = DateTime.Now;
                    row["Remark"] = "";
                }
                row["SolutionID"] = int.Parse(ddlSoule);
            }

            // 2. 批量插入
            handle = BatchInsert(dt, "gat_ua_material_transfer");
            return handle;
        }
        // 辅助方法：添加字段（如果不存在）
        private void AddColumnIfNotExists(DataTable dt, string columnName, Type columnType)
        {
            if (!dt.Columns.Contains(columnName))
            {
                dt.Columns.Add(columnName, columnType);
            }
        }

        public HandleArgs BatchInsert(DataTable dt, string tableName)
        {
            HandleArgs handle = new HandleArgs();

            using (SqlConnection conn = new SqlConnection(_Service.ConnectString))
            {
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = 10000;      // 每批1万条
                    bulkCopy.BulkCopyTimeout = 600;   // 超时时间（秒）

                    // 映射列（如果列名不完全一致）
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    try
                    {
                        bulkCopy.WriteToServer(dt);
                        handle.Status = HandleStatus.Completed;
                        handle.Message = $"成功插入{dt.Rows.Count}条数据";
                    }
                    catch (Exception ex)
                    {
                        handle.Status = HandleStatus.Fail;
                        handle.Message = $"批量插入失败：{ex.Message}";
                    }
                }
            }
            return handle;
        }

        public HandleArgs DeleteUaMaterialTransfer(string TransIDstr)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = "";
                    if (TransIDstr == "C")
                    {
                        command = @" DELETE Gat_Ua_Material_transfer  ";
                    }
                    else
                    {
                        command = @" DELETE Gat_Ua_Material_transfer WHERE ID IN ({0}) ";
                    }
                    command = string.Format(command, TransIDstr);
                    HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return HANDLE;
        }

        public HandleArgs UpdateUaMaterialTransferQty(int ActualQty, int pathQty, int TransID)
        {
            string command = @" UPDATE Gat_Ua_Material_transfer SET ActualQty={0},pathQty={1} WHERE TransID={2} ";
            command = string.Format(command, ActualQty, pathQty, TransID);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }


        public int GatUaMaterialTransferCount(int solutionID)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaMaterialTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);
            return query.Count();
        }
        public List<GatUaMaterialTransferB> QueryGatUaMaterialTransfer(int solutionID, int currentPage, int itemsPerPage)
        {
            var query = _Service.QueryEntity(ExpHelper.True<GatUaMaterialTransfer>());
            if (solutionID > 0)
                query = query.Where(q => q.SolutionID == solutionID);

            return query
                    .OrderByDescending(p => p.ID)
                   .ThenBy(p => p.ShopCode)
                   .ThenBy(p => p.MaterialCode)
                   .Skip((currentPage - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .Select(g => new GatUaMaterialTransferB(g))
                   .ToList();
        }

        /// <summary>
        /// 保存中转数据到真实库
        /// </summary>
        /// <param name="TransIDstr"></param>
        /// <returns></returns>
        public HandleArgs TransferUaMaterialSaveAs(string TransIDstr)
        {
            HandleArgs handle = new HandleArgs();
            if (!string.IsNullOrEmpty(TransIDstr))
            {
                if (!HasDangerousContents(TransIDstr))
                {
                    string command = @" EXEC Gat_Ua_transferAs_Material '{0}' ";
                    command = string.Format(command, TransIDstr);
                    HandleArgs HANDLE = _Service.ExecuteCommand(command);
                }
            }
            return handle;
        }

        /// <summary>
        ///  删除Pos销售数据
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="startBillTime"></param>
        /// <param name="endBillTime"></param>
        /// <param name="startBillDate"></param>
        /// <param name="endBillDate"></param>
        /// <returns></returns>
        public HandleArgs DeleteUaPosSalesData(int solutionId, string startBillTime, string endBillTime, string startBillDate, string endBillDate)
        {
            string where = "";
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)>=CONVERT(varchar(10),'{startBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(startBillTime))
            {
                where += $" AND CONVERT(varchar(10),billTime,121)<=CONVERT(varchar(10),'{endBillTime}',121)";
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                where += $" AND CONVERT(varchar(10),billdate,121)>=CONVERT(varchar(10),'{startBillDate}',121)";
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                where += $" AND CONVERT(varchar(10),billdate,121)<=CONVERT(varchar(10),'{endBillDate}',121)";
            }
            HandleArgs HANDLE = new HandleArgs();
            string command = $" DELETE Gat_Ua_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        /// <summary> 
        /// 通过ID删除Pos销售数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HandleArgs DeleteUaPosSalesData(int solutionId, string ids)
        {
            string where = "";
            HandleArgs HANDLE = new HandleArgs();
            if (string.IsNullOrEmpty(ids))
            {
                return HANDLE;
            }
            where = $" and id in({ids})";
            string command = $" DELETE Gat_Ua_PosSale WHERE solutionId='{solutionId}'" + where;
            command = string.Format(command);
            HANDLE = _Service.ExecuteCommand(command);
            return HANDLE;
        }

        #endregion

        #endregion

        #region
        /// <summary>
        /// 检测是否含有危险字符（防止Sql注入）
        /// </summary>
        /// <param name="contents">预检测的内容</param>
        /// <returns>返回True或false</returns>
        private bool HasDangerousContents(string contents)
        {
            bool bReturnValue = false;
            if (contents.Length > 0)
            {
                //convert to lower
                string sLowerStr = contents.ToLower();
                //RegularExpressions
                string sRxStr = @"(/sand/s)|(/sand/s)|(/slike/s)|(select/s)|(insert/s)|(delete/s)|(update/s[/s/S].*/sset)|(create/s)|(/stable)|(<[iframe|/iframe|script|/script])|(')|(/sexec)|(/sdeclare)|(/struncate)|(/smaster)|(/sbackup)|(/smid)|(/scount)";
                //Match
                bool bIsMatch = false;
                System.Text.RegularExpressions.Regex sRx = new System.Text.RegularExpressions.Regex(sRxStr);
                bIsMatch = sRx.IsMatch(sLowerStr, 0);
                if (bIsMatch)
                {
                    bReturnValue = true;
                }
            }
            return bReturnValue;
        }
        #endregion
    }
}


