//------------------ BEGIN LICENSE BLOCK ------------------
//
// Project : Green Code Lab Plugwyse Library
// Description :
// Author: Green Code Lab
// Website: http://greencodelab.fr
// Version: 1.0
// Supports: Windows
//
// Original project : http://plugwiselib.codeplex.com/
// Copyright (c) 2012 Green Code Lab
// Licensed under the GPL license.
// See http://www.gnu.org/licenses/gpl.html
//
//------------------- END LICENSE BLOCK -------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlugwiseLib.BLL.BC
{
    public class PlugwiseMessage
    {
        private string _owner;

        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        private int _type;

        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public PlugwiseMessage()
        {
        }
    }
}
