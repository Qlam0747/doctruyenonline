# Đọc Truyện Online

Ứng dụng đọc và đăng truyện online trên Windows, xây dựng bằng **C# WinForms** và tích hợp **Firebase** để xử lý đăng nhập, lưu dữ liệu truyện, chương, bình luận, thông báo, lịch sử đọc, yêu thích và chat giữa người dùng. Dự án cũng tích hợp **Google Gemini API** để hỗ trợ tìm kiếm truyện bằng AI và hỗ trợ tác giả viết nội dung.

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Chức năng chính](#chức-năng-chính)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Yêu cầu cài đặt](#yêu-cầu-cài-đặt)
- [Cấu hình Firebase và API](#cấu-hình-firebase-và-api)
- [Cách chạy project](#cách-chạy-project)
- [Một số màn hình](#một-số-màn-hình)
- [Ghi chú khi đăng nhập](#ghi-chú-khi-đăng-nhập)
- [Hướng phát triển](#hướng-phát-triển)

## Giới thiệu

**Đọc Truyện Online** là ứng dụng desktop cho phép người dùng đọc truyện, tìm kiếm truyện, theo dõi truyện yêu thích, lưu lịch sử đọc, bình luận, đánh giá, chat với người dùng khác và đăng tải truyện/chương mới. Ứng dụng phù hợp với mô hình một nền tảng đọc truyện đơn giản, có phân quyền theo người dùng và lưu trữ dữ liệu trên Firebase Realtime Database.

## Chức năng chính

### Người dùng

- Đăng ký tài khoản bằng email và mật khẩu.
- Đăng nhập bằng email/mật khẩu.
- Đăng nhập bằng Google.
- Đăng nhập bằng Facebook.
- Quên mật khẩu và gửi email đặt lại mật khẩu.
- Chỉnh sửa thông tin cá nhân.
- Cập nhật ảnh đại diện và tiểu sử.

### Truyện và chương

- Hiển thị danh sách truyện ở trang chủ.
- Xem thông tin chi tiết truyện.
- Đọc nội dung chương truyện.
- Tăng lượt xem truyện và chương.
- Đăng truyện mới.
- Thêm chương mới cho truyện đã đăng.
- Quản lý trạng thái truyện, ví dụ: đang tiến hành.

### Tương tác

- Yêu thích truyện.
- Theo dõi tác giả.
- Đánh giá truyện.
- Bình luận truyện.
- Thích bình luận.
- Lưu bookmark theo dòng/đoạn khi đọc truyện.
- Lưu lịch sử đọc.
- Nhận thông báo khi có tương tác hoặc cập nhật liên quan.

### Chat

- Xem danh sách người dùng.
- Nhắn tin giữa các người dùng.
- Gửi tin nhắn văn bản.
- Gửi file trong tin nhắn.
- Đánh dấu tin nhắn chưa đọc.
- Chặn hoặc bỏ chặn người dùng.
- Xóa cuộc trò chuyện.

### AI

- Tìm kiếm truyện bằng AI dựa trên mô tả, thể loại hoặc ý tưởng người dùng nhập.
- Hỗ trợ tác giả viết tiếp nội dung chương.
- Hỗ trợ trau chuốt đoạn văn.
- Gợi ý ý tưởng/cốt truyện cho chương mới.

## Công nghệ sử dụng

- **Ngôn ngữ:** C#
- **Giao diện:** Windows Forms
- **Framework:** .NET Framework 4.8
- **Database:** Firebase Realtime Database
- **Authentication:** Firebase Authentication
- **AI:** Google Gemini API
- **Thư viện giao diện:** ReaLTaiizor
- **Xử lý JSON:** Newtonsoft.Json
- **OAuth:** Google APIs, Facebook OAuth

## Cấu trúc thư mục

```text
.
├── Design/                         # Ảnh thiết kế/giao diện minh họa
├── client_firebase/                # Source code chính của ứng dụng WinForms
│   ├── AppConfig.cs                # Đọc cấu hình từ config.json
│   ├── AuthSession.cs              # Lưu phiên đăng nhập hiện tại
│   ├── FirebaseAuthService.cs      # Xử lý đăng ký, đăng nhập, reset mật khẩu, OAuth
│   ├── FirebaseDatabaseService.cs  # Xử lý dữ liệu truyện, user, chat, bookmark, comment...
│   ├── GeminiService.cs            # Tích hợp Gemini API
│   ├── MainForm.cs                 # Form chính sau khi đăng nhập
│   ├── dangnhap.cs                 # Màn hình đăng nhập
│   ├── dangky.cs                   # Màn hình đăng ký
│   ├── Forget_verify.cs            # Màn hình quên mật khẩu
│   ├── UC_Home.cs                  # Trang chủ
│   ├── UC_Search.cs                # Tìm kiếm truyện
│   ├── UC_BookDetail.cs            # Chi tiết truyện
│   ├── UC_Reading.cs               # Đọc truyện
│   ├── UC_Library.cs               # Thư viện/yêu thích/lịch sử
│   ├── UC_Chat.cs                  # Chat
│   ├── UC_Notification.cs          # Thông báo
│   ├── UC_Upload.cs                # Đăng truyện
│   └── Resources/                  # Icon và tài nguyên giao diện
├── packages/                       # Các package NuGet đã tải về
├── client_firebase.slnx            # File solution
└── README.md
```

## Yêu cầu cài đặt

Trước khi chạy project, cần chuẩn bị:

- Windows 10/11.
- Visual Studio 2022 hoặc Visual Studio hỗ trợ .NET Framework.
- .NET Framework 4.8 Developer Pack.
- Tài khoản Firebase.
- Firebase project đã bật Authentication và Realtime Database.
- API key cho Google Gemini nếu muốn dùng chức năng AI.

## Cấu hình Firebase và API

Project đọc cấu hình từ file `config.json`. File này cần đặt trong thư mục:

```text
client_firebase/config.json
```

Nội dung mẫu:

```json
{
  "FirebaseApiKey": "YOUR_FIREBASE_API_KEY",
  "FirebaseDatabaseUrl": "https://your-project-id-default-rtdb.firebaseio.com",
  "GoogleClientId": "YOUR_GOOGLE_CLIENT_ID",
  "GoogleClientSecret": "YOUR_GOOGLE_CLIENT_SECRET",
  "FacebookAppId": "YOUR_FACEBOOK_APP_ID",
  "FacebookClientSecret": "YOUR_FACEBOOK_CLIENT_SECRET",
  "BackendRevokeUrl": "",
  "GeminiApiKey": "YOUR_GEMINI_API_KEY"
}
```


### Bật Authentication trong Firebase

Vào Firebase Console:

```text
Build → Authentication → Sign-in method
```

Bật các phương thức cần dùng:

- Email/Password
- Google
- Facebook nếu dùng đăng nhập Facebook

Sau đó vào:

```text
Build → Authentication → Users
```

để kiểm tra tài khoản đã được tạo thành công hay chưa.

### Cấu hình Realtime Database

Vào Firebase Console:

```text
Build → Realtime Database
```

Tạo database và lấy URL dạng:

```text
https://your-project-id-default-rtdb.firebaseio.com
```

URL này sẽ được đưa vào trường `FirebaseDatabaseUrl` trong `config.json`.

## Cách chạy project

1. Clone hoặc tải project về máy.

```bash
git clone <repository-url>
```

2. Mở project bằng Visual Studio.

3. Mở file solution:

```text
client_firebase.slnx
```

hoặc mở trực tiếp project:

```text
client_firebase/client_firebase.csproj
```

4. Restore NuGet packages nếu Visual Studio yêu cầu.

5. Tạo file `config.json` trong thư mục `client_firebase/` theo mẫu ở trên.

6. Kiểm tra project đang chạy với target framework:

```text
.NET Framework 4.8
```

7. Build project.

8. Chạy ứng dụng bằng nút **Start** trong Visual Studio.

## Một số màn hình

Thư mục `Design/` chứa các ảnh giao diện minh họa của ứng dụng, gồm:

- Màn hình đăng nhập.
- Màn hình đăng ký.
- Màn hình quên mật khẩu.
- Trang chủ.
- Tìm kiếm truyện.
- Thông tin truyện và chương.
- Màn hình đọc truyện.
- Bookmark.
- Lịch sử đọc.
- Chat.
- Thông báo.
- Upload truyện và chương.

## Ghi chú khi đăng nhập

Firebase xem các phương thức đăng nhập là các provider riêng nhau.

Ví dụ:

- Nếu tài khoản được tạo bằng Google thì provider là `google.com`.
- Nếu tài khoản được tạo bằng email/mật khẩu thì provider là `password`.

Vì vậy, một email đăng nhập được bằng Google chưa chắc đăng nhập được bằng email/mật khẩu. Muốn đăng nhập bằng email/mật khẩu thì tài khoản phải được tạo bằng phương thức Email/Password hoặc được liên kết thêm provider `password`.

Khi gặp lỗi đăng nhập bằng email/mật khẩu, nên kiểm tra:

```text
Firebase Console → Authentication → Users → Provider
```

Nếu provider chỉ là Google thì tài khoản đó chưa có mật khẩu trong Firebase Authentication.

## Hướng phát triển

- Thêm phân quyền quản trị viên để duyệt truyện và quản lý người dùng.
- Thêm chức năng sửa/xóa truyện và chương đã đăng.
- Tối ưu giao diện đọc truyện trên nhiều kích thước màn hình.
- Thêm phân trang khi tải danh sách truyện, bình luận và tin nhắn.
- Cải thiện bảo mật Firebase rules.
- Tách API key ra khỏi source code và dùng file cấu hình mẫu.
- Thêm thống kê lượt đọc, lượt thích, đánh giá theo thời gian.
- Thêm tìm kiếm nâng cao theo thể loại, tác giả, trạng thái và độ phổ biến.

## Tác giả

Dự án được xây dựng phục vụ mục đích học tập và thực hành phát triển ứng dụng desktop kết hợp Firebase.
