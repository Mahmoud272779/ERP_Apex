using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.POS
{
    public class POSTouchSettings
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public int UserId { get; set; }
        public double PosTouch_FontSize { get; set; }
        public double PosTouch_CategoryImgWidth { get; set; }
        public double PosTouch_CategoryImgHeight { get; set; }
        public double PosTouch_TableWidth { get; set; }
        public double PosTouch_ItemsImgWidth { get; set; }
        public double PosTouch_ItemsImgHeight { get; set; }
        public bool PosTouch_DisplayItemPrice { get; set; }
    }
}
