# Authorization System

This is where all of the actual work is done to authorize our endpoints based on the data coming from the database.

## Files Overview

### AttributeAuthorizer.cs

**Purpose:** Contains logic for attribute-based authorization. This service is called after every execution to an endpoint with the `[UseAttributeAuthorizer]` decorator. This allows us to restrict attributes for these endpoints based on their entries in the database.

### CacheRefreshService.cs

**Purpose:** Responsible for refreshing or maintaining the Index cache. This is called every 5 minutes to always return fresh data while not requesting data from the database every execution cycle.

### IndexBuilder.cs

**Purpose:** Builds an index structure for quick lookup of attributes. This uses a trie structure to hold all of the records neatly with instant lookups.

### JwtTokenGenerator.cs

**Purpose:** Generates JSON Web Tokens (JWTs) used for authentication and authorization purposes. It holds the ClientId for every user and also holds the RoleId, if present.

### Mapper.cs

**Purpose:** Maps input data into a format compatible with the authorization system.

### ResponseBuilder.cs

**Purpose:** Constructs responses while ensuring only authorized attributes are included in the output. This is the component that filters out all of the unauthorized attributes.

### Retriever.cs

**Purpose:** This components converts a Role/ClientId to a list of authorized attributes. This data is then passed to the `ResponseBuilder` for processing.

### UseAttributeAuthorizer.cs

**Purpose:** This is a decorator that allows us to enable AttributeAuthorization on specific endpoints while leaving others open to the public. For example, our `Login` endpoint is open to the public, while all of the `Movie` endpoints are authorized.