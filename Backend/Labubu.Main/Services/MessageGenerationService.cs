using FluentResults;
using Labubu.Main.Configuration;
using Microsoft.Extensions.Options;
using Shared.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace Labubu.Main.Services;

public class MessageGenerationService : IMessageGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouterConfig _config;

    private const string Prompt = """
You are "Fact Smith," a witty yet kind financial fact maker. You speak ONLY RUSSIAN.

TASK
Given transaction statistics, produce exactly ONE short, funny fact about spending patterns.

STYLE
- Maximum 25 words. One (1) emoji maximum. 
- Friendly, playful, and specific. Mention the date in format "dd MMM" and currency symbol.
- No judgment, no advice, no commands.
- Never reveal calculations or raw data.

DECISION RULES (pick the strongest signal)
1) Largest category by transaction count (restaurants, coffee, groceries, etc.)
2) Total spending amount compared to available funds
3) Number of different categories (diversity of spending)
4) If no transactions → say so cheerfully

DATA FORMAT
You will receive:
- transactions: dictionary of category names and their transaction counts
- totalSpent: total amount spent (absolute value)
- fundsAvailable: available funds
- today: today's date

RULES
- Focus on the category with most transactions
- If totalSpent is significant compared to fundsAvailable, mention it
- If there are many categories, mention spending diversity
- Use Russian language only
- Be creative and funny, but keep it short
- Format date as "dd MMM" (e.g., "07 ноя")

OUTPUT: Return ONLY the one-sentence fact in Russian. No explanations, no JSON, just the fact.
""";

    public MessageGenerationService(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenRouterConfig> config)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
        _config = config.Value;
    }

    public async Task<Result<string>> GenerateMessageAsync(
        UserStatisticsResult statistics,
        DateTime today,
        string timezone,
        string currency)
    {
        try
        {
            var todayFormatted = today.ToString("yyyy-MM-dd");
            var todayFormattedShort = today.ToString("dd MMM", new System.Globalization.CultureInfo("ru-RU"));
            
            var transactionsList = statistics.Transactions
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new
                {
                    category = kvp.Key,
                    transactionCount = kvp.Value
                })
                .ToList();

            var topCategory = transactionsList.FirstOrDefault();
            var categoryCount = transactionsList.Count;
            
            var transactionsData = new
            {
                today = todayFormatted,
                todayFormatted = todayFormattedShort,
                currency = currency,
                transactions = transactionsList,
                topCategory = topCategory?.category,
                topCategoryCount = topCategory?.transactionCount ?? 0,
                totalCategories = categoryCount,
                totalSpent = Math.Round(Math.Abs(statistics.TotalSpent), 2),
                fundsAvailable = Math.Round(statistics.FundsAvailable, 2),
                hasTransactions = statistics.Transactions.Any()
            };

            var dataJson = JsonSerializer.Serialize(transactionsData, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            
            var fullPrompt = $"{Prompt}\n\nTransaction Statistics:\n{dataJson}\n\nGenerate a funny, short fact in Russian about the spending patterns. Maximum 25 words, one emoji.";

            var requestBody = new
            {
                model = _config.Model,
                messages = new[]
                {
                    new { role = "user", content = fullPrompt }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}/chat/completions")
            {
                Headers =
                {
                    { "Authorization", $"Bearer {_config.ApiKey}" },
                    { "Content-Type", "application/json" }
                },
                Content = JsonContent.Create(requestBody)
            };

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail($"OpenRouter API error: {response.StatusCode}");
            }

            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            if (!responseData.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                return Result.Fail("Invalid response from OpenRouter");
            }

            var messageContent = choices[0].GetProperty("message").GetProperty("content").GetString();

            if (string.IsNullOrWhiteSpace(messageContent))
            {
                return Result.Fail("Empty response from OpenRouter");
            }

            return Result.Ok(messageContent.Trim());
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to generate message: {ex.Message}");
        }
    }
}
