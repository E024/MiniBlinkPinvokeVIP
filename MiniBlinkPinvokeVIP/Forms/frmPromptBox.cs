using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiniBlinkPinvokeVIP.Forms
{
    public partial class frmPromptBox : Form
    {
        public delegate void UserInput(string msg, bool isOK);
        public event UserInput UserInputEvent;
        public frmPromptBox(string title, string defaultValue)
        {
            InitializeComponent();
        }
    }
}
