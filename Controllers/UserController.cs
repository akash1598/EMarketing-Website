using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        dbemarketingEntities2 db = new dbemarketingEntities2();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(tbl_user uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadingfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not be uploaded.....";

            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("login");
            }
            return View();
        }


        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "invalid username or password";
            }
            return View();
        }
        
        public ActionResult CreateAd()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categoryList = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase imgfile)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categoryList = new SelectList(li, "cat_id", "cat_name");
            string path = uploadingfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not be uploaded.....";

            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = pvm.pro_name;
                p.pro_price = pvm.pro_price;
                p.pro_image = path;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_des = pvm.pro_des;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("Index");
                
            }
            return View();

        }

        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
            
        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page,string search)
        {
            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList() ;
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);

        }

        public ActionResult ViewAd(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_des;
            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;
            return View(ad);
        }

        public ActionResult Signout()
        {

            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAd(int? id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();
            
            return View("Index");
        }



            public string uploadingfile(HttpPostedFileBase file)
            {
                Random r = new Random();
                string path = "-1";
                int random = r.Next();
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png") || extension.ToLower().Equals(".jfif"))
                    {
                        try
                        {
                            path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                            file.SaveAs(path);
                            path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        }
                        catch (Exception ex)
                        {
                            path = "-1";
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('only jpg,jpeg or png formats are acceptable.....');</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('Please select a file');</script>");
                    path = "-1";
                }
                return path;
            }
        }

    }

