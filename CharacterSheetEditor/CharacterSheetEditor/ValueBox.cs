using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CharacterSheetEditor.Interfaces;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using Point = System.Windows.Point;

namespace CharacterSheetEditor
{
    class ValueBox : ChangeableBox, IFormulable
    {
        public string Formula { get; set; }



        public string ID { get; set; }


      


        private void ChangeFormula(string s)
        {
            ParentSheet.Formulas.ChangeExpression(ID, s);
            Formula = s;


        }

       
        
      

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
           
            HandleMouse(e,true);
            if (e.Handled)
            {
                
               
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ParentSheet.AddToText("{" + ID + "}");
            }
            else
            {
                isEdited = true;
                e.Handled = true;
                Text = Formula;
                HandleMouse(e, true);
            }

        }

        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
        //        Keyboard.IsKeyDown(Key.LeftCtrl)) return;
        //        base.OnMouseDown(e);
        //}

        public void Append(string s)
        {
            Text += s;
        }


        public ValueBox(CharacterSheet sheet, Canvas panel, Point location) : base(sheet,panel,location)
        {
            
          
            
            
            ID = sheet.GetFreeTempID();
           

            
            sheet.Boxes.Add(this);
            Formula = "0";
            sheet.Formulas.Add(ID, Formula);
            ChangeText(Formula);
           
            ContextMenu.Items.Add(GenerateMenuItem("Change ID", ID_Click));
          
        }

       
        


        private void ChangeText(string s)
        {

            Text = s;

        }

        protected void UpdateText()
        {
            ChangeFormula(Text);
            ChangeText(ParentSheet.Formulas[ID].ToString(CultureInfo.InvariantCulture));
        }
        protected override void OnLostFocus(RoutedEventArgs eventArgs)
        {
            isEdited = false;
            base.OnLostFocus(eventArgs);

            UpdateText();
            ParentSheet.Update();

        }

       
      

      
        private void ChangeID(string newID)
        {
            if (ID == newID) return;
            if (ParentSheet.IsFreeID(newID))
            {
                ParentSheet.ChangeID(ID, newID);
                ID = newID;
            }
            else MessageBox.Show("ID already taken!", "ERROR");
        }
        protected override void Clone_Click(object sender, RoutedEventArgs e)
        {
            ValueBox VB = new ValueBox(ParentSheet, Panel, new Point(X + 5, Y + 5)) {Text = Formula};
            VB.UpdateText();
            VB.UpdateFont(FontSize,FontStyle,FontWeight,FontFamily);
            Unfocus();
            
        }
        private void ID_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Input New ID", "New ID", ID, -1, -1);
            ChangeID(input);
            Unfocus();
        }
       


        public void Update()
        {
            ChangeText(ParentSheet.Formulas[ID].ToString(CultureInfo.InvariantCulture));
        }


        public void UpdateFormula(string key1,string key2)
        {
            Formula = Formula.Replace("{" + key1 + "}", "{" + key2 + "}");
        }
    }
}
