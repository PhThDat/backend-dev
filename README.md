# PtDat's back-end application

# What can this application do?
- Serving static webpages
- GET/POST HTTP API

# How to operate this application?
1. Make sure Docker is installed on your machine
2. Run the following commands:
- $ sudo docker compose up
- $ sudo docker exec -it backend-cs bash
3. Inside the container's bash terminal, execute the following commands:
- $ cd home/backend
- $ dotnet run

# What are the URLs?
### Default static page:
127.0.0.1:4000
### Signing up:
127.0.0.1:4000/user/signup/
### Signing in:
127.0.0.1:4000/user/signin/