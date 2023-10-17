namespace ServiceBroker.ConsoleApplication.Interfaces;
public interface IPublisherService
{
    Task PublishMessage(string message = "Hello World!");
}
