# Just Planning Poker
Just Planning Poker is a simple lightweight planning poker web application. It runs in-memory, and thus only requires a web application to host and run the tool.

It is written in .Net 9, using [Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-9.0#blazor-webassembly).

It is designed to supplement face-to-face planning poker, since many development teams are often working remotely, following the global pandemic that resulted in office shut-downs in most of the world.

## Running with Docker

### Quick Start
1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```

2. Build the image:
   ```bash
   docker build -t planning-poker .
   ```

3. Run the container:
   ```bash
   docker run -p 8080:8080 --name justplanningpoker planning-poker
   ```

4. Access the application:
   - HTTP: http://localhost:8080

### Environment Variables
The following environment variables can be configured:
- `ASPNETCORE_ENVIRONMENT`: Set to `Production` for production deployment
- `ASPNETCORE_URLS`: Configure the URLs the application listens on