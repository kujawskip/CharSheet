using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class ChangeableBox : TextBox,IPlaceable
    {
        public new int Width { get; set; }


        public new int Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
        public bool Visible { get; set; }

        public CharacterSheet ParentSheet { get; set; }

        public Canvas Panel { get; set; }
        protected MenuItem GenerateMenuItem(string name, RoutedEventHandler e, bool Checkable = false, RoutedEventHandler e1 = null)
        {
            var mi = new MenuItem()
            {
                IsCheckable = Checkable,
                Header = name,
            };
            if (!Checkable)
            {
                mi.Click += e;
            }
            else
            {
                mi.Checked += e;
                mi.Unchecked += e1;
            }
            return mi;
        }
        public bool isSizeChangeable { get; private set; }
        public void SetSize(int height, int width)
        {
            if (!isSizeChangeable) return;
            Height = height;
            Width = width;
            MinHeight = Height;
            MinWidth = Width;
            MaxHeight = Height;
            MaxWidth = Width;
        }
        public bool isSelected { get; set; }
        public bool CanMove { get; set; }
        public void Move(int _x, int _y)
        {
            if (!CanMove) return;
            X = _x;
            Y = _y;
            Canvas.SetTop(this, Y);
            Canvas.SetLeft(this, X);
        }
        public void Unfocus()
        {
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), Panel);
            Keyboard.ClearFocus();

        }
        public bool isToMove { get; set; }
        private void Move_UnClick(object sender, RoutedEventArgs e)
        {
            CanMove = true;
            Unfocus();
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            CanMove = false;
            Unfocus();
        }
        private void Font_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog F = new System.Windows.Forms.FontDialog();
            F.Font = new Font(new System.Drawing.FontFamily(FontFamily.FamilyNames[XmlLanguage.GetLanguage("en-us")]), (float)FontSize,
                (FontStyle == FontStyles.Italic ? System.Drawing.FontStyle.Italic : 0) | (FontWeight == FontWeights.Bold ? System.Drawing.FontStyle.Bold : 0));
            F.ShowDialog();
            UpdateFont(F.Font.Size, F.Font.Italic ? FontStyles.Italic : FontStyles.Normal, F.Font.Bold ? FontWeights.Bold : FontWeights.Normal, new FontFamily(F.Font.FontFamily.Name));
            Unfocus();
        }

        protected void UpdateFont(double fontSize, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
            FontWeight = fontWeight;
            FontStyle = fontStyle;
        }
        protected virtual void Clone_Click(object sender, RoutedEventArgs e)
        {
            ChangeableBox VB = new ChangeableBox(ParentSheet, Panel, new Point(X + 5, Y + 5));
            {
                Text = this.Text;
            };
     
            VB.UpdateFont(FontSize, FontStyle, FontWeight, FontFamily);
            Unfocus();

        }
        private void Size_Click(object sender, RoutedEventArgs e)
        {
            isSizeChangeable = false;
            Unfocus();
        }

        protected void HandleMouse(MouseButtonEventArgs e, bool Check)
        {
            if (e.Handled)
            {
                e.Handled = false;
                base.OnMouseDown(e);
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;


                isToMove = true;


            }

            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                e.Handled = true;
                isSelected = true;
            }
            else if (!Check)
            {
                base.OnMouseDown(e);
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
           HandleMouse(e,false);
           
        }

        public bool isEdited { get; set; }
        private void Size_UnClick(object sender, RoutedEventArgs e)
        {
            isSizeChangeable = true;
            Unfocus();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Unfocus();
            }
        }
        public ChangeableBox(CharacterSheet sheet, Canvas panel, Point location)
        {
            X = 0;
            Y = 0;
            FontFamily = new FontFamily("Arial");
            FontSize = 16;
            isSizeChangeable = true;
            CanMove = true;
            SetSize(25, 15);
            Panel = panel;
            Move((int)location.X, (int)location.Y);
            panel.Children.Add(this);
            ParentSheet = sheet;
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(GenerateMenuItem("Edit font", Font_Click));
            ContextMenu.Items.Add(GenerateMenuItem("Clone", Clone_Click));
            
            ContextMenu.Items.Add(GenerateMenuItem("Lockdown Size", Size_Click, true, Size_UnClick));
            ContextMenu.Items.Add(GenerateMenuItem("Lockdown Move", Move_Click, true, Move_UnClick));
        }
    }
}
