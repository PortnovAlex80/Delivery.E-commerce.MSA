# Delivery service. E-commerce OMS.Microservices and DDD, Clean architecture.

This template demonstrates a delivery service using Clean Architecture and DDD within a microservices framework. It focuses on modularity and efficient communication between services through ports and adapters.

Based on the course:["Microarch. Microservice Architecture"](https://microarch.ru/courses/hexagonal-architecture)

# Introduction
What has been developed and is in the repository is an example of a delivery service (the other services have already been developed).

We have developed an online store consisting of:

1. Basket
2. Delivery
3. Discount
4. Geo
5. Notification
6. Front-end

Additionally:

7. Project with infrastructure dependencies (infrastructure).
8. Service template (delivery-template).

# First step. Environment setup:

1. Install DotNet 8.0 SDK.
2. Install DotNet 8.0 Runtime.
3. Install or update EF Tools to the latest version by executing the following commands in the terminal:
   - To install: `dotnet tool install --global dotnet-ef`
   - To update: `dotnet tool update --global dotnet-ef`
4. Install Docker.

Integrated Development Environment (IDE):

You can use any IDE that suits you:

- VS Code (free)
  - Download and install Visual Studio Code.
  - Install the C# extension from the Marketplace for Visual Studio Code.
- Visual Studio Community (free)
- JetBrains Rider (30-day free trial)
  - [Download JetBrains Rider](https://www.jetbrains.com/rider/download/)
- Visual Studio (30-day free trial)
  - [Download Visual Studio](https://visualstudio.microsoft.com/#vs-section)

Utilities:

- Install PgAdmin (tool for working with PosrgreSql databases).
- Install Postman (tool for API testing).
