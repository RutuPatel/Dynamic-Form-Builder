using Form_Builder;
using Form_Builder.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Form_Builder.Controllers
{
    public class HomeController : Controller
    {
        DynamicFormEntities db = new DynamicFormEntities();

        public ActionResult Index(string id)
        {

            var model = new FormModel();
            if (!string.IsNullOrEmpty(id) && !id.Trim().ToUpper().Equals("HOME"))
            {
                ViewBag.IsForm = true;

                var formTable = db.Forms.Where(x => x.HashUrl == id).FirstOrDefault();

                if (formTable != null)
                {
                    foreach (var item in formTable.FormDetails)
                    {
                        FormFieldModel ffModel = new FormFieldModel
                        {
                            FormDetailId = item.FormDetailId,
                            FieldType = item.FieldType,
                            Id = (int)item.Id,
                            FieldName = item.FieldName,
                            IsRequired = item.Isrequired,
                            Icons = item.Icons,
                            SortIndex = item.Sortindex,
                            Class = item.Class,
                            IsRunMode = true,
                        };
                        model.FormId = formTable.FormId;
                        model.FormName = formTable.FormName;
                        model.FormDescription = formTable.FormDescription;
                        model.formField.Add(ffModel);
                    }
                }

            }
            else
            {
                model = null;
            }
            return View(model);
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserEmail"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Fields(int formId = 0, int userid = 0)
        {
            var fields = db.FormFields.ToList();
            ViewBag.FormField = db.FormFields.Select(x => new FormFieldModel
            {
                FieldName = x.FieldName,
                FieldType = x.FieldType,
                Id = x.Id,
                IsRequired = x.IsRequired,
                SortIndex = x.SortIndex,
                Icons = x.Icons,
                Class = x.Class
            }).ToList();
            var form = db.Forms.ToList();
            var model = new FormModel();

            var usertable = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (usertable != null)
            {
                model.UserId = usertable.UserId;
            }


            if (formId > 0)
            {

                var formTable = db.Forms.Where(x => x.FormId == formId).FirstOrDefault();
                if (formTable != null)
                {
                    foreach (var item in formTable.FormDetails)
                    {
                        FormFieldModel ffModel = new FormFieldModel
                        {
                            FormDetailId = item.FormDetailId,
                            FieldType = item.FieldType,
                            Id = (int)item.Id,
                            FieldName = item.FieldName,
                            IsRequired = item.Isrequired,
                            Icons = item.Icons,
                            SortIndex = item.Sortindex,
                            Class = item.Class
                        };
                        model.FormName = formTable.FormName;
                        model.FormDescription = formTable.FormDescription;
                        model.UserId = formTable.UserId;
                        model.formField.Add(ffModel);
                    }
                }
            }
            return View(model);
        }

        public ActionResult GetField(string type, int fieldId)
        {
            var field = db.FormFields.SingleOrDefault(x => x.Id == fieldId);
            ViewBag.Id = fieldId;
            ViewBag.Type = type;
            return PartialView("FormField", new FormFieldModel { FieldType = field.FieldType, Id = field.Id, FieldName = field.FieldName, IsRequired = field.IsRequired, Icons = field.Icons, SortIndex = field.SortIndex, Class = field.Class });
        }

        public ActionResult FormField(string type, int fieldId)
        {
            var field = db.FormFields.SingleOrDefault(x => x.Id == fieldId);
            ViewBag.Id = fieldId;
            ViewBag.Type = type;
            return PartialView("FormField", new FormFieldModel { FieldType = field.FieldType, Id = field.Id, FieldName = field.FieldName, IsRequired = field.IsRequired, Icons = field.Icons, SortIndex = field.SortIndex, Class = field.Class });
        }


        public ActionResult Submit()
        {
            FormFieldModel model = new FormFieldModel();
            return View("Thanks");
        }


        [HttpPost]
        public ActionResult Submit(FormModel model)
        {

            var ExistingForm = db.Forms.FirstOrDefault(x => x.FormId == model.FormId);
            if (ExistingForm != null)
            {
                ExistingForm.FormName = model.FormName;
                ExistingForm.FormDescription = model.FormDescription;
                ExistingForm.UserId = Convert.ToInt32(Session["UserID"].ToString());
                foreach (var fields in ExistingForm.FormDetails.ToList())
                {
                    db.FormDetails.Remove(fields);
                }

                foreach (var fields in model.formField)
                {

                    FormDetail formdetails = new FormDetail();
                    formdetails.FormId = model.FormId;
                    formdetails.Id = fields.Id;
                    formdetails.FieldType = fields.FieldType;
                    formdetails.FieldName = fields.FieldName;
                    formdetails.Isrequired = fields.IsRequired;
                    formdetails.Sortindex = fields.SortIndex;
                    formdetails.Icons = fields.Icons;
                    formdetails.Class = fields.Class;
                    ExistingForm.FormDetails.Add(formdetails);
                }
                db.SaveChanges();
                return Json(new { IsSuccess = true, Message = "Your " + @model.FormName + " Form is successfully Saved" });
            }
            else
            {
                Form userform = new Form();
                userform.FormName = model.FormName;
                userform.FormDescription = model.FormDescription;
                userform.FormId = model.FormId;
                userform.UserId = Convert.ToInt32(Session["UserID"].ToString());
                db.Forms.Add(userform);
                db.SaveChanges();
                FormDetail formdetails = new FormDetail();
                foreach (var fields in model.formField)
                {
                    formdetails.FormId = userform.FormId;
                    formdetails.Id = fields.Id;
                    formdetails.FieldType = fields.FieldType;
                    formdetails.FieldName = fields.FieldName;
                    formdetails.Isrequired = fields.IsRequired;
                    formdetails.Sortindex = fields.SortIndex;
                    formdetails.Icons = fields.Icons;
                    formdetails.Class = fields.Class;
                    db.FormDetails.Add(formdetails);
                    db.SaveChanges();
                }
            }
            return Json(new { IsSuccess = true, Message = "Your " + @model.FormName + " Form is successfully Saved" });
        }


        public ActionResult MyForms()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            try
            {
                var UserID = Convert.ToInt32(Session["UserID"].ToString());
                List<FormModel> form = new List<FormModel>();
                var FormItem = db.Forms.Where(X => X.UserId == UserID).ToList();
                if (FormItem.Count > 0)
                {
                    foreach (var item in FormItem)
                    {

                        FormModel displayform = new FormModel();
                        displayform.UserId = item.UserId;
                        displayform.FormId = item.FormId;
                        displayform.FormName = item.FormName;
                        displayform.FormDescription = item.FormDescription;
                        displayform.FormUrl = item.FormUrl;
                        displayform.HashUrl = item.HashUrl;
                        displayform.FieldCount = db.FormDetails.Where(x => x.FormId == item.FormId).Count();
                        form.Add(displayform);
                    }
                }
                return PartialView(form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult DeleteForm(int FormId)
        {
            Form deleteform = new Form();
            var DeleteItem = db.Forms.SingleOrDefault(x => x.FormId == FormId);
            if (DeleteItem != null)
            {
                db.Forms.Remove(DeleteItem);
                db.SaveChanges();
            }
            return RedirectToAction("MyForms");
        }

        public ActionResult GenerateUrl(int id)
        {
            var model = new FormModel();
            if (id > 0)
            {
                var url = db.Forms.SingleOrDefault(x => x.FormId == id);
                if (url != null)
                {
                    model.FormId = id;
                    model.FormName = url.FormName;
                    model.FormDescription = url.FormDescription;
                    model.FormUrl = url.FormUrl;
                }
            }
            return PartialView("GenerateUrl", model);

        }

        [HttpPost]
        public ActionResult GenerateUrl(FormModel model)
        {

            var url = db.Forms.SingleOrDefault(x => x.FormId == model.FormId);
            if (url != null)
            {
                url.FormId = model.FormId;
                url.FormName = model.FormName;
                url.FormDescription = model.FormDescription;
                string hashCode = String.Format("{0:X}", model.GetHashCode());
                url.HashUrl = hashCode;
                var baseurl = string.Format("http://dforms.aavitechsolutions.com/" + hashCode);
                url.FormUrl = baseurl;
                db.SaveChanges();
            }
            return RedirectToAction("MyForms");
        }

        private string ShareFormBody(string FormUrl)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/Shared/ShareFormTemplate.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("~Username", FormUrl);
            return body;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult MyForms(FormModel model)
        {
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("info.formbuilder@gmail.com");
            mm.Subject = "Sharable Form";
            mm.Body = ShareFormBody(model.FormUrl);
            if (model.Sharetoemail != null)
            {
                foreach (var address in model.Sharetoemail.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mm.To.Add(new MailAddress(address));
                }

            }
            else
            {
                mm.To.Add(new MailAddress("patelrutu1203@gmail.com"));
            }

            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            NetworkCredential NetworkCred = new NetworkCredential("info.formbuilder@gmail.com", "high@low123");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = NetworkCred;
            smtp.Timeout = 10000;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Send(mm);
            return RedirectToAction("MyForms");
        }

        public ActionResult Response()
        {
            try
            {
                //var FormId = Convert.ToInt32(Session["FormId"].ToString());
                List<ResponseFormModal> form = new List<ResponseFormModal>();
                //var FormItem = db.ResponseForms.Where(x=>x.FormId==FormId).ToList();
                var FormItem = db.ResponseForms.ToList();
                 
                if (FormItem.Count > 0)
                {
                    foreach (var item in FormItem)
                    {
                        ResponseFormModal displayform = new ResponseFormModal();
                        displayform.ResponseFormID = item.ResponseFormID;
                        displayform.FormName = item.FormName;
                        displayform.UserEmail = item.UserEmail;
                        displayform.Name = item.Name;
                        displayform.FormId = item.FormId;
                        form.Add(displayform);
                    }
                }
                return PartialView(form);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [HttpPost]
        public ActionResult GetResponse(FormModel model)
        {

            ResponseForm responseForm = new ResponseForm();
            Session["FormId"] = responseForm.FormId.ToString();
            responseForm.FormId = model.FormId;
            responseForm.FormName = model.FormName;
            responseForm.Name = model.FirstName + " " + model.LastName;
            responseForm.UserEmail = model.Email;
            db.ResponseForms.Add(responseForm);
            db.SaveChanges();

            Response data = new Response();
            data.UserEmail = model.Email;

            foreach (var fields in model.formField.OrderBy(x => x.SortIndex))
            {
                data.Id = fields.Id;
                data.FormId = model.FormId;
                data.FormName = model.FormName;
                data.FieldType = fields.FieldType;
                data.FieldName = fields.FieldName;
                var NameData = model.FirstName + " " + model.LastName;
                var Emaildata = model.Email;
                if (fields.FieldType == "Name")
                {

                    data.FieldResponse = NameData;

                }
                else if (fields.FieldType == "Email")
                {
                    data.FieldResponse = Emaildata;

                }
                else if (fields.FieldType == "Address")
                {
                    var value = string.Format(fields.StreetAdress + ", " + fields.Adressline2 + ", " + fields.City + "-" + fields.PinCode + ", " + model.state + ", " + model.country);
                    data.FieldResponse = value;
                }
                else
                {
                    data.FieldResponse = fields.FieldResponse;
                }
                db.Responses.Add(data);
                db.SaveChanges();
            }
            return RedirectToAction("Thanks");
        }


        public ActionResult Thanks()
        {
            return PartialView();
        }

        public ActionResult ExporttoExcel(int FormId)
        {
            List<ResponseFormModal> ResponseFormModalList = new List<ResponseFormModal>();
            var data = db.ResponseForms.Where(x => x.FormId == FormId).ToList();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    ResponseFormModal rm = new ResponseFormModal();
                    rm.ResponseFormID = item.ResponseFormID;
                    rm.FormId = item.FormId;
                    rm.FormName = item.FormName;
                    rm.UserEmail = item.UserEmail;
                    rm.Name = item.Name;
                    ResponseFormModalList.Add(rm);
                }
            }
             
            MemoryStream ms = new MemoryStream(ExportToExcel(ResponseFormModalList));
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }
        private byte[] ExportToExcel(List<ResponseFormModal> ResponseList)
        {
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add(string.Format("{0}", "Hiren"));
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.Scale = 100;
                ws.PrinterSettings.PaperSize = ePaperSize.A4;

                ws.PrinterSettings.BottomMargin = (decimal)0.5;
                ws.PrinterSettings.TopMargin = (decimal)0.5;
                ws.PrinterSettings.LeftMargin = (decimal)0.2;
                ws.PrinterSettings.RightMargin = (decimal)0.2;
                ws.PrinterSettings.HeaderMargin = (decimal)0.3;
                ws.PrinterSettings.FooterMargin = (decimal)0.3; 

                var index = 1;
                if (ResponseList.Count > 0)
                {
                    ws.Cells[string.Format("A{0}", index)].Value = "ResponseFormID";
                    ws.Cells[string.Format("B{0}", index)].Value = "FormId";
                    ws.Cells[string.Format("C{0}", index)].Value = "FormName";
                    ws.Cells[string.Format("D{0}", index)].Value = "UserEmail";
                    ws.Cells[string.Format("E{0}", index)].Value = "Name";


                    foreach (var item in ResponseList)
                    {
                        ws.Cells[string.Format("A{0}", index)].Value = item.ResponseFormID;
                        ws.Cells[string.Format("B{0}", index)].Value = item.FormId;
                        ws.Cells[string.Format("C{0}", index)].Value = item.FormName;
                        ws.Cells[string.Format("D{0}", index)].Value = item.UserEmail;
                        ws.Cells[string.Format("E{0}", index)].Value = item.Name;
                        index++;
                    }
                } 
                ws.Cells[string.Format("A{0}:E{0}", 1, index)].Style.Font.Bold = true;
                ws.Cells[string.Format("A{0}:E{0}", 1, index)].Style.Font.Name = "Trebuchet MS";
                ws.Cells[string.Format("A{0}:E{0}", 1, index)].Style.Font.Size = 10;
                ws.Cells[string.Format("A{0}:E{0}", 1, index)].AutoFitColumns();
               HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=Excel.xlsx");
                return pck.GetAsByteArray();
            }
        }
        private string SaveFormPopup()
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/Shared/SaveFormPopup.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }


        public void SendMail(string ToEmail, string Body, bool IsBodyHtml, string Subject)
        {
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("info.formbuilder@gmail.com");
            mm.Subject = Subject;
            mm.IsBodyHtml = IsBodyHtml;
            mm.Body = string.Format(Body);
            mm.To.Add(new MailAddress(ToEmail));
            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = true;
            smtp.Send(mm);
            return;
        }

        private string SignUpBody(string username)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/Shared/SignUpTemplate.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("~Username", username);
            return body;
        }

        [AllowAnonymous]
        public ActionResult Signup()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Signup(UserModel model)
        {
            var existingUser = db.Users.Where(x => x.Email == model.Email && x.UserName == model.UserName);
            if (existingUser.Any())
            {
                ModelState.AddModelError("UserName", "Username or Email already exist");
            }
            else
            {

                User Userdetails = new User();
                Guid guid = Guid.NewGuid();
                Userdetails.GUID = guid;
                Userdetails.UserName = model.UserName;
                Userdetails.Email = model.Email;
                Userdetails.Password = model.Password;
                db.Users.Add(Userdetails);
                db.SaveChanges();

                var ToEmail = model.Email;
                var Body = string.Format("<html><body style='border:7px solid #ef6556'><div style='background-color:#34495e'><center><img src='http://www.esolutionsgroup.ca/en/resourcesGeneral/form_builder_logo.png'><center></div><p align='center'>Your account has been succesfully created click below to Activate your account</p><br /><center><a style='font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 2px; -webkit-border-radius: 2px; -moz-border-radius: 2px; background-color: #EB7035; border-top: 12px solid #EB7035; border-bottom: 12px solid #EB7035; margin-bottom: 10px; border-right: 18px solid #EB7035; border-left: 18px solid #EB7035; display: inline-block; margin-bottom: 10px;' href='" + string.Format("{0}://{1}/Home/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, guid) + "' target = '_blank'>Click Here to Verify your Account</a></center><br /></body></html>");
                var IsBodyHtml = true;
                var subject = "Account Activation";
                SendMail(ToEmail, Body, IsBodyHtml, subject);
                return RedirectToAction("SignUpConfirmation", "Home");
            }
            return PartialView();
        }

        public ActionResult Activation()
        {
            if (RouteData.Values["id"] != null)
            {
                Guid guid = new Guid(RouteData.Values["id"].ToString());
                User user = db.Users.Where(x => x.GUID == guid).FirstOrDefault();
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    user.IsActive = true;
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }
            return PartialView();

        }

        public ActionResult NotActivateAccount()
        {
            return PartialView();
        }

        public ActionResult SignUpConfirmation()
        {
            return PartialView();
        }



        private string LoginBody(string username)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/Shared/LoginTemplate.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("~Username", username);
            return body;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel model)
        {
            var loginCheck = db.Users.Where(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password)).FirstOrDefault();
            if (loginCheck != null)
            {
                if (loginCheck.IsActive == true)
                {

                    Session["UserEmail"] = loginCheck.Email.ToString();
                    Session["UserName"] = loginCheck.UserName.ToString();
                    Session["UserID"] = loginCheck.UserId.ToString();
                    return RedirectToAction("UserDashBoard");
                }
                if (loginCheck.IsActive == null)
                {
                    var guid = loginCheck.GUID;
                    var ToEmail = model.Email;
                    var Body = string.Format("<html><body style='border:7px solid #ef6556'><div style='background-color:#34495e'><center><img src='http://www.esolutionsgroup.ca/en/resourcesGeneral/form_builder_logo.png'><center></div><p align='center'>Your account has been succesfully created click below to Activate your account</p><br /><center><a style='font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 2px; -webkit-border-radius: 2px; -moz-border-radius: 2px; background-color: #EB7035; border-top: 12px solid #EB7035; border-bottom: 12px solid #EB7035; margin-bottom: 10px; border-right: 18px solid #EB7035; border-left: 18px solid #EB7035; display: inline-block; margin-bottom: 10px;' href='" + string.Format("{0}://{1}/Home/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, guid) + "' target = '_blank'>Click Here to Verify your Account</a></center><br /></body></html>");
                    //var Body = string.Format("<html><body><h2>Form Builder</h2><p>Your account has been succesfully created click below to Activate your account</p><br/><a style='font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 2px; -webkit-border-radius: 2px; -moz-border-radius: 2px; background-color: #EB7035; border-top: 12px solid #EB7035; border-bottom: 12px solid #EB7035; border-right: 18px solid #EB7035; border-left: 18px solid #EB7035; display: inline-block;' href='" + string.Format("{0}://{1}/Home/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, guid) + "' target='_blank'> Click Here to Verify your Account </a></body></html>");
                    var IsBodyHtml = true;
                    var subject = "Pending Account Actication";
                    SendMail(ToEmail, Body, IsBodyHtml, subject);
                    return RedirectToAction("NotActivateAccount", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("Email", "Username or Password Invalid");
            }
            return PartialView(loginCheck);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        private string ForgotBody(string username)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/Shared/ForgotpasswordTemplate.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("~Username", username);
            return body;
        }

        public ActionResult ForgotPassword()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(UserModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == model.Email);
            if (user != null)
            {
                var guid = user.GUID;
                var ToEmail = model.Email;
                var Body = string.Format("<html><body style='border:7px solid #ef6556'><div style='background-color:#34495e'><center><img src='http://www.esolutionsgroup.ca/en/resourcesGeneral/form_builder_logo.png'><center></div><p align='center'>Your reset link is created click below to reset your password</p><br /><center><a style='font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 2px; -webkit-border-radius: 2px; -moz-border-radius: 2px; background-color: #EB7035; border-top: 12px solid #EB7035; border-bottom: 12px solid #EB7035; margin-bottom: 10px; border-right: 18px solid #EB7035; border-left: 18px solid #EB7035; display: inline-block; margin-bottom: 10px;' href='" + string.Format("{0}://{1}/Home/Reset/{2}", Request.Url.Scheme, Request.Url.Authority, guid) + "' target = '_blank'>Click Here to Reset your password</a></center><br /></body></html>");
                //var Body = string.Format("<html><body><h2>Form Builder</h2><p>reset your password</p><br/><a style='font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 2px; -webkit-border-radius: 2px; -moz-border-radius: 2px; background-color: #EB7035; border-top: 12px solid #EB7035; border-bottom: 12px solid #EB7035; border-right: 18px solid #EB7035; border-left: 18px solid #EB7035; display: inline-block;' href='" + string.Format("{0}://{1}/Home/Reset/{2}", Request.Url.Scheme, Request.Url.Authority, guid) + "' target='_blank'> Click Here to Reset Password</a></body></html>");
                var IsBodyHtml = true;
                var subject = "Password Reset link";
                SendMail(ToEmail, Body, IsBodyHtml, subject);
                return RedirectToAction("ForgotPasswordConfirmation", "Home");
            }
            else
            {
                ModelState.AddModelError("Email", "Please provide the correct email address!");
            }
            return PartialView();
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return PartialView();
        }

        public ActionResult Reset()
        {
            UserModel userinform = new UserModel();
            if (RouteData.Values["id"] != null)
            {
                Guid guid = new Guid(RouteData.Values["id"].ToString());
                User user = db.Users.Where(x => x.GUID == guid).FirstOrDefault();
                if (user != null)
                {
                    userinform.Email = user.Email;
                    userinform.GUID = user.GUID;
                    userinform.UserName = user.UserName;
                }
            }
            return PartialView(userinform);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(UserModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == model.Email);
            if (user != null)
            {
                user.Password = model.Password;
                db.SaveChanges();
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ModelState.AddModelError("Email", "Please provide the correct email address!");
            }
            return PartialView();
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