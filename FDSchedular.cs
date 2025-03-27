
using Newtonsoft.Json;
using PavanPortfolio.Controllers; 
using Quartz;

namespace PavanPortfolio
{
    public class FDSchedular:IJob
    {
        string rootpath = string.Empty; 

        public FDSchedular(IWebHostEnvironment webhost)
        {
            rootpath = webhost.ContentRootPath;
        }

        Task IJob.Execute(IJobExecutionContext context)
        { 
            HitToFreeJokeAPI(); 
            return Task.CompletedTask;
        }

        public async Task HitToFreeJokeAPI()
        {
            try
            {
                string apiUrl = "https://official-joke-api.appspot.com/random_joke";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // Make the GET request
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode(); // Ensure the request was successful

                        // Read the response content as a string
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response into a dynamic object
                        dynamic jokeData = JsonConvert.DeserializeObject(responseBody);

                        // Access the joke's setup and punchline
                        string setup = jokeData.setup;
                        string punchline = jokeData.punchline; 
                    }
                    catch (Exception ex)
                    {
                        // Handle errors
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            } 
        }
    }
}
