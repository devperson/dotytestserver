using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using DotyAppServer.DataAccess;
using DotyAppServer.Models;
using DotyAppServer.Models.Request;
using DotyAppServer.Models.Response;

namespace DotyAppServer.Controllers
{
    public class PhotosController : BaseApiController
    {
        [HttpGet]
        [ActionName("GetFiles")]
        public ListResponse<DocumentResponse> GetFiles(int lastId)
        {
            try
            {
                List<DocumentResponse> list;
                using (DataBaseContext db = new DataBaseContext())
                {
                    var docs = db.Documents.Where(d => d.Id > lastId).ToList();                    
                    list = docs.Where(d => File.Exists(d.Path))
                        .Select(d => new DocumentResponse
                        {
                            ID = d.Id,
                            Name = d.Name,
                            Link = this.ToUrl(d.Path),
                            Thumbnail = this.ToUrl(d.ThubnailPath)                            
                        }).ToList();

                    if (docs.Any(d => !File.Exists(d.Path)))
                    {
                        foreach (var doc in docs.Where(d => !File.Exists(d.Path)).ToList())
                            db.Documents.Remove(doc);
                        db.SaveChanges();
                    }
                }

                var listresponse = new ListResponse<DocumentResponse>();
                listresponse.ListResult = list;
                listresponse.Success = true;
                return listresponse;                
            }
            catch (Exception ex)
            {
                return this.ReturnFailed<ListResponse<DocumentResponse>>(ex);
            }
        }


        [MimeMultipart]
        [HttpPost]
        [ActionName("AddPhoto")]
        public async Task<NewDocumentResponse> AddPhoto()
        {
            try
            {
                var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
                
                //var ownerEmail = this.Request.Headers.FirstOrDefault(k => k.Key == "ownerEmail").Value.First();                                                
                // Read the MIME multipart asynchronously 
                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                var _localFileName = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                var thumbPath = this.CreateThumbnail(_localFileName);
                var doc = new Document();
                using (DataBaseContext db = new DataBaseContext())
                {
                    doc.Name = Path.GetFileName(_localFileName);
                    //doc.User = db.Users.Single(u => u.Email == ownerEmail);
                    doc.Path = _localFileName;
                    doc.ThubnailPath = thumbPath;                   
                    db.Documents.Add(doc);
                    db.SaveChanges();
                }
                // Create response
                return new NewDocumentResponse() { ID = doc.Id, Link = this.ToUrl(doc.Path), Thumbnail= this.ToUrl(doc.ThubnailPath), Success = true };
            }
            catch (Exception ex)
            {
                return new NewDocumentResponse { ErrorMessage = ex.ToString() };
            }
        }


        [HttpPost]
        [ActionName("DeletePhoto")]
        public ModelWithStatus DeletePhoto(int docID)
        {
            try
            {
                using (DataBaseContext db = new DataBaseContext())
                {
                    var doc = db.Documents.First(e => e.Id == docID);
                    db.Documents.Remove(doc);
                    db.SaveChanges();

                    if (File.Exists(doc.Path))
                        File.Delete(doc.Path);
                    if (File.Exists(doc.ThubnailPath))
                        File.Delete(doc.ThubnailPath);
                }                

                return ReturnSuccess();
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }





        [NonAction]
        private string CreateThumbnail(string originalImagePath)
        {
            var path = Path.GetDirectoryName(originalImagePath) + "\\Thumb_" + Path.GetFileName(originalImagePath);
            using (var originalImage = System.Drawing.Image.FromFile(originalImagePath))
            {                
                using (var resizedImage = originalImage.GetThumbnailImage(150, (150 * originalImage.Height) / originalImage.Width, null, IntPtr.Zero))//(110 * originalImage.Height) / originalImage.Width                
                {
                    using (var fs = new FileStream(path, FileMode.Create))
                        resizedImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            return path;     
        }
    }
}