using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharacterSheetEditor.Interfaces;
using ExpressionTrees;

namespace CharacterSheetEditor
{
    public class CharacterSheet
    {
        public AlgexSet Formulas;
        public List<IFormulable> Boxes;
        public List<IPlaceable> Blocks; 
        bool PreviewMode { get; set; }

        public void Update()
        {
            foreach (var box in Boxes)
            {
                box.Update();
            }
        }
        public string GetFreeTempID()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("temp");
            Random R = new Random();
            while ( !IsFreeID(sb.ToString()))
            {
                char c = (char) ('a' + ((char) R.Next(26)));
                sb.Append(c);


            }
            return sb.ToString();
        }

        public CharacterSheet()
        {
            Formulas = new AlgexSet(){ImmediateSolve = true};
            Boxes = new List<IFormulable>();
            PreviewMode = true;
            Blocks = new List<IPlaceable>();
        }

        internal bool IsFreeID(string newID)
        {
          return !Boxes.Any((s => s.ID == newID));
        }

        internal void ChangeID(string ID, string newID)
        {
            Formulas.RenameVariable(ID,newID);
            foreach (var box in Boxes)
            {
                box.UpdateFormula(ID,newID);
            }
        }

        internal void AddToText(string p)
        {
            foreach (var box in Boxes.Where((s)=>(s.isEdited)))
            {
                box.Append(p);
            }
        }
    }
}
