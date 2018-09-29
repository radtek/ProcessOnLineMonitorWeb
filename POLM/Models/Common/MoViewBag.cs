using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyModels.Common
{
    public class MoViewBag
    {
        public string User { get; set; }
        public string Title { get; set; }
        public string jsonData { get; set; }
        public string jsonOut { get; set; }

        public string SignalR_ID { get; set; }

        public string Message { get; set; }
        public string MessageWarn { get; set; }
        public string imgFile { get; set; }
        public string PcName { get; set; }
        public string Project { get; set; }
        public string Model { get; set; }
        public string Para1 { get; set; }
        public string Para2 { get; set; }
        public string Para3 { get; set; }
        public string Para4 { get; set; }
        public string Para5 { get; set; }
        public string Para6 { get; set; }
        public string Para7 { get; set; }
        public string Para8 { get; set; }
        public string Para9 { get; set; }
        public string Para10 { get; set; }

        public string ParaRet1 { get; set; }
        public string ParaRet2 { get; set; }

        public List<string> List_ResultData { get; set; }
        public List<string> List_Track { get; set; }
        
        public MoViewBag()
        {
            User = "";
            Title = "";
            jsonData = "";
            jsonOut = "";
            Message = "";
            SignalR_ID = "";
            MessageWarn = "";
            PcName = "";
            Project = "";
            imgFile = "";
            Model = "";
            Para1 = "";
            Para2 = "";
            Para3 = "";
            Para4 = "";
            Para5 = "";
            Para6 = "";
            Para7 = "";
            Para8 = "";
            Para9 = "";
            Para10 = "";
            ParaRet1 = "";
            ParaRet2 = "";

            List_ResultData = new List<string>();
            List_Track = new List<string>();
        }

    }

    public class ViewBagAcnt
    {
        public string Model { get; set; }
        public string Title { get; set; }
        public string jsonUser { get; set; }
        public string jsonProject { get; set; }
        public string UserPw { get; set; }
        public string Message { get; set; }
        public string Project { get; set; }
        public string jsonDt { get; set; }
        public string MntModel { get; set; }
        public string Para1 { get; set; }
        public string Para2 { get; set; }
        public ViewBagAcnt()
        {
            Model = "";
            Title = "";
            jsonUser = "";
            UserPw = "";
            Message = "";
            jsonProject = "";
            Project = "";
            jsonDt = "";
            MntModel = "";
            Para1 = "";
            Para2 = "";
        }
    }

    public class TorRec
    {
        public string Project { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
        public string Time { get; set; }
        public string Model { get; set; }
        public string ScrewSN { get; set; }
        public string Station { get; set; }
        public string TorsionSpec { get; set; }
        public string TestVal1 { get; set; }
        public string TestVal2 { get; set; }
        public string TestVal3 { get; set; }
        public string Result { get; set; }
        public string CalibBy { get; set; }
        public string CheckedBy { get; set; }
        public string Meter_SN { get; set; }
        public string Meter_Type { get; set; }
        public string Meter_DateCalib { get; set; }
        public string Meter_DateLose { get; set; }
        public string Remark { get; set; }
        public TorRec()
        {
            Project = "";
            Date = "";
            Shift = "";
            Time = "";
            Model = "";
            ScrewSN = "";
            Station = "";
            TestVal1 = "";
            TestVal2 = "";
            TestVal3 = "";
            Result = "";
            CalibBy = "";
            CheckedBy = "";
            Meter_SN = "";
            Meter_Type = "";
            Meter_DateCalib = "";
            Meter_DateLose = "";
            Remark = "";
            TorsionSpec = "";
        }
    }


    public class StencilRec
    {
        public string Project { get; set; }
        public string Date { get; set; }
        public string Slot { get; set; }
        public string EN { get; set; }
        public string Time { get; set; }
        public string Chk_A { get; set; }
        public string Chk_B { get; set; }
        public string Chk_C { get; set; }
        public string Chk_D { get; set; }
        public string Chk_E { get; set; }
        public string Chk_F { get; set; }
        public string Res_A { get; set; }
        public string Res_B { get; set; }
        public string Res_C { get; set; }
        public string Res_D { get; set; }
        public string Remark { get; set; }
        public StencilRec()
        {
            Project = "";
            Date = "";
            Slot = "";
            Time = "";
            EN = "";
            Chk_A = "";
            Chk_B = "";
            Chk_C = "";
            Chk_D = "";
            Chk_E = "";
            Chk_F = "";
            Res_A = "";
            Res_B = "";
            Res_C = "";
            Res_D = "";
            Remark = "";
        }
    }

    public class ChartOptions
    {
        public int LineQty { get; set; }
        public int nInterval { get; set; }
        public int nMin { get; set; }
        public int nMax { get; set; }

        public string Line1_Name { get; set; }
        public string Line2_Name { get; set; }
        public string Line3_Name { get; set; }
        public string Line4_Name { get; set; }

        public ChartOptions()
        {
            LineQty = 0;
            nInterval = 0;
            nMax = 0;
            nMin = 0;
            Line1_Name = "";
            Line2_Name = "";
            Line3_Name = "";
            Line4_Name = "";
        }
    }

}