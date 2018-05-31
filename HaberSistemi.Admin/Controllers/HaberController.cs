using HaberSistemi.Admin.CustomFilter;
using HaberSistemi.Core.Infrastructure;
using HaberSistemi.Data.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HaberSistemi.Admin.Controllers
{
    public class HaberController : Controller
    {
        #region Veritabanı
        private readonly IHaberRepository _haberRepository;
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IKategoriRepository _kategoriRepository;
        private readonly IResimRepository _resimRepository;
        #endregion


        public HaberController(IHaberRepository haberRepository, IKullaniciRepository kullaniciRepository, IKategoriRepository kategoriRepository, IResimRepository resimRepository)
        {
            _haberRepository = haberRepository;
            _kullaniciRepository = kullaniciRepository;
            _kategoriRepository = kategoriRepository;
            _resimRepository = resimRepository;
        }

        // GET: Haber
        [LoginFilter]
        public ActionResult Index(int Sayfa = 1)
        {
            var HaberListesi = _haberRepository.GetAll();

            return View(HaberListesi.OrderByDescending(x => x.ID).ToPagedList(Sayfa, 20));
        }
        #region Haber Ekle
        [HttpGet]
        [LoginFilter]

        public ActionResult Ekle()
        {
            SetKategoriListele();
            return View();
        }

        [HttpPost]
        [LoginFilter]
        public ActionResult Ekle(Haber haber, int KategoriID, HttpPostedFileBase VitrinResmi, IEnumerable<HttpPostedFileBase> DetayResim)
        {
            var SessionControl = HttpContext.Session["KullaniciId"];

            if (ModelState.IsValid) //Haber classı içindeki şartlar uyuyorsa bunları uygula
            {
                Kullanici kullanici = _kullaniciRepository.GetById(Convert.ToInt32(SessionControl));

                haber.KullaniciID = kullanici.ID;
                haber.KategoriID = KategoriID;
                if (VitrinResmi != null)
                {
                    string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                    string Uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                    string TamYol = "/External/Haber/" + DosyaAdi + Uzanti;
                    Request.Files[0].SaveAs(Server.MapPath(TamYol));
                    haber.Resim = TamYol;
                }
                _haberRepository.Insert(haber);
                _haberRepository.Save();

                string cokluResim = System.IO.Path.GetExtension(Request.Files[1].FileName);
                if (cokluResim != "")
                {
                    foreach (var file in DetayResim)
                    {
                        if (file.ContentLength > 0)
                        {
                            string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                            string Uzanti = System.IO.Path.GetExtension(Request.Files[1].FileName);
                            string TamYol = "/External/Haber/" + DosyaAdi + Uzanti;
                            file.SaveAs(Server.MapPath(TamYol));

                            var resim = new Resim
                            {
                                ResimUrl = TamYol,


                            };

                            resim.HaberID = haber.ID;
                            _resimRepository.Insert(resim);
                            _resimRepository.Save();

                        }
                    }
                }

                TempData["Bilgi"] = "Haber Ekleme Başarılı";
                return RedirectToAction("Index","Haber");
            }

            return View();
        }
        #endregion

        #region Haber Sil
        public ActionResult Sil(int id)
        {
            Haber dbHaber = _haberRepository.GetById(id);
            var dbDetayResim = _resimRepository.GetMany(x => x.HaberID == id);
            if(dbHaber==null)
            {
                throw new Exception("Haber Bulunamadı");
            }
           
            string file_name = dbHaber.Resim;
            string path = Server.MapPath(file_name); //Resimin yolu
            FileInfo file = new FileInfo(path); //resime erişme
            if(file.Exists) //dosya fiziksel olarak var mı kontrol edıyoruz varsa sılınıyor
            {
                file.Delete();
            }
            if(dbDetayResim!=null)
            {
                foreach(var item in dbDetayResim)
                {
                    string detayPath = Server.MapPath(item.ResimUrl);
                    FileInfo files = new FileInfo(detayPath);
                    if(files.Exists)
                    {
                        files.Delete();
                    }
                }
             
            }
            _haberRepository.Delete(id);
            _haberRepository.Save();

            TempData["Bilgi"] = "Haber Başarılı Olarak Silindi";

            return RedirectToAction("Index","Haber");
        }

        #endregion

        #region Set Kategori Listesi
        public void SetKategoriListele(object kategori = null)
        {
            var KategoriList = _kategoriRepository.GetMany(x => x.ParentID == 0).ToList();
            ViewBag.Kategori = KategoriList;

        }
        #endregion

        #region Durum Değiştirme
        public ActionResult Onay(int id)
        {
            Haber gelenHaber = _haberRepository.GetById(id);
            if(gelenHaber.AktifMi==true)
            {
                gelenHaber.AktifMi = false;
                _haberRepository.Save();
                TempData["Bilgi"] = "İşlem Başarılı";
                return RedirectToAction("Index", "Haber");
            }
            else if(gelenHaber.AktifMi==false)
            {
                gelenHaber.AktifMi = true;
                _haberRepository.Save();
                TempData["Bilgi"] = "İşlem Başarılı";
                return RedirectToAction("Index", "Haber");
            }
            return View();
        }
        #endregion

        #region Haber Düzenle
        [HttpGet]
        [LoginFilter]
        public ActionResult Duzenle(int id)
        {
            Haber gelenHaber = _haberRepository.GetById(id);
            if(gelenHaber==null)
            {
                throw new Exception("Haber Bulunamadı");
            }
            else
            {
                SetKategoriListele();
                return View(gelenHaber);  
            }
            
        }
        [HttpPost]
        [LoginFilter]
        public ActionResult Duzenle(Haber haber, int KategoriID, HttpPostedFileBase VitrinResmi,IEnumerable<HttpPostedFileBase>DetayResim)
        {
            Haber gelenHaber = _haberRepository.GetById(haber.ID);
            gelenHaber.Aciklama = haber.Aciklama;
            gelenHaber.AktifMi = haber.AktifMi;
            gelenHaber.Baslik = haber.Baslik;
            gelenHaber.KisaAciklama = haber.KisaAciklama;
            gelenHaber.KategoriID = KategoriID;

            if(VitrinResmi!=null)
            {
                string dosyaAdi = gelenHaber.Resim;
                string dosyaYolu = Server.MapPath(dosyaAdi);
                FileInfo dosya = new FileInfo(dosyaYolu);
                if(dosya.Exists)
                {
                    dosya.Delete();
                }

                string file_name = Guid.NewGuid().ToString().Replace("-", "");  //rastgele isim olusturma
                string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string TamYol = "/External/Haber/" + file_name + uzanti;
                Request.Files[0].SaveAs(Server.MapPath(TamYol));
                gelenHaber.Resim = TamYol;
            }
            string CokluResim = System.IO.Path.GetExtension(Request.Files[1].FileName);
            if(CokluResim!="")
            {
                foreach(var dosya in DetayResim)
                {
                    string file_name = Guid.NewGuid().ToString().Replace("-", "");  //rastgele isim olusturma
                    string uzanti = System.IO.Path.GetExtension(Request.Files[1].FileName);
                    string TamYol = "/External/Haber/" + file_name + uzanti;
                    Request.Files[1].SaveAs(Server.MapPath(TamYol));
                    var img = new Resim
                    {
                        ResimUrl=TamYol
                    };
                    img.HaberID = gelenHaber.ID;
                    img.EklenmeTarihi = DateTime.Now;
                    _resimRepository.Insert(img);
                    _resimRepository.Save();
                }
            }
            _haberRepository.Save();
            TempData["Bilgi"] = "Güncelleme İşlemi Başarılı";
            return RedirectToAction("Index","Haber");
        }
        #endregion
    }
}