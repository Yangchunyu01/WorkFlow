using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Performance.WebApp.ViewModels
{
    public class JsonMessage
    {
        #region Properties
        // / <summary>
        // / This id just use on ExecuteConfig function which need return a id to the stage.
        // / </summary>
        public string Id { get; set; }

        // / <summary>
        // / Recirect Url or Show Message directly.
        // / (Url or Msg)
        // / </summary>
        public JsonType JsonType { get; set; }

        // / <summary>
        // / Whether current operation is successfully
        // / (true or false)
        // / </summary>
        public bool Success { get; set; }

        // / <summary>
        // / Show Message for users
        // / </summary>
        public string Message { get; set; }

        // / <summary>
        // / Unique Code for locate corresponding json result
        // / </summary>
        public string JsonCode { get; set; }

        // / <summary>
        // / Redirect Url
        // / </summary>
        public string RedirectUrl { get; set; }

        // / <summary>
        // / Show Message After Redirecting another url
        // / </summary>
        public string RedirectMessage { get; set; }

        // / <summary>
        // / Whether the Exception was showed
        // / </summary>
        public bool MessageShowed { get; set; }
        #endregion

        #region Constructors
        public JsonMessage(bool success, string message, string id)
        {
            this.Success = success;
            this.Message = message;
            this.Id = id;
        }

        public JsonMessage(bool success, string message) : this(success, message, null)
        {
        }

        public JsonMessage() : this(false, null, null)
        {
        }
        #endregion
    }

    public enum JsonType
    {
        Url = 0,
        Msg,
        Exception
    }
}