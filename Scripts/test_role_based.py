import requests
import random
import threading
import time

# API base URL
BASE_URL = "http://localhost:5020"

# Global lists for movie and person IDs
movie_ids = []
person_ids = []

# Thread-safe counter for successful operations
successful_operations = threading.Lock()
operation_counts = []

def login_and_get_movies(user_id):
    # Step 1: Login and retrieve the access token
    login_url = f"{BASE_URL}/login?userId={user_id}"
    login_response = requests.post(login_url)

    if login_response.status_code != 200:
        print(f"Failed to login as user {user_id}. Status code: {login_response.status_code}")
        print(f"Response: {login_response.text}")
        return

    login_data = login_response.json()
    access_token = login_data.get("access_token")
    if not access_token:
        print("Access token not found in the login response.")
        return

    print(f"Logged in as user {user_id}. Access token: {access_token}")

    # Step 2: Fetch the movies list
    movies_url = f"{BASE_URL}/movie/all"
    headers = {"Authorization": f"Bearer {access_token}"}
    movies_response = requests.get(movies_url, headers=headers)

    if movies_response.status_code != 200:
        print(f"Failed to fetch movies. Status code: {movies_response.status_code}")
        print(f"Response: {movies_response.text}")
        return

    movies_data = movies_response.json()

    # Step 3: Extract all IDs from the "movies" list
    movie_ids = [movie.get("id") for movie in movies_data.get("movies", []) if "id" in movie]

    # Step 2: Fetch the movies list
    persons_url = f"{BASE_URL}/person/all"
    persons_response = requests.get(persons_url, headers=headers)

    if persons_response.status_code != 200:
        print(f"Failed to fetch movies. Status code: {persons_response.status_code}")
        print(f"Response: {persons_response.text}")
        return

    persons_data = persons_response.json()

    # Step 3: Extract all IDs from the "movies" list
    person_ids = [person.get("id") for person in persons_data.get("persons", []) if "id" in person]

    return movie_ids, person_ids

def login(user_id):
    """Login and retrieve an access token."""
    login_url = f"{BASE_URL}/login?userId={user_id}"
    response = requests.post(login_url)
    if response.status_code == 200:
        data = response.json()
        return data.get("access_token")
    return None

def perform_operation():
    """Perform a single operation by logging in and making a request."""
    user_id = f"{random.randint(1, 10000):06}"  # Generate a new random user ID
    access_token = login(user_id)
    if not access_token:
        return False  # Login failed, count as unsuccessful

    headers = {"Authorization": f"Bearer {access_token}"}
    choice = random.random()

    # 40% chance of `/movie?id=XXX`
    if choice < 0.4 and movie_ids:
        movie_id = random.choice(movie_ids)
        response = requests.get(f"{BASE_URL}/movie?id={movie_id}", headers=headers)
        return response.status_code == 200

    # 40% chance of `/person?id=XXX`
    elif choice < 0.8 and person_ids:
        person_id = random.choice(person_ids)
        response = requests.get(f"{BASE_URL}/person?id={person_id}", headers=headers)
        return response.status_code == 200

    # 10% chance of `/movie/all`
    elif choice < 0.9:
        response = requests.get(f"{BASE_URL}/movie/all", headers=headers)
        return response.status_code == 200

    # 10% chance of `/person/all`
    else:
        response = requests.get(f"{BASE_URL}/person/all", headers=headers)
        return response.status_code == 200

def thread_function(thread_id, duration):
    """Run a thread for the specified duration, performing random operations."""
    global successful_operations

    successful_count = 0
    start_time = time.time()

    while time.time() - start_time < duration:
        if perform_operation():
            successful_count += 1

    # Store the result in a thread-safe way
    with successful_operations:
        operation_counts.append((thread_id, successful_count))

    print(f"Thread {thread_id}: Completed {successful_count} successful operations.")

def multithreaded_test():
    """Run the multithreaded test with 10 threads for 5 minutes."""
    global movie_ids, person_ids

    # Login and fetch movie and person IDs using user 000002
    movie_ids, person_ids = login_and_get_movies("000002")
    if not movie_ids or not person_ids:
        print("Failed to retrieve movie or person IDs.")
        return

    print(f"Retrieved {len(movie_ids)} movie IDs and {len(person_ids)} person IDs.")

    # Number of threads and duration
    num_threads = 10
    duration = 5 * 60  # 5 minutes in seconds

    # Create and start threads
    threads = []
    for i in range(num_threads):
        thread = threading.Thread(target=thread_function, args=(i + 1, duration))
        threads.append(thread)
        thread.start()

    # Wait for all threads to complete
    for thread in threads:
        thread.join()

    # Print the results
    total_operations = sum(count for _, count in operation_counts)
    print(f"\nTotal successful operations: {total_operations}")
    for thread_id, count in operation_counts:
        print(f"Thread {thread_id}: {count} successful operations.")

# Run the test
multithreaded_test()
