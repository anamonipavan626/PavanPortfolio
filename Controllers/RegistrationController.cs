using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Net.Mail;
using System.Net;

namespace PavanPortfolio.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly string FolderPath =
            System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")) + "Models\\";

        //public IActionResult Registration()
        [HttpPost]
        public IActionResult Registration([FromBody] RegistrationRequest request)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string excelFilePath = Path.Combine(FolderPath, "SystemRegistrations.xlsx");

                if (!System.IO.File.Exists(excelFilePath))
                    excelFilePath = Path.Combine(System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory)) + "Models\\", "SystemRegistrations.xlsx");
                if (!System.IO.File.Exists(excelFilePath))
                    System.IO.File.Create(excelFilePath);

               var systemId = GetSystemId();
                var existingRecords = LoadExcelData(excelFilePath);

                if (existingRecords.Any(x => x.SystemId == systemId))
                {
                    return BadRequest("This system is already registered.and request also submited. Please use another system.");
                }

                var systemDetails = new SystemRegistration
                {
                    SystemId = systemId,
                    ConfigurationDetails = GetSystemConfiguration(),
                    RegisteredAt = DateTime.Now
                };

                SaveToExcel(systemDetails, excelFilePath);
                SendRequestTomail(request.Name, request.Email, request.Body);
                return Ok("Request sent to user and  your System successfully registered.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message+"  Exception Details Are "+ ex);
                return Ok(ex.Message+"  Exception Details Are "+ ex);
            }

        }

        private string GetSystemId()
        {
            string systemId = System.Environment.MachineName + "_" + System.Environment.OSVersion.VersionString;
            return systemId; // Generate a unique identifier
        }

        private string GetSystemConfiguration()
        {
            string osVersion = System.Environment.OSVersion.ToString();
            string machineName = System.Environment.MachineName;
            string userDomain = System.Environment.UserDomainName;

            return $"OS: {osVersion}, Machine: {machineName}, Domain: {userDomain}";
        }

        private List<SystemRegistration> LoadExcelData(string excelFilePath)
        {
            var records = new List<SystemRegistration>();

            if (!System.IO.File.Exists(excelFilePath)) return records;

            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) return records;

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    records.Add(new SystemRegistration
                    {
                        SystemId = worksheet.Cells[row, 1].Value?.ToString(),
                        ConfigurationDetails = worksheet.Cells[row, 2].Value?.ToString(),
                        RegisteredAt = DateTime.Parse(worksheet.Cells[row, 3].Value?.ToString())
                    });
                }
            }
            return records;
        }

        private void SaveToExcel(SystemRegistration systemDetails, string excelFilePath)
        {
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Registrations");

                // Add headers if they don't exist
                if (worksheet.Dimension == null)
                {
                    worksheet.Cells[1, 1].Value = "SystemId";
                    worksheet.Cells[1, 2].Value = "ConfigurationDetails";
                    worksheet.Cells[1, 3].Value = "RegisteredAt";
                }

                int lastRow = worksheet.Dimension?.End.Row ?? 1;
                worksheet.Cells[lastRow + 1, 1].Value = systemDetails.SystemId;
                worksheet.Cells[lastRow + 1, 2].Value = systemDetails.ConfigurationDetails;
                worksheet.Cells[lastRow + 1, 3].Value = systemDetails.RegisteredAt.ToString();

                package.Save();
            }
        }


        public static async void SendRequestTomail(string name,string Email,string Body)
        {
            string subject = "Reg : "+name+" Trying  to Contact You";
            string body = "Dear "+"\n"+Body+"\n\n\n"+"Regards," + "\n\n\n"+name + ",\n"+Email;
            string to = "anamonipavan626@gmail.com";// "babubattina @gmail.com";// "pratyusha9171 @gmail.com";
            string attachmentFilePath =  Path.Combine(System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")) + "Models\\", "SystemRegistrations.xlsx");
            try
            {

                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("anamonipavan626@gmail.com", "cfrmdzohdsafzpbi");
                    client.EnableSsl = true;

                    using (var message = new MailMessage("anamonipavan626@gmail.com", to))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;

                        if (!string.IsNullOrEmpty(attachmentFilePath))
                        {
                            var attachment2 = new Attachment(attachmentFilePath);
                            message.Attachments.Add(attachment2);
                        }

                        await client.SendMailAsync(message);
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

    }

    public class SystemRegistration
    {
        public string SystemId { get; set; }
        public string ConfigurationDetails { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
    public class RegistrationRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Body { get; set; }
        public SystemRegistration? SystemRegistration { get; set; }
    }
}
