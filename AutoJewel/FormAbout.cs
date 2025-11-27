/***********************************************************************
Author:     Wudi <wudicgi@gmail.com>
License:    GPL (GNU General Public License)
Copyright:  2011 Wudi Labs
Link:       http://blog.wudilabs.org/entry/d26e8ee5/
***********************************************************************/

/***********************************************************************
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

The GNU General Public License can be found at
http://www.gnu.org/copyleft/gpl.html
***********************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WudiLabs.AutoJewel
{
    /// <summary>
    /// A simple About dialog that displays application information and a website link.
    /// </summary>
    public partial class FormAbout : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormAbout"/> class.
        /// </summary>
        public FormAbout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles clicks on the website link label and launches the default browser.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">Link click event arguments.</param>
        private void linkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.linkWebsite.Text);
        }

        /// <summary>
        /// Handles the OK button click and closes the dialog.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">Event arguments.</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
