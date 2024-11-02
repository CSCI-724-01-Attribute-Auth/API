# Movie API

## Overview
This project implements a sophisticated API authorization and response filtering system that allows fine-grained control over JSON response data based on client permissions. The system uses JSONPath expressions to specify which attributes clients are authorized to access and efficiently filters API responses to include only permitted data.

## Installation
1. Clone the repository on your local machine and make sure you have .NET installed.

## Running the application
Navigate to the root directory and use the followig command to run the application -
`dotnet run`

## Architecture

### Key Components

### Program.cs

-- Application entry point
-- Configures and initializes the web host
-- Handles database initialization
-- Triggers data seeding when necessary

### Startup.cs

-- Configures the application's service container
-- Sets up middleware pipeline
-- Configures:
    CORS policies
    Database context
    JSON serialization options
    API routing
    Dependency injection

### Controllers
-- MoviesController
    Handles movie-related endpoints
    Implements CRUD operations for movies
    Integrates with response filtering system

-- PersonsController
    Manages crew member data
    Handles person-related operations
    Implements person-movie relationship endpoints

-- AuthorizedAttributesController
    Manages authorization rules
    Handles CRUD operations for attribute permissions
    Provides endpoints for authorization management

### Core Components
### Response Builder
The ResponseBuilder class is responsible for filtering JSON responses based on authorized attributes:

    Uses a Trie data structure for efficient pattern matching
    Removes unauthorized attributes from JSON responses
    Handles nested objects and array patterns
    Preserves JSON structure while removing unauthorized data

### Retriever
The Retriever class manages the authorization lookup process:

    Fetches authorized attributes based on client ID, HTTP method, and path
    Supports template-based path matching for flexible endpoint definitions
    Works with the IndexBuilder to maintain efficient access patterns

### Data Layer

    DBContext: Manages database connections and entity sets
    DataFactory: Handles data seeding and sample data generation
    IndexBuilder: Creates and maintains authorization indexes

### Models

    Movie: Represents movie data with properties like title, budget, and costs
    Person: Stores crew member information
    CrewMember: Manages movie-person relationships
    AuthorizedAttributes: Defines client authorization rules

### Dependency Management

    Utilizes dependency injection for loose coupling
    Manages service lifetimes through DependencyRegistration
    Supports testing through interface-based design

#### Data Layer
- `DBContext`: Manages database connections and entity sets
- `DataFactory`: Handles data seeding and sample data generation
- `IndexBuilder`: Creates and maintains authorization indexes

#### Models
- `Movie`: Represents movie data with properties like title, budget, and costs
- `Person`: Stores crew member information
- `CrewMember`: Manages movie-person relationships
- `AuthorizedAttributes`: Defines client authorization rules

#### Response Builder
The `ResponseBuilder` class is responsible for filtering JSON responses based on authorized attributes:
- Uses a Trie data structure for efficient pattern matching
- Removes unauthorized attributes from JSON responses
- Handles nested objects and array patterns
- Preserves JSON structure while removing unauthorized data

#### Retriever
The `Retriever` class manages the authorization lookup process:
- Fetches authorized attributes based on client ID, HTTP method, and path
- Supports template-based path matching for flexible endpoint definitions
- Works with the IndexBuilder to maintain efficient access patterns


## Technical Details

### Response Filtering Process
1. Build a Trie from authorized JSONPath patterns
2. Extract all attributes from the original JSON response
3. Identify unauthorized attributes using Trie pattern matching
4. Return the filtered JSON response

## Error Handling
The system includes comprehensive error handling for:
- Invalid JSON responses
- Unauthorized client access
- Malformed JSONPath expressions
- Non-existent endpoints
- Template matching failures

## Performance Considerations
- Uses Trie data structure for O(m) pattern matching (where m is pattern length)
- Efficient index-based lookup for client authorizations
- Optimized JSON parsing and manipulation
- Template matching with regular expressions for flexibility

## Security Features
- Attribute-level access control
- No information leakage through error messages
- Secure handling of sensitive data


