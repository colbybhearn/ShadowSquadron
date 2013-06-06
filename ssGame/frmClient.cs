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
    public partial class frmClient : GameHelper.Gui.Forms.frmClientBase
    {
        public frmClient() : base(new ClientGame())
        {
            InitializeComponent();
        }
    }
}
