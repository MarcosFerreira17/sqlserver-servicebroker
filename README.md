### How Service Broker works

As its objective is to create a simplified, secure and decoupled way of exchanging messages, Service Broker has a set of easy tasks to carry out this messaging process, between applications and Database Server instances. The Service Broker architecture is made up of the following components, tasks and characteristics:

- Conversations;
- Ordering and coordination of messages;
- Transactional asynchronous programming;
- Support for loosely coupled applications;
- Service Broker components.

### Conversation

Designed as a basic resource for sending and receiving messages, the Service Broker presents a task called “Conversation” carried out during the message exchange process. Each conversation task is recognized and treated as a reliable and persistent communication channel, the messages present a specific type of conversation that is treated by the Service Broker individually, which makes it possible to reinforce and guarantee security in the recording of data exchanged by the applications.

During this exchange of messages, the “Conversation” task allows the applications involved in this process to establish this communication channel within a “queue” that represents a display of an internal table related to the database that the Service Broker is doing. use.

For each conversation handled by the Service Broker, a sequence and ordering of Messages is established through the “Message Ordering and Coordination” task, this guarantees that each message is part of a single and exclusive conversation, that is, the same message exchanged will be handled by the same task and conversation.

A simpler way to represent and illustrate how a Service Broker works is to think about how a postal service works. To maintain a conversation with a distant colleague, you can communicate by sending letters via postal service.

The postal service sorts and delivers letters. You and your colleague then retrieve the letters from the mailboxes, read them, write responses, and send new letters until the conversation ends. Letter delivery happens asynchronously while you and your colleague handle other tasks.

### Message Ordering and Coordination

The entire control and maintenance process of message queues is carried out by the Service Broker directly in the Database Engine, adopting a traditional treatment through two aspects:

- The queues managed by the Service Broker are directly integrated with the database to which the service is involved.
- Queues are responsible for coordinating and ordering messages in transit.

Through strong control over sending and receiving messages, the task of “Message Ordering and Coordination” provides the Service Broker with guarantees in the message exchange flow, establishing two sides in the communication process, called:

- Starting side called the initiator;
- Destination side of the message called the receiver.

A basic Service Broker message exchange process consists of:

Below is an illustration of the exchange of messages in a typical dialogue:

#### In the launcher:

- A program starts the conversation.
- The program creates a message that contains the data needed to perform a task.
- The program sends the message to the destination service.

#### On the receiver:

- The message is placed on the queue associated with the destination service.
- A program receives the message from the queue and performs the work.
- The program responds by sending a message to the initiating service.

#### In the launcher:

- The response message is placed on the queue associated with the initiating service.
- A program receives the response and processes it.

Until the end of sending and receiving messages, this cycle repeats itself cyclically and continuously.

This cycle repeats until the initiator ends the conversation because it has no more requests to send to the destination.

### Transactional asynchronous programming

The “Transactional asynchronous programming” component is completely related to the Service Broker operating infrastructure, serving as a message transfer area, treating each process in a transactional way, which allows it to be reversed at any time.

In this component, the Service Broker controls the data that is being exchanged, establishing a scalability process, which guarantees that the service can be automated and grow according to the situation, one of which is the ability to automatically initiate the processes that process a queue, so it is possible for the application that is using the sending/receiving process to identify the moment in which the message is running or in the processing queue.

Asynchronous programming allows developers to create software solutions capable of writing data to message queues, making use of the database itself through one or more tables as internal message queue repositories.

### Support for loosely coupled applications

Considered as a feature and not a component or task, “Support for flexibly coupled applications” guarantees and enables the Service Broker to work with a very distinct set of independent applications that can be identified as possible message exchange components. These applications must internally contain the same structure and mechanism for exchanging messages that exists in the Service Broker, which in some situations allows this component to be coupled to the messaging service.

### Service Broker Components

The Service Broker is made up of three basic components, existing according to the task being performed:

- Conversation components: Known as dialogue, this is any conversation carried out through the Service Broker during the messaging process, allowing groups of conversions, conversations and messages to be handled by your applications by establishing the participants.
- Service definition components: Responsible for establishing the conversation flow, storing data in a database, this component defines the basic structure of the conversation carried out between the Service Broker and the application.
- Network and security components: Through this component, Service Broker allows database administrators to manage their environments without generating impacts on the application's source code, focusing on changes or changes in scenario, establishing a channel for exchanging messages in a secure and reliable, for the applications that are consuming the Service Broker.

_Observation_: Service definition components, network components, and security components are part of the database and SQL Server instance metadata. Talkgroups, conversations, and messages are part of the data that the database contains.
