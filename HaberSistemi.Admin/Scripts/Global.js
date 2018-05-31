function KategoriEkle()
{
    Kategori = new Object();
    Kategori.KategoriAdi = $("#kategoriAdi").val();
    Kategori.Url = $("#kategoriUrl").val();
    Kategori.AktifMi = $("#kategoriAktif").is(":checked");
    Kategori.ParentID = $("#ParentID").val();

    $.ajax({
        url: "/Kategori/Ekle",
        data: Kategori,
        type: "POST",
        datatype:"json",
        success: function (response) {
            if(response.Success)
            {
                bootbox.alert(response.Message, function () {
                    location.reload();
                });
            }
            else {
                bootbox.alert(response.Message, function () {
                    //geri dondugunde bır sey yapılmasını ıstenılırse buraya yazılır
                });
            }
        }
    })
}

function KategoriSil()
{
    var gelenID = $("#KategoriDelete").attr("data-id");
    $.ajax({
        url: '/Kategori/Sil/' + gelenID,
        type: "POST",
        datatype: 'json',
        success: function (response) {
            if(response.Success)
            {
                bootbox.alert(response.Message, function () {
                    location.reload();
                });

            }
            else {
                bootbox.alert(response.Message, function () {
                    //geri dondugunde bır sey yaptırabılırız.
                });
            }

        }

    })
}

function KategoriDuzenle()
{

    Kategori = new Object();
    Kategori.KategoriAdi = $("#kategoriAdi").val();
    Kategori.Url = $("#kategoriUrl").val();
    Kategori.AktifMi = $("#kategoriAktif").is(":checked");
    Kategori.ParentID = $("#ParentID").val();
    Kategori.ID = $("#ID").val();

    $.ajax({
        url: "/Kategori/Duzenle",
        data: Kategori,
        type: "POST",
        datatype: "json",
        success: function (response) {
            if (response.Success) {
                bootbox.alert(response.Message, function () {
                    location.reload();
                });
            }
            else {
                bootbox.alert(response.Message, function () {
                    //geri dondugunde bır sey yapılmasını ıstenılırse buraya yazılır
                });
            }
        }
    })
}

function KullaniciEkle() {
  
    Kullanici = new Object();
    Kullanici.AdSoyad = $("#AdSoyad").val();
    Kullanici.Email = $("#email").val();
    Kullanici.Sifre = $("#sifre").val();
    Kullanici.EklenmeTarihi = $("#EklenmeTarihi").val();
    Kullanici.Rol_ID = $("#RolID").val();
    Kullanici.AktifMi = $("#kullaniciAktif").is(":checked");

    $.ajax({
        url: "/Kullanici/Ekle",
        data: Kullanici,
        type: "POST",
        datatype: "json",
        success: function (response) {
            if (response.Success) {
                bootbox.alert(response.Message, function () {
                    location.reload();
                });
            }
            else {
                bootbox.alert(response.Message, function () {
                    //geri dondugunde bır sey yapılmasını ıstenılırse buraya yazılır
                });
            }
        }
    })
}


  