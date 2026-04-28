using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Drag
{
    public class OnConnectedEventArgs : EventArgs
    {
        public VectorPoint End_pos { get; set; }

        public OnConnectedEventArgs(VectorPoint end_pos)
        {
            End_pos = end_pos;
        }
    }
}
