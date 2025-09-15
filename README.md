# CoSpeakerProxy

CoSpeakerProxy is a .NET 9 web API for Belarusian speech-to-text and grammar correction, integrating Deepgram ASR and AWS Bedrock AI models.

## Features
- **ASR Transcription**: Transcribes Belarusian audio using Deepgram.
- **Grammar Correction**: Checks and corrects grammar in Belarusian text using AWS Bedrock.
- **JWT Authentication**: Secure endpoints with device-based JWT tokens.
- **CORS Support**: Configurable CORS for cross-origin requests.
- **Docker Support**: Ready-to-deploy Dockerfile for containerized environments.

## API Endpoints

### Authentication
- `POST /auth/token` — Get JWT token for device authentication.
  - Request: `{ "deviceId": "string" }`
  - Response: `{ "token": "string" }`

### ASR
- `POST /asr/transcribe` — Transcribe audio (test.m4a) to text.
  - Request: `{ "mock": false, "lang": "be" }`
  - Response: `{ "text": "...", "words": [ ... ] }`

### Grammar
- `POST /grammar/check` — Check and correct grammar in text.
  - Request: `{ "text": "string", "lang": "be" }`
  - Response: JSON with grammar corrections (see prompt in `PromptStorageService`).

## Configuration
Edit `appsettings.json` and provide:
- `Jwt:Issuer` — JWT token issuer
- `Jwt:Key` — JWT signing key
- `Deepgram:ApiKey` — Deepgram API key
- `Deepgram:Model` — Deepgram model name
- `AmazonBedrock:BaseUrl` — AWS Bedrock API base URL
- `AmazonBedrock:Model` — Bedrock model name
- `AmazonBedrock:ApiKey` — Bedrock API key

## Build & Run

### Local
```powershell
# Restore dependencies
 dotnet restore
# Build and run
 dotnet run --project CoSpeakerProxy.csproj
```

### Docker
```sh
# Build image
 docker build -t cospeakerproxy .
# Run container
 docker run -p 8080:8080 cospeakerproxy
```

## Dependencies
- .NET 9
- Deepgram SDK
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt

## Folder Structure
- `Clients/` — Deepgram client factory
- `Extensions/` — Service registration extensions
- `Models/` — DTOs and request builders
- `Routing/` — API route definitions
- `Services/` — Business logic (ASR, grammar, prompts)
- `test_records/` — Sample audio files

## License
MIT (see repository)
