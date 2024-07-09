using Common.Models.Requests;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AlertApi.RouteBuilders
{
    public static class EndpointFactory
    {
        public static void RegisterEndpoints(this WebApplication app)
        {
            app.MapPost("/sendSms", async ([FromBody] SmsAlertRequest smsAlertRequest) =>
            {
                // Invoke a binding to an sms service provider (You will need to create your own twilion account) 
                // No body as free tier of twilio offers no message capability
                using var client = new DaprClientBuilder().Build();

                var jsonString = JsonSerializer.Serialize(smsAlertRequest);
                var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

                await client.InvokeBindingAsync("twilio", "create", content);

                app.Logger.LogInformation("SMS sent succesfully");
            });

            app.MapPost("/sendEmail", async ([FromBody] EmailAlertRequest emailAlertRequest) =>
            {
                // Invoke a binding to an email service provider (you will need to create your own sendgrid account)
                using var client = new DaprClientBuilder().Build();

                var emailMessage = new
                {
                    data = new
                    {
                        personalizations = new[]
                        {
                            new
                            {
                                to = new[]
                                {
                                    new { email = emailAlertRequest.EmailAddress }
                                },
                                subject = emailAlertRequest.Subject
                            }
                        },
                        from = new { email = "f_pamire@yahoo.com" },
                        operation = "create"
                    }
                };

                var response = await client.InvokeBindingAsync<object, object>("sendgrid", "create", emailMessage);

                app.Logger.LogInformation("Email sent succesfully");
            });

            app.MapPost("/sendAlert", async ([FromBody] AlertRequest alertRequest) =>
            {
                // Publish multiple events via rabbitmq (could also use redis but only in self-hosted kubernetes mode) 
                using var client = new DaprClientBuilder().Build();                

                for (int i = 0; i < alertRequest.AlertTypes.Count(); i++)
                {
                    app.Logger.LogInformation("Publishing alert: " + alertRequest.AlertTypes[i] + "\n      For client: " + alertRequest.ClientName);
                    DateTime now = DateTime.Now;

                    PublishAlertRequest pubRequest = new PublishAlertRequest()
                    {
                        AlertType = alertRequest.AlertTypes[i],
                        PublishRequestTime = now
                    };

                    await client.PublishEventAsync("rabbitmq-pubsub", "alert-topic", pubRequest);

                    app.Logger.LogInformation("Published data at: " + pubRequest.PublishRequestTime);    

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }
    }
}
