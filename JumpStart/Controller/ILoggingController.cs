using System;
using System.Web.Mvc;
using Castle.Core.Logging;

namespace JumpStart
{
    public interface ILoggingController : IController
    {
        ILogger Logger { get; set; }
    }
}
