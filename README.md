# API Project

This project is a C# ASP.NET Core API that includes an attribute-based authorization middleware, allowing fine-grained control over the API responses based on either the `ClientId` (IBAC) or the `RoleId` (RBAC). This README provides instructions on setting up, running, and using the API, along with details on its attribute-based authorization features.

## Table of Contents
- [Project Overview](#project-overview)
- [Setup Instructions](#setup-instructions)
- [Running the API](#running-the-api)
- [API Structure and Key Endpoints](#api-structure-and-key-endpoints)
- [Attribute-Based Authorization](#attribute-based-authorization)
- [Error Handling](#error-handling)

---

## Project Overview

This API enables authorized clients to access specific data fields based on their permissions. A middleware component (`Authorization/AttributeAuthorizer.cs`) inspects each request, validates the client's identity using an Bearer Token, and tailors the response by excluding unauthorized attributes based on client-specific rules.

An overview of the authorization files (the meat and potatos of what we added), see the `Authorization/README.md` file in each project (`RBAC_API` and `IBAC_API`).

## Setup Instructions

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher)
- SQL Database
- Ensure environment variables and configurations are correctly set in `appsettings.json` if needed.

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/CSCI-724-01-Attribute-Auth/API
   cd API
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Database Setup**:
   The API uses a `DBContext` to interact with the database. Upon the first run, the application seeds the database with initial data if it is empty.

   Ensure the database configuration in `appsettings.json` or relevant environment variables are set correctly.

## Running the API

To run the API, use the following command from the project root directory:

```bash
dotnet run --project API
```

The API will start on the configured port (e.g., `https://localhost:5020` or `http://localhost:5000` by default).

## API Structure and Key Endpoints

Here is an overview of the key endpoints and their purposes:

- **Movies Endpoint** (`/movie`)
  - **GET /movie/all**: Retrieves a list of all movies in the database, with attributes tailored to the client's authorization.
  - **POST /movie?id=XYZ**: Retrieves a movie in the database, with attributes tailored to the client's authorization.
  - **POST /movie**: Allows creating a new movie entry.

- **Movies Endpoint** (`/person`)
  - **GET /person/all**: Retrieves a list of all persons in the database, with attributes tailored to the client's authorization.
  - **POST /person?id=XYZ**: Retrieves a person in the database, with attributes tailored to the client's authorization.
  - **POST /person**: Allows creating a new person entry.

## Attribute-Based Authorization

The attribute-based authorization middleware (`AttributeAuthorizer`) filters API responses based on the Bearer Token. This mechanism limits access to specific attributes within the response based on client permissions.

### How It Works

1. **Middleware Interception**: The `AttributeAuthorizer` middleware intercepts every request that is specified with the `[UseAttributeAuthorizer]` decorator, reading the Bearer Token header to identify the client.
   
2. **Authorization Rules**: The `Retriever` class checks the client's permissions in an in-memory `IndexCache`, which stores authorized paths and attributes for each client. If the client is authorized, the middleware will proceed to filter the response.

3. **Response Filtering**: If the client's ID and requested endpoint path match a template in their authorization profile, only the permitted attributes are included in the response. Unauthorized fields are removed from the JSON response, ensuring data confidentiality.

### Adding New Authorization Rules

To modify authorization rules or add new client configurations:
- Update the data the database as necessary, see the `Scripts` folder for examples.
- The `IndexCache` and `Retriever` services will use these updated configurations.

## Error Handling

- **Missing Client ID**: If a request does not include an Bearer token, the middleware will throw an `InvalidOperationException`.
- **Unauthorized Access**: If the client attempts to access an endpoint they do not have permissions for, the response will exclude restricted attributes.
- **Database Errors**: Any issues during database seeding or connection are logged.

## Additional Configuration

The following components are configurable:
- **Database Connection**: Configure in `appsettings.json`.

## License

This project is licensed under the terms specified in the `LICENSE` file.