using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ssGame
{
    public partial class frmServer : GameHelper.Gui.Forms.frmServerBase
    {
        public frmServer() : base(new ServerGame())
        {
            InitializeComponent();
        }
    }
}
