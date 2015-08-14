using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CharacterSheetEditor.Interfaces
{
    public interface IPlaceable
    {
        int Width { get; set; }
        int Height { get; set; }
        int X { get; set; }
        int Y { get; set; }
        bool Visible { get; set; }
        CharacterSheet ParentSheet { get; set; }
        Canvas Panel { get; set; }
    }
}
