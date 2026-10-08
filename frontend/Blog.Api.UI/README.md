# NCA

Vue 3, Vite, JavaScript, Vue Router ve Pinia ile oluşturulmuş açık temalı kişisel blog. Gerçek ASP.NET Core API kullanır; mock API veya sahte içerik içermez.

## Kurulum ve çalıştırma

```sh
npm install
```

`.env.example` dosyasını `.env` olarak kopyalayın:

```env
VITE_API_BASE_URL=https://localhost:7226/api
```

Backend'i ayrıca Visual Studio'dan çalıştırın. Frontend: http://localhost:5173. Backend örneği: https://localhost:7226.

```sh
npm run dev
```

Yerel HTTPS sertifikasına tarayıcının güvenmesi ve backend CORS ayarlarının frontend adresine izin vermesi gerekir. API çalışmıyorsa hata ve tekrar deneme seçeneği gösterilir. Backend kodu değiştirilmez.

## Kontroller

```sh
npm run format
npm run lint
npm run build
npm run preview
```

## Yapı ve API sınırları

- `src/api`: Axios instance, hata parser'ı ve endpoint servisleri.
- `src/stores`: auth ve toast sistemi.
- `src/assets/styles`: merkezi tokenlar, açık tema ve responsive stiller.
- `/`: sayfalı yazılar, array response kullanan arama, kategori ve tag gösterimi.
- `/post/:id`: düz metin içerik, yorum listeleme ve giriş yapan kullanıcı için yorum ekleme.
- `/login`, `/register`, `/profile`: oturum işlemleri ve salt okunur profil.
- `/admin`: yalnızca Admin rolüne açık dashboard, yazı/kategori/tag yönetimi.

Tokenlar local/dev API sözleşmesine uygun olarak `nca_access_token` ve `nca_refresh_token` localStorage anahtarlarında saklanır. Eşzamanlı 401 yanıtları tek refresh isteğini paylaşır, yeni access ve refresh token kaydedilir. İstek yalnızca bir kez tekrar edilir. Çıkış API başarısız olsa da yerel oturumu temizler.

Yayın durumu DTO'da olmadığı için sahte durum badge'i gösterilmez. Yayınla ve Taslağa Al ayrı işlemlerdir. Taslak düzenlemek için `/Post/admin` listesi kullanılır. DTO kategori ID'si içermediğinden kategori adı mevcut kategori listesiyle eşleştirilir; eşleşme bulunamazsa açık seçim gerekir. Yorumlarda userId olmadığı için sahiplik tahmin edilmez; yorum düzenle/sil servisleri hazırdır ancak butonları gösterilmez. Kategori/tag filtreleme endpointi bulunmadığından bu bölümler yalnızca taxonomy gösterir.

Logo `/public/nca-logo.png` dosyasıdır ve favicon olarak da kullanılır. Orijinal görsel `public/nca-logo-source` içinde korunur. Sosyal URL'leri `src/config/site.js` içinde doldurun; boş bağlantılar render edilmez.

Production sunucusunda Vue Router history rotalarının `index.html` dosyasına yönlendirilmesi gerekir. API yolları bu yönlendirmeden hariç tutulmalıdır.
