using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class BarCodeDto
    {

        public int BarCodeId { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool IsDefault { get; set; }
        public bool CanDelete { get; set; }
        public int BranchId { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int BarcodeItemType { get; set; }
    }
    public class BarCodeByIdDto
    {
        public BarCodeByIdDto()
        {
            barCodeItemsDtos = new List<BarCodeItemsDto>();
        }
        public int BarCodeId { get; set; }
        public int Code { get; set; } 
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool IsDefault { get; set; }
        public bool CanDelete { get; set; }
        public int BranchId { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int BarcodeItemType { get; set; }
        public List<BarCodeItemsDto> barCodeItemsDtos { get; set; }
    }
    public class BarCodeItemsDto
    {
        public int BarcodeId { get; set; }
        public int BarcodeItemType { get; set; } // باركود او تكست
        public double Width { get; set; }
        public double Height { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public string FontType { get; set; }
        public double FontSize { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool UnderLine { get; set; }
        public string AlignX { get; set; }
        public string AlignY { get; set; }
        public string Dock { get; set; }
        public byte[] Image { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public bool IsLogo { get; set; }
        public int Direction { get; set; }
        public int TextType { get; set; }
        public string TextContent { get; set; }
        public int VariableContent { get; set; }
        public string BeginSplitter { get; set; }
        public string EndSplitter { get; set; }
        public int BarcodeType { get; set; }// نوع الباركود
    }
    public class BarcodeSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
    }
    public class Printers
    {
        public string Name { get; set; }
    }
}
