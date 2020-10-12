# acs-messaging-app
A sample SMS messaging app built in Blazor using the Azure Communication Services to send and received text messages.

> This app has can send and received SMS messages in its current state, but is very WIP and needs a lot of work to be useful for developers hoping to experiment with SMS features in Azure Communication services. Documentation in full will be coming shortly.

## Getting Started
Follow these steps to get started with this sample app:

1. Have Azure Pay-as-you-Go subscription enabled in your Azure account (Visual Studio subscriptions are not currently supported for phone number acquisition; more to come on that front)
2. Provision an Azure Communication Services (ACS) resource in the Azure Portal on that subscription
3. Acquire a US Phone Number on the ACS resource you provision and enable it for inbound and outbound SMS messages
4. Register the Microsoft.EventGrid resource provider on your Azure subscription (the same sub on which you provisioned the ACS resource)
5. After enabling the Event Grid resource provider on your Azure subscription, add an Event Subscription for SMS Received events on your ACS resource.
    * Provide a name for the event subscription of your choosing
    * Provide a name for the topic for this events (isn't referenced in code, so use whatever you like)
    * Select Webhook as the target; if you're developing on a local machine, you can use [ngrok](https://ngrok.com/) to get through to your box and specify that address this field, e.g. `https://ffaa00cc11dd.ngrok.io/api/eventgrid/update`.
    * Validate and save the event subscription
6. Clone this repository and update the following values in your appsettings.json files for each environment:
    * `DbConnectionString`: If you've moved the included sqlite database, update this path (otherwise you can leave as is and it should work)
    * `AcsConnectionString`: The connection string for your ACS resource, found in the Azure portal
    * `PrimaryPhoneNumber`: The phone number you acquired for your ACS resource. Note that you should include the country code and the full number; e.g. for US numbers it would be 11 digits in total, 1 plus the 10 digit number
7. Built and run the app using .NET core in VS code (or within Visual Studio, or by running `dotnet run` from the command line)
## Feedback
If you have feedback to share, please create an [issue](https://github.com/svanvliet/acs-messaging-app/issues) on this repo or hit me up on Twitter [@scottvanvliet](https://twitter.com/scottvanvliet).