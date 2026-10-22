<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gat_error.aspx.cs" Inherits="Gat.Web.admin.gat_error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>您找的页面不存在</title>
    <style type="text/css">
        BODY
        {
            padding-right: 0px;
            padding-bottom: 0px;
            margin: 0px;
            font: 12px Arial, Helvetica, sans-serif;
            color: #333;
            padding-top: 35px;
            background:url(/images/HK_logo_bg.gif) no-repeat center 200px;
        }
        A
        {
            color: #007ab7;
            text-decoration: none;
        }
        A:hover
        {
            color: #007ab7;
            text-decoration: none;
        }
        A:hover
        {
            color: #de1d6a;
        }
        .hidehr
        {
            display: none;
        }
        .show12
        {
            padding-right: 0px;
            display: block;
            padding-left: 0px;
            padding-bottom: 0px;
            margin: 5px 0px;
            padding-top: 0px;
        }
        .show13
        {
            padding-right: 0px;
            display: block;
            padding-left: 0px;
            padding-bottom: 0px;
            margin: 5px 0px;
            padding-top: 0px;
        }
        .show12 A
        {
            border-right: #bfdeed 1px solid;
            padding-right: 6px;
            border-top: #bfdeed 1px solid;
            display: inline-block;
            padding-left: 6px;
            background: #d8ebf4;
            padding-bottom: 2px;
            overflow: hidden;
            border-left: #bfdeed 1px solid;
            line-height: 17px;
            padding-top: 2px;
            border-bottom: #bfdeed 1px solid;
            height: 16px;
        }
        .show13 A
        {
            border-right: #bfdeed 1px solid;
            padding-right: 6px;
            border-top: #bfdeed 1px solid;
            display: inline-block;
            padding-left: 6px;
            background: #d8ebf4;
            padding-bottom: 2px;
            overflow: hidden;
            border-left: #bfdeed 1px solid;
            line-height: 17px;
            padding-top: 2px;
            border-bottom: #bfdeed 1px solid;
            height: 16px;
        }
        .show12 A:hover
        {
            border-right: #ea5e96 1px solid;
            border-top: #ea5e96 1px solid;
            background: #fce8f0;
            border-left: #ea5e96 1px solid;
            color: #de1d6a;
            border-bottom: #ea5e96 1px solid;
            text-decoration: none;
        }
        .show13 A:hover
        {
            border-right: #ea5e96 1px solid;
            border-top: #ea5e96 1px solid;
            background: #fce8f0;
            border-left: #ea5e96 1px solid;
            color: #de1d6a;
            border-bottom: #ea5e96 1px solid;
            text-decoration: none;
        }
        .show12 A
        {
            font-weight: normal;
            font-size: 12px;
        }
        .show13 A
        {
            border-right: #ed268c 1px solid;
            border-top: #ed268c 1px solid;
            background: #dd137b;
            border-left: #ed268c 1px solid;
            color: #fff;
            border-bottom: #ed268c 1px solid;
        }
        .img404
        {
            padding-right: 0px;
            padding-left: 0px;
            /*background: url(/images/404.gif) no-repeat left top;*/
            float: left;
            padding-bottom: 0px;
            margin: 0px;
            width: 80px;
            padding-top: 0px;
            position: relative;
            height: 90px;
        }
        h1
        {
            margin:0;
            padding:0px;
            padding-left:5px;
            padding-bottom:3px;
            }
        h2
        {
            padding-right: 0px;
            padding-left: 0px;
            font-size: 16px;
            padding-bottom: 15px;
            margin: 0px;
            width: 80%;
            line-height: 0;
            padding-top: 25px;
            color:#ee6a00;
            position: relative;
        }
        h3.wearesorry
        {
            padding-right: 0px;
            padding-left: 0px;
            font-weight: normal;
            font-size: 10px;
            padding-bottom: 0px;
            margin: 0px;
            color: #ccc;
            line-height: 10px;
            padding-top: 0px;
            position: relative;
        }
        .content
        {
            clear: both;
            padding-right: 0px;
            padding-left: 0px;
            font-size: 13px;
            left: 80px;
            float: left;
            padding-bottom: 0px;
            margin: 0px;
            width: 80%;
            line-height: 19px;
            padding-top: 0px;
            position: relative;
        }
        .content ul
        {
            padding-right: 35px;
            padding-left: 35px;
            padding-bottom: 20px;
            margin: 0px;
            padding-top: 10px;
        }
        .content ul li
        {
            color:#F00;
            }
        .show12 ul
        {
            padding-right: 0px;
            padding-left: 0px;
            padding-bottom: 0px;
            margin: 0px 0px 0px 20px;
            padding-top: 0px;
            list-style-type: none;
        }
        .show14 ul LI
        {
            margin-bottom: 5px;
        }
    </style>
</head>
<body>
    <div style="padding-left: 80px;">
        <table cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td>
                    <img src="images/handuplogo.jpg" alt="" />
                </td>
                <td height="46" valign="bottom">
                    <h1>
                        恒康提示</h1>
                </td>
            </tr>
        </table>
    </div>
    <form id="form1" runat="server">
    <div class="img404">
    </div>
    <div>
        <h2>
            抱歉，找不到您要的页面……</h2>
    </div>
    <div style="border-bottom: #ccc 1px solid; padding-bottom: 8px;">
        <h3 class="wearesorry">
            We're sorry but the page your are looking for is Not Found...</h3>
    </div>
    <div class="content">
        仔细找过啦，没有发现你要找的页面。最可能的原因是：
        <ul>
            <li>在地址中可能存在键入错误。 </li>
            <li>当你点击某个链接时，它可能已过期。 </li>
            <li>不存在您访问的资源。 </li>
        </ul>
        <strong>点击以下链接继续浏览其它内容</strong><!--（<A href="http://www.duice.net/" target=_blank>http://www.duice.net/</A>）： -->
        <asp:Panel ID="pnlLinks" runat="server" CssClass="show14">
        </asp:Panel>
    </div>
    <span style="visibility: hidden"></span>
    <div style=" position:absolute; bottom:0px; left:40%; width:450px; margin:0 auto; height:30px; line-height:30px; color:#CCC;">
        ©2010-2011 Hengkang Technology Co.,Ltd. All Rights Reserved.
    </div>
    </form>
</body>
</html>
