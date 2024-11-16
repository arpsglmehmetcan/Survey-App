// Install the C# / .NET helper library from twilio.com/docs/csharp/install

using System;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using System.Threading.Tasks;

class DenemeSMS {
    public static async Task Main(string[] args) {
        // Find your Account SID and Auth Token at twilio.com/console
        // and set the environment variables. See http://twil.io/secure
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string accountSid = Environment.GetEnvironmentVariable("AC0207641989a64e367119ae3b8862ea94");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        string authToken = Environment.GetEnvironmentVariable("4f34ae76dacd1d9193c9a1f8cbb1f0d7");

        TwilioClient.Init(accountSid, authToken);

        var verification = await VerificationResource.CreateAsync(
            to: "+90 507 971 07 98",
            channel: "sms",
            pathServiceSid: "VAadc9a492c8210038c928a1c28362973c");

        Console.WriteLine(verification.Sid);
    }
}