# GeniusOpenKasaIslemleri

![image](https://github.com/user-attachments/assets/632a1fbb-6376-454d-a63f-781e48b15ee6)


IpAdresleri ve Kaynak isimli klasörler programın çalıştırıldığı bilgisayarın C:\ dizininde olmalı.
ips.txtye ip adresleri girilmeli
Kaynak klasörü içerisinde fashion klasörüne fashion içerisine atılmak istenen dosya bırakılmalı
(DİKKAT FASHION KLASORUNE ATILAN DOSYANIN ADI PARAMETERS.BAT KASA VERSIYON JARI ALANINA YAZILIYOR.)
Kaynak klasörü içerisinde config klasörüne config içerisine atılmak istenen dosya bırakılmalı
Kaynak klasörü içerisinde lib klasörüne lib içerisine aktarılmak istenen dosya bırakılmalı

Eve teslim servis linki xx.xx.xx.xx\c\fashion\geniuspos.ini dosyası içeriğini düzenler.
Cp.bat > xx.xx.xx.xx\c\fashion\cp.bat dosyasının en altına istenilen jarın aktif edilmesini sağlar.
Texte dogukan.jar girerseniz;
cp.bat en alt satıra 
set CLASSPATH=%CLASSPATH%;%FASHION_HOME%\lib\dogukan.jar
yazar.

Not: 
programın çalıştırıldığı bilgisayarda smb1.0 paylaşım desteği açılmalı(program ekle kaldır kısmındaki gelişmiş Windows seçeneklerinden açılabiliyor) (xp cihazlara erişebilmek için)
tcx cihazlarda güvenlik duvarı kapalı olmalı.
Programın çalışması için .NET 6.0 ve üzeri bilgisayarınızda yüklü olmalı.Yüklü değilse program açılışında size bu sürümü yüklemeniz için bilgi veriyor.

Program açılırken masaüstünde log.xlsx dosyası oluşturuyor. Eğer bu dosya mevcut ise program açılışında içeriğinin temizlenmesini ister misiniz sorusu oluyor. Program açıkken ve işlem yapılırken arka planda log.xlsx dosyası açıksa hata alırsınız. Excel açıkken program içeriğine veri yazamıyor.

Log için excelde'ki ip adresi ve başarılı başarısız kısımlarını göz önünde bulundurmanız yeterlidir.



