using System.Web;

namespace ZTOMessage
{
	/// <summary>
	/// ZTORMessage 的摘要说明
	/// </summary>
	public class ZTORMessage : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			context.Response.Write("Hello World");
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}