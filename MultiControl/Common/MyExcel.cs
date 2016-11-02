using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
//using System.Drawing;
//using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;
namespace ExcelOperaNamespace
{
    /// <summary>
    /// 常用颜色定义,对就Excel中颜色名
    /// </summary>
    public enum ColorIndex
    {
        无色 = -4142,
        自动 = -4105,
        黑色 = 1,
        褐色 = 53,
        橄榄 = 52,
        深绿 = 51,
        深青 = 49,
        深蓝 = 11,
        靛蓝 = 55,
        灰色80 = 56,
        深红 = 9,
        橙色 = 46,
        深黄 = 12,
        绿色 = 10,
        青色 = 14,
        蓝色 = 5,
        蓝灰 = 47,
        灰色50 = 16,
        红色 = 3,
        浅橙色 = 45,
        酸橙色 = 43,
        海绿 = 50,
        水绿色 = 42,
        浅蓝 = 41,
        紫罗兰 = 13,
        灰色40 = 48,
        粉红 = 7,
        金色 = 44,
        黄色 = 6,
        鲜绿 = 4,
        青绿 = 8,
        天蓝 = 33,
        梅红 = 54,
        灰色25 = 15,
        玫瑰红 = 38,
        茶色 = 40,
        浅黄 = 36,
        浅绿 = 35,
        浅青绿 = 34,
        淡蓝 = 37,
        淡紫 = 39,
        白色 = 2
    }
    /// <summary>
    /// 单元格填充方式
    /// </summary>
    public enum Pattern
    {
        Automatic = -4105,
        Checker = 9,
        CrissCross = 16,
        Down = -4121,
        Gray16 = 17,
        Gray25 = -4124,
        Gray50 = -4125,
        Gray75 = -4126,
        Gray8 = 18,
        Grid = 15,
        Horizontal = -4128,
        LightDown = 13,
        LightHorizontal = 11,
        LightUp = 14,
        LightVertical = 12,
        None = -4142,
        SemiGray75 = 10,
        Solid = 1,
        Up = -4162,
        Vertical = -4166
    }
    /// <summary>
    /// 下划线方式
    /// </summary>
    public enum UnderlineStyle
    {
        无下划线 = -4142,
        双线 = -4119,
        双线充满全格 = 5,
        单线 = 2,
        单线充满全格 = 4
    }
    /// <summary>
    /// 线样式
    /// </summary>
    public enum LineStyle
    {
        连续直线 = 1,
        短线 = -4115,
        线点相间 = 4,
        短线间两点 = 5,
        点 = -4118,
        双线 = -4119,
        无 = -4142,
        少量倾斜点 = 13
    }
    /// <summary>
    /// 线粗
    /// </summary>
    public enum BorderWeight
    {
        极细 = 1,
        细 = 2,
        粗 = -4138,
        极粗 = 4
    }
    /// <summary>
    /// 垂直对齐方式
    /// </summary>
    public enum ExcelVAlign
    {
        靠上 = 1,
        居中,
        靠下,
        两端对齐,
        分散对齐
    }
    /// <summary>
    /// 水平对齐方式
    /// </summary>
    public enum ExcelHAlign
    {
        常规 = 1,
        靠左,
        居中,
        靠右,
        填充,
        两端对齐,
        跨列居中,
        分散对齐
    }
    public class PictureCell
    {
        public int nIndex;// 图片在Excel中的索引号
        public int nRow;
    }
    /// <summary>
    /// EXCEL操作类， 可以实现EXCEL表的打开， 关闭， 保存， 读， 写和指定写入数据格式等 
    /// EXCEL operating class, EXCEL table open, close, save, read, write, and specify the write data format
    /// </summary>
    public class MyExcel
    {
        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        public MyExcel()
        {
            _MyExcelApp = new Excel.Application();
            _MyWorkbooks = _MyExcelApp.Workbooks;
        }
        /// <summary>
        /// 构造函数, 默认打开一个Excel文件
        /// </summary>
        /// <param name="strFileName"></param>
        public MyExcel(string strFileName)
        {
            _MyExcelApp = new Excel.Application();
            _MyWorkbooks = _MyExcelApp.Workbooks;
            Open(strFileName);
        }
        ~MyExcel()
        {
            //Exit();
        }
        // 公共变量
        public Excel.Application _MyExcelApp = null;  //应用程序服务器Application server
        public Excel.Workbooks _MyWorkbooks = null;   //EXCEL工作布集合EXCEL Workbook collection
        public Excel._Workbook _MyWorkbook = null;     //指定EXCEL工作布Specified EXCEL workbook
        public Excel.Sheets _MyWorksheets = null; //EXCEL工作表集合Excel Worksheet collection
        public Excel._Worksheet _MyWorksheet = null;   //指定EXCEL工作表Specified EXCEL worksheet
        public Excel.Range _MyRange = null;
        public Excel.Shapes _MyShapes = null;
        public Excel.Shape _MyShape = null;
        public string _strFileName;

