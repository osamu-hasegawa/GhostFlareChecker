using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraDriver;

using System.Runtime.InteropServices;

namespace GhostFlareChecker
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }

        public void GetPreviewWindow(out IntPtr imagehandle, out int width, out int height)
        {
            imagehandle = this.Handle;
            width = this.Width;
            height = this.Height;
        }

        public void GetPreviewWindow(out int width, out int height)
        {
            width = this.Width;
            height = this.Height;
        }

    }
}
