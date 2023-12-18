<%@ WebHandler Language="C#" Class="imageManager" %>
/**
 * Created by visual studio2010
 * User: xuheng
 * Date: 12-3-7
 * Time: 下午16:29
 * To change this template use File | Settings | File Templates.
 */
using System;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

public class imageManager : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string[] paths = { "upload", "upload1" }; //需要遍历的目录列表，最好使用缩略图地址，否则当网速慢时可能会造成严重的延时
        string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };                //文件允许格式

        //string action = context.Server.HtmlEncode(context.Request["action"]);
        //string pathbase = "";
        //string ArticleType = CookieCustom.GetValue("ArticleType");
        //ChineseCode china = new ChineseCode();//中文转换类
        //if (ArticleType == "2")
        //{
        //    int expertId = CookieCustom.GetValue("expertId") == "" ? 0 : Convert.ToInt32(CookieCustom.GetValue("expertId"));
        //    Bas_Expert ExpertModel = (new Bas_ExpertBLL()).Get(expertId);
        //    if (ExpertModel != null && expertId != 0)
        //    {
        //        pathbase = "~/Files/" + TextUtility.ClearHtml(china.GetSpell(ExpertModel.ExpertName)) + "/ArticlePhoto/";
        //    }
        //    else
        //    {

        //        pathbase = "~/Files/Attachment_Temp/";
        //    }
            
        //}
        //else if (ArticleType == "3")
        //{
        //    int expertApplicationId = CookieCustom.GetValue("expertApplicationId") == "" ? 0 : Convert.ToInt32(CookieCustom.GetValue("expertApplicationId"));
        //    Bas_ExpertApplication ExpertApplicationModel = (new Bas_ExpertApplicationBLL()).Get(expertApplicationId);
        //    if (ExpertApplicationModel != null && expertApplicationId != 0)
        //    {
        //        pathbase = "~/Files/" + TextUtility.ClearHtml(china.GetSpell(ExpertApplicationModel.ExpertName)) + "/ArticlePhoto/";
        //    }
        //    else
        //    {

        //        pathbase = "~/Files/Attachment_Temp/";
        //    }
        //}
        //else
        //{
        //    int columnId = CookieCustom.GetValue("columnId") == "" ? 0 : Convert.ToInt32(CookieCustom.GetValue("columnId"));
        //    Bas_Column ColumnModel = (new Bas_ColumnBLL()).Get(columnId);
        //    if (ColumnModel != null && columnId != 0)
        //    {
        //        pathbase = "~/Files/" + TextUtility.ClearHtml(ColumnModel.ColumnPathName) + "/ArticlePhoto/";
        //    }
        //    else
        //    {

        //        pathbase = "~/Files/Attachment_Temp/";
        //    }
        //}

        //if (action == "get")
        //{
        //    String str = String.Empty;

        //    //foreach (string path in paths)
        //    //{
        //    //    DirectoryInfo info = new DirectoryInfo(context.Server.MapPath(path));
        //    DirectoryInfo info = new DirectoryInfo(context.Server.MapPath(pathbase));
        //    //目录验证
        //    if (info.Exists)
        //    {

        //        //DirectoryInfo[] infoArr = info.GetDirectories();

        //        foreach (FileInfo fi in info.GetFiles())
        //        {
        //            if (Array.IndexOf(filetype, fi.Extension) != -1)
        //            {
        //                str += pathbase.Substring(1) + "/" + fi.Name + "ue_separate_ue";
        //            }
        //        }
        //        //}
        //    }
        //    //}

        //    context.Response.Write(str);
        //}
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}