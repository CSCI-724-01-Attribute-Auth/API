import random

paths_and_attributes = {
    "/movie": [
        "$.movie.id", "$.movie.title", "$.movie.description", "$.movie.releaseDate",
        "$.movie.totalCost", "$.movie.totalBudget", "$.movie.crew[*].id",
        "$.movie.crew[*].name", "$.movie.crew[*].birthDate", "$.movie.crew[*].mostFamousMovieId"
    ],
    "/movie/all": [
        "$.movies[*].id", "$.movies[*].title", "$.movies[*].description", "$.movies[*].releaseDate",
        "$.movies[*].totalCost", "$.movies[*].totalBudget", "$.movies[*].crew[*].id",
        "$.movies[*].crew[*].name", "$.movies[*].crew[*].birthDate", "$.movies[*].crew[*].mostFamousMovieId"
    ],
    "/person": [
        "$.person.id", "$.person.name", "$.person.birthDate", "$.person.mostFamousMovieId",
        "$.person.mostFamousMovie.id", "$.person.mostFamousMovie.title",
        "$.person.mostFamousMovie.description", "$.person.mostFamousMovie.totalBudget",
        "$.person.mostFamousMovie.totalCost", "$.person.mostFamousMovie.releaseDate",
        "$.person.workedOn[*].id", "$.person.workedOn[*].title", "$.person.workedOn[*].description",
        "$.person.workedOn[*].totalBudget", "$.person.workedOn[*].totalCost", "$.person.workedOn[*].releaseDate"
    ],
    "/person/all": [
        "$.persons[*].id", "$.persons[*].name", "$.persons[*].birthDate", "$.persons[*].mostFamousMovieId",
        "$.persons[*].mostFamousMovie.id", "$.persons[*].mostFamousMovie.title",
        "$.persons[*].mostFamousMovie.description", "$.persons[*].mostFamousMovie.totalBudget",
        "$.persons[*].mostFamousMovie.totalCost", "$.persons[*].mostFamousMovie.releaseDate",
        "$.persons[*].workedOn[*].id", "$.persons[*].workedOn[*].title", "$.persons[*].workedOn[*].description",
        "$.persons[*].workedOn[*].totalBudget", "$.persons[*].workedOn[*].totalCost", "$.persons[*].workedOn[*].releaseDate"
    ]
}

def generate_sql_for_roles(seed=0, batch_size=1000, output_file="insert_authorized_attributes_by_role.sql"):
    roles = [f"{i:06}" for i in range(1, 101)]  # Role IDs: "000001" to "000100"
    random.seed(seed)

    values = []
    with open(output_file, "w") as file:
        for role in roles:
            for path, attributes in paths_and_attributes.items():
                methods = ["GET"] if path in ["/movie/all", "/person/all"] else ["GET", "POST"]
                for method in methods:
                    subset = random.sample(attributes, k=random.randint(0, len(attributes)))
                    attribute_list = "[" + ",".join(f'"{attr}"' for attr in subset) + "]"
                    values.append(f"('{role}', '{method}', '{path}', '{attribute_list}')")
                    
                    # If batch size is reached, write to the file
                    if len(values) == batch_size:
                        file.write(
                            "INSERT INTO AuthorizedAttributesByRole (RoleID, Method, Path, AttributeList) VALUES\n"
                            + ",\n".join(values) + ";\n"
                        )
                        values = []  # Reset for the next batch

        # Write any remaining values to the file
        if values:
            file.write(
                "INSERT INTO AuthorizedAttributesByRole (RoleID, Method, Path, AttributeList) VALUES\n"
                + ",\n".join(values) + ";\n"
            )

# Generate the SQL
generate_sql_for_roles(seed=42, batch_size=1000)

def generate_sql_for_users(seed=0, batch_size=1000, output_file="insert_authorized_attributes.sql"):
    users = [f"{i:06}" for i in range(1, 10001)]  # User IDs: "000001" to "010000"
    random.seed(seed)

    values = []
    with open(output_file, "w") as file:
        for user in users:
            for path, attributes in paths_and_attributes.items():
                methods = ["GET"] if path in ["/movie/all", "/person/all"] else ["GET", "POST"]
                for method in methods:
                    subset = random.sample(attributes, k=random.randint(0, len(attributes)))
                    attribute_list = "[" + ",".join(f'"{attr}"' for attr in subset) + "]"
                    values.append(f"('{user}', '{method}', '{path}', '{attribute_list}')")
                    
                    # If batch size is reached, write to the file
                    if len(values) == batch_size:
                        file.write(
                            "INSERT INTO AuthorizedAttributes (ClientId, Method, Path, AttributeList) VALUES\n"
                            + ",\n".join(values) + ";\n"
                        )
                        values = []  # Reset for the next batch

        # Write any remaining values to the file
        if values:
            file.write(
                "INSERT INTO AuthorizedAttributes (ClientId, Method, Path, AttributeList) VALUES\n"
                + ",\n".join(values) + ";\n"
            )

# Generate the SQL
generate_sql_for_users(seed=42, batch_size=1000)