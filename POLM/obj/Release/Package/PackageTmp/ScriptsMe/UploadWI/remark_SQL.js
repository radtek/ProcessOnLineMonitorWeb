                        
                        
//select WorkSheet,DocNum,DocRev,Model,Program,Para_Name, Val_Cen, Val_Max, Val_Min, OV_ID, Para_Desc from
//(
//   select WorkSheet,DocNum,DocRev,Model,Program,Para_Name, Val_Cen, Val_Max, Val_Min, OV_ID, d.Data as Para_Desc from
//   (
//       select WorkSheet,DocNum,DocRev,b.Model,b.Program,b.Para_Name,b.Val_Cen,b.Val_Max,b.Val_Min,b.OV_ID from
//       (
//           select * from POLM_WI_OV where McType='Wave' and Project='DI' and Line='3'
//        ) a left join
//        (
//            select * from POLM_WI_PARA  
//        ) b on a.ID = b.OV_ID
//    ) c left join
//    ( 
//        select ParValue,Data from POLM_Config WHERE Family='WorkI' and Type='Parameter' and ParKey='Desc'
//    ) d on Upper(c.Para_Name)=Upper(d.ParValue)
//) e left join 
//(
//    select Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, num, (sysdate - UpdateTime) day_diff from
//    (
//        select  Project, Line, Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, 
//            row_number() over (partition by Model,Program, Para_Name order by UpdateTime desc) as num
//        from POLM_WI_TEMPORY where Machine='Wave' and Project='DI' AND Line='3'
//    ) where num=1
//) f on e.Model=f.Model and e.Program=f.Program and e.Para_Name=f.Para_Name
    
    
    
//select Project, Line, Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, num, (sysdate - UpdateTime) day_diff from
//(
//    select  Project, Line, Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, 
//        row_number() over (partition by Model,Program, Para_Name order by UpdateTime desc) as num
//    from POLM_WI_TEMPORY where Machine='Wave' and Project='DI' AND Line='3'
//) where num=1



//select Project, Line, Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, num, 
//sysdate - to_date('2018-08-25 21:45', 'YYYY-MM-DD hh24:mi') day_diff from
//(
//    select  Project, Line, Model, Program, Para_Name, Val_Max, Val_Min, EN, UpdateTime, 
//        row_number() over (partition by Model,Program, Para_Name order by UpdateTime desc) as num
//    from POLM_WI_TEMPORY where Machine='Wave' and Project='DI' AND Line='3'
//) where num=1


    
    
    
    
        
        