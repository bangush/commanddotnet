﻿using System;

namespace CommandDotNet.Attributes
{
    public class ArguementAttribute : Attribute
    {
        public string ShortName { get; set; }
        
        public string LongName { get; set; }
        
        public string Description { get; set; }

        public bool RequiredString { get; set; }
    }
}