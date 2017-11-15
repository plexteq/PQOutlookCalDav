﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface IContact : IHasParameters
    {
        string Name { get; set; }
        string Email { get; set; }
        string SentBy { get; set; }
        string Directory { get; set; }
        void Deserialize(string value, XNameValueCollection parameters);
    }
}
