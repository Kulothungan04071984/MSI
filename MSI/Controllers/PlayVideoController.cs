using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MSI.Models;
using System.Diagnostics;
using System.Management;

namespace MSI.Controllers
{
    public class PlayVideoController : Controller
    {
        private DataManagementcs _domainServices;

        public PlayVideoController(DataManagementcs domainServices)
        {
            _domainServices = domainServices;

        }
        //public IActionResult Privacy()
        //{
        //    string fileName = "sys1.pdf";  // Hard-code the file name for PDF
        //    string filePath = Path.Combine(@"\\192.168.1.181\sap_xmls\MSI_WO_Display\SYSTEM1\", fileName);

        //    // Check if the file exists
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        // Open the file as a stream for better memory management
        //        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //        return File(fileStream, "application/pdf");
        //    }
        //    else
        //    {
        //        // Return 404 if the file is not found
        //        return NotFound($"The file {fileName} was not found.");
        //    }
        //}

        public IActionResult VideoPlaying()
        {
            // Get the device name (machine name)
            string deviceName = Process_systemname();
           // string deviceName = "STPLPC903";

            // Fetch file path using DataAccess
            string filePath1 = _domainServices.getfilepath(deviceName);
            string filePath = $@"{filePath1}";

            // ------------Code for displaying video file 

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                // Set a flag or a message to indicate that the file was not found
                ViewBag.FileNotFound = true;
                return View();  // Return the view indicating the file is not found
            }
            else
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                // Explicitly set the Content-Type for the response
                Response.Headers.Add("Content-Type", "video/mp4");
                return File(fileStream, "video/mp4");  // Return the MP4 file as a response
            }


            //------------- Code for displaying pdf file 

            //if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            //{
            //    // Set a flag or a message to indicate that the file was not found
            //    ViewBag.FileNotFound = true;
            //    return View();  // Return the view indicating the file is not found
            //}

            //else
            //{
            //    // Open the file as a stream for better memory management
            //    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //    return File(fileStream, "application/pdf");
            //}
            

        }


        // Error handling for the app
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Get the machine's system name
        private string Process_systemname()
        {
            try
            {
                // Query to get system information (device name)
                string machineName = Environment.MachineName;
                Console.WriteLine("Machine Name: " + machineName);
                return machineName;
            }
            catch (ManagementException me)
            {
                Console.WriteLine("Error retrieving data: " + me.Message);
                return string.Empty;
            }
        }

    }
}
