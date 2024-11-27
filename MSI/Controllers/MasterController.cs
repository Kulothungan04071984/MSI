using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MSI.Models;

namespace MSI.Controllers
{
	public class MasterController : Controller
	{
		private DataManagementcs _domainServices;
      
        // private readonly IWebHostEnvironment _webHostEnvironment;, IWebHostEnvironment webHostEnvironment
        public MasterController(DataManagementcs domainServices)
        {
            _domainServices = domainServices;
            //_webHostEnvironment = webHostEnvironment;
           // _iinsertDetails = iinsertDetails;
        }
        public IActionResult MasterDetails()
		{
            UploadFileDetails uploadFileDetails = new UploadFileDetails();
			var domainSid = _domainServices.GetDomainSid();
			//var systemName=_domainServices.GetAllConnectedSystemNames();
			ViewBag.DomainSid = domainSid;
            //ViewBag.computerName = systemName;
            uploadFileDetails.lstSystem = _domainServices.getSystemNames();

            return View(uploadFileDetails);
		}

        [HttpPost]
        public async Task<IActionResult> MasterDetails(IFormFile file,UploadFileDetails uploadFileDetails)
        {
            int result = 0;
            UploadFileDetails objupload =new UploadFileDetails();
            try
            {
                if (file != null && file.Length > 0)
                {
                    var path = "\\\\192.168.1.188\\MSI_Videos";
                    //var uploadVideoFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uploadVideoFile = Path.Combine(path, "uploads");
                    if (Directory.Exists(path))
                    {
                        //Directory.CreateDirectory(uploadVideoFile);
                        var filePath = Path.Combine(uploadVideoFile, file.FileName);
                        using (var filestream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(filestream);
                        }
                        var thumbnailPath = Path.Combine(uploadVideoFile, $"{Path.GetFileNameWithoutExtension(file.FileName)}.jpg");
                        ExtractThumbnail(filePath, thumbnailPath);
                        var uploadDetails = new UploadFileDetails();
                        uploadDetails.systemid = string.IsNullOrEmpty(uploadFileDetails.systemid.ToString()) ? 0 : uploadFileDetails.systemid;
                        uploadDetails.filepath = filePath;
                        uploadDetails.uploaddatetime = DateTime.Today.ToString();
                        uploadDetails.uploadEmployee = "70192";
                        //uploadFileDetails.systemname = uploadFileDetails.lstSystem.Where(a => a.Value == uploadFileDetails.systemid.ToString()).Select(a => a.Text.ToString()).FirstOrDefault();
                        //uploadFileDetails.systemname = "";
                        result = _domainServices.uploaddatainserted(uploadDetails);
                        if (result > 0) {
                            ViewBag.Message = "Video uploaded successfully";
                            ViewBag.ThumbnailPath = $"/uploads/{Path.GetFileName(thumbnailPath)}";
                            uploadFileDetails.lstSystem = _domainServices.getSystemNames();
                            uploadFileDetails.lstFileMappings = _domainServices.getFileMappingDetails();
                            objupload = uploadFileDetails;
                        }
                        else
                        {
                            ViewBag.Message = "Video Not uploaded ";
                            ViewBag.ThumbnailPath = "";
                        }
                      
                    }

                }
                else
                {
                    ViewBag.Message = "Please Give Proper Input";
                }

                return View(objupload);
               
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objupload);
            }
           
        }
        private void ExtractThumbnail(string videoPath, string thumbnailPath)
        {
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = videoPath };
            var outputFile = new MediaToolkit.Model.MediaFile { Filename = thumbnailPath };

            using (var engine = new MediaToolkit.Engine())
            {
                engine.GetMetadata(inputFile);
                var options = new MediaToolkit.Options.ConversionOptions { Seek = TimeSpan.FromSeconds(1) };
                engine.GetThumbnail(inputFile, outputFile, options);
            }
        }

      
    }
}