        /////////////////////////////////////////////////////////////////////
        // 公开的属性
        ////////////////////////////////////////////////////////////////////
        #region 公开的属性
        /// <summary>
        // 显示警告信息
        /// </summary>
        public bool DisplayAlerts
        {
            get { return _MyExcelApp.DisplayAlerts; }
            set { _MyExcelApp.DisplayAlerts = value; }
        }
        /// <summary>
        /// 使工作表可见
        /// </summary>
        public bool Visibled
        {
            get { return _MyExcelApp.Visible;}
            set { _MyExcelApp.Visible = value; }
        }
        /// <summary>
        /// 获取或者设置工作表名
        /// </summary>
        public string WorkSheetName
        {
            get
            {
                return GetWorkSheetName();
            }
            set
            {
                if (_MyWorksheet != null)
                    _MyWorksheet.Name = value;
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 公开的方法
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region 公开的方法
        /// <summary>
        /// 打开EXCEL服务器， 新建一个EXCEL工作布
        /// Open the EXCEL server, create a new EXCEL working
        /// </summary>
        /// <returns></returns>
        public bool NewExcel()
        {
            //_MyExcelApp = new Excel.Application();
            try
            {
                _MyWorkbook = _MyWorkbooks.Add(Missing.Value);
                _MyWorksheets = _MyWorkbook.Worksheets;
               // _MyWorksheet = (Excel._Worksheet)_MyWorksheets.get_Item((long)1);
            }
            catch (Exception e)
            {
                string errorMessage;
                errorMessage = "Error: ";
                errorMessage = string.Concat(errorMessage, e.Message);
                errorMessage = string.Concat(errorMessage, "Line: ");
                errorMessage = string.Concat(errorMessage, e.Source);
                //System.Windows.Forms.MessageBox.Show(errorMessage, "Error");
                return false;
            }
            return true;
        }
        public void DeleteSheet()
        {
            _MyWorksheet.Delete();
        }
        public void DeleteSheetByIndex(int sheetIndex)
        {
            for (int index = 1; index <= _MyWorksheets.Count; index++)
            {
                _Worksheet sheet = (_Worksheet)_MyWorksheets[index];
                sheet.Delete();
            }
        }
        public void DeleteSheet(string sheetName)
        {
            if (OpenSheet(sheetName))
                DeleteSheet();
        }
        /// <summary>
        /// 判断文件是否存在
        /// ///Determine whether a file exists
        /// </summary>
        /// <param name="sFileName">文件名</param>
        /// <param name="IsDir">是否要判断目录</param>
        /// <returns></returns>
        public bool DoesFileExisted(string sFileName, bool IsDir)
        {
            if (IsDir)
            {
                if (!Directory.Exists(sFileName))
                {
                    //System.Windows.Forms.MessageBox.Show("The Directory does not exist！", "Error");
                    return false;
                }
            }
            else
            {
                if (!File.Exists(sFileName))
                {
                    //System.Windows.Forms.MessageBox.Show("The file does not exist！", "Error");
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 打开一个已存在的EXCEL文件
        /// Open an existing EXCEL file
        /// </summary>
        /// <param name="sFileName">文件名</param>
        /// <returns></returns>
        public bool Open(string sFileName)
        {
            try
            {
                if (DoesFileExisted(sFileName, false))
                {
                    _MyWorkbook = _MyWorkbooks.Open(sFileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value
                        , Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value
                        , Missing.Value, Missing.Value, Missing.Value);
                    _MyWorksheets = _MyWorkbook.Worksheets;
                    _strFileName = sFileName;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                string errorMessage;
                errorMessage = "Error: ";
                errorMessage = string.Concat(errorMessage, e.Message);
                errorMessage = string.Concat(errorMessage, "Line: ");
                errorMessage = string.Concat(errorMessage, e.Source);
                //System.Windows.Forms.MessageBox.Show(errorMessage, "Error");
                return false;
            }
            return true;
//            return true;
        }
        /// <summary>
        /// 新添加一个工作表
        /// ///Add a new worksheet
        /// </summary>
        /// <param name="sSheetName"></param>
        /// <returns></returns>
        public bool AddSheet(string sSheetName)
        {
            int nLen = _MyWorksheets.Count;
            int nIndex = 1;
            for (nIndex = 1; nIndex <= nLen; nIndex++)
            {
                _MyWorksheet = (_Worksheet)_MyWorksheets.get_Item((long)nIndex);
                if (_MyWorksheet.Name == sSheetName)
                {
                    break;
                }
            }
            if (nIndex > nLen)
            {
                _MyWorksheet = (_Worksheet)_MyWorksheets.Add(Missing.Value, _MyWorksheets.get_Item((long)_MyWorksheets.Count), Missing.Value, Missing.Value);
                _MyWorksheet.Name = sSheetName;
            }
            return true;
        }
        /// <summary>
        /// 新添加一个工作表
        /// ///Add a new worksheet
        /// </summary>
        /// <param name="sSheetName"></param>
        /// <returns></returns>
        public bool AddSheet(string sSheetName, string beforeSheets)
        {
            int nLen = _MyWorksheets.Count;
            int nIndex = 1;
            for (nIndex = 1; nIndex <= nLen; nIndex++)
            {
                _MyWorksheet = (_Worksheet)_MyWorksheets.get_Item((long)nIndex);
                if (_MyWorksheet.Name == sSheetName)
                {
                    break;
                }
            }
            if (nIndex > nLen)
            {
                _MyWorksheet = (_Worksheet)_MyWorksheets.Add(Missing.Value, _MyWorksheets[beforeSheets], Missing.Value, Missing.Value);
                _MyWorksheet.Name = sSheetName;
            }
            return true;
        }
        /// <summary>
        /// 打开一个工作表
        /// ///Open a worksheet
        /// </summary>
        /// <param name="sSheetName"></param>
        /// <returns></returns>
        public bool OpenSheet(string sSheetName)
        {
            int nLen = _MyWorksheets.Count;
            int nIndex = 1;
            for (nIndex = 1; nIndex <= nLen; nIndex++)
            {
                _MyWorksheet = (_Worksheet)_MyWorksheets.get_Item((long)nIndex);
                if (_MyWorksheet.Name.CompareTo(sSheetName) == 0)
                {
                    _MyWorksheet.Activate();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 通过索引号打开工作表
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool OpenSheetByIndex(int index)
        {
            if (index > _MyWorksheets.Count)
                return false;
            _MyWorksheet = (_Worksheet)_MyWorksheets.get_Item((long)index);
            _MyWorksheet.Activate();
            return true;
        }
        /// <summary>
        /// 获取当前工作表的名称
        /// </summary>
        /// <returns></returns>
        public string GetWorkSheetName()
        {
            if(_MyWorksheet != null)
                return _MyWorksheet.Name;
            return "";
        }
        /// <summary>
        /// 获取表格数
        /// </summary>
        /// <returns></returns>
        public int GetSheetsNum()
        {
            return _MyWorksheets.Count;
        }
        /// <summary>
        /// 通过索引获取表名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetSheetNameByIndex(int index)
        {
            _Worksheet sheet = (_Worksheet)_MyWorksheets.get_Item((long)index);
            //sheet.Visible = XlSheetVisibility
            return sheet.Name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sBegin"></param>
        /// <param name="sEnd"></param>
        public void GetRange(string sBegin, string sEnd)
        {
            _MyRange = _MyWorksheet.get_Range((object)sBegin, (object)sEnd);
            //_MyRange = _MyWorksheet.Cells;
        }
        /// <summary>
        /// 
        /// </summary>
        public void AutoRange()
        {
            _MyRange = _MyWorksheet.Cells;
        }
        /// <summary>
        /// 自适应单元格
        /// Adaptive cell
        /// </summary>
        public void AutoFitAll()
        {
            Excel.Range rg = _MyWorksheet.Cells;
            rg.EntireColumn.AutoFit();
        }

        public void AutoFit()
        {
            _MyRange.EntireColumn.AutoFit();
        }
        /// <summary>
        /// 获取指定单元格的内容
        /// </summary>
        /// <param name="nRow">行号</param>
        /// <param name="nCol">列号</param>
        /// <returns>返回该行的数据</returns>
        public string GetItemText(long nRow, long nCol)
        {
            string sData = "";
            Excel.Range rg = (Range)_MyRange.get_Item(nRow, nCol);
#if DEBUG
            //rg.Activate();
#endif
            sData = System.Convert.ToString(rg.get_Value(Missing.Value));
            return sData;
        }
        public string GetItemText(string cellName)
        {
            string sData = "";
            Excel.Range rg = (Range)_MyRange.get_Item(Convert.ToInt32(cellName.Substring(1)), GetNumber(cellName.Substring(0, 1)));
            sData = System.Convert.ToString(rg.get_Value(Missing.Value));
            return sData;
        }
        /// <summary>
        /// 获取一整行数据
        /// </summary>
        /// <param name="nRow">行号</param>
        /// <returns></returns>
        public string GetEntireRowText(int nRow)
        {
            return GetItemText(nRow, 1) + GetItemText(nRow, 2);
        }
        /// <summary>
        ///  返回背景样式
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <returns></returns>
        public Interior GetCellbackgroundStyle(int nRow, int nCol)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            //rg = rg.Columns[nCol];
            return rg.Interior;
        }
        /// <summary>
        /// 设置背景样式
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="interior"></param>
        public void SetCellbackgroundStyle(int nRow, int nCol, Interior interior)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Interior.ColorIndex = interior.ColorIndex;
            rg.Interior.Pattern = interior.Pattern;
        }
        /// <summary>
        /// 设置背景样式
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="interior"></param>
        public void SetCellbackgroundStyle(int nRow, int nCol, ColorIndex colorIndex)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Interior.ColorIndex = colorIndex;
        }
        /// <summary>
        /// 设置单元格边框
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="lineStyle"></param>
        public void SetBordersLineStyle(int nRow, int nCol, LineStyle lineStyle)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Borders.LineStyle = lineStyle;
        }
        /// <summary>
        /// 设置表格区域边框
        /// </summary>
        /// <param name="strBegin"></param>
        /// <param name="strEnd"></param>
        /// <param name="lineStyle"></param>
        public void SetBordersLineStyle(string strBegin, string strEnd, LineStyle lineStyle)
        {
            Range rg = _MyWorksheet.get_Range((object)strBegin, (object)strEnd);
            rg.Borders.LineStyle = lineStyle;
        }
        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="nRow">行号</param>
        /// <param name="nCol">列号</param>
        /// <returns></returns>
        public Microsoft.Office.Interop.Excel.Font GetCellFontStyle(int nRow, int nCol)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            return rg.Font;
        }
        /// <summary>
        /// 设置字体样式
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="font"></param>
        public void SetCellFontStyle(int nRow, int nCol, Microsoft.Office.Interop.Excel.Font font)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Font.Color = font.Color;
            rg.Font.Size = font.Size;
        }

        public void SetCellFontColor(int nRow, int nCol, ColorIndex color)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Font.ColorIndex = color;
        }

        public void SetCellFontSize(int nRow, int nCol, int nSize)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Font.Size = nSize;
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="strBegin"></param>
        /// <param name="strEnd"></param>
        public void MergeCells(string strBegin, string strEnd)
        {
            Range rg = _MyRange.get_Range(strBegin, strEnd);
            rg.Merge(Type.Missing);
            //GetRange(strBegin, strEnd);
            //_MyRange.Merge(Type.Missing);
        }
        /// <summary>
        /// 判断单元格是由合并单元格组成
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsMergeCells(string cell)
        {
            Range rg = _MyWorksheet.get_Range(cell);
            return (bool)rg.MergeCells;
        }
        /// <summary>
        /// 判断单元格是由合并单元格组成
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool IsMergeCells(int row, int col)
        {
            Range rg = (Range)_MyWorksheet.Cells[row, col];
            return (bool)rg.MergeCells;
        }
        /// <summary>
        /// 设置删除线格式
        /// </summary>
        /// <param name="cell"></param>
        public void SetStrikeThrough(string cell)
        {
            Range rg = _MyWorksheet.get_Range(cell);
            rg.Font.Strikethrough = true;
        }
        /// <summary>
        /// 是否有删除线
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsStrikeThrough(string cell)
        {
            Range rg = _MyWorksheet.get_Range(cell);
            return (bool)rg.Font.Strikethrough;
        }
        /// <summary>
        /// 是否有删除线
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsStrikeThrough(int row, int col)
        {
            Range rg = (Range)_MyWorksheet.Cells[row, col];
            return (bool)rg.Font.Strikethrough;
        }
        /// <summary>
        /// 设置单元格水平对齐方式
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        public void HorAligment(int nRow, int nCol, Excel.XlHAlign hAlign)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.HorizontalAlignment = hAlign;
            rg.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        }
        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="nCol"></param>
        /// <param name="nWidth"></param>
        public void SetColumeWidth(int nCol, int nWidth)
        {
            Range rg = (Range)_MyRange.get_Item(1, nCol);
            rg.ColumnWidth = nWidth;
        }
        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nHeight"></param>
        public void SetRowHeight(int nRow, int nHeight)
        {
            Range rg = (Range)_MyRange.get_Item(nRow, 1);
            rg.RowHeight = nHeight;
        }
        /// <summary>
        /// 向单元格写入数据
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="sText"></param>
        /// <returns></returns>
        public bool SetItemText(long nRow, long nCol, string sText)
        {
            Excel.Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            if (this.Visibled)
                rg.Activate();
            rg.NumberFormat = "";
            rg.set_Value(Missing.Value, sText);
            return true;
        }
        /// <summary>
        /// 向单元格写入数字
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="sText"></param>
        /// <returns></returns>
        public bool SetItemText(long nRow, long nCol, double data, string numberFormat)
        {
            Excel.Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            if (this.Visibled)
                rg.Activate();
            rg.NumberFormat = numberFormat;
            if (data == 0.0)
                SetItemText(nRow, nCol, "0");
            rg.set_Value(Missing.Value, data);

            return true;
        }
        /// <summary>
        /// 向单元格写入数据
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="sText"></param>
        /// <returns></returns>
        public bool SetItemText(long nRow, long nCol, string sText, string numberFormat)
        {
            Excel.Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            if (this.Visibled)
                rg.Activate();
            rg.NumberFormat = numberFormat;
            rg.set_Value(Missing.Value, sText);
            return true;
        }
        /// <summary>
        /// 光标定位到某一个单元格中
        /// </summary>
        /// <param name="nRow"></param>
        /// <returns></returns>
        public bool Activate(int nRow, int nCol)
        {
            Excel.Range rg = (Range)_MyRange.get_Item(nRow, nCol);
            rg.Activate();
            return true;
        }
        /// <summary>
        /// 获取Excel文档中的所有图片
        /// </summary>
        /// <returns>返回图片的数量</returns>
        public int GetShapes()
        {
            _MyShapes = _MyWorksheet.Shapes;
            return _MyShapes.Count;
        }

        /// <summary>
        /// 获取工作表中所有图形图表的数量
        /// </summary>
        /// <returns></returns>
        public long GetShapesCount()
        {
            return _MyWorksheet.Shapes.Count;
        }
        

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="index">图片索引号</param>
        public void DeleteShape(int index)
        {
            _MyShape = _MyShapes.Item(index);
            _MyShape.Delete();
        }
        /// <summary>
        /// 将图片拷贝到内存中， 剪贴板中
        /// </summary>
        /// <param name="index"></param>
        public void CopyPicture(int index)
        {
            _MyShape = _MyShapes.Item(index);
            _MyShape.CopyPicture(XlPictureAppearance.xlScreen, XlCopyPictureFormat.xlBitmap);     
        }
        /// <summary>
        /// 将剪贴板中的内容粘贴到指定单元内
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        public void Paste(int nRow, int nCol)
        {
            Range rg = (Range)_MyWorksheet.Cells[nRow, nCol];
            rg.Select();
            _MyWorksheet.Paste(Type.Missing, Type.Missing);
        }

        public void InsertPicture(int row, int col, string picturePath)
        {
            Range rg = (Range)_MyWorksheet.Cells[row, col];
            rg.Select();
            float PicLeft = Convert.ToSingle(rg.Left);
            float PicTop = Convert.ToSingle(rg.Top);
            _MyWorksheet.Shapes.AddPicture(picturePath, MsoTriState.msoCTrue, MsoTriState.msoCTrue, PicLeft, PicTop, 100, 200);

            Excel.Pictures pics = (Excel.Pictures)_MyWorksheet.Pictures(Type.Missing);
            pics.Insert(picturePath, Type.Missing);
        }
        public void AddPicture(int row, int col, string picturePath, float picLeft, float picRight, int width, int height)
        {
            Range rg = (Range)_MyWorksheet.Cells[row, col];
            rg.Select();
            float PicLeft = Convert.ToSingle(rg.Left) + picLeft;
            float PicTop = Convert.ToSingle(rg.Top) + picRight;
            _MyWorksheet.Shapes.AddPicture(picturePath, MsoTriState.msoCTrue, MsoTriState.msoCTrue, PicLeft, PicTop, width, height);

            //Excel.Pictures pics = (Excel.Pictures)_MyWorksheet.Pictures(Type.Missing);
            //pics.Insert(picturePath, Type.Missing);
        }

        /// <summary>
        /// 获取图片的位置
        /// </summary>
        /// <param name="index">图片索引</param>
        /// <returns>返回图片左上角的单元格坐标</returns>
        public PictureCell GetShapeRange(int index)
        {
            _MyShape = _MyShapes.Item(index);
            Excel.Range rg = _MyShape.TopLeftCell;
            PictureCell picCell = new PictureCell();
            picCell.nRow = rg.Row;
            picCell.nIndex = index;
            return picCell;
        }
        /// <summary>
        /// 插入多行空白行
        /// </summary>
        /// <param name="nRowIndex">当前行号</param>
        /// <param name="nNum">要插入的行数</param>
        /// <returns></returns>
        public bool AddRow(int nRowIndex, int nNum)
        {
            if (_MyWorksheet != null)
            {
                for (int index = 0; index != nNum; index++)
                {
                    Excel.Range rg = (Range)_MyWorksheet.Rows[nRowIndex, Type.Missing];
                    rg.Insert(Excel.XlDirection.xlUp, Missing.Value);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 当前行的上面插入一个空白行
        /// </summary>
        /// <param name="nRowIndex">行号</param>
        /// <returns></returns>
        public bool AddRow(int nRowIndex)
        {
            if (_MyWorksheet != null)
            {
                Excel.Range rg = (Range)_MyWorksheet.Rows[nRowIndex, Type.Missing];
                rg.Insert(Excel.XlDirection.xlUp, Missing.Value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除一行
        /// </summary>
        /// <param name="nRowIndex">行号</param>
        /// <returns></returns>
        public bool DeleteRow(int nRowIndex)
        {
            if (_MyWorksheet != null)
            {
                Excel.Range rg = (Range)_MyWorksheet.Rows[nRowIndex, Type.Missing];
                rg.Delete(Type.Missing);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除多行
        /// </summary>
        /// <param name="nStartIndex">起始行号</param>
        /// <param name="nNum">行数</param>
        /// <returns></returns>
        public bool DeleteRows(int nStartIndex, int nNum)
        {
            for (int index = 0; index != nNum; index++)
            {
                if (!DeleteRow(nStartIndex)) return false;
            }
            return true;
        }
        /// <summary>
        /// 在当前列之后插入一个空白列
        /// </summary>
        /// <param name="ColIndex"></param>
        /// <returns></returns>
        public bool AddCol(int ColIndex)
        {
            if (_MyWorksheet != null)
            {
                Excel.Range rg = (Range)_MyWorksheet.Columns[ColIndex + 1, Type.Missing];
                rg.Insert(XlInsertShiftDirection.xlShiftToRight, Missing.Value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置自动换行
        /// Set Word wrap
        /// </summary>
        /// <param name="bWrap"></param>
        public void SetAutoWrap(bool bWrap)
        {
            _MyRange.WrapText = true;
        }

        public void SetHyperLink(string strBegin, string strEnd, string sheetName, string CellStr)
        {
            Range rg = _MyWorksheet.get_Range((object)strBegin, (object)strEnd);
            
            string hlStr = "#'" + sheetName + "'!" + CellStr;
            rg.Hyperlinks.Add(rg, hlStr);
        }
        /// <summary>
        /// 添加备注
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <param name="comment"></param>
        public void InsertComment(int nRow, int nCol, string comment)
        {
            Range rg = (Range)_MyWorksheet.Cells[nRow, nCol];
            if (rg.Comment == null)
                rg.AddComment(comment);
            else
            {
                string commentStr = rg.Comment.Text();
                commentStr += "\r\n" + comment;
                rg.Comment.Text(commentStr, Type.Missing, Type.Missing);
            }

            string[] commentArr = comment.Split('\n');
            if (commentArr.Length <= 1)
            {
                float height = (commentArr.Length) * 12;
                float width = (comment.Length / (commentArr.Length)) * 8;
                SetCommentWidth(nRow, nCol, width);
                SetCommentHeight(nRow, nCol, height);
            }
            else
            {
                float height = (commentArr.Length - 1) * 12;
                float width = (comment.Length / (commentArr.Length - 1)) * 8;
                SetCommentWidth(nRow, nCol, width);
                SetCommentHeight(nRow, nCol, height);
            }
        }
        private void SetCommentWidth(int nRow, int nCol, float width)
        {
             Range rg = (Range)_MyWorksheet.Cells[nRow, nCol];
             if (rg.Comment != null)
             {
                 rg.Comment.Shape.Width = width;
                 //rg.Comment.Shape.ScaleWidth(100, Microsoft.Office.Core.MsoTriState.msoCTrue, 100);
             }
        }
        private void SetCommentHeight(int nRow, int nCol, float height)
        {
            Range rg = (Range)_MyWorksheet.Cells[nRow, nCol];
            if (rg.Comment != null)
            {
                rg.Comment.Shape.Height = height;
                //rg.Comment.Shape.ScaleWidth(100, Microsoft.Office.Core.MsoTriState.msoCTrue, 100);
            }
        }
        public void InsertComment(int nRow, int nCol, string comment, float commentShapeWidth)
        {
            Range rg = (Range)_MyWorksheet.Cells[nRow, nCol];
            if (rg.Comment == null)
            {
                rg.AddComment(comment);
            }
            else
            {
                string commentStr = rg.Comment.Text();
                commentStr += "\r\n" + comment;
                rg.Comment.Text(commentStr, Type.Missing, Type.Missing);
            }

            string[] commentArr = comment.Split('\n');
            float height = commentArr.Length * 10;
            float width = (comment.Length / commentArr.Length) * 10;
            SetCommentWidth(nRow, nCol, width);
            SetCommentHeight(nRow, nCol, height);
        }
        public void InsertComment(int nRow, int nCol, string comment, float commentShapeWidth, float commentShapeHeight)
        {
            Range rg = (Range)_MyWorksheet.Cells[nRow, nCol];
            if (rg.Comment == null)
            {
                rg.AddComment(comment);
            }
            else
            {
                string commentStr = rg.Comment.Text();
                commentStr += "\r\n" + comment;
                rg.Comment.Text(commentStr, Type.Missing, Type.Missing);
            }


            string[] commentArr = comment.Split('\n');
            float height = commentArr.Length * 10;
            float width = (comment.Length / commentArr.Length) * 10;
            SetCommentWidth(nRow, nCol, width);
            SetCommentHeight(nRow, nCol, height);
        }
        /// <summary>
        /// 冻结窗格
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void FreezePanes(int row, int col)
        {
            Range rg = (Range)_MyWorksheet.Cells[row, col];
            rg.Select();
            _MyExcelApp.ActiveWindow.FreezePanes = true;
            
        }
        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="filter"></param>
        public void AutoFilter(bool filter)
        {
            //_MyWorksheet.AutoFilter. = filter;
        }
        /// <summary>
        /// 保存当前工作表
        /// ///Saves the current worksheet
        /// </summary>
        public void Save()
        {
            if(_MyWorkbook != null)
                _MyWorkbook.Save();
        }
        /// <summary>
        /// 另存为
        /// SaveAs
        /// </summary>
        public void SaveAs(string sFileName)
        {
            _MyWorkbook.SaveAs(sFileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value
                , Missing.Value, 0, Missing.Value, Missing.Value, Missing.Value
                , Missing.Value, Missing.Value);
            _strFileName = sFileName;
        }
        public void SaveCopyAs(string sFileName)
        {
            _MyWorkbook.SaveCopyAs(sFileName);
        }
        /// <summary>
        /// Excel是否可见
        /// </summary>
        /// <param name="bVisible"></param>
        public void Visible(bool bVisible)
        {
            if (_MyExcelApp != null)
            {
                _MyExcelApp.Visible = bVisible;
            }
        }
        /// <summary>
        /// 退出
        /// Exit
        /// </summary>
        public void Exit()
        {
            GC.Collect();
            if (_MyWorkbook != null)
            {
                _MyWorkbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_MyWorkbook);
            }
            if (_MyWorkbooks != null)
            { _MyWorkbooks.Close(); }
            if (_MyExcelApp != null)
            {
                _MyExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_MyExcelApp);
            }
            if (_MyRange != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_MyRange);
            }
            if (_MyWorksheet != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_MyWorksheet);
            }
            _MyWorkbook = null;
            _MyWorkbooks = null;
            _MyExcelApp = null;
            _MyWorksheet = null;
            _MyRange = null;
            GC.Collect();
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void AddOLEObject(string filePath, int row, int col, int width, int height)
        {
            Range rg = (Range)_MyWorksheet.Cells[row, col];
            rg.Select();
            _MyWorksheet.Shapes.AddOLEObject(Type.Missing, filePath, Type.Missing,
                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, rg.Left, rg.Top);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 公开的静态方法
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region 公开的静态方法
        /// <summary>
        /// 根据数字获取对应的字符
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetLetter(int index)
        {
            if (index <= 0)
            {
                throw new Exception("Invalid parameter");
            }
            index--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return column;
        }
        /// <summary>
        /// 根据字符获取对应的数字
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static int GetNumber(string column)
        {
            if (!Regex.IsMatch(column.ToUpper(), @"[A-Z]+"))
            {
                throw new Exception("Invalid parameter");
            }
            int index = 0;
            char[] chars = column.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index;
        }
        public static string IndexToColumn(int index)
        {
            if (index <= 0)
            {
                throw new Exception("Invalid parameter");
            }
            index--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return column;



        }
        /// <summary>
        /// 静态方法： 删除Excel进程
        /// </summary>
        public static void DeleteExcelExe()
        {
            Process[] ExcelProcess = Process.GetProcessesByName("EXCEL");
            foreach (var o in ExcelProcess)
                if (o.MainWindowTitle == "")
                {
                    try
                    {
                        o.Kill();
                    }
                    catch { }
                }
        }
        /// <summary>
        /// 静态方法： 提供初始化调试文件的方法
        /// </summary>
        /// <param name="pathName"></param>
        public static void IniDebugFile(string pathName)
        {
            File.WriteAllText(pathName, "===Start: " + DateTime.Now.ToLongTimeString() + "===\r\n");
        }
        /// <summary>
        /// 静态方法： 提供删除文件夹的操作
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static bool DeleteFolder(string pathName)
        {
            if (Directory.Exists(pathName))
            {
                string[] files = Directory.GetFiles(pathName);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                Directory.Delete(pathName);
                return true;
            }
            return false;
        }
        #endregion
    }
}