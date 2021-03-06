﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ssGame
{
    public partial class frmSplash : Form
    {
        public frmSplash()
        {
            InitializeComponent();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            frmClient c = new frmClient();
            c.Show();            
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            GameHelper.Gui.Forms.frmServerBase frm = new GameHelper.Gui.Forms.frmServerBase(new ServerGame());
            frm.Show();

            //frmServer s = new frmServer();
            //s.Show();
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {

        }

        private void btnExample_Click(object sender, EventArgs e)
        {
            frmExample ex = new frmExample();
            ex.Show();
        }
    }
}
