using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace client_firebase
{
    public static class GeminiService
    {
        private static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        public static async Task<string> GenerateTextAsync(string prompt)
        {
            string apiKey = AppConfig.GeminiApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "Lỗi: Chưa cấu hình API Key cho Gemini (GeminiApiKey) trong config.json.";
            }

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject json = JObject.Parse(responseString);
                    string text = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                    return text ?? "Không nhận được phản hồi từ AI.";
                }
                else
                {
                    try
                    {
                        JObject errJson = JObject.Parse(responseString);
                        string errMsg = errJson["error"]?["message"]?.ToString();
                        if (!string.IsNullOrEmpty(errMsg)) return "Lỗi AI API: " + errMsg;
                    }
                    catch {}
                    return $"Lỗi API (Mã lỗi {response.StatusCode}): " + responseString;
                }
            }
            catch (Exception ex)
            {
                return "Lỗi kết nối AI: " + ex.Message;
            }
        }

        private static string CleanJsonString(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson)) return "[]";

            string clean = rawJson.Trim();

            // Extract content from markdown code fences if present
            if (clean.StartsWith("```"))
            {
                int firstNewLine = clean.IndexOf('\n');
                if (firstNewLine != -1)
                {
                    // Skip the language tag (e.g. ```json)
                    clean = clean.Substring(firstNewLine).Trim();
                }
                if (clean.EndsWith("```"))
                {
                    clean = clean.Substring(0, clean.Length - 3).Trim();
                }
            }

            // Fallback: search for first '[' and last ']' to isolate the JSON array
            int firstBracket = clean.IndexOf('[');
            int lastBracket = clean.LastIndexOf(']');
            if (firstBracket != -1 && lastBracket != -1 && lastBracket > firstBracket)
            {
                clean = clean.Substring(firstBracket, lastBracket - firstBracket + 1);
            }

            return clean;
        }

        public static async Task<List<string>> SearchBooksAIAsync(string userQuery, List<BookModel> books)
        {
            var matchedIds = new List<string>();
            if (books == null || books.Count == 0 || string.IsNullOrWhiteSpace(userQuery))
            {
                return matchedIds;
            }

            // Build metadata list of stories for Gemini
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Dưới đây là danh sách các cuốn truyện hiện có trong hệ thống:");
            foreach (var b in books)
            {
                string genresStr = b.Genres != null ? string.Join(", ", b.Genres) : "Không có";
                sb.AppendLine($"- ID: \"{b.Id}\" | Tên truyện: \"{b.Title}\" | Tác giả: \"{b.AuthorName}\" | Thể loại: \"{genresStr}\" | Mô tả: \"{b.Description}\"");
            }
            sb.AppendLine();
            sb.AppendLine($"Yêu cầu tìm kiếm của người dùng (có thể là mô tả, thể loại, ý tưởng hoặc từ khóa): \"{userQuery}\"");
            sb.AppendLine("Hãy chọn lọc các cuốn truyện phù hợp nhất với yêu cầu trên và sắp xếp chúng theo độ khớp giảm dần.");
            sb.AppendLine("Trả về kết quả DƯỚI DẠNG MẢNG JSON của các ID truyện (chuỗi), ví dụ: [\"id1\", \"id2\"].");
            sb.AppendLine("Lưu ý quan trọng: Chỉ trả về mảng JSON, KHÔNG THÊM bất kỳ lời giải thích, tiêu đề hay định dạng phụ nào khác ngoài chuỗi JSON.");

            string response = await GenerateTextAsync(sb.ToString());
            if (string.IsNullOrEmpty(response) || response.StartsWith("Lỗi"))
            {
                return matchedIds;
            }

            try
            {
                string cleanedJson = CleanJsonString(response);
                var ids = JsonConvert.DeserializeObject<List<string>>(cleanedJson);
                if (ids != null)
                {
                    matchedIds.AddRange(ids);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error parsing AI Search response: " + ex.Message + " | Response was: " + response);
            }

            return matchedIds;
        }

        public static async Task<string> AssistWritingAsync(string bookTitle, string bookDesc, string existingContent, string actionType)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Bạn là một trợ lý viết lách AI chuyên nghiệp hỗ trợ tác giả viết truyện.");
            sb.AppendLine($"Thông tin tác phẩm:");
            sb.AppendLine($"- Tên truyện: {bookTitle}");
            sb.AppendLine($"- Mô tả tác phẩm: {bookDesc}");
            sb.AppendLine();

            if (actionType == "continue")
            {
                sb.AppendLine("Nhiệm vụ của bạn: Hãy VIẾT TIẾP một phân đoạn tiếp theo (khoảng 200-400 từ) cho chương truyện.");
                sb.AppendLine("Nội dung đã viết trước đó của chương:");
                sb.AppendLine("--- Cốt truyện hiện tại ---");
                sb.AppendLine(existingContent);
                sb.AppendLine("--- Hết cốt truyện hiện tại ---");
                sb.AppendLine("Hãy tiếp tục mạch truyện một cách mạch lạc, lôi cuốn, giữ đúng giọng văn và phong cách.");
                sb.AppendLine("Chỉ trả về phần nội dung viết tiếp, không thêm lời dẫn giải của trợ lý.");
            }
            else if (actionType == "polish")
            {
                sb.AppendLine("Nhiệm vụ của bạn: Hãy TRẢU CHUỐT và BÚT CHIẾN lại phân đoạn sau để trở nên sinh động, giàu cảm xúc, văn phong mượt mà và mô tả chi tiết hơn.");
                sb.AppendLine("Đoạn văn cần cải thiện:");
                sb.AppendLine("--- Đoạn văn gốc ---");
                sb.AppendLine(existingContent);
                sb.AppendLine("--- Hết đoạn văn gốc ---");
                sb.AppendLine("Hãy viết lại đoạn văn trên một cách hay nhất có thể. Chỉ trả về đoạn văn sau khi đã chỉnh sửa, không kèm lời giải thích.");
            }
            else if (actionType == "ideas")
            {
                sb.AppendLine("Nhiệm vụ của bạn: Hãy GỢI Ý CỐT TRUYỆN/Ý TƯỞNG cho chương mới dựa trên thông tin tác phẩm.");
                if (!string.IsNullOrWhiteSpace(existingContent))
                {
                    sb.AppendLine("Gợi ý bổ sung từ tác giả: " + existingContent);
                }
                sb.AppendLine("Hãy đề xuất 3 hướng phát triển cốt truyện hoặc ý tưởng tình tiết hấp dẫn cho chương tiếp theo. Trình bày ngắn gọn, súc tích, dễ theo dõi.");
            }

            return await GenerateTextAsync(sb.ToString());
        }
    }
}
