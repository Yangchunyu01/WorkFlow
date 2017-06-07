using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Performance.WebApp.ViewModels
{
    public class ProjectCreationStatusViewModel
    {
        public int All { get; set; }
        public int Finished { get; set; }
        public string NamePrefix { get; set; }
    }
}