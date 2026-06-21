using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public Dictionary<string, ChapterModel> Chapters { get; set; } = new Dictionary<string, ChapterModel>();
    }

    public class ChapterModel
    {
        public string ChapterNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long CreatedAt { get; set; }
    }

    public class UserModel
    {
        public string LocalId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Birthdate { get; set; }
    }

    public class MessageModel
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public long Timestamp { get; set; }
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

            string chatId = GetChatId(AuthSession.FirebaseLocalId, partnerId);
            var message = new MessageModel
            {
                SenderId = AuthSession.FirebaseLocalId,
                ReceiverId = partnerId,
                Text = text,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            try
            {
                await PostAsync($"messages/{chatId}.json", message);
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

        public static async Task<string> UploadBookAsync(string title, string description, string coverBase64, List<string> genres, string chapterNum, string chapterTitle, string chapterContent)
        {
            try
            {
                string authorName = "Ẩn danh";
                var profile = await GetCurrentUserProfileAsync();
                if (profile != null && !string.IsNullOrEmpty(profile.Username))
                {
                    authorName = profile.Username;
                }

                var book = new BookModel
                {
                    Title = title,
                    Description = description,
                    CoverBase64 = coverBase64,
                    Genres = genres,
                    AuthorId = AuthSession.FirebaseLocalId,
                    AuthorName = authorName,
                    Views = new Random().Next(100, 1500),
                    Rating = Math.Round(4.0 + new Random().NextDouble(), 1)
                };

                // Add first chapter
                var chapter = new ChapterModel
                {
                    ChapterNumber = chapterNum,
                    Title = chapterTitle,
                    Content = chapterContent,
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
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

                return "Success|" + generatedId;
            }
            catch (Exception ex)
            {
                return "Error|" + ex.Message;
            }
        }

        public static async Task<string> UploadChapterAsync(string bookId, string chapterNum, string chapterTitle, string chapterContent)
        {
            try
            {
                var chapter = new ChapterModel
                {
                    ChapterNumber = chapterNum,
                    Title = chapterTitle,
                    Content = chapterContent,
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                // 1. Post the new chapter
                await PostAsync($"books/{bookId}/chapters.json", chapter);

                // 2. Query the book title to construct a notification
                string bookJson = await GetAsync($"books/{bookId}.json");
                if (!string.IsNullOrEmpty(bookJson) && bookJson != "null")
                {
                    var book = JsonConvert.DeserializeObject<BookModel>(bookJson);
                    if (book != null)
                    {
                        // 3. Send notifications to all readers who bookmarked this book
                        string subsJson = await GetAsync($"book_subscribers/{bookId}.json");
                        if (!string.IsNullOrEmpty(subsJson) && subsJson != "null")
                        {
                            var subscribers = JsonConvert.DeserializeObject<Dictionary<string, bool>>(subsJson);
                            if (subscribers != null)
                            {
                                foreach (var subscriberId in subscribers.Keys)
                                {
                                    // Avoid self-notification if author is subscribed
                                    if (subscriberId == AuthSession.FirebaseLocalId) continue;

                                    var noti = new NotificationModel
                                    {
                                        Title = $"Truyện \"{book.Title}\" vừa ra chương mới!",
                                        BookId = bookId,
                                        ChapterName = $"Chương {chapterNum}: {chapterTitle}",
                                        TimeAgo = "Vừa xong",
                                        IsRead = false
                                    };
                                    await PostAsync($"notifications/{subscriberId}.json", noti);
                                }
                            }
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
                Likes = new Random().Next(1, 100)
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
}
