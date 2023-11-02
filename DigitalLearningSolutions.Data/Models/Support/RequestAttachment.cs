﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.Support
{
    public class RequestAttachment
    {
        public string? Id { get; set; }
        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FullFileName { get; set; }
        public byte[] Content { get; set; }
    }
}
