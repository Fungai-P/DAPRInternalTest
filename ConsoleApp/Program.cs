using Common.Models.Requests;
using Dapr.Client;
using System.Net.Http.Json;

using var client = DaprClient.CreateInvokeHttpClient(appId: "alertapi");

var response = await client.PostAsJsonAsync<SmsAlertRequest>(
    "sendSms",
    new SmsAlertRequest
    {
        PhoneNumber = "12345678"
    });
response.EnsureSuccessStatusCode();

response = await client.PostAsJsonAsync<EmailAlertRequest>(
    "sendEmail",
    new EmailAlertRequest
    {
        EmailAddress = "me@home.com",
        Subject = "This is a subject"
    });
response.EnsureSuccessStatusCode();

var response = await client.PostAsJsonAsync<AlertRequest>(
    "sendAlert",
    new AlertRequest
    {
        AlertTypes = new List<string> { "Alert1", "Alert2", "Alert3" },
        ClientName = "Generic Client"
    });
response.EnsureSuccessStatusCode();

Console.Read();

