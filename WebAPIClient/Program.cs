using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIClient
{
    class Program
    {
        private static string _baseUrl = "https://localhost:7002/api/";
        static void Main(string[] args)
        {            
            var newTodo = new TodoItem { Id = 100, Name = "New Client Item", IsComplete = false };
            PostTodoItemAsync(newTodo).Wait();
            GetTodoAsync(newTodo.Id).Wait();
            newTodo.IsComplete = true;
            PutTodoItemAsync(newTodo.Id, newTodo).Wait();
            GetTodosAsync().Wait();
            DeleteTodoItemAsync(newTodo.Id).Wait();
            GetTodosAsync().Wait();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        static async Task PostTodoItemAsync(TodoItem item)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent httpContent = new StringContent(JsonSerializer.Serialize(item), System.Text.Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("TodoItems", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var itemCreated = response.Headers.GetValues("location").FirstOrDefault();                    
                    Console.WriteLine($"Item created at: {itemCreated}");
                }
                else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                {
                    var error = response.Content.ReadAsStringAsync();

                    Console.Error.WriteLine($"Error {response.StatusCode}: {error}");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync();
                    Console.Error.WriteLine("Internal server Error");
                    Console.Error.WriteLine($"Error {response.StatusCode}: {error}");                    
                }
            }
        }

        static async Task PutTodoItemAsync(long id, TodoItem item)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent httpContent = new StringContent(JsonSerializer.Serialize(item), System.Text.Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PutAsync($"TodoItems/{id}", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Item Updated");
                }
                else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                {
                    var error = response.Content.ReadAsStringAsync();

                    Console.Error.WriteLine($"Error {response.StatusCode}: {error}");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync();
                    Console.Error.WriteLine("Internal server Error");
                    Console.Error.WriteLine($"Error {response.StatusCode}: {error}");
                }
            }
        }

        static async Task GetTodosAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("TodoItems");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(result))
                        Console.WriteLine("No Todo Items Found");
                    else
                    {
                        Console.WriteLine(result);
                        var todos = JsonSerializer.Deserialize<List<TodoItem>>(result);
                    }
                }
                else
                {
                    Console.Error.WriteLine("Internal server Error");
                }

            }
        }

        static async Task GetTodoAsync(long id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"TodoItems/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(result))
                        Console.WriteLine("No Todo Item Found");
                    else
                    {
                        Console.WriteLine(result);
                        var todo = JsonSerializer.Deserialize<TodoItem>(result);
                    }

                }
                else
                {
                    Console.Error.WriteLine("Internal server Error");
                }

            }
        }

        static async Task DeleteTodoItemAsync(long id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"TodoItems/{id}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Item Deleted");
                }
                else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                {
                    var error = response.Content.ReadAsStringAsync();

                    Console.Error.WriteLine($"Error {response.StatusCode}: {error}");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync();
                    Console.Error.WriteLine("Internal server Error");
                    Console.Error.WriteLine($"Error {response.StatusCode}: {error}");
                }
            }
        }


    }
}