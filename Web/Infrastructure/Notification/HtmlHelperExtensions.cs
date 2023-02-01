using System;
using System.Web;
using System.Web.Mvc;

namespace Simco.Infrastructure.Notification
{
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Render all messages that have been set during execution of the controller action.
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static HtmlString RenderMessages(this HtmlHelper htmlHelper)
		{
			var messages = String.Empty;
			foreach (var messageType in Enum.GetNames(typeof(MessageType)))
			{
				var message = htmlHelper.ViewContext.ViewData.ContainsKey(messageType)
								? htmlHelper.ViewContext.ViewData[messageType]
								: htmlHelper.ViewContext.TempData.ContainsKey(messageType)
									? htmlHelper.ViewContext.TempData[messageType]
									: null;

                //messages += string.Format(@"<script type=""text/javascript""> $(document).ready(function () {{ noty({{ layout: ""top"", type: ""warning"", text: ""Super warning message everybody run!"", dismissQueue: true, animation: {{ open: {{height: ""toggle""}}, close: {{height: ""toggle""}}, easing: ""swing"", speed: 500 }}, timeout: 0 }}); }}); </script>");
                if (message != null)
                    messages += string.Format(@"<script type=""text/javascript"">$(document).ready(function () {{ noty({{""text"":""{0}"",""layout"":""top"",""type"":""{1}"",""animateOpen"":{{""height"":""toggle""}},""animateClose"":{{""height"":""toggle""}},""speed"":400,""timeout"":5000,""closeButton"":false,""closeOnSelfClick"":true,""closeOnSelfOver"":false}}); }});</script>", message, messageType.ToLowerInvariant());

            }
			return MvcHtmlString.Create(messages);
		}

        public static string Truncate(string input, int length)
        {
            if (string.IsNullOrEmpty(input) || length < 0)
                return input;

            if (input.Length <= length)
            {
                return input;
            }

            return input.Substring(0, length) + "...";
        }
	}
}