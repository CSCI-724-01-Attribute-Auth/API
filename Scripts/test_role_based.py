import requests
import random
import threading
import time
import sys

# API base URL
BASE_URL = "http://localhost:5020"

# Global lists for movie and person IDs
movie_ids = []
person_ids = []

# Thread-safe counter for successful operations
successful_operations = threading.Lock()
operation_counts = []

def login_and_get_movies(user_id, session):
    """Login and retrieve access token, then fetch movie and person IDs."""
    login_url = f"{BASE_URL}/login?userId={user_id}"
    login_response = session.post(login_url)

    if login_response.status_code != 200:
        print(f"Failed to login as user {user_id}. Status code: {login_response.status_code}")
        print(f"Response: {login_response.text}")
        return [], []

    login_data = login_response.json()
    access_token = login_data.get("access_token")
    if not access_token:
        print("Access token not found in the login response.")
        return [], []

    headers = {"Authorization": f"Bearer {access_token}"}

    # Fetch movies
    movies_url = f"{BASE_URL}/movie/all"
    movies_response = session.get(movies_url, headers=headers)
    if movies_response.status_code != 200:
        print(f"Failed to fetch movies. Status code: {movies_response.status_code}")
        return [], []

    movies_data = movies_response.json()
    movie_ids = [movie.get("id") for movie in movies_data.get("movies", []) if "id" in movie]

    # Fetch persons
    persons_url = f"{BASE_URL}/person/all"
    persons_response = session.get(persons_url, headers=headers)
    if persons_response.status_code != 200:
        print(f"Failed to fetch persons. Status code: {persons_response.status_code}")
        return [], []

    persons_data = persons_response.json()
    person_ids = [person.get("id") for person in persons_data.get("persons", []) if "id" in person]

    return movie_ids, person_ids

def perform_operation(session):
    """Perform a single operation by logging in and making a request."""
    user_id = f"{random.randint(1, 10000):06}"  # Generate a new random user ID
    login_url = f"{BASE_URL}/login?userId={user_id}"
    login_response = session.post(login_url)

    if login_response.status_code != 200:
        return False

    login_data = login_response.json()
    access_token = login_data.get("access_token")
    if not access_token:
        return False

    headers = {"Authorization": f"Bearer {access_token}"}
    choice = random.random()

    # 40% chance of `/movie?id=XXX`
    if choice < 0.4 and movie_ids:
        movie_id = random.choice(movie_ids)
        response = session.get(f"{BASE_URL}/movie?id={movie_id}", headers=headers)
        return response.status_code == 200

    # 40% chance of `/person?id=XXX`
    elif choice < 0.8 and person_ids:
        person_id = random.choice(person_ids)
        response = session.get(f"{BASE_URL}/person?id={person_id}", headers=headers)
        return response.status_code == 200

    # 10% chance of `/movie/all`
    elif choice < 0.9:
        response = session.get(f"{BASE_URL}/movie/all", headers=headers)
        return response.status_code == 200

    # 10% chance of `/person/all`
    else:
        response = session.get(f"{BASE_URL}/person/all", headers=headers)
        return response.status_code == 200

def thread_function(thread_id, duration):
    """Run a thread for the specified duration, performing random operations."""
    global successful_operations

    session = requests.Session()  # Create a session for this thread
    successful_count = 0
    start_time = time.time()

    while time.time() - start_time < duration:
        if perform_operation(session):
            successful_count += 1

    # Store the result in a thread-safe way
    with successful_operations:
        operation_counts.append((thread_id, successful_count))

    # print(f"Thread {thread_id}: Completed {successful_count} successful operations.")

def multithreaded_test(t_count, dur_min):
    """Run the multithreaded test with 10 threads for the specified duration."""
    global movie_ids, person_ids

    # Use a temporary session to fetch movie and person IDs
    with requests.Session() as temp_session:
        movie_ids, person_ids = login_and_get_movies("000002", temp_session)

    if not movie_ids or not person_ids:
        print("Failed to retrieve movie or person IDs.")
        return

    # Number of threads and duration
    duration = dur_min * 60  # Convert minutes to seconds

    # Create and start threads
    threads = []
    for i in range(t_count):
        thread = threading.Thread(target=thread_function, args=(i + 1, duration))
        threads.append(thread)
        thread.start()

    # Wait for all threads to complete
    for thread in threads:
        thread.join()

    # Print the results
    total_operations = sum(count for _, count in operation_counts)
    print(f"Total successful operations: {total_operations}")

# Run the test
if __name__ == "__main__":
    if len(sys.argv) > 2:
        t_count, dur_min = int(sys.argv[1]), float(sys.argv[2])
        multithreaded_test(t_count, dur_min)
    else:
        dur_min = 5
        for i in range(15):
            print(f'Running {i + 1} threads...')
            multithreaded_test(i + 1, dur_min)
