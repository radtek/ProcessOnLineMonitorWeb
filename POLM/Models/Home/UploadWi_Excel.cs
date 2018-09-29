using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace POLM.Models.Home
{
    public class xlsSc_Title
    {
        public int Title_Row { get; set; }

        public int Title_Column_1 { get; set; }
        public int Title_Column_2 { get; set; }
        public int Title_Column_3 { get; set; }
        public int Title_Column_4 { get; set; }
        public int Title_Column_5 { get; set; }
        public int Title_Column_6 { get; set; }


        public string Title_Mark_1 { get; set; }
        /// <summary>
        /// EtQ Doc#
        /// </summary>
        public string Title_Mark_2 { get; set; }
        /// <summary>
        /// EtQ Rev
        /// </summary>
        public string Title_Mark_3 { get; set; }
        /// <summary>
        /// ^Eff[.,\w\s]*Date$
        /// </summary>
        public string Title_Mark_4 { get; set; }
        public string Title_Mark_5 { get; set; }
        public string Title_Mark_6 { get; set; }

        public string Etq_Doc { get; set; }
        public string Etq_Rev { get; set; }

        public string Etq_EffDate { get; set; }

        public string ODC { get; set; }

        public xlsSc_Title()
        {
            Title_Row = -1;
            Title_Column_1 = 0;
            Title_Column_2 = 0;
            Title_Column_3 = 0;
            Title_Column_4 = 0;
            Title_Column_5 = 0;
            Title_Column_5 = 6;

            Title_Mark_1 = @"^Page\s*Cover+$|^Cover\s*Page+$"; 
            Title_Mark_2 = @"^EtQ\s*Doc\s*#$";
            Title_Mark_3 = @"^EtQ\s*Rev$";
            Title_Mark_4 = @"^Eff[.,\w\s]*Date$";
            Title_Mark_5 = @"";
            Title_Mark_6 = @"";

            Etq_Doc = "";
            Etq_Rev = "";
            Etq_EffDate = "";
            ODC = "";
        }
    }

    public class xlsColMark
    {
        /// <summary>
        /// 数据开始行
        /// </summary>
        public int Row_Data { get; set; }
        /// <summary>
        /// 有效的最右边的列
        /// </summary>
        public int Col_LastRight { get; set; }
        public int Title_Row { get; set; }
        /// <summary>
        /// @"^Item$"
        /// </summary>
        public int Tit_Col_01 { get; set; }
        /// <summary>
        /// @"^Model\s*+Name$"
        /// </summary>
        public int Tit_Col_02 { get; set; }
        /// <summary>
        /// @"^Program[a-z]*\s*Name$"
        /// </summary>
        public int Tit_Col_03 { get; set; }
        /// <summary>
        /// @"Board_Width"
        /// </summary>
        public int Tit_Col_04 { get; set; }
        /// <summary>
        /// @"Conveyor_speed"
        /// </summary>
        public int Tit_Col_05 { get; set; }
        /// <summary>
        /// @"Nozzle_Speed"
        /// </summary>
        public int Tit_Col_06 { get; set; }
        /// <summary>
        /// "Flux_Volume"
        /// </summary>
        public int Tit_Col_07 { get; set; }
        /// <summary>
        /// "Nozzle_Spray_Width"
        /// </summary>
        public int Tit_Col_08 { get; set; }
        /// <summary>
        /// "_Power"
        /// </summary>
        public int Tit_Col_09 { get; set; }
        /// <summary>
        /// "_Pressure"
        /// </summary>
        public int Tit_Col_10 { get; set; }
        /// <summary>
        /// "Bi-direction_Model"
        /// </summary>
        public int Tit_Col_11 { get; set; }
        /// <summary>
        /// @"Lower_p\s*/\s*h1"
        /// </summary>
        public int Tit_Col_12 { get; set; }
        /// <summary>
        /// @"Lower_p\s*/\s*h2"
        /// </summary>
        public int Tit_Col_13 { get; set; }
        /// <summary>
        /// @"Lower_p\s*/\s*h3"
        /// </summary>
        public int Tit_Col_14 { get; set; }
        /// <summary>
        /// @"Upper_p\s*/\s*h1"
        /// </summary>
        public int Tit_Col_15 { get; set; }
        /// <summary>
        /// @"Upper_p\s*/\s*h2"
        /// </summary>
        public int Tit_Col_16 { get; set; }
        /// <summary>
        /// @"Upper_p\s*/\s*h3"
        /// </summary>
        public int Tit_Col_17 { get; set; }
        /// <summary>
        /// Solder_TEMP
        /// </summary>
        public int Tit_Col_18 { get; set; }
        /// <summary>
        /// Contour_WAVE
        /// </summary>
        public int Tit_Col_19 { get; set; }
        /// <summary>
        /// Lead_Clearance
        /// </summary>
        public int Tit_Col_20 { get; set; }
        /// <summary>
        /// Converyor=> _Speed
        /// </summary>
        public int Tit_Col_21 { get; set; }
        /// <summary>
        /// Converyor=> _Width
        /// </summary>
        public int Tit_Col_22 { get; set; }
        /// <summary>
        /// Nitrogen
        /// </summary>
        public int Tit_Col_23 { get; set; }
        public int Tit_Col_24 { get; set; }
        public int Tit_Col_25 { get; set; }

        #region Mark for column
        /// <summary>
        /// @"^Item$"
        /// </summary>
        public string Col_Mark_01 { get; set; }
        /// <summary>
        /// @"^Model\s*+Name$"
        /// </summary>
        public string Col_Mark_02 { get; set; }
        /// <summary>
        ///  @"^Programme\s*+Name$"
        /// </summary>
        public string Col_Mark_03 { get; set; }
        /// <summary>
        /// @"Board_Width"
        /// </summary>
        public string Col_Mark_04 { get; set; }
        /// <summary>
        /// @"Conveyor_speed"
        /// </summary>
        public string Col_Mark_05 { get; set; }
        /// <summary>
        /// @"Nozzle_Speed"
        /// </summary>
        public string Col_Mark_06 { get; set; }
        /// <summary>
        /// "Flux_Volume"
        /// </summary>
        public string Col_Mark_07 { get; set; }
        /// <summary>
        /// "Nozzle_Spray_Width"
        /// </summary>
        public string Col_Mark_08 { get; set; }
        /// <summary>
        /// "_Power"
        /// </summary>
        public string Col_Mark_09 { get; set; }
        /// <summary>
        /// "_Pressure"
        /// </summary>
        public string Col_Mark_10 { get; set; }
        /// <summary>
        /// "Bi-direction_Model"
        /// </summary>
        public string Col_Mark_11 { get; set; }
        /// <summary>
        /// @"Lower_p\s*/\s*h1"
        /// </summary>
        public string Col_Mark_12 { get; set; }
        /// <summary>
        /// @"Lower_p\s*/\s*h2"
        /// </summary>
        public string Col_Mark_13 { get; set; }
        /// <summary>
        /// @"Lower_p\s*/\s*h3"
        /// </summary>
        public string Col_Mark_14 { get; set; }
        /// <summary>
        /// @"Upper_p\s*/\s*h1"
        /// </summary>
        public string Col_Mark_15 { get; set; }
        /// <summary>
        ///  @"Upper_p\s*/\s*h2"
        /// </summary>
        public string Col_Mark_16 { get; set; }
        /// <summary>
        /// @"Upper_p\s*/\s*h3"
        /// </summary>
        public string Col_Mark_17 { get; set; }
        /// <summary>
        /// "Solder_TEMP"
        /// </summary>
        public string Col_Mark_18 { get; set; }
        /// <summary>
        /// "Contour_WAVE"
        /// </summary>
        public string Col_Mark_19 { get; set; }
        /// <summary>
        /// @"Lead_Clearance"
        /// </summary>
        public string Col_Mark_20 { get; set; }
        /// <summary>
        /// @"_Speed"
        /// </summary>
        public string Col_Mark_21 { get; set; }
        /// <summary>
        /// @"_Width"
        /// </summary>
        public string Col_Mark_22 { get; set; }
        /// <summary>
        /// @"Nitrogen"
        /// </summary>
        public string Col_Mark_23 { get; set; }
        public string Col_Mark_24 { get; set; }
        public string Col_Mark_25 { get; set; }
        #endregion

        public xlsColMark()
        {
            #region initilize value
            Row_Data = 0;
            Title_Row = -1;
            Tit_Col_01 = 0;
            Tit_Col_02 = 0;
            Tit_Col_03 = 0;
            Tit_Col_04 = 0;
            Tit_Col_05 = 0;
            Tit_Col_06 = 0;
            Tit_Col_07 = 0;
            Tit_Col_08 = 0;
            Tit_Col_09 = 0;
            Tit_Col_10 = 0;
            Tit_Col_12 = 0;
            Tit_Col_13 = 0;
            Tit_Col_14 = 0;
            Tit_Col_15 = 0;
            Tit_Col_16 = 0;
            Tit_Col_17 = 0;
            Tit_Col_18 = 0;
            Tit_Col_19 = 0;
            Tit_Col_20 = 0;
            Tit_Col_21 = 0;
            Tit_Col_22 = 0;
            Tit_Col_23 = 0;
            Tit_Col_24 = 0;
            Tit_Col_25 = 0;


            Col_Mark_01 = @"^Item$";
            Col_Mark_02 = @"^Model\s*Name$";
            Col_Mark_03 = @"^Program[a-z]*\s*Name$";
            Col_Mark_04 = @"Board_Width";
            Col_Mark_05 = @"Conveyor_speed";
            Col_Mark_06 = @"Nozzle_Speed";
            Col_Mark_07 = "Flux_Volume";
            Col_Mark_08 = "Nozzle_Spray_Width";
            Col_Mark_09 = "_Power";
            Col_Mark_10 = "_Pressure";
            Col_Mark_11 = "Bi-direction_Model";
            Col_Mark_12 = @"Lower_p\s*/\s*h1";
            Col_Mark_13 = @"Lower_p\s*/\s*h2";
            Col_Mark_14 = @"Lower_p\s*/\s*h3";
            Col_Mark_15 = @"Upper_p\s*/\s*h1";
            Col_Mark_16 = @"Upper_p\s*/\s*h2";
            Col_Mark_17 = @"Upper_p\s*/\s*h3";
            Col_Mark_18 = "Solder_TEMP";
            Col_Mark_19 = "Contour_WAVE";
            Col_Mark_20 = @"Lead_Clearance";
            Col_Mark_21 = @"_Speed";
            Col_Mark_22 = @"_Width";
            Col_Mark_23 = @"Nitrogen";
            Col_Mark_24 = @"";
            Col_Mark_25 = @""; 
            #endregion
        }
    }

    public class Wave_Xls
    {
        #region Wave_Xls
        public string ModelName = "";
        public string ProgramName = "";
        public float Flux_Bd_Wid = -1.0F;
        public float Flux_Bd_Wid_Min = -1.0F;
        public float Flux_Bd_Wid_Max = -1.0F;

        public float Flux_ConvSpd = -1.0F;
        public float Flux_ConvSpd_Min = -1.0F;
        public float Flux_ConvSpd_Max = -1.0F;

        public float Flux_NozSpd = -1.0F;
        public float Flux_NozSpd_Min = -1.0F;
        public float Flux_NozSpd_Max = -1.0F;

        public float Flux_Volume = -1.0F;
        public float Flux_Volume_Min = -1.0F;
        public float Flux_Volume_Max = -1.0F;

        public float Flux_NozSpray = -1.0F;
        public float Flux_NozSpray_Min = -1.0F;
        public float Flux_NozSpray_Max = -1.0F;

        public float Power = -1.0F;
        public float Power_Min = -1.0F;
        public float Power_Max = -1.0F;

        public float Pressure = -1.0F;
        public float Pressure_Min = -1.0F;
        public float Pressure_Max = -1.0F;

        public string Bi_direction_Model = "";

        public float Lower_ph1 = -1.0F;
        public float Lower_ph1_Min = -1.0F;
        public float Lower_ph1_Max = -1.0F;

        public float Lower_ph2 = -1.0F;
        public float Lower_ph2_Min = -1.0F;
        public float Lower_ph2_Max = -1.0F;

        public float Lower_ph3 = -1.0F;
        public float Lower_ph3_Min = -1.0F;
        public float Lower_ph3_Max = -1.0F;

        public float Upper_ph1 = -1.0F;
        public float Upper_ph1_Min = -1.0F;
        public float Upper_ph1_Max = -1.0F;

        public float Upper_ph2 = -1.0F;
        public float Upper_ph2_Min = -1.0F;
        public float Upper_ph2_Max = -1.0F;

        public float Upper_ph3 = -1.0F;
        public float Upper_ph3_Min = -1.0F;
        public float Upper_ph3_Max = -1.0F;

        public float SP_Temp = -1.0F;
        public float SP_Temp_Min = -1.0F;
        public float SP_Temp_Max = -1.0F;

        public float SP_ConWave = -1.0F;
        public float SP_ConWave_Min = -1.0F;
        public float SP_ConWave_Max = -1.0F;

        public float Lead_Clear = -1.0F;
        public float Lead_Clear_Min = -1.0F;
        public float Lead_Clear_Max = -1.0F;
    

        public float Conv_Speed = -1.0F;
        public float Conv_Speed_Min = -1.0F;
        public float Conv_Speed_Max = -1.0F;

        public float Conv_Width = -1.0F;
        public float Conv_Width_Min = -1.0F;
        public float Conv_Width_Max = -1.0F;

        public string Other_Ni = "";

        public string Remark = "";
        #endregion

    }

    public class UploadWi_Excel
    {
        public List<string> List_Track = new List<string>();

        #region Over page

        public string WI_GetOverPage(string sFileName, xlsSc_Title m_XlsTitle)
        {
            string ErrMsg = "";
            try
            {
                List_Track.Add("  ");

                if (File.Exists(sFileName))
                {
                    #region Read file
                    List_Track.Add("----Process file = " + sFileName);

                    HSSFWorkbook hssfworkbook;
                    using (FileStream file = new FileStream(sFileName, FileMode.Open, FileAccess.Read))
                    {
                        hssfworkbook = new HSSFWorkbook(file);
                    }

                    #region 是否找到 Page Cover
                    List_Track.Add("----Find index for cover page = " + m_XlsTitle.Title_Mark_1);

                    int CoverPage_Index = -1;
                    for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
                    {
                        ISheet wsItem = hssfworkbook.GetSheetAt(i);
                        string sName = wsItem.SheetName.Trim();
                        if (Regex.IsMatch(sName, m_XlsTitle.Title_Mark_1, RegexOptions.IgnoreCase))
                        {
                            CoverPage_Index = i;
                            break;
                        }
                    }
                    #endregion
                    if (CoverPage_Index >= 0)
                    {
                        List_Track.Add("----CoverPage_Index = " + CoverPage_Index);

                        ISheet sheet = hssfworkbook.GetSheetAt(CoverPage_Index);
                        //System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                        for (int iRow = 0; iRow < 10; iRow++)
                        {
                            IRow row = sheet.GetRow(iRow);
                            for (int i = 0; i < row.LastCellNum; i++)
                            {
                                ICell cell = row.GetCell(i);
                                if (cell != null)
                                {
                                    cell.SetCellType(CellType.String);
                                    string sValue = cell.StringCellValue;
                                    if (Regex.IsMatch(sValue, m_XlsTitle.Title_Mark_2, RegexOptions.IgnoreCase)) //EtQ Doc#
                                    {
                                        #region EQT Doc# 
                                        int nC_EtqVal = i;
                                        if (cell.IsMergedCell)
                                        {
                                            //CellRangeAddress region = sheet.GetMergedRegion(i);
                                            //nC_EtqVal = nC_EtqVal + region.NumberOfCells;
                                            int nNumCells = MyPoiXls.POI_Excel.GetRegionNumber(sheet, cell.RowIndex, cell.ColumnIndex);
                                            nC_EtqVal = nC_EtqVal + nNumCells;
                                        }

                                        #region 获取 ETQ 的 值
                                        string sValue_etqVal = "";
                                        for (int k = nC_EtqVal; k < nC_EtqVal + 13; k++)
                                        {
                                            ICell cell_etqVal = row.GetCell(k);
                                            if (cell_etqVal != null)
                                            {
                                                string sValueThisCell = cell_etqVal.StringCellValue.Trim();
                                                if (sValueThisCell != "")
                                                {
                                                    sValue_etqVal = sValueThisCell;
                                                    break;
                                                }
                                            }
                                        }
                                        m_XlsTitle.Etq_Doc = sValue_etqVal;
                                        #endregion

                                        #endregion
                                    }
                                    else if (Regex.IsMatch(sValue, m_XlsTitle.Title_Mark_3, RegexOptions.IgnoreCase)) //EtQ Rev
                                    {
                                        #region EQT Rev
                                        int nC_EtqRev = i;
                                        if (cell.IsMergedCell)
                                        {
                                            //CellRangeAddress region = sheet.GetMergedRegion(i);
                                            //nC_EtqRev = nC_EtqRev + region.NumberOfCells;

                                            int nNumCells = MyPoiXls.POI_Excel.GetRegionNumber(sheet, cell.RowIndex, cell.ColumnIndex);
                                            nC_EtqRev = nC_EtqRev + nNumCells;

                                        }
                                        #region 获取 ETQ Rev 的 值
                                        string sValue_etqRev = "";
                                        for (int k = nC_EtqRev; k < nC_EtqRev + 13; k++)
                                        {
                                            ICell cell_etqVal = row.GetCell(k);
                                            if (cell_etqVal != null)
                                            {
                                                string sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell_etqVal).Trim();
                                                if (sValueThisCell != "")
                                                {
                                                    sValue_etqRev = sValueThisCell;
                                                    break;
                                                }
                                            }
                                        }
                                        m_XlsTitle.Etq_Rev = sValue_etqRev;
                                        #endregion
                                        #endregion

                                        List_Track.Add("----" + ErrMsg);
                                        List_Track.Add("----ETQ Doc = " + m_XlsTitle.Etq_Doc);
                                        List_Track.Add("----ETQ Rev = " + m_XlsTitle.Etq_Rev);
                                    }
                                    else if (Regex.IsMatch(sValue, m_XlsTitle.Title_Mark_4, RegexOptions.IgnoreCase)) //EtQ Eff. Date
                                    {
                                        #region Eff. Date
                                        int nC_EtqVal = i;
                                        if (cell.IsMergedCell)
                                        {
                                            int nNumCells = MyPoiXls.POI_Excel.GetRegionNumber(sheet, cell.RowIndex, cell.ColumnIndex);
                                            nC_EtqVal = nC_EtqVal + nNumCells;
                                        }

                                        #region 获取 ETQ 的 值
                                        string sValue_etqVal = "";
                                        for (int k = nC_EtqVal; k < nC_EtqVal + 13; k++)
                                        {
                                            ICell cell_etqVal = row.GetCell(k);
                                            if (cell_etqVal != null)
                                            {
                                                if (cell_etqVal.CellType == CellType.Numeric)
                                                {
                                                    if (HSSFDateUtil.IsCellDateFormatted(cell_etqVal))
                                                    {
                                                        sValue_etqVal = cell_etqVal.DateCellValue.ToString("yyyy-MM-dd");
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    string sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell_etqVal).Trim();
                                                    //string sValueThisCell = cell_etqVal.StringCellValue.Trim();
                                                    if (sValueThisCell != "")
                                                    {
                                                        sValue_etqVal = sValueThisCell;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        m_XlsTitle.Etq_EffDate = sValue_etqVal;
                                        List_Track.Add("----ETQ Eff. Date = " + m_XlsTitle.Etq_EffDate);

                                        return ErrMsg;

                                        #endregion
                                        #endregion
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        ErrMsg = "Fail to get sheet -- Page Cover";
                        List_Track.Add("----" + ErrMsg);
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "Fail to get file";
                    List_Track.Add("----" + ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return ErrMsg;
        }

        public string WI_GetOverPage_xlsx(string sFileName, xlsSc_Title m_XlsTitle)
        {
            string ErrMsg = "";
            try
            {
                if (File.Exists(sFileName))
                {
                    #region Read file
                    List_Track.Add("----Process file = " + sFileName);

                    XSSFWorkbook xssfworkbook;
                    using (FileStream file = new FileStream(sFileName, FileMode.Open, FileAccess.Read))
                    {
                        xssfworkbook = new XSSFWorkbook(file);
                    }

                    #region 是否找到 Page Cover
                    List_Track.Add("----Find index for cover page = " + m_XlsTitle.Title_Mark_1);

                    int CoverPage_Index = -1;
                    for (int i = 0; i < xssfworkbook.NumberOfSheets; i++)
                    {
                        ISheet wsItem = xssfworkbook.GetSheetAt(i);
                        string sName = wsItem.SheetName.Trim();
                        if (Regex.IsMatch(sName, m_XlsTitle.Title_Mark_1, RegexOptions.IgnoreCase))
                        {
                            CoverPage_Index = i;
                            break;
                        }
                    }
                    #endregion
                    if (CoverPage_Index >= 0)
                    {
                        List_Track.Add("----CoverPage_Index = " + CoverPage_Index);

                        ISheet sheet = xssfworkbook.GetSheetAt(CoverPage_Index);
                        //System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                        for (int iRow = 0; iRow < 10; iRow++)
                        {
                            IRow row = sheet.GetRow(iRow);
                            for (int i = 0; i < row.LastCellNum; i++)
                            {
                                ICell cell = row.GetCell(i);
                                if (cell != null)
                                {
                                    cell.SetCellType(CellType.String);
                                    string sValue = cell.StringCellValue;
                                    if (Regex.IsMatch(sValue, m_XlsTitle.Title_Mark_2, RegexOptions.IgnoreCase)) //EtQ Doc#
                                    {
                                        #region EQT Doc# 
                                        int nC_EtqVal = i;
                                        if (cell.IsMergedCell)
                                        {
                                            //CellRangeAddress region = sheet.GetMergedRegion(i);
                                            //nC_EtqVal = nC_EtqVal + region.NumberOfCells;
                                            int nNumCells = MyPoiXls.POI_Excel.GetRegionNumber(sheet, cell.RowIndex, cell.ColumnIndex);
                                            nC_EtqVal = nC_EtqVal + nNumCells;
                                        }

                                        #region 获取 ETQ 的 值
                                        string sValue_etqVal = "";
                                        for (int k = nC_EtqVal; k < nC_EtqVal + 13; k++)
                                        {
                                            ICell cell_etqVal = row.GetCell(k);
                                            if (cell_etqVal != null)
                                            {
                                                string sValueThisCell = cell_etqVal.StringCellValue.Trim();
                                                if (sValueThisCell != "")
                                                {
                                                    sValue_etqVal = sValueThisCell;
                                                    break;
                                                }
                                            }
                                        }
                                        m_XlsTitle.Etq_Doc = sValue_etqVal;
                                        #endregion

                                        #endregion
                                    }
                                    else if (Regex.IsMatch(sValue, m_XlsTitle.Title_Mark_3, RegexOptions.IgnoreCase)) //EtQ Rev
                                    {
                                        #region EQT Rev
                                        int nC_EtqRev = i;
                                        if (cell.IsMergedCell)
                                        {
                                            //CellRangeAddress region = sheet.GetMergedRegion(i);
                                            //nC_EtqRev = nC_EtqRev + region.NumberOfCells;
                                            int nNumCells = MyPoiXls.POI_Excel.GetRegionNumber(sheet, cell.RowIndex, cell.ColumnIndex);
                                            nC_EtqRev = nC_EtqRev + nNumCells;
                                        }
                                        #region 获取 ETQ Rev 的 值
                                        string sValue_etqRev = "";
                                        for (int k = nC_EtqRev; k < nC_EtqRev + 13; k++)
                                        {
                                            ICell cell_etqVal = row.GetCell(k);
                                            if (cell_etqVal != null)
                                            {
                                                string sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell_etqVal).Trim();
                                                if (sValueThisCell != "")
                                                {
                                                    sValue_etqRev = sValueThisCell;
                                                    break;
                                                }
                                            }
                                        }
                                        m_XlsTitle.Etq_Rev = sValue_etqRev;
                                        #endregion
                                        #endregion

                                        List_Track.Add("----ETQ Doc = " + m_XlsTitle.Etq_Doc);
                                        List_Track.Add("----ETQ Rev = " + m_XlsTitle.Etq_Rev);
                                    }
                                    else if (Regex.IsMatch(sValue, m_XlsTitle.Title_Mark_4, RegexOptions.IgnoreCase)) //EtQ Eff. Date
                                    {
                                        #region Eff. Date
                                        int nC_EtqVal = i;
                                        if (cell.IsMergedCell)
                                        {
                                            int nNumCells = MyPoiXls.POI_Excel.GetRegionNumber(sheet, cell.RowIndex, cell.ColumnIndex);
                                            nC_EtqVal = nC_EtqVal + nNumCells;
                                        }

                                        #region 获取 ETQ 的 值
                                        string sValue_etqVal = "";
                                        for (int k = nC_EtqVal; k < nC_EtqVal + 13; k++)
                                        {
                                            ICell cell_etqVal = row.GetCell(k);
                                            if (cell_etqVal != null)
                                            {
                                                if (cell_etqVal.CellType == CellType.Numeric)
                                                {
                                                    if (HSSFDateUtil.IsCellDateFormatted(cell_etqVal))
                                                    {
                                                        sValue_etqVal = cell_etqVal.DateCellValue.ToString("yyyy-MM-dd");
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    string sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell_etqVal).Trim();
                                                    //string sValueThisCell = cell_etqVal.StringCellValue.Trim();
                                                    if (sValueThisCell != "")
                                                    {
                                                        sValue_etqVal = sValueThisCell;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        m_XlsTitle.Etq_EffDate = sValue_etqVal;
                                        List_Track.Add("----ETQ Eff. Date = " + m_XlsTitle.Etq_EffDate);

                                        return ErrMsg;

                                        #endregion
                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrMsg = "Fail to get sheet -- Page Cover";
                        List_Track.Add("----" + ErrMsg);
                    }
                    #endregion
                }
                else
                {
                    ErrMsg = "Fail to get file";
                    List_Track.Add("----" + ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return ErrMsg;
        }

        private string getCellValue(ICell cell)
        {
            var dataFormatter = new DataFormatter(CultureInfo.CurrentCulture);

            // If this is not part of a merge cell,
            // just get this cell's value like normal.
            if (!cell.IsMergedCell)
            {
                return dataFormatter.FormatCellValue(cell);
            }

            // Otherwise, we need to find the value of this merged cell.
            else
            {
                // Get current sheet.
                var currentSheet = cell.Sheet;

                // Loop through all merge regions in this sheet.
                for (int i = 0; i < currentSheet.NumMergedRegions; i++)
                {
                    var mergeRegion = currentSheet.GetMergedRegion(i);

                    // If this merged region contains this cell.
                    if (mergeRegion.FirstRow <= cell.RowIndex && cell.RowIndex <= mergeRegion.LastRow &&
                        mergeRegion.FirstColumn <= cell.ColumnIndex && cell.ColumnIndex <= mergeRegion.LastColumn)
                    {
                        // Find the top-most and left-most cell in this region.
                        var firstRegionCell = currentSheet.GetRow(mergeRegion.FirstRow)
                                                .GetCell(mergeRegion.FirstColumn);

                        // And return its value.
                        return dataFormatter.FormatCellValue(firstRegionCell);
                    }
                }
                // This should never happen.
                throw new Exception("Cannot find this cell in any merged region");
            }
        }

        /**    
        * 获取单元格的值    
        * @param cell    
        * @return    
        */
        //public String getCellValue2(ICell cell)
        //{

        //    if (cell == null) return "";

        //    if (cell.getCellType() == ICell.CELL_TYPE_STRING)
        //    {

        //        return cell.getStringCellValue();

        //    }
        //    else if (cell.getCellType() == Cell.CELL_TYPE_BOOLEAN)
        //    {

        //        return String.valueOf(cell.getBooleanCellValue());

        //    }
        //    else if (cell.getCellType() == Cell.CELL_TYPE_FORMULA)
        //    {

        //        return cell.getCellFormula();

        //    }
        //    else if (cell.getCellType() == Cell.CELL_TYPE_NUMERIC)
        //    {

        //        return String.valueOf(cell.getNumericCellValue());

        //    }
        //    return "";
        //}

        //private int findRow(HSSFSheet sheet, String cellContent)
        //{
        //    string ErrMsg = "";
        //    try
        //    {
        //        //This is the method to find the row number
        //        for (Row row : sheet)
        //        {
        //            for (Cell cell : row)
        //            {
        //                if (cell.getCellType() == Cell.CELL_TYPE_STRING)
        //                {
        //                    if (cell.getRichStringCellValue().getString().trim().equals(cellContent))
        //                    {
        //                        return row.getRowNum();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
        //    }
        //    return -1;
        //}


        /**    
* 获取合并单元格的值    
* @param sheet    
* @param row    
* @param column    
* @return    
*/
        #endregion

        public List<string> WI_GetContent_SheetList(string sFileFull, ref string ErrMsg)
        {
            List<string> List_WS_Name = new List<string>();
            ErrMsg = "";
            try
            {
                //object hWorkBook = null;

                string sFileExt = Path.GetExtension(sFileFull);
                if (sFileExt == ".xls")
                {
                    HSSFWorkbook hWorkBook;
                    using (FileStream file = new FileStream(sFileFull, FileMode.Open, FileAccess.Read))
                    {
                        hWorkBook = new HSSFWorkbook(file);
                    }

                    for (int i = 0; i < hWorkBook.NumberOfSheets; i++)
                    {
                        ISheet wsItem = hWorkBook.GetSheetAt(i);
                        string sName = wsItem.SheetName.Trim();
                        if (!List_WS_Name.Contains(sName))
                        {
                            List_WS_Name.Add(sName);
                        }
                    }

                }
                else if (sFileExt == ".xlsx")
                {
                    XSSFWorkbook hWorkBook;
                    using (FileStream file = new FileStream(sFileFull, FileMode.Open, FileAccess.Read))
                    {
                        hWorkBook = new XSSFWorkbook(file);
                    }
                    for (int i = 0; i < hWorkBook.NumberOfSheets; i++)
                    {
                        ISheet wsItem = hWorkBook.GetSheetAt(i);
                        string sName = wsItem.SheetName.Trim();
                        if (!List_WS_Name.Contains(sName))
                        {
                            List_WS_Name.Add(sName);
                        }
                    }
                }

                //HSSFWorkbook hssfworkbook;
                //XSSFWorkbook xssfworkbook;

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return List_WS_Name;
        }

        public DataTable WI_GetContent_Wave(string sFileFull, string WS_Name, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                string xlsExt = Path.GetExtension(sFileFull).ToLower();

                ISheet SheetRd = null;
                if (xlsExt == ".xls")
                {
                    HSSFWorkbook fWorkBook;
                    using (FileStream file = new FileStream(sFileFull, FileMode.Open, FileAccess.Read))
                    {
                        fWorkBook = new HSSFWorkbook(file);
                    }
                    SheetRd = fWorkBook.GetSheet(WS_Name);
                }
                else if( xlsExt == ".xlsx")
                {
                    XSSFWorkbook fWorkBook;
                    using (FileStream file = new FileStream(sFileFull, FileMode.Open, FileAccess.Read))
                    {
                        fWorkBook = new XSSFWorkbook(file);
                    }
                    SheetRd = fWorkBook.GetSheet(WS_Name);
                }
                if (SheetRd != null)
                {
                    DataTable dtWave = WI_GetContent_Wave_xls(SheetRd, ref ErrMsg);

                    return dtWave;
                }
                else
                {
                    ErrMsg = "Fail to get ISheet";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }

        public DataTable  WI_GetContent_Wave_xls(ISheet SheetRd, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                xlsColMark m_XlsMark = Get_Column_ByTitle(SheetRd, ref ErrMsg);
                if (ErrMsg.IndexOf("Success") == 0)
                {
                    DataTable dtWave = CreateNewDataTable_Wave();
                    if (m_XlsMark.Tit_Col_02 > 0 && m_XlsMark.Tit_Col_03 > 0 )
                    {
                        #region read excel contents
                        int nRw_Data = m_XlsMark.Row_Data;

                        for (int nRw = nRw_Data; nRw < SheetRd.LastRowNum; nRw++)
                        {
                            IRow rowDt = SheetRd.GetRow(nRw);
                            string ErrMsg_Row = "";
                            Wave_Xls waveXls = Get_Para_RowValue(rowDt, m_XlsMark, ref ErrMsg_Row);
                            if (waveXls == null)
                            {
                                continue;
                            }
                            if(waveXls.ProgramName == "RH - Setting 26")
                            {
                                waveXls.ProgramName = "RH - Setting 26";
                            }
                            #region add row data into datatable
                            DataRow dr = dtWave.NewRow();
                            dr["ModelName"] = waveXls.ModelName;
                            dr["ProgName"] = waveXls.ProgramName;
                            dr["Flux_BdWid"] = waveXls.Flux_Bd_Wid == -1 ? "NA" : waveXls.Flux_Bd_Wid.ToString();
                            dr["Flux_BdWid_Max"] = waveXls.Flux_Bd_Wid_Max == -1 ? "NA" : waveXls.Flux_Bd_Wid_Max.ToString();
                            dr["Flux_BdWid_Min"] = waveXls.Flux_Bd_Wid_Min == -1 ? "NA" : waveXls.Flux_Bd_Wid_Min.ToString();

                            dr["Flux_ConvSpd"] = waveXls.Flux_ConvSpd == -1 ? "NA" : waveXls.Flux_ConvSpd.ToString();
                            dr["Flux_ConvSpd_Max"] = waveXls.Flux_ConvSpd_Max == -1 ? "NA" : waveXls.Flux_ConvSpd_Max.ToString();
                            dr["Flux_ConvSpd_Min"] = waveXls.Flux_ConvSpd_Min == -1 ? "NA" : waveXls.Flux_ConvSpd_Min.ToString();

                            dr["Flux_NozSpd"] = waveXls.Flux_NozSpd == -1 ? "NA" : waveXls.Flux_NozSpd.ToString();
                            dr["Flux_NozSpd_Max"] = waveXls.Flux_NozSpd_Max == -1 ? "NA" : waveXls.Flux_NozSpd_Max.ToString();
                            dr["Flux_NozSpd_Min"] = waveXls.Flux_NozSpd_Min == -1 ? "NA" : waveXls.Flux_NozSpd_Min.ToString();

                            dr["Flux_Volumn"] = waveXls.Flux_Volume == -1 ? "NA" : waveXls.Flux_Volume.ToString();
                            dr["Flux_Volumn_Max"] = waveXls.Flux_Volume_Max == -1 ? "NA" : waveXls.Flux_Volume_Max.ToString();
                            dr["Flux_Volumn_Min"] = waveXls.Flux_Volume_Min == -1 ? "NA" : waveXls.Flux_Volume_Min.ToString();

                            dr["Flux_NozSpray"] = waveXls.Flux_NozSpray == -1 ? "NA" : waveXls.Flux_NozSpray.ToString();
                            dr["Flux_NozSpray_Max"] = waveXls.Flux_NozSpray_Max == -1 ? "NA" : waveXls.Flux_NozSpray_Max.ToString();
                            dr["Flux_NozSpray_Min"] = waveXls.Flux_NozSpray_Min == -1 ? "NA" : waveXls.Flux_NozSpray_Min.ToString();

                            dr["Flux_Power"] = waveXls.Power == -1 ? "NA" : waveXls.Power.ToString();
                            dr["Flux_Power_Max"] = waveXls.Power_Max == -1 ? "NA" : waveXls.Power_Max.ToString();
                            dr["Flux_Power_Min"] = waveXls.Power_Min == -1 ? "NA" : waveXls.Power_Min.ToString();

                            dr["Flux_Pres"] = waveXls.Pressure == -1 ? "NA" : waveXls.Pressure.ToString();
                            dr["Flux_Pres_Max"] = waveXls.Pressure_Max == -1 ? "NA" : waveXls.Pressure_Max.ToString();
                            dr["Flux_Pres_Min"] = waveXls.Pressure_Min == -1 ? "NA" : waveXls.Pressure_Min.ToString();

                            dr["Flux_BiModel"] = waveXls.Bi_direction_Model == "" ? "NA" : waveXls.Bi_direction_Model;  //waveXls.Bi_direction_Model;

                            dr["Heat_Low1"] = waveXls.Lower_ph1 == -1 ? "NA" : waveXls.Lower_ph1.ToString();
                            dr["Heat_Low1_Max"] = waveXls.Lower_ph1_Max == -1 ? "NA" : waveXls.Lower_ph1_Max.ToString();
                            dr["Heat_Low1_Min"] = waveXls.Lower_ph1_Min == -1 ? "NA" : waveXls.Lower_ph1_Min.ToString();

                            dr["Heat_Low2"] = waveXls.Lower_ph2 == -1 ? "NA" : waveXls.Lower_ph2.ToString();
                            dr["Heat_Low2_Max"] = waveXls.Lower_ph2_Max == -1 ? "NA" : waveXls.Lower_ph2_Max.ToString();
                            dr["Heat_Low2_Min"] = waveXls.Lower_ph2_Min == -1 ? "NA" : waveXls.Lower_ph2_Min.ToString();

                            dr["Heat_Low3"] = waveXls.Lower_ph3 == -1 ? "NA" : waveXls.Lower_ph3.ToString();
                            dr["Heat_Low3_Max"] = waveXls.Lower_ph3_Max == -1 ? "NA" : waveXls.Lower_ph3_Max.ToString();
                            dr["Heat_Low3_Min"] = waveXls.Lower_ph3_Min == -1 ? "NA" : waveXls.Lower_ph3_Min.ToString();

                            dr["Heat_Upp1"] = waveXls.Upper_ph1 == -1 ? "NA" : waveXls.Upper_ph1.ToString();
                            dr["Heat_Upp1_Max"] = waveXls.Upper_ph1_Max == -1 ? "NA" : waveXls.Upper_ph1_Max.ToString();
                            dr["Heat_Upp1_Min"] = waveXls.Upper_ph1_Min == -1 ? "NA" : waveXls.Upper_ph1_Min.ToString();

                            dr["Heat_Upp2"] = waveXls.Upper_ph2 == -1 ? "NA" : waveXls.Upper_ph2.ToString();
                            dr["Heat_Upp2_Max"] = waveXls.Upper_ph2_Max == -1 ? "NA" : waveXls.Upper_ph2_Max.ToString();
                            dr["Heat_Upp2_Min"] = waveXls.Upper_ph2_Min == -1 ? "NA" : waveXls.Upper_ph2_Min.ToString();

                            dr["Heat_Upp3"] = waveXls.Upper_ph3 == -1 ? "NA" : waveXls.Upper_ph3.ToString();
                            dr["Heat_Upp3_Max"] = waveXls.Upper_ph3_Max == -1 ? "NA" : waveXls.Upper_ph3_Max.ToString();
                            dr["Heat_Upp3_Min"] = waveXls.Upper_ph3_Min == -1 ? "NA" : waveXls.Upper_ph3_Min.ToString();

                            dr["SP_Temp"] = waveXls.SP_Temp == -1 ? "NA" : waveXls.SP_Temp.ToString();
                            dr["SP_Temp_Max"] = waveXls.SP_Temp_Max == -1 ? "NA" : waveXls.SP_Temp_Max.ToString();
                            dr["SP_Temp_Min"] = waveXls.SP_Temp_Min == -1 ? "NA" : waveXls.SP_Temp_Min.ToString();

                            dr["SP_ConWave"] = waveXls.SP_ConWave == -1 ? "NA" : waveXls.SP_ConWave.ToString();
                            dr["SP_ConWave_Max"] = waveXls.SP_ConWave_Max == -1 ? "NA" : waveXls.SP_ConWave_Max.ToString();
                            dr["SP_ConWave_Min"] = waveXls.SP_ConWave_Min == -1 ? "NA" : waveXls.SP_ConWave_Min.ToString();

                            dr["SP_LdClear"] = waveXls.Lead_Clear == -1 ? "NA" : waveXls.Lead_Clear.ToString();
                            dr["SP_LdClear_Max"] = waveXls.Lead_Clear_Max == -1 ? "NA" : waveXls.Lead_Clear_Max.ToString();
                            dr["SP_LdClear_Min"] = waveXls.Lead_Clear_Min == -1 ? "NA" : waveXls.Lead_Clear_Min.ToString();

                            dr["Conv_Speed"] = waveXls.Conv_Speed == -1 ? "NA" : waveXls.Conv_Speed.ToString();
                            dr["Conv_Speed_Max"] = waveXls.Conv_Speed_Max == -1 ? "NA" : waveXls.Conv_Speed_Max.ToString();
                            dr["Conv_Speed_Min"] = waveXls.Conv_Speed_Min == -1 ? "NA" : waveXls.Conv_Speed_Min.ToString();

                            dr["Conv_Width"] = waveXls.Conv_Width == -1 ? "NA" : waveXls.Conv_Width.ToString();
                            dr["Conv_Width_Max"] = waveXls.Conv_Width_Max == -1 ? "NA" : waveXls.Conv_Width_Max.ToString();
                            dr["Conv_Width_Min"] = waveXls.Conv_Width_Min == -1 ? "NA" : waveXls.Conv_Width_Min.ToString();

                            dr["Other_Ni"] = waveXls.Other_Ni == "" ? "NA" : waveXls.Other_Ni; //waveXls.Other_Ni;

                            dr["Remark"] = waveXls.Remark;
                            dr["Check"] = true;

                            dtWave.Rows.Add(dr);
                            #endregion
                        }
                        #endregion
                    }
                    return dtWave;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;// + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }


        public xlsColMark Get_Column_ByTitle(ISheet sheet, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                xlsColMark m_XlsMark = new xlsColMark();
                var dataFormatter = new DataFormatter(CultureInfo.CurrentCulture);

                int nRow_Title = 0;
                for(int nRw=4; nRw<7; nRw++)
                {
                    #region get row of title
                    IRow row = sheet.GetRow(nRw);
                    for (int nCl = 0; nCl < 5; nCl++)
                    {
                        #region get row of title
                        string sCellVal = dataFormatter.FormatCellValue(row.GetCell(nCl)).Trim();
                        if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_01, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_01 = nCl;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_02, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_02 = nCl;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_03, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_03 = nCl;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_04, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_04 = nCl;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_05, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_05 = nCl;
                        }


                        #endregion
                    }
                    if (m_XlsMark.Tit_Col_01 >= 0 && m_XlsMark.Tit_Col_02 > 0 && m_XlsMark.Tit_Col_03 > 0)
                    {
                        nRow_Title = nRw;
                        break;
                    }
                    #endregion
                }

                if(nRow_Title > 0)
                {
                    #region nRow_Title
                    int nRow_Title_All = nRow_Title + 1;  //"Flux Flowrate"	"Lower Preheater1"	"Lower Preheater2"	"Lower Preheater3"	"Upper
                    IRow row_Title = sheet.GetRow(nRow_Title_All);

                    for(int nCl=3; nCl< row_Title.LastCellNum; nCl++)
                    {
                        #region get column for each
                        string sCellVal = dataFormatter.FormatCellValue(row_Title.GetCell(nCl)).Trim();
                        if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_04, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_04 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_04 ?  m_XlsMark.Tit_Col_04 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_05, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_05 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_05 ? m_XlsMark.Tit_Col_05 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_06, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_06 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_06 ? m_XlsMark.Tit_Col_06 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_07, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_07 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_07 ? m_XlsMark.Tit_Col_07 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_08, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_08 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_08 ? m_XlsMark.Tit_Col_08 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_09, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_09 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_09 ? m_XlsMark.Tit_Col_09 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_10, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_10 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_10 ? m_XlsMark.Tit_Col_10 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_11, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_11 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_11 ? m_XlsMark.Tit_Col_11 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_12, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_12 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_12 ? m_XlsMark.Tit_Col_12 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_13, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_13 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_13 ? m_XlsMark.Tit_Col_13 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_14, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_14 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_14 ? m_XlsMark.Tit_Col_14 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_15, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_15 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_15 ? m_XlsMark.Tit_Col_15 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_16, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_16 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_16 ? m_XlsMark.Tit_Col_16 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_17, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_17 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_17 ? m_XlsMark.Tit_Col_17 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_18, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_18 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_18 ? m_XlsMark.Tit_Col_18 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_19, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_19 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_19 ? m_XlsMark.Tit_Col_19 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_20, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_20 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_20 ? m_XlsMark.Tit_Col_20 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_21, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_21 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_21 ? m_XlsMark.Tit_Col_21 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_22, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_22 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_22 ? m_XlsMark.Tit_Col_22 : m_XlsMark.Col_LastRight;
                        }
                        else if (Regex.IsMatch(sCellVal, m_XlsMark.Col_Mark_23, RegexOptions.IgnoreCase))
                        {
                            m_XlsMark.Tit_Col_23 = nCl;
                            m_XlsMark.Col_LastRight = m_XlsMark.Col_LastRight < m_XlsMark.Tit_Col_23 ? m_XlsMark.Tit_Col_23 : m_XlsMark.Col_LastRight;
                        }
                        #endregion
                    }

                    m_XlsMark.Row_Data = nRow_Title_All + 1;
                    #endregion
                    ErrMsg = "Success";
                }
                else
                {
                    ErrMsg = "Fail to get title.";
                }

                return m_XlsMark;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }


        /// <summary>
        /// 获取Wave WorkI 的数据行的数据 -- 每一行
        /// </summary>
        private Wave_Xls Get_Para_RowValue(IRow rowDt, xlsColMark xlsMark, ref string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                Wave_Xls waveXls = new Wave_Xls();

                ICell cell = rowDt.GetCell(xlsMark.Tit_Col_02);
                string sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                waveXls.ModelName = sValueThisCell;

                if (waveXls.ModelName == "")
                {
                    ErrMsg = "Model is null => Row=" + cell.RowIndex;
                    return null;
                }
                else if (Regex.IsMatch(sValueThisCell, @"^Model\s*Name", RegexOptions.IgnoreCase))
                {
                    return null;
                }
                #region get value
                POLM_Para polm_Para = new POLM_Para();

                cell = rowDt.GetCell(xlsMark.Tit_Col_03);
                sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                waveXls.ProgramName = sValueThisCell;

                if (xlsMark.Tit_Col_04 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_04);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Flux_Bd_Wid = polm_Para.Para_Cen;
                    waveXls.Flux_Bd_Wid_Max = polm_Para.Para_Max;
                    waveXls.Flux_Bd_Wid_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_05 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_05);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Flux_ConvSpd = polm_Para.Para_Cen;
                    waveXls.Flux_ConvSpd_Max = polm_Para.Para_Max;
                    waveXls.Flux_ConvSpd_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_06 > 0)
                { 
                    cell = rowDt.GetCell(xlsMark.Tit_Col_06);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Flux_NozSpd = polm_Para.Para_Cen;
                    waveXls.Flux_NozSpd_Max = polm_Para.Para_Max;
                    waveXls.Flux_NozSpd_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_07 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_07);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Flux_Volume = polm_Para.Para_Cen;
                    waveXls.Flux_Volume_Max = polm_Para.Para_Max;
                    waveXls.Flux_Volume_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_08 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_08);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Flux_NozSpray = polm_Para.Para_Cen;
                    waveXls.Flux_NozSpray_Max = polm_Para.Para_Max;
                    waveXls.Flux_NozSpray_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_09 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_09);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Power = polm_Para.Para_Cen;
                    waveXls.Power_Max = polm_Para.Para_Max;
                    waveXls.Power_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_10 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_10);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Pressure = polm_Para.Para_Cen;
                    waveXls.Pressure_Max = polm_Para.Para_Max;
                    waveXls.Pressure_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_11 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_11);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    waveXls.Bi_direction_Model = sValueThisCell;
                }

                if (xlsMark.Tit_Col_12 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_12);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Lower_ph1 = polm_Para.Para_Cen;
                    waveXls.Lower_ph1_Max = polm_Para.Para_Max;
                    waveXls.Lower_ph1_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_13 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_13);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Lower_ph2 = polm_Para.Para_Cen;
                    waveXls.Lower_ph2_Max = polm_Para.Para_Max;
                    waveXls.Lower_ph2_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_14 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_14);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Lower_ph3 = polm_Para.Para_Cen;
                    waveXls.Lower_ph3_Max = polm_Para.Para_Max;
                    waveXls.Lower_ph3_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_15 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_15);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Upper_ph1 = polm_Para.Para_Cen;
                    waveXls.Upper_ph1_Max = polm_Para.Para_Max;
                    waveXls.Upper_ph1_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_16 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_16);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Upper_ph2 = polm_Para.Para_Cen;
                    waveXls.Upper_ph2_Max = polm_Para.Para_Max;
                    waveXls.Upper_ph2_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_17 > 0)
                {

                    cell = rowDt.GetCell(xlsMark.Tit_Col_17);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Upper_ph3 = polm_Para.Para_Cen;
                    waveXls.Upper_ph3_Max = polm_Para.Para_Max;
                    waveXls.Upper_ph3_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_18 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_18);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.SP_Temp = polm_Para.Para_Cen;
                    waveXls.SP_Temp_Max = polm_Para.Para_Max;
                    waveXls.SP_Temp_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_19 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_19);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.SP_ConWave = polm_Para.Para_Cen;
                    waveXls.SP_ConWave_Max = polm_Para.Para_Max;
                    waveXls.SP_ConWave_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_20 > 0)
                {

                    cell = rowDt.GetCell(xlsMark.Tit_Col_20);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Lead_Clear = polm_Para.Para_Cen;
                    waveXls.Lead_Clear_Max = polm_Para.Para_Max;
                    waveXls.Lead_Clear_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_21 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_21);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Conv_Speed = polm_Para.Para_Cen;
                    waveXls.Conv_Speed_Max = polm_Para.Para_Max;
                    waveXls.Conv_Speed_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_22 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_22);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    polm_Para = Para_Wave_Helper.Get_Wave_Parameter(sValueThisCell);
                    waveXls.Conv_Width = polm_Para.Para_Cen;
                    waveXls.Conv_Width_Max = polm_Para.Para_Max;
                    waveXls.Conv_Width_Min = polm_Para.Para_Min;
                }

                if (xlsMark.Tit_Col_23 > 0)
                {
                    cell = rowDt.GetCell(xlsMark.Tit_Col_23);
                    sValueThisCell = new DataFormatter(CultureInfo.CurrentCulture).FormatCellValue(cell).Trim();
                    waveXls.Other_Ni = sValueThisCell;
                }
                #endregion

                ErrMsg = "S,R=" + (rowDt.RowNum + 1);
                waveXls.Remark = ErrMsg;


                return waveXls;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message; // + Environment.NewLine + ex.StackTrace;
            }
            return null;
        }


        public DataTable CreateNewDataTable_Wave()
        {
            try
            {
                #region define datatable
                DataTable m_DT = new DataTable();
                m_DT.TableName = "WaveCont";
                DataColumn column = new DataColumn("ModelName", System.Type.GetType("System.String")); // System.Type.GetType("System.Int32"));
                m_DT.Columns.Add(column);
                column = new DataColumn("ProgName", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_BdWid", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_BdWid_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_BdWid_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Flux_ConvSpd", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_ConvSpd_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_ConvSpd_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Flux_NozSpd", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_NozSpd_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_NozSpd_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Flux_Volumn", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_Volumn_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_Volumn_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Flux_NozSpray", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_NozSpray_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_NozSpray_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Flux_Power", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_Power_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_Power_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Flux_Pres", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_Pres_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Flux_Pres_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);


                column = new DataColumn("Flux_BiModel", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Low1", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Low1_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Low1_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Low2", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Low2_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Low2_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Low3", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Low3_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Low3_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Upp1", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Upp1_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Upp1_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Upp2", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Upp2_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Upp2_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Heat_Upp3", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Upp3_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Heat_Upp3_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("SP_Temp", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("SP_Temp_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("SP_Temp_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);


                column = new DataColumn("SP_ConWave", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("SP_ConWave_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("SP_ConWave_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("SP_LdClear", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("SP_LdClear_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("SP_LdClear_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Conv_Speed", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Conv_Speed_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Conv_Speed_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Conv_Width", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Conv_Width_Max", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);
                column = new DataColumn("Conv_Width_Min", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Other_Ni", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);

                column = new DataColumn("Remark", System.Type.GetType("System.String"));
                m_DT.Columns.Add(column);


                column = new DataColumn("Check", System.Type.GetType("System.Boolean"));
                m_DT.Columns.Add(column);

                #endregion

                return m_DT;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

    }





}