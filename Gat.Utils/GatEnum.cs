using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.Utils
{
    /// <summary>
    /// 连接类型
    /// </summary>
    public enum ConnnectType
    {
        /// <summary>
        /// WEB正式库
        /// </summary>
        Web = 1,

        /// <summary>
        /// ERP正式库
        /// </summary>
        ERP = 2,

        /// <summary>
        /// WEB测试库
        /// </summary>
        WebTest = 3,

        /// <summary>
        /// ERP测试库
        /// </summary>
        ERPTest = 4,

        None = 5
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 数据提取
        /// </summary>
        Gat = 1,
        /// <summary>
        /// 应用程序
        /// </summary>
        App = 2,

        /// <summary>
        /// 数据提取测试
        /// </summary>
        GatTest = 3,

        /// <summary>
        /// 应用程序测试
        /// </summary>
        AppTest = 4
    }

    /// <summary>
    /// 运行环境
    /// </summary>
    public enum RunEnvironment
    {
        /// <summary>
        /// 测试
        /// </summary>
        Test = 2,

        /// <summary>
        /// 正式
        /// </summary>
        Due = 1
    }

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// 超级管理员
        /// </summary>
        SupAdmin = 1,

        /// <summary>
        /// 开发人员
        /// </summary>
        Developer = 2,

        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 3
    }

    /// <summary>
    /// 提报类型
    /// </summary>
    public enum GatType
    {
        /// <summary>
        /// 从数据库提取
        /// </summary>
        FromDatabase = 1,
        /// <summary>
        /// 从文件提取
        /// </summary>
        FromFile = 2
    }

    /// <summary>
    ///  执行类型，1：天，2：周，3：月。目前仅支持“天”执行。
    /// </summary>
    public enum ExecType
    {
        /// <summary>
        /// 天
        /// </summary>
        Daily = 1,
        /// <summary>
        /// 周
        /// </summary>
        Week = 2,
        /// <summary>
        /// 月
        /// </summary>
        Month = 3
    }

    /// <summary>
    /// 每天执行类型，1：执行一次(Once)，2：重复执行(Repeat)
    /// </summary>
    public enum DailyType
    {
        /// <summary>
        /// 每天只执行一次
        /// </summary>
        Once = 1,

        /// <summary>
        /// 重复执行
        /// </summary>
        Repeat  =2
    }

    /// <summary>
    /// 重复执行类型，1：秒，2：分，3：时；仅DailyType=2时有效
    /// </summary>
    public enum RepeatType
    {
        /// <summary>
        /// 秒
        /// </summary>
        Second = 1,
        /// <summary>
        /// 分
        /// </summary>
        Minute =2,

        /// <summary>
        /// 时
        /// </summary>
        Hour = 3
    }

    /// <summary>
    /// 关联报表
    /// </summary>
    public enum ReportMap
    {
        /// <summary>
        /// 耐克
        /// </summary>
        Nike = 1,

        /// <summary>
        /// 阿迪
        /// </summary>
        Adidas = 2,

        /// <summary>
        /// 卡帕
        /// </summary>
        Kappa = 3,

        /// <summary>
        /// 彪马
        /// </summary>
        Puma = 4,

        /// <summary>
        /// 李宁
        /// </summary>
        Lining = 5,
        
        /// <summary>
        /// 诺斯兰
        /// </summary>
        Northland = 6,

        /// <summary>
        /// 萨洛蒙(亚玛芬)
        /// </summary>
        Amer=7,
        /// <summary>
        /// 哥伦比亚
        /// </summary>
        Columbia=8,
        /// <summary>
        ///亚瑟士
        /// </summary>
        Asics = 9,
        /// <summary>
        /// UA
        /// </summary>
        Ua=10,
        /// <summary>
        /// 狼爪
        /// </summary>
        JackWolfSkin,
        /// <summary>
        /// 斯凯奇
        /// </summary>
        Skechers,
        /// <summary>
        /// Crocs
        /// </summary>
        Crocs,
        /// <summary>
        /// 李宁会员
        /// </summary>
        LiningMember,
        /// <summary>
        /// 李宁分销
        /// </summary>
        LiningFX,
        /// <summary>
        /// 阿迪_全渠道
        /// </summary>
        AdidasOmniChannel,
        /// <summary>
        /// 阿迪_智慧零售
        /// </summary>
        AdidasWifSale=17,
        /// <summary>
        /// 途明
        /// </summary>
        TuMi = 18,
        /// <summary>
        /// On昂跑
        /// </summary>
        On = 19,
        /// <summary>
        /// VANS范斯
        /// </summary> 
        Vans = 20,
        /// <summary>
        /// 华润置地
        /// </summary>
        ArcTeryx = 21,

        /// <summary>
        /// 李宁全渠道
        /// </summary>
        LiningOms = 22,

        /// <summary>
        /// 拓冰者
        /// </summary>
        IceBreaker = 23,

        /// <summary> 
        /// LEE 
        /// </summary>
        LEE = 24,

        /// <summary>
        /// Nike_全渠道
        /// </summary>
        NikeOmniChannel=25,

        /// <summary>
        /// 北面
        /// </summary>
        TheNorthFace = 26,

        /// <summary>
        /// Iker
        /// </summary>
        Iker = 27,

        /// <summary>
        /// 伯俊
        /// </summary>
        Burgeon = 28,

        /// <summary>
        /// Nike API 对接
        /// </summary>
        NikeApi = 29,

    }

    /// <summary>
    /// 数据库
    /// </summary>
    public enum Database
    {
        /// <summary>
        /// 平台正式库
        /// </summary>
        Web = 1,

        /// <summary>
        /// 平台测试库
        /// </summary>
        WebTest = 2,

        /// <summary>
        /// ERP正式库
        /// </summary>
        ERP = 3,

        /// <summary>
        /// ERP测试库
        /// </summary>
        ERPTest = 4
    }
}
