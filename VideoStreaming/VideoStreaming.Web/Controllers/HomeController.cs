using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoStreaming.Web.Models;

namespace VideoStreaming.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			//return View();
			List<VideoVM> list = new List<VideoVM>(); //List object of View Model class

			using (VideoStreamingEntities db = new VideoStreamingEntities())
			{
				//select list from database using linq query
				list = db.VideoSteams.Select(m => new VideoVM
				{
					ext = m.FileType,
					Name = m.FileName,
					id = m.SrNo
				}).ToList();
			}
			return View(list);
		}

		[HttpPost]
		public ActionResult Index(HttpPostedFileBase file)
		{
			VideoSteam video = new VideoSteam();

			if (file != null)
			{
				// store uploaded file to temporary folder
				string path = Server.MapPath("~/Upload/TempUpload/" + file.FileName);

				file.SaveAs(path);

				// Create new object of FFMpegConvertor
				var ffMpeg = new FFMpegConverter();
				var tname = Guid.NewGuid();

				// Path for converted videos
				string output = Server.MapPath("~/Upload/TempUpload/" + tname + ".mp4");

				// after conversion, video will be saved in Temporary folder.
				ffMpeg.ConvertMedia(path, output, Format.mp4);

				byte[] buffer = new byte[file.ContentLength];

				using (MemoryStream ms = new MemoryStream())
				{
					//open converted video file, read it and write it to buffer
					using (FileStream tempfile = new FileStream(output, FileMode.Open, FileAccess.Read))
					{
						buffer = new byte[tempfile.Length];
						tempfile.Read(buffer, 0, (int)tempfile.Length);
						ms.Write(buffer, 0, (int)tempfile.Length);
					}
				}

				string fname = Guid.NewGuid().ToString();
				string ftype = "mp4";

				// Store video in database
				using (VideoStreamingEntities db = new VideoStreamingEntities())
				{
					video.FileName = fname;
					video.FileType = ftype;
					video.Video = buffer;

					db.VideoSteams.Add(video);
					db.SaveChanges();
				}
			}
			return RedirectToAction("Index", "Home");
		}


		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}