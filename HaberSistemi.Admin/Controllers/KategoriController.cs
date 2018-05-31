using HaberSistemi.Admin.Class;
using HaberSistemi.Core.Infrastructure;
using HaberSistemi.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using HaberSistemi.Admin.CustomFilter;

namespace HaberSistemi.Admin.Controllers
{
    public class KategoriController : Controller
    {
        #region Kategori
        private readonly IKategoriRepository _kategoriRepository;

        public KategoriController(IKategoriRepository kategoriRepository)
        {
            _kategoriRepository = kategoriRepository;
        }
        #endregion

        [HttpGet]
        [LoginFilter]
        public ActionResult Index(int Sayfa = 1)
        {

            return View(_kategoriRepository.GetAll().OrderByDescending(x => x.ID).ToPagedList(Sayfa, 2));
        }
        #region Kategori Düzenle
        [HttpGet]
        public ActionResult Duzenle(int id)
        {
            Kategori dbKategori = _kategoriRepository.GetById(id);
            if (dbKategori == null)
            {
                throw new Exception("Kategori Bulunamadı");

            }
            SetKategoriListele();
            return View(dbKategori);
        }

        [HttpPost]
        [LoginFilter]
        public JsonResult Duzenle(Kategori kategori)
        { 
                Kategori dbKategori = _kategoriRepository.GetById(kategori.ID);
                dbKategori.AktifMi = kategori.AktifMi;
                dbKategori.KategoriAdi = kategori.KategoriAdi;
                dbKategori.ParentID = kategori.ParentID;
                dbKategori.URL = kategori.URL;
                _kategoriRepository.Save();
                return Json(new ResultJson { Success = true, Message = "Düzenleme işlemi Başarılı" });

            
           
        }

        #endregion

        #region Kategori Ekle
        [HttpGet]
        public ActionResult Ekle()
        {
            SetKategoriListele();
            return View();
        }
        [HttpPost]
        public JsonResult Ekle(Kategori kategori)
        {
            try
            {
                _kategoriRepository.Insert(kategori);
                _kategoriRepository.Save();
                return Json(new ResultJson { Success = true, Message = "Kategori Ekleme Başarılı" });
            }
            catch
            {
                return Json(new ResultJson { Success = false, Message = "Kategori Ekleme Başarısız" });
            }

        }
        #endregion
        #region Kategori Sil
        public JsonResult Sil(int ID)
        {
            Kategori dbKategori = _kategoriRepository.GetById(ID);
            if (dbKategori == null)
            {
                return Json(new ResultJson { Success = true, Message = "Kategori Bulunamadı" });
            }
            _kategoriRepository.Delete(ID);
            _kategoriRepository.Save();

            return Json(new ResultJson { Success = true, Message = "Kategori Silme İşleminiz Başarılı" });
        }
        #endregion
        #region Set Kategori
        public void SetKategoriListele()
        {
            var KategoriList = _kategoriRepository.GetMany(x => x.ParentID == 0).ToList();
            ViewBag.Kategori = KategoriList;
        }
        #endregion
    }
}