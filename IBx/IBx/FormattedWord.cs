﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
//using Color = SharpDX.Color;
//using FontStyle = SharpDX.DirectWrite.FontStyle;
//using FontWeight = SharpDX.DirectWrite.FontWeight;

namespace IBx
{
    public class FormattedWord
    {
        public string text = "";
        public string color = "white";
        public string style = "normal";
        //public Color color = Color.White;
        //public FontStyle fontStyle = FontStyle.Normal;
        //public FontWeight fontWeight = FontWeight.Normal;
        public bool underlined = false;
        public string fontSize = "regular";

        public FormattedWord()
        {

        }
    }
}
