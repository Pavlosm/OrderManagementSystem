# OrderManagementSystem

## App specs at a glance

- `.NET version`: 9.0
- `Database`: SQL Server - Entity Framework Core - Code First. (During startup it seeds the data if the environment is set to `Development`)
- `Project structure`: The application is implemented based on the `Clean Architecture` principles. It contains the following projects: 
  - `OrderManagementSystem.Api`: A Wb API project exposing rest endpoints
  - `OrderManagementSystem.Core`: Contains all the business logic
  - `OrderManagementSystem.Infrastructure`: Contains the database context and the repository implementations
  - `OrderManagementSystem.Auth`: contains the authentication logic - token generation and validation
  - `OrderManagementSystem.Tests`: Contains the unit and integration tests
  - The dependency graph is as follows:
    - `OrderManagementSystem.Api` depends on `OrderManagementSystem.Core`, `OrderManagementSystem.Infrastructure` and `OrderManagementSystem.Auth`
    - `OrderManagementSystem.Infrastructure` depends on `OrderManagementSystem.Core`
    - `OrderManagementSystem.Core` provides the required interfaces for the `OrderManagementSystem.Infrastructure` to implement data access
  - The `OrderManagementSystem.Core` project contains the following folders:
    - `Entities`: Contains the domain entities that map to the database tables, and some other `Domain Model` classes. Given time the domain model would have beer richer especially for the entities/aggregates with a more complex logic.
    - `Interfaces`: Contains the interfaces for the repositories and the services
    - `Services`: Contains the services that implement the business logic on top of th domain model

## Further improvements Known limitations

This could be long, but here are a few things that could be improved:

- The domain model has a lot of room for improvement.
  - making it richer by adding more business logic to the entities
  - generating value types that embed the business rules (like valid address, contact info, etc)
  - even use event sourcing for some parts of the domain model
- The application is not using any caching mechanism, which might be a good idea to implement in order to improve the performance of the application.
  - examples are MenuItems, Users, Live Orders etc
- Metrics should be added (ex grafana-prometheus) to monitor the application.
- View models, or front facing contracts should be added
- Authentication is very basic, it doesn't even have refresh token
- DB access is done using Entity Framework Core, which is a good choice for small to medium applications, but for larger applications, it might not be the best choice. The application is not using any caching mechanism, which might be a good idea to implement in order to improve the performance of the application.
- Statistics and Auditability are poor, but at least there are the domain events that a service could consume to extract more miningful data
- there are no integration tests at the moment
- security has not been the main concern, settings should have added as environment variables, or secrets

## DB & access

The application is using a SQL Server database to persist its data.
For simplicity and ease of use for this assignment it uses entity framework code first approach to create the database and its tables.
During the startup of the application, is the environment is set to `Developement`, the database is created and seeded with some initial data.

## Run the application

The application is using the following environment variables:

- `ASPNETCORE_ENVIRONMENT`: The value should be set to `Development` in order to run the `DB` migrations.
- `SQL_CONNECTION_STRING`: The connection string for the database.
- `JWT_SECRET_KEY`: The secret key used to sign the JWT tokens.

How to run:
- `cd` into the root directory of the application that contains the `OrderManagementSystem.sln`, `Docker` and `docker-compose` files.
- `Docker`: To run the application using `docker` just `cd` into the root directory of the application and run the following command: `docker-compose up -d`. 
  - Access the application at http://localhost:5000/swagger/index.html
  - The solution contains a `Dockerfile` and a `docker-compose.yml` file that contain all the necessary configurations to run the application in a docker container.
- `Local`: 
  - install the .NET 9.0 if not already installed - https://dotnet.microsoft.com/en-us/download/dotnet/9.0
  - set the `SQL_CONNECTION_STRING` variables mentioned above to point to a valid SQL Server instance.
  - run: `dotnet run --project src/OrderManagementService.WebApi/OrderManagementService.WebApi.csproj`
  - Access the application at http://localhost:5001/swagger/index.html

The app uses Authentication and Authorization for some endpoints, the following users are pre-seeded in the database:

| Email                 | Password     | Role            |
|-----------------------|--------------|-----------------|
| superuser@gmail.com   | Admin1!      | Admin           |
| delivery1@gmail.com   | Delivery1!   | DeliveryStaff   |
| delivery2@gmail.com   | Delivery2!   | DeliveryStaff   |
| restaurant1@gmail.com | Restaurant1! | RestaurantStuff |
| restaurant2@gmail.com | Restaurant2! | RestaurantStuff |

