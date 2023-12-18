<%@ WebHandler Language="C#" Class="imageUp" %>
<%@ Assembly Src="Uploader.cs" %>

using System;
using System.Web;
using System.IO;
using System.Collections;
using Microsoft.SqlServer;

public class imageUp : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        //上传配置
        int size = 2;           //文件大小限制,单位MB                             //文件大小限制，单位MB
        string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };         //文件允许格式


        //上传图片
        Hashtable info = new Hashtable();
        Uploader up = new Uploader();

        string pathbase = "Editor/";//先写死Editor
        
        //int path = Convert.ToInt32(up.getOtherInfo(context, "dir"));
        //string ArticleType = CookieCustom.GetValue("ArticleType");
        //ChineseCode china = new ChineseCode();//中文转换类
        //if (ArticleType == "2")
        //{
        //    int expertId = CookieCustom.GetValue("expertId") == "" ? 0 : Convert.ToInt32(CookieCustom.GetValue("expertId"));
        //    Bas_Expert ExpertModel = (new Bas_ExpertBLL()).Get(expertId);
        //    if (ExpertModel != null && expertId != 0)
        //    {
        //        pathbase = TextUtility.ClearHtml(china.GetSpell(ExpertModel.ExpertName)) + "/ArticlePhoto/";
        //    }
        //    else
        //    {
        //        pathbase = "Attachment_Temp/";
        //    }
        //}
        //else if (ArticleType == "3")
        //{
        //    int expertApplicationId = CookieCustom.GetValue("expertApplicationId") == "" ? 0 : Convert.ToInt32(CookieCustom.GetValue("expertApplicationId"));
        //    Bas_ExpertApplication ExpertApplicationModel = (new Bas_ExpertApplicationBLL()).Get(expertApplicationId);
        //    if (ExpertApplicationModel != null && expertApplicationId != 0)
        //    {
        //        pathbase = TextUtility.ClearHtml(china.GetSpell(ExpertApplicationModel.ExpertName)) + "/ArticlePhoto/";
        //    }
        //    else
        //    {

        //        pathbase = "Attachment_Temp/";
        //    }
        //}
        //else if (ArticleType == "4")//专家团
        //{
        //    pathbase = "zhuanjiatuanGray/";

        //}
        //else
        //{
        //    int columnId = CookieCustom.GetValue("columnId") == "" ? 0 : Convert.ToInt32(CookieCustom.GetValue("columnId"));
        //    Bas_Column ColumnModel = (new Bas_ColumnBLL()).Get(columnId);
        //    if (ColumnModel != null && columnId != 0)
        //    {
        //        //pathbase = "~/Files/" + TextUtility.ClearHtml(ColumnModel.ColumnName) + "/ArticlePhoto/"; 
        //        pathbase = TextUtility.ClearHtml(ColumnModel.ColumnPathName) + "/ArticlePhoto/";
        //    }
        //    else
        //    {
        //        //pathbase = "~/Files/Attachment_Temp/";
        //        pathbase = "Attachment_Temp/";
        //    }
        //}
        if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Files/" + pathbase)))
        {
            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/" + pathbase));
        }
        //if (path == 1)
        //{
        //    pathbase = "upload/" ;                  

        //}else{
        //    pathbase = "upload1/";
        //}
        int water = Convert.ToInt32(up.getOtherInfo(context, "water"));


        info = up.upFile2(context, "~/Files/" + pathbase, filetype, size);                   //获取上传状态

        string title = up.getOtherInfo(context, "pictitle");                   //获取图片描述
        string oriName = up.getOtherInfo(context, "fileName");                //获取原始文件名

        //if (water == 1)//加水印
        //{
        //    UploadFile.AddShuiYinPic(HttpContext.Current.Server.MapPath(info["url"].ToString()), HttpContext.Current.Server.MapPath("~/Files/Attachment_Temp/" + oriName), HttpContext.Current.Server.MapPath("~/System/Resources/images/watermark.png"));
        //    IO.DelReadOnlyFile(HttpContext.Current.Server.MapPath(info["url"].ToString()));
        //    File.Move(HttpContext.Current.Server.MapPath("~/Files/Attachment_Temp/" + oriName), HttpContext.Current.Server.MapPath(info["url"].ToString()));
        //}
        HttpContext.Current.Response.Write("{'url':'" + info["url"].ToString().Substring(1) + "','title':'" + title + "','original':'" + oriName + "','state':'" + info["state"] + "'}");  //向浏览器返回数据json数据
        
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}