using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace client_firebase
{
    public class BookModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverBase64 { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int Views { get; set; } = 0;
        public double Rating { get; set; } = 5.0;
        public int Likes { get; set; } = 0;
        public long UpdatedAt { get; set; } = 0;
        public string Status { get; set; } = "Đang tiến hành";
        public Dictionary<string, ChapterModel> Chapters { get; set; } = new Dictionary<string, ChapterModel>();
    }

    public class ChapterModel
    {
        public string Id { get; set; }
        public string ChapterNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long CreatedAt { get; set; }
        public int Views { get; set; } = 0;
    }

    public class UserModel
    {
        public string LocalId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Birthdate { get; set; }
        public string Avatar { get; set; }
    }

    public class MessageModel
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public long Timestamp { get; set; }
        public string FileType { get; set; } = "text"; // "text", "image", "file"
        public string FileBase64 { get; set; }
        public string FileName { get; set; }
    }

    public static class FirebaseDatabaseService
    {
        private static async Task<string> GetAsync(string path)
        {
            try
            {
                string dbUrl = AppConfig.FirebaseDatabaseUrl;
                if (string.IsNullOrWhiteSpace(dbUrl)) return null;
                dbUrl = dbUrl.TrimEnd('/');

                string idToken = AuthSession.FirebaseIdToken;
                if (string.IsNullOrEmpty(idToken)) return null;

                string url = $"{dbUrl}/{path}?auth={idToken}";
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Firebase GET error: " + ex.Message);
            }
            return null;
        }

        private static async Task<string> PostAsync(string path, object payload)
        {
            string dbUrl = AppConfig.FirebaseDatabaseUrl;
            if (string.IsNullOrWhiteSpace(dbUrl)) throw new Exception("Database URL is not configured.");
            dbUrl = dbUrl.TrimEnd('/');

            string idToken = AuthSession.FirebaseIdToken;
            if (string.IsNullOrEmpty(idToken)) throw new Exception("User is not authenticated.");

            string url = $"{dbUrl}/{path}?auth={idToken}";
            string json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, content);
                string responseStr = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return responseStr;
                throw new Exception("Firebase POST error: " + responseStr);
            }
        }

        private static async Task<string> PutAsync(string path, object payload)
        {
            string dbUrl = AppConfig.FirebaseDatabaseUrl;
            if (string.IsNullOrWhiteSpace(dbUrl)) throw new Exception("Database URL is not configured.");
            dbUrl = dbUrl.TrimEnd('/');

            string idToken = AuthSession.FirebaseIdToken;
            if (string.IsNullOrEmpty(idToken)) throw new Exception("User is not authenticated.");

            string url = $"{dbUrl}/{path}?auth={idToken}";
            string json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(url, content);
                string responseStr = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return responseStr;
                throw new Exception("Firebase PUT error: " + responseStr);
            }
        }

        public static string GetChatId(string uid1, string uid2)
        {
            if (string.IsNullOrEmpty(uid1) || string.IsNullOrEmpty(uid2)) return "default_chat";
            return string.Compare(uid1, uid2) < 0 ? $"{uid1}_{uid2}" : $"{uid2}_{uid1}";
        }

        public static async Task<List<UserModel>> GetAllUsersAsync()
        {
            var users = new List<UserModel>();
            string json = await GetAsync("users.json");
            if (string.IsNullOrEmpty(json) || json == "null") return users;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, UserModel>>(json);
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        if (kvp.Key == AuthSession.FirebaseLocalId) continue; // Skip self
                        var u = kvp.Value;
                        u.LocalId = kvp.Key;
                        if (string.IsNullOrEmpty(u.Username)) u.Username = u.Email ?? "Người dùng";
                        users.Add(u);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing users: " + ex.Message);
            }
            return users;
        }

        public static async Task<UserModel> GetCurrentUserProfileAsync()
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return null;
            string json = await GetAsync($"users/{AuthSession.FirebaseLocalId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return null;

            try
            {
                var user = JsonConvert.DeserializeObject<UserModel>(json);
                if (user != null)
                {
                    user.LocalId = AuthSession.FirebaseLocalId;
                }
                return user;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<List<MessageModel>> GetMessagesAsync(string partnerId)
        {
            var messages = new List<MessageModel>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(partnerId)) return messages;

            string chatId = GetChatId(AuthSession.FirebaseLocalId, partnerId);
            string json = await GetAsync($"messages/{chatId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return messages;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, MessageModel>>(json);
                if (dict != null)
                {
                    messages.AddRange(dict.Values);
                    messages.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing messages: " + ex.Message);
            }
            return messages;
        }

        public static async Task<bool> SendMessageAsync(string partnerId, string text)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(partnerId)) return false;

            if (await IsBlockedByEitherAsync(partnerId))
            {
                return false;
            }

            string chatId = GetChatId(AuthSession.FirebaseLocalId, partnerId);
            var message = new MessageModel
            {
                SenderId = AuthSession.FirebaseLocalId,
                ReceiverId = partnerId,
                Text = text,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                FileType = "text"
            };

            try
            {
                await PostAsync($"messages/{chatId}.json", message);
                await SetChatUnreadAsync(partnerId, true);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error sending message: " + ex.Message);
                return false;
            }
        }

        public static async Task<MessageModel> GetLastMessageAsync(string partnerId)
        {
            var msgs = await GetMessagesAsync(partnerId);
            if (msgs.Count > 0)
                return msgs[msgs.Count - 1];
            return null;
        }

        public static async Task<string> UploadBookAsync(string title, string description, string coverBase64, List<string> genres, string chapterNum, string chapterTitle, string chapterContent, string status = "Đang tiến hành")
        {
            try
            {
                string authorName = "Ẩn danh";
                var profile = await GetCurrentUserProfileAsync();
                if (profile != null && !string.IsNullOrEmpty(profile.Username))
                {
                    authorName = profile.Username;
                }

                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var book = new BookModel
                {
                    Title = title,
                    Description = description,
                    CoverBase64 = coverBase64,
                    Genres = genres,
                    AuthorId = AuthSession.FirebaseLocalId,
                    AuthorName = authorName,
                    Views = 0,
                    Rating = 5.0,
                    Likes = 0,
                    Status = status,
                    UpdatedAt = now
                };

                // Add first chapter
                var chapter = new ChapterModel
                {
                    ChapterNumber = chapterNum,
                    Title = chapterTitle,
                    Content = chapterContent,
                    CreatedAt = now
                };

                string response = await PostAsync("books.json", book);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                string generatedId = null;
                if (dict != null && dict.ContainsKey("name"))
                {
                    generatedId = dict["name"];
                }

                if (string.IsNullOrEmpty(generatedId))
                {
                    return "Error|Failed to parse book ID from response";
                }

                // Update book ID and upload the chapter
                await PutAsync($"books/{generatedId}/id.json", generatedId);
                await PostAsync($"books/{generatedId}/chapters.json", chapter);

                // Send notification to author's followers about the new book!
                string followersJson = await GetAsync($"author_followers/{AuthSession.FirebaseLocalId}.json");
                if (!string.IsNullOrEmpty(followersJson) && followersJson != "null")
                {
                    var followers = JsonConvert.DeserializeObject<Dictionary<string, bool>>(followersJson);
                    if (followers != null)
                    {
                        foreach (var followerId in followers.Keys)
                        {
                            if (followerId == AuthSession.FirebaseLocalId) continue;
                            var noti = new NotificationModel
                            {
                                Title = $"Tác giả \"{authorName}\" vừa đăng truyện mới!",
                                BookId = generatedId,
                                ChapterName = $"Tác phẩm mới: {title}",
                                TimeAgo = "Vừa xong",
                                IsRead = false
                            };
                            await PostAsync($"notifications/{followerId}.json", noti);
                        }
                    }
                }

                return "Success|" + generatedId;
            }
            catch (Exception ex)
            {
                return "Error|" + ex.Message;
            }
        }

        public static async Task<string> UploadChapterAsync(string bookId, string chapterNum, string chapterTitle, string chapterContent, string status = null)
        {
            try
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var chapter = new ChapterModel
                {
                    ChapterNumber = chapterNum,
                    Title = chapterTitle,
                    Content = chapterContent,
                    CreatedAt = now
                };

                // 1. Post the new chapter
                await PostAsync($"books/{bookId}/chapters.json", chapter);

                // Update book's UpdatedAt timestamp
                await PutAsync($"books/{bookId}/updatedAt.json", now);

                if (!string.IsNullOrEmpty(status))
                {
                    await PutAsync($"books/{bookId}/status.json", status);
                }

                // 2. Query the book title to construct a notification
                string bookJson = await GetAsync($"books/{bookId}.json");
                if (!string.IsNullOrEmpty(bookJson) && bookJson != "null")
                {
                    var book = JsonConvert.DeserializeObject<BookModel>(bookJson);
                    if (book != null)
                    {
                        var recipients = new HashSet<string>();

                        // 3. Send notifications to all readers who bookmarked this book
                        string subsJson = await GetAsync($"book_subscribers/{bookId}.json");
                        if (!string.IsNullOrEmpty(subsJson) && subsJson != "null")
                        {
                            var subscribers = JsonConvert.DeserializeObject<Dictionary<string, bool>>(subsJson);
                            if (subscribers != null)
                            {
                                foreach (var id in subscribers.Keys) recipients.Add(id);
                            }
                        }

                        // 4. Send notifications to all followers of the author
                        if (!string.IsNullOrEmpty(book.AuthorId))
                        {
                            string followersJson = await GetAsync($"author_followers/{book.AuthorId}.json");
                            if (!string.IsNullOrEmpty(followersJson) && followersJson != "null")
                            {
                                var followers = JsonConvert.DeserializeObject<Dictionary<string, bool>>(followersJson);
                                if (followers != null)
                                {
                                    foreach (var id in followers.Keys) recipients.Add(id);
                                }
                            }
                        }

                        // Send notifications
                        foreach (var recipientId in recipients)
                        {
                            // Avoid self-notification if author is recipient
                            if (recipientId == AuthSession.FirebaseLocalId) continue;

                            var noti = new NotificationModel
                            {
                                Title = $"Truyện \"{book.Title}\" vừa ra chương mới!",
                                BookId = bookId,
                                ChapterName = $"Chương {chapterNum}: {chapterTitle}",
                                TimeAgo = "Vừa xong",
                                IsRead = false
                            };
                            await PostAsync($"notifications/{recipientId}.json", noti);
                        }
                    }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return "Error|" + ex.Message;
            }
        }


        public static async Task<List<BookModel>> GetAllBooksAsync()
        {
            var books = new List<BookModel>();
            string json = await GetAsync("books.json");
            if (string.IsNullOrEmpty(json) || json == "null") return books;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, BookModel>>(json);
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        var b = kvp.Value;
                        b.Id = kvp.Key;
                        books.Add(b);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing books: " + ex.Message);
            }
            return books;
        }

        public static async Task<BookModel> GetBookByIdAsync(string bookId)
        {
            try
            {
                string json = await GetAsync($"books/{bookId}.json");
                if (string.IsNullOrEmpty(json) || json == "null") return null;

                var book = JsonConvert.DeserializeObject<BookModel>(json);
                if (book != null)
                {
                    book.Id = bookId;
                }
                return book;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing book by id: " + ex.Message);
                return null;
            }
        }

        private static async Task DeleteAsync(string path)
        {
            string dbUrl = AppConfig.FirebaseDatabaseUrl;
            if (string.IsNullOrWhiteSpace(dbUrl)) return;
            dbUrl = dbUrl.TrimEnd('/');

            string idToken = AuthSession.FirebaseIdToken;
            string url = $"{dbUrl}/{path}?auth={idToken}";
            using (var client = new HttpClient())
            {
                await client.DeleteAsync(url);
            }
        }

        private static async Task PatchAsync(string path, object payload)
        {
            string dbUrl = AppConfig.FirebaseDatabaseUrl;
            if (string.IsNullOrWhiteSpace(dbUrl)) return;
            dbUrl = dbUrl.TrimEnd('/');

            string idToken = AuthSession.FirebaseIdToken;
            string url = $"{dbUrl}/{path}?auth={idToken}";
            string json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
                await client.SendAsync(request);
            }
        }

        public static async Task<List<CommentModel>> GetCommentsAsync(string bookId)
        {
            var comments = new List<CommentModel>();
            string json = await GetAsync($"comments/{bookId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return comments;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, CommentModel>>(json);
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        var c = kvp.Value;
                        c.Id = kvp.Key;
                        comments.Add(c);
                    }
                    comments.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing comments: " + ex.Message);
            }
            return comments;
        }

        public static async Task<bool> PostCommentAsync(string bookId, string text)
        {
            string username = "Ẩn danh";
            var profile = await GetCurrentUserProfileAsync();
            if (profile != null && !string.IsNullOrEmpty(profile.Username))
            {
                username = profile.Username;
            }
            var comment = new CommentModel
            {
                Username = username,
                Text = text,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Likes = 0
            };
            try
            {
                await PostAsync($"comments/{bookId}.json", comment);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<NotificationModel>> GetNotificationsAsync()
        {
            var list = new List<NotificationModel>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return list;
            string json = await GetAsync($"notifications/{AuthSession.FirebaseLocalId}.json");
            
            // Check if null or empty, seed dummy notifications if so
            if (string.IsNullOrEmpty(json) || json == "null")
            {
                var dummy1 = new NotificationModel
                {
                    Title = "Chương 45 của Doraemon vừa được đăng",
                    BookId = "doraemon_dummy",
                    ChapterName = "Doraemon - Chương 45",
                    TimeAgo = "2 giờ trước",
                    IsRead = false
                };
                var dummy2 = new NotificationModel
                {
                    Title = "Chương 1 của Harry Potter vừa được đăng",
                    BookId = "harry_dummy",
                    ChapterName = "Harry Potter - Chương 1",
                    TimeAgo = "1 ngày trước",
                    IsRead = false
                };
                try
                {
                    await PostAsync($"notifications/{AuthSession.FirebaseLocalId}.json", dummy1);
                    await PostAsync($"notifications/{AuthSession.FirebaseLocalId}.json", dummy2);
                    json = await GetAsync($"notifications/{AuthSession.FirebaseLocalId}.json");
                }
                catch {}
            }

            if (string.IsNullOrEmpty(json) || json == "null") return list;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, NotificationModel>>(json);
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        var n = kvp.Value;
                        n.Id = kvp.Key;
                        list.Add(n);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing notifications: " + ex.Message);
            }
            return list;
        }

        public static async Task<bool> MarkAllNotificationsAsReadAsync()
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            try
            {
                var list = await GetNotificationsAsync();
                foreach (var n in list)
                {
                    if (!n.IsRead)
                    {
                        var update = new Dictionary<string, bool> { { "IsRead", true } };
                        await PatchAsync($"notifications/{AuthSession.FirebaseLocalId}/{n.Id}.json", update);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> MarkNotificationAsReadAsync(string notiId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(notiId)) return false;
            try
            {
                var update = new Dictionary<string, bool> { { "IsRead", true } };
                await PatchAsync($"notifications/{AuthSession.FirebaseLocalId}/{notiId}.json", update);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> DeleteNotificationAsync(string notiId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(notiId)) return false;
            try
            {
                await DeleteAsync($"notifications/{AuthSession.FirebaseLocalId}/{notiId}.json");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<ChapterModel>> GetChaptersAsync(string bookId)
        {
            var list = new List<ChapterModel>();
            if (string.IsNullOrEmpty(bookId)) return list;
            string json = await GetAsync($"books/{bookId}/chapters.json");
            if (string.IsNullOrEmpty(json) || json == "null") return list;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, ChapterModel>>(json);
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        var c = kvp.Value;
                        c.Id = kvp.Key;
                        c.ChapterNumber = c.ChapterNumber ?? "1";
                        list.Add(c);
                    }
                    list.Sort((x, y) => x.ChapterNumber.CompareTo(y.ChapterNumber));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing chapters: " + ex.Message);
            }
            return list;
        }

        public static async Task<bool> AddToBookmarksAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            try
            {
                await PutAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}.json", true);
                await PutAsync($"book_subscribers/{bookId}/{AuthSession.FirebaseLocalId}.json", true);
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> RemoveFromBookmarksAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            try
            {
                await DeleteAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}.json");
                await DeleteAsync($"book_subscribers/{bookId}/{AuthSession.FirebaseLocalId}.json");
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> IsBookmarkedAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            string res = await GetAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}.json");
            return !string.IsNullOrEmpty(res) && res != "null";
        }

        public static async Task<List<string>> GetBookmarkedBookIdsAsync()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return list;
            string json = await GetAsync($"bookmarks/{AuthSession.FirebaseLocalId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return list;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (dict != null)
                {
                    list.AddRange(dict.Keys);
                }
            }
            catch {}
            return list;
        }

        public static async Task<bool> AddToHistoryAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            try
            {
                await PutAsync($"history/{AuthSession.FirebaseLocalId}/{bookId}.json", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                return true;
            }
            catch { return false; }
        }

        public static async Task<List<string>> GetHistoryBookIdsAsync()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return list;
            string json = await GetAsync($"history/{AuthSession.FirebaseLocalId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return list;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, long>>(json);
                if (dict != null)
                {
                    var items = new List<KeyValuePair<string, long>>(dict);
                    items.Sort((x, y) => y.Value.CompareTo(x.Value)); // Sort by timestamp descending
                    foreach (var kvp in items)
                    {
                        list.Add(kvp.Key);
                    }
                }
            }
            catch {}
            return list;
        }

        public static async Task<bool> AddToFavoritesAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            try
            {
                await PutAsync($"favorites/{AuthSession.FirebaseLocalId}/{bookId}.json", true);
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> RemoveFromFavoritesAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            try
            {
                await DeleteAsync($"favorites/{AuthSession.FirebaseLocalId}/{bookId}.json");
                return true;
            }
            catch { return false; }
        }

        public static async Task<List<string>> GetFavoriteBookIdsAsync()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return list;
            string json = await GetAsync($"favorites/{AuthSession.FirebaseLocalId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return list;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (dict != null)
                {
                    list.AddRange(dict.Keys);
                }
            }
            catch {}
            return list;
        }

        public static async Task<bool> UpdateUserBioAsync(string bio)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            try
            {
                await PutAsync($"users/{AuthSession.FirebaseLocalId}/bio.json", bio);
                return true;
            }
            catch { return false; }
        }

        public static async Task<string> GetUserBioAsync()
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return "Yêu thích đọc và sáng tác truyện";
            string res = await GetAsync($"users/{AuthSession.FirebaseLocalId}/bio.json");
            if (string.IsNullOrEmpty(res) || res == "null") return "Yêu thích đọc và sáng tác truyện";
            return res.Trim('\"');
        }

        public static async Task<int> GetUserBookCountAsync()
        {
            int count = 0;
            try
            {
                var books = await GetAllBooksAsync();
                foreach (var b in books)
                {
                    if (b.AuthorId == AuthSession.FirebaseLocalId) count++;
                }
            }
            catch {}
            return count;
        }

        public static async Task<bool> UpdateUserProfileAsync(string username, string birthdate)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            try
            {
                var payload = new
                {
                    username = username,
                    birthdate = birthdate
                };
                await PatchAsync($"users/{AuthSession.FirebaseLocalId}.json", payload);
                return true;
            }
            catch { return false; }
        }

        public static async Task IncrementBookViewsAsync(string bookId)
        {
            if (string.IsNullOrEmpty(bookId)) return;
            try
            {
                string viewsStr = await GetAsync($"books/{bookId}/views.json");
                int currentViews = 0;
                if (!string.IsNullOrEmpty(viewsStr) && viewsStr != "null")
                {
                    int.TryParse(viewsStr, out currentViews);
                }
                currentViews++;
                await PutAsync($"books/{bookId}/views.json", currentViews);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error incrementing views: " + ex.Message);
            }
        }

        public static async Task IncrementChapterViewsAsync(string bookId, string chapterId)
        {
            if (string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(chapterId)) return;
            try
            {
                string viewsStr = await GetAsync($"books/{bookId}/chapters/{chapterId}/views.json");
                int currentViews = 0;
                if (!string.IsNullOrEmpty(viewsStr) && viewsStr != "null")
                {
                    int.TryParse(viewsStr, out currentViews);
                }
                currentViews++;
                await PutAsync($"books/{bookId}/chapters/{chapterId}/views.json", currentViews);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error incrementing chapter views: " + ex.Message);
            }
        }

        public static async Task<int> ToggleBookLikeAsync(string bookId, bool isAdd)
        {
            if (string.IsNullOrEmpty(bookId)) return 0;
            try
            {
                string likesStr = await GetAsync($"books/{bookId}/likes.json");
                int currentLikes = 0;
                if (!string.IsNullOrEmpty(likesStr) && likesStr != "null")
                {
                    int.TryParse(likesStr, out currentLikes);
                }
                currentLikes = isAdd ? currentLikes + 1 : Math.Max(0, currentLikes - 1);
                await PutAsync($"books/{bookId}/likes.json", currentLikes);
                return currentLikes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error toggling likes: " + ex.Message);
                return 0;
            }
        }

        public static async Task<bool> IsFavoriteAsync(string bookId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            string res = await GetAsync($"favorites/{AuthSession.FirebaseLocalId}/{bookId}.json");
            return !string.IsNullOrEmpty(res) && res != "null";
        }

        public static async Task<bool> IsFollowingAuthorAsync(string authorId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(authorId)) return false;
            string res = await GetAsync($"author_followers/{authorId}/{AuthSession.FirebaseLocalId}.json");
            return !string.IsNullOrEmpty(res) && res != "null";
        }

        public static async Task<bool> FollowAuthorAsync(string authorId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(authorId)) return false;
            try
            {
                await PutAsync($"author_followers/{authorId}/{AuthSession.FirebaseLocalId}.json", true);
                await PutAsync($"user_following/{AuthSession.FirebaseLocalId}/{authorId}.json", true);
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> UnfollowAuthorAsync(string authorId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(authorId)) return false;
            try
            {
                await DeleteAsync($"author_followers/{authorId}/{AuthSession.FirebaseLocalId}.json");
                await DeleteAsync($"user_following/{AuthSession.FirebaseLocalId}/{authorId}.json");
                return true;
            }
            catch { return false; }
        }

        public static async Task<int> GetFollowersCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return 0;
            string json = await GetAsync($"author_followers/{userId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return 0;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                return dict?.Count ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<int> GetFollowingCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return 0;
            string json = await GetAsync($"user_following/{userId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return 0;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                return dict?.Count ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<UserModel> GetUserProfileAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;
            string json = await GetAsync($"users/{userId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return null;
            try
            {
                var user = JsonConvert.DeserializeObject<UserModel>(json);
                if (user != null)
                {
                    user.LocalId = userId;
                }
                return user;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<List<string>> GetFollowerIdsAsync(string userId)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(userId)) return list;
            string json = await GetAsync($"author_followers/{userId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return list;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                if (dict != null)
                {
                    list.AddRange(dict.Keys);
                }
            }
            catch { }
            return list;
        }

        public static async Task<List<string>> GetFollowingIdsAsync(string userId)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(userId)) return list;
            string json = await GetAsync($"user_following/{userId}.json");
            if (string.IsNullOrEmpty(json) || json == "null") return list;
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                if (dict != null)
                {
                    list.AddRange(dict.Keys);
                }
            }
            catch { }
            return list;
        }

        public static async Task<double> RateBookAsync(string bookId, double ratingValue)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return 5.0;
            try
            {
                // Save user's rating
                await PutAsync($"book_ratings/{bookId}/{AuthSession.FirebaseLocalId}.json", ratingValue);

                // Fetch all ratings for this book
                string allRatingsJson = await GetAsync($"book_ratings/{bookId}.json");
                if (!string.IsNullOrEmpty(allRatingsJson) && allRatingsJson != "null")
                {
                    var ratings = JsonConvert.DeserializeObject<Dictionary<string, double>>(allRatingsJson);
                    if (ratings != null && ratings.Count > 0)
                    {
                        double sum = 0;
                        foreach (var r in ratings.Values)
                        {
                            sum += r;
                        }
                        double avg = sum / ratings.Count;
                        avg = Math.Round(avg, 1);
                        // Save average rating to the book
                        await PutAsync($"books/{bookId}/rating.json", avg);
                        return avg;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error rating book: " + ex.Message);
            }
            return 5.0;
        }

        public static async Task<bool> UpdateUserAvatarAsync(string base64Avatar)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            try
            {
                await PutAsync($"users/{AuthSession.FirebaseLocalId}/avatar.json", base64Avatar);
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> DeleteConversationAsync(string partnerId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(partnerId)) return false;
            try
            {
                string chatId = GetChatId(AuthSession.FirebaseLocalId, partnerId);
                await DeleteAsync($"messages/{chatId}.json");
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> BlockUserAsync(string blockedUserId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(blockedUserId)) return false;
            try
            {
                await PutAsync($"blocked_users/{AuthSession.FirebaseLocalId}/{blockedUserId}.json", true);
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> UnblockUserAsync(string blockedUserId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(blockedUserId)) return false;
            try
            {
                await DeleteAsync($"blocked_users/{AuthSession.FirebaseLocalId}/{blockedUserId}.json");
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> IsUserBlockedAsync(string blockedUserId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(blockedUserId)) return false;
            string res = await GetAsync($"blocked_users/{AuthSession.FirebaseLocalId}/{blockedUserId}.json");
            return !string.IsNullOrEmpty(res) && res != "null";
        }

        public static async Task<bool> IsBlockedByEitherAsync(string partnerId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(partnerId)) return false;
            // Check if we blocked them
            string res1 = await GetAsync($"blocked_users/{AuthSession.FirebaseLocalId}/{partnerId}.json");
            if (!string.IsNullOrEmpty(res1) && res1 != "null") return true;
            // Check if they blocked us
            string res2 = await GetAsync($"blocked_users/{partnerId}/{AuthSession.FirebaseLocalId}.json");
            if (!string.IsNullOrEmpty(res2) && res2 != "null") return true;
            return false;
        }

        public static async Task<bool> SendMessageWithFileAsync(string partnerId, string text, string fileType, string fileBase64, string fileName)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(partnerId)) return false;

            if (await IsBlockedByEitherAsync(partnerId))
            {
                System.Windows.Forms.MessageBox.Show("Không thể gửi tin nhắn. Người dùng đã bị chặn hoặc bạn đã bị chặn bởi người này.", "Lỗi");
                return false;
            }

            string chatId = GetChatId(AuthSession.FirebaseLocalId, partnerId);
            var message = new MessageModel
            {
                SenderId = AuthSession.FirebaseLocalId,
                ReceiverId = partnerId,
                Text = text,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                FileType = fileType,
                FileBase64 = fileBase64,
                FileName = fileName
            };

            try
            {
                await PostAsync($"messages/{chatId}.json", message);
                await SetChatUnreadAsync(partnerId, true);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error sending file message: " + ex.Message);
                return false;
            }
        }

        public static async Task SetChatUnreadAsync(string partnerId, bool isUnread)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(partnerId)) return;
            try
            {
                if (isUnread)
                {
                    await PutAsync($"unread_chats/{partnerId}/{AuthSession.FirebaseLocalId}.json", true);
                }
                else
                {
                    await DeleteAsync($"unread_chats/{AuthSession.FirebaseLocalId}/{partnerId}.json");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error setting chat unread: " + ex.Message);
            }
        }

        public static async Task<bool> HasAnyUnreadChatsAsync()
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            try
            {
                string json = await GetAsync($"unread_chats/{AuthSession.FirebaseLocalId}.json");
                return !string.IsNullOrEmpty(json) && json != "null";
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> HasAnyUnreadNotificationsAsync()
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            try
            {
                var notifications = await GetNotificationsAsync();
                return notifications.Any(n => !n.IsRead);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> IsCommentLikedAsync(string bookId, string commentId)
        {
            if (string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(commentId) || string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return false;
            string res = await GetAsync($"comment_likes/{bookId}/{commentId}/{AuthSession.FirebaseLocalId}.json");
            return !string.IsNullOrEmpty(res) && res != "null";
        }

        public static async Task<int> ToggleCommentLikeAsync(string bookId, string commentId)
        {
            if (string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(commentId) || string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return 0;
            try
            {
                bool isLiked = await IsCommentLikedAsync(bookId, commentId);

                string likesStr = await GetAsync($"comments/{bookId}/{commentId}/Likes.json");
                int currentLikes = 0;
                if (!string.IsNullOrEmpty(likesStr) && likesStr != "null")
                {
                    int.TryParse(likesStr, out currentLikes);
                }

                if (isLiked)
                {
                    currentLikes = Math.Max(0, currentLikes - 1);
                    await DeleteAsync($"comment_likes/{bookId}/{commentId}/{AuthSession.FirebaseLocalId}.json");
                }
                else
                {
                    currentLikes++;
                    await PutAsync($"comment_likes/{bookId}/{commentId}/{AuthSession.FirebaseLocalId}.json", true);
                }

                await PutAsync($"comments/{bookId}/{commentId}/Likes.json", currentLikes);
                return currentLikes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error toggling comment like: " + ex.Message);
                return 0;
            }
        }

        public static async Task<bool> AddLineBookmarkAsync(string bookId, string chapterNumber, int paragraphIndex, string lineText)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return false;
            try
            {
                var bm = new BookmarkModel
                {
                    Id = Guid.NewGuid().ToString("N"),
                    BookId = bookId,
                    ChapterNumber = chapterNumber,
                    ParagraphIndex = paragraphIndex,
                    LineText = lineText.Length > 60 ? lineText.Substring(0, 60) + "..." : lineText,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                await PutAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}/{bm.Id}.json", bm);
                await PutAsync($"book_subscribers/{bookId}/{AuthSession.FirebaseLocalId}.json", true);
                return true;
            }
            catch { return false; }
        }

        public static async Task<List<BookmarkModel>> GetLineBookmarksAsync(string bookId)
        {
            var list = new List<BookmarkModel>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId)) return list;
            try
            {
                string json = await GetAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}.json");
                if (string.IsNullOrEmpty(json) || json == "null") return list;
                if (json.Trim() == "true") return list;

                var dict = JsonConvert.DeserializeObject<Dictionary<string, BookmarkModel>>(json);
                if (dict != null)
                {
                    list.AddRange(dict.Values);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing line bookmarks: " + ex.Message);
            }
            return list;
        }

        public static async Task<bool> RemoveLineBookmarkAsync(string bookId, string bookmarkId)
        {
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId) || string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(bookmarkId)) return false;
            try
            {
                await DeleteAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}/{bookmarkId}.json");
                var remaining = await GetLineBookmarksAsync(bookId);
                if (remaining.Count == 0)
                {
                    await DeleteAsync($"bookmarks/{AuthSession.FirebaseLocalId}/{bookId}.json");
                    await DeleteAsync($"book_subscribers/{bookId}/{AuthSession.FirebaseLocalId}.json");
                }
                return true;
            }
            catch { return false; }
        }

        public static async Task<Dictionary<string, List<BookmarkModel>>> GetAllUserBookmarksAsync()
        {
            var dictResult = new Dictionary<string, List<BookmarkModel>>();
            if (string.IsNullOrEmpty(AuthSession.FirebaseLocalId)) return dictResult;
            try
            {
                string json = await GetAsync($"bookmarks/{AuthSession.FirebaseLocalId}.json");
                if (string.IsNullOrEmpty(json) || json == "null") return dictResult;

                var rawDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (rawDict != null)
                {
                    foreach (var kvp in rawDict)
                    {
                        string bookId = kvp.Key;
                        string valJson = kvp.Value.ToString();
                        if (valJson.Trim() == "True" || valJson.Trim() == "true")
                        {
                            dictResult[bookId] = new List<BookmarkModel>();
                        }
                        else
                        {
                            try
                            {
                                var bmDict = JsonConvert.DeserializeObject<Dictionary<string, BookmarkModel>>(valJson);
                                if (bmDict != null)
                                {
                                    dictResult[bookId] = bmDict.Values.ToList();
                                }
                            }
                            catch
                            {
                                dictResult[bookId] = new List<BookmarkModel>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting all user bookmarks: " + ex.Message);
            }
            return dictResult;
        }
    }

    public class CommentModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public long Timestamp { get; set; }
        public int Likes { get; set; }
    }

    public class NotificationModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string BookId { get; set; }
        public string ChapterName { get; set; }
        public string TimeAgo { get; set; }
        public bool IsRead { get; set; }
    }

    public class BookmarkModel
    {
        public string Id { get; set; }
        public string BookId { get; set; }
        public string ChapterNumber { get; set; }
        public int ParagraphIndex { get; set; }
        public string LineText { get; set; }
        public long Timestamp { get; set; }
    }
}
