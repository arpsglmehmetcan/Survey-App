import qrcode
import os
import sys

def generate_qr(store_code):
    base_url = "http://192.168.1.33:5139/survey"
    url = f"{base_url}?storeCode={store_code}"

    # QR kodunu oluştur
    qr = qrcode.make(url)

    # Kayıt dosya yolunu belirle
    file_path = os.path.join("wwwroot", "qrcodes", f"{store_code}_qrcode.png")
    
    # Kayıt dizinini oluştur (eğer yoksa)
    os.makedirs(os.path.dirname(file_path), exist_ok=True)
    
    # Dosyayı kaydet
    qr.save(file_path)
    print(f"QR kod '{file_path}' olarak oluşturuldu.")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        store_code = sys.argv[1]
        generate_qr(store_code)
    else:
        print("Lütfen bir mağaza kodu girin.")
