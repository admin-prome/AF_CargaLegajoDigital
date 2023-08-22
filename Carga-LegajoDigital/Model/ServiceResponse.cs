using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LegajoDigitalDemoApp.Model
{
    internal class ServiceResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessages { get; set; }

        public APIDavoServiceResponse Result { get; set; }

        
    }
}
