using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CharacterSheetEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ControlCreator(Point L,Canvas P );

        private CharacterSheet sheet;
        private Dictionary<String, ControlCreator> controlCreators; 
        public MainWindow()
        {
            InitializeComponent();
            controlCreators= new Dictionary<string, ControlCreator>();
            controlCreators.Add("ValueBox",CreateValueBox);
            controlCreators.Add("TextBox", CreateTextBox);
            sheet  = new CharacterSheet();
        }

        private void CreateValueBox(Point L, Canvas P)
        {
            ValueBox T = new ValueBox(sheet,P,L);
           

        }
        private void CreateTextBox(Point L, Canvas P)
        {
            ChangeableBox T = new ChangeableBox(sheet, P, L);


        }
        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled) return;
            Point P = e.GetPosition(MainCanvas);
            foreach (ChangeableBox vb in MainCanvas.Children)
                {
                    if (vb.isToMove)
                    {
                        vb.isToMove = false;
                        
                        vb.Move((int) (P.X ), (int) (P.Y));
                        
                    }
                }
            foreach (ChangeableBox vb in MainCanvas.Children)
            {
                if (vb.isSelected)
                {
                    vb.SetSize( (int)Math.Abs(P.Y - vb.Y),(int)Math.Abs(P.X - vb.X));
                    
                    vb.isSelected = false;
                }
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) return;
            foreach (ChangeableBox Child in MainCanvas.Children)
            {
               Child.Unfocus();
            }
           
            if (P.X >= MainCanvas.ActualWidth || P.Y >= MainCanvas.ActualHeight) return;
            var S = SelectableElements.SelectedItems;
          
            if (S != null && S.Count>0)
            {
                var s = S[0];
                string key = s.ToString().Split(' ')[1];
                if (controlCreators.ContainsKey(key))
                {
                    controlCreators[key](P,MainCanvas);
                }
                if(!Keyboard.IsKeyDown(Key.LeftCtrl)) SelectableElements.SelectedIndex = -1;
            }
        }

    
        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            Point P = e.GetPosition(MainCanvas);

            foreach (ChangeableBox vb in MainCanvas.Children)
            {
                if (vb.isSelected)
                {
                    vb.SetSize((int)Math.Abs(P.Y - vb.Y), (int)Math.Abs(P.X - vb.X));
                }
            }
            foreach (ChangeableBox vb in MainCanvas.Children)
                {
                    if (vb.isToMove)
                    {
                   
                        vb.Move((int) (P.X ), (int) (P.Y));
                       
                    }
                }
        }
    }
}
