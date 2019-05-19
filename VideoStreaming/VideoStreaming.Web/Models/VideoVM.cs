using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoStreaming.Web.Models
{
	public class VideoVM
	{
		public string Name { get; set; }

		public string ext { get; set; }

		public byte[] video { get; set; }

		public long id { get; set; }
	}
}