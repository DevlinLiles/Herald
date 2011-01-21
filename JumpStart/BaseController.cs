using System;
using System.Web.Mvc;
using Castle.Core.Logging;

namespace JumpStart
{
    public abstract class BaseController : Controller, ILoggingController
    {
        public ILogger Logger { get; set; }

        public BaseController()
        {
            Logger = new NullLogger();
        }
    }
}
