using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionTrees;

namespace CharacterSheetEditor.Interfaces
{
    public interface IFormulable : IPlaceable
    {
        string Formula { get; set; }
        bool isEdited { get; set; }
        string ID { get; set; }
        void Append(string s);
        void Update();

        void UpdateFormula(string key1,string key2);
    }
}
