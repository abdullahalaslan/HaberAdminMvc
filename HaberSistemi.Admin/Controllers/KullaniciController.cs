using HaberSistemi.Admin.Class;
using HaberSistemi.Admin.CustomFilter;
using HaberSistemi.Core.Infrastructure;
using HaberSistemi.Data.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HaberSistemi.Admin.Controllers
{
    public class KullaniciController : Controller
    {

        #region Kullanıcı
        private readonly IKullaniciRepository _kullaniciRepository;

        public KullaniciController(IKullaniciRepository kullaniciRepository)
        {
            _kullaniciRepository = kullaniciRepository;
        }
        #endregion

        // GET: Kullanici
        [LoginFilter]
        public ActionResult Kullanici(int Sayfa=1)
        {
            var KullaniciListe = _kullaniciRepository.GetAll();

            return View(KullaniciListe.OrderByDescending(x => x.ID).ToPagedList(Sayfa, 20));
        }
        #region Kullanıcı Ekle
        [HttpGet]
        public ActionResult Ekle()
        {
           
            return View();
        }
        [HttpPost]
        [LoginFilter]
        public JsonResult Ekle(Kullanici kullanici)
        {
            try
            {
                _kullaniciRepository.Insert(kullanici);
                _kullaniciRepository.Save();
                return Json(new ResultJson { Success = true, Message = "Kullanıcı Ekleme Başarılı" });
            }
            catch
            {
                return Json(new ResultJson { Success = false, Message = "Kullanıcı Ekleme Başarısız" });
            }

        }
        #endregion

    }
}