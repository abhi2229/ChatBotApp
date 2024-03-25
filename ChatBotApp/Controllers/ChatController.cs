using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ChatBotApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly string _connectionString;

        public ChatController(string connectionString) // Inject connection string through dependency injection
        {
            _connectionString = connectionString;
        }

        public class ChatMessage // Model class for chat messages
        {
            public int Id { get; set; }
            public string Sender { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
        }

        [HttpPost]
        public IActionResult SendMessage(string sender, string message)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "INSERT INTO ChatMessages (Sender, Message, Timestamp) VALUES (@Sender, @Message, @Timestamp)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Sender", sender);
                        command.Parameters.AddWithValue("@Message", message);
                        command.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving message: " + ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            var messages = new List<ChatMessage>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "SELECT * FROM ChatMessages ORDER BY Timestamp DESC";
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                messages.Add(new ChatMessage
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Sender = reader.GetString(reader.GetOrdinal("Sender")),
                                    Message = reader.GetString(reader.GetOrdinal("Message")),
                                    Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving messages: " + ex.Message);
                return StatusCode(500, "Internal Server Error");
            }

            return Json(messages);
        }
    }
}
