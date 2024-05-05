using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace csbemt
{
    class ImportExcel
    {
        static Excel.Application excelApp = null;
        static Excel.Workbook workBook = null;
        static Excel.Worksheet workSheet = null;

        public void Import(string csv_path)
        {
            Queue que = new Queue();

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);  // 바탕화면 경로
                string path = Path.Combine(desktopPath, csv_path);                              // 엑셀 파일 저장 경로

                excelApp = new Excel.Application();                             // 엑셀 어플리케이션 생성
                workBook = excelApp.Workbooks.Open(path);                       // 워크북 열기
                workSheet = workBook.Worksheets.get_Item(1) as Excel.Worksheet; // 엑셀 첫번째 워크시트 가져오기

                Excel.Range range = workSheet.UsedRange;    // 사용중인 셀 범위를 가져오기

                for (int row = 1; row <= range.Rows.Count; row++) // 가져온 행 만큼 반복
                {
                    for (int column = 1; column <= range.Columns.Count; column++)  // 가져온 열 만큼 반복
                    {
                        try
                        {
                            string str = (string)(range.Cells[row, column] as Excel.Range).Value2;  // 셀 string 데이터 가져옴
                            Console.Write(str + " ");

                            que.Enqueue(str);
                        }
                        catch
                        {
                            double num = (double)(range.Cells[row, column] as Excel.Range).Value2;  // 셀 double 데이터 가져옴
                            Console.Write(num + " ");

                            que.Enqueue(num);
                        }
                    }
                    Console.WriteLine();
                }

                workBook.Close(true);   // 워크북 닫기
                excelApp.Quit();        // 엑셀 어플리케이션 종료
            }
            finally
            {
                ReleaseObject(workSheet);
                ReleaseObject(workBook);
                ReleaseObject(excelApp);

                PrintValues(que);
            }
        }

        public static void PrintValues(IEnumerable myCollection)
        {
            foreach (Object obj in myCollection)
                Console.Write("{0} ", obj);
            Console.WriteLine();
        }


        /// <summary>
        /// 액셀 객체 해제 메소드
        /// </summary>
        /// <param name="obj"></param>
        static void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);  // 액셀 객체 해제
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();   // 가비지 수집
            }
        }
    }
}
