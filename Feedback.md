### Feedback

*Please add below any feedback you want to send to the team*

Onion Architecture, CQRS (Only the Command part was required), MediatR, AutoMapper, DependencyInjection implemented.

An example extension method for Expiration Check and Middlewares for Custom Exception Handling and Request Timer Logging are added.

Test coverage provided for only the implemented features Handlers.
NUnit, Moq and Fluent Assertion were used for unit testing.

Lookup table had to be added for relation of Ticket and Seats

Seats needed to be deleted from Tickets as we've used the lookup table

ServiceId added into MovieEntity and SampleData as the type of the Id was different on both in grpc and in-memory db

Following the document, the Buy Ticket was only used for validating if the tickes is Buyable.
I've added just a simple function that might use the same logic whilst confirming the ticket in 'ConfirmReservationCommandHandler'
Line 23-25

For ApiClientGrpc, I'd like to write unit test for it but I didn't wanna change how it works.
I could be able to mock it by deriving a new class + interface from it but it wouldn't be an original test.
However, If it was required, I could do it.

Lodgify.postman_collection.json added to the Solution Items