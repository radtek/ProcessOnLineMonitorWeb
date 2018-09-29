using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;


namespace MyPoiXls
{
    public static class POI_Excel
    {
        public static int GetRegionNumber(ISheet sheet, int nRow, int nCol)
        {
            string ErrMsg = "";
            try
            {
                int nReg = sheet.NumMergedRegions;
                for(int i=0; i<nReg; i++)
                {
                    CellRangeAddress region = sheet.GetMergedRegion(i);
                    if(nRow >= region.FirstRow && nRow <= region.LastRow)
                    {
                        if(nCol >= region.FirstColumn && nCol <= region.LastColumn)
                        {
                            int nNumCells = region.NumberOfCells;
                            return nNumCells;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return -1;
        }


    }


}